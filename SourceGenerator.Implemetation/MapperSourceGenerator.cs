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
        var generator = new MapperClassGenerator(classSymbol, sourceType, targetType);
        return generator.GetCode();
    }
}
