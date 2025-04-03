using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace SourceGenerator.Implementation;

// created from chatgpt :-)
// https://chatgpt.com/c/67ee5326-532c-8004-806e-814b73f8254e

[Generator]
public class MapperSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Get class declarations with attributes
        var classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => s is ClassDeclarationSyntax cds && cds.AttributeLists.Count > 0,
                transform: static (ctx, _) => (ClassDeclarationSyntax)ctx.Node)
            .Where(static c => c is not null);

        // Combine class declarations with the Compilation
        var classDeclarationsWithCompilation = classDeclarations.Combine(context.CompilationProvider);

        // Register the source output using the combined data
        context.RegisterSourceOutput(classDeclarationsWithCompilation, (spc, source) =>
        {
            var (classDeclaration, compilation) = source;

            var semanticModel = compilation.GetSemanticModel(classDeclaration.SyntaxTree);
            var classSymbol = semanticModel.GetDeclaredSymbol(classDeclaration) as INamedTypeSymbol;

            if (classSymbol is null)
            {
                return;
            }

            var mapperAttribute = classSymbol.GetAttributes()
                .FirstOrDefault(a => a.AttributeClass?.Name == "MapperAttribute");

            if (mapperAttribute == null)
            {
                return;
            }

            var sourceType = mapperAttribute.ConstructorArguments[0].Value as INamedTypeSymbol;
            var targetType = mapperAttribute.ConstructorArguments[1].Value as INamedTypeSymbol;

            if (sourceType is null || targetType is null)
            {
                return;
            }

            var generatedCode = GenerateMapperClass(classSymbol, sourceType, targetType);
            spc.AddSource($"{classSymbol.Name}_Generated.cs", SourceText.From(generatedCode, Encoding.UTF8));
        });
    }

    private string GenerateMapperClass(INamedTypeSymbol classSymbol, INamedTypeSymbol sourceType, INamedTypeSymbol targetType)
    {
        bool isSourceStruct = sourceType.IsValueType;
        bool isTargetStruct = targetType.IsValueType;

        string sourceTypeName = sourceType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        string targetTypeName = targetType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        string classNamespace = classSymbol.ContainingNamespace?.ToDisplayString() ?? string.Empty;

        var usings = new HashSet<string> { "using System;", "using System.Diagnostics.CodeAnalysis;" };
        if (!string.IsNullOrEmpty(sourceType.ContainingNamespace?.ToString()))
        {
            usings.Add($"using {sourceType.ContainingNamespace};");
        }

        if (!string.IsNullOrEmpty(targetType.ContainingNamespace?.ToString()))
        {
            usings.Add($"using {targetType.ContainingNamespace};");
        }

        var sourceProperties = sourceType.GetMembers()
            .OfType<IPropertySymbol>()
            .Where(p => p.GetMethod != null); // Properties with getters

        var targetProperties = targetType.GetMembers()
            .OfType<IPropertySymbol>()
            .Where(p => p.SetMethod != null); // Properties with setters

        var commonProperties = sourceProperties
            .Join(targetProperties, sp => sp.Name, tp => tp.Name, (sp, tp) => new { sp, tp })
            .ToList();

        bool hasBuildTarget = classSymbol.GetMembers().OfType<IMethodSymbol>()
            .Any(m => m.Name == "BuildTarget" && m.ReturnType.Equals(targetType, SymbolEqualityComparer.Default));

        var sb = new StringBuilder();

        // Add using statements
        foreach (var use in usings)
        {
            sb.AppendLine(use);
        }

        if (!string.IsNullOrEmpty(classNamespace))
        {
            sb.AppendLine($"namespace {classNamespace}");
            sb.AppendLine("{");
        }

        sb.AppendLine($"public partial class {classSymbol.Name}");
        sb.AppendLine("{");

        if (!hasBuildTarget)
        {
            sb.AppendLine($"    protected {targetTypeName} BuildTarget({sourceTypeName} source)");
            sb.AppendLine("    {");
            sb.AppendLine($"        return new {targetTypeName}();");
            sb.AppendLine("    }");
            sb.AppendLine();
        }

        if (!isSourceStruct && !isTargetStruct)
        {
            sb.AppendLine("    [return: NotNullIfNotNull(nameof(source))]");
        }

        sb.AppendLine($"    public {(isTargetStruct ? targetTypeName : $"{targetTypeName}?")} Convert({(isSourceStruct ? sourceTypeName : $"{sourceTypeName}?")} source)");
        sb.AppendLine("    {");
        if (!isSourceStruct)
        {
            sb.AppendLine("        if (source == null) return " + (isTargetStruct ? "default;" : "null;"));
        }
        sb.AppendLine("        var result = BuildTarget(source);");

        foreach (var prop in commonProperties)
        {
            bool needsConversion = NeedsConversion(prop.sp.Type, prop.tp.Type);
            if (needsConversion)
            {
                sb.AppendLine($"        result.{prop.tp.Name} = ({prop.tp.Type})Convert.ChangeType(source.{prop.sp.Name}, typeof({prop.tp.Type}));");
            }
            else
            {
                sb.AppendLine($"        result.{prop.tp.Name} = source.{prop.sp.Name};");
            }
        }

        sb.AppendLine("        return result;");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine($"    public void Map({sourceTypeName} source, {targetTypeName} target)");
        sb.AppendLine("    {");
        if (!isSourceStruct)
        {
            sb.AppendLine("        if (source == null) { throw new ArgumentNullException(nameof(source)); }");

        }
        if (!isTargetStruct)
        {
            sb.AppendLine("        if (target == null) { throw new ArgumentNullException(nameof(target)); }");

        }

        foreach (var prop in commonProperties)
        {
            bool needsConversion = NeedsConversion(prop.sp.Type, prop.tp.Type);
            if (needsConversion)
            {
                sb.AppendLine($"        target.{prop.tp.Name} = ({prop.tp.Type})Convert.ChangeType(source.{prop.sp.Name}, typeof({prop.tp.Type}));");
            }
            else
            {
                sb.AppendLine($"        target.{prop.tp.Name} = source.{prop.sp.Name};");
            }
        }

        sb.AppendLine("    }");
        sb.AppendLine("}");

        if (!string.IsNullOrEmpty(classNamespace))
        {
            sb.AppendLine("}");
        }

        return sb.ToString();
    }

    private bool NeedsConversion(ITypeSymbol sourceType, ITypeSymbol targetType)
    {
        return !SymbolEqualityComparer.Default.Equals(sourceType, targetType) &&
               sourceType.AllInterfaces.Any(i => i.ToDisplayString() == "System.IConvertible") &&
               targetType.AllInterfaces.Any(i => i.ToDisplayString() == "System.IConvertible");
    }

}
