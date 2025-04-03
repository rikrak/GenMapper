using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SourceGenerator.Implementation
{
    internal class MapperClassGenerator
    {
        private readonly INamedTypeSymbol _classSymbol;
        private readonly INamedTypeSymbol _sourceType;
        private readonly INamedTypeSymbol _targetType;
        private readonly bool _isSourceStruct;
        private readonly bool _isTargetStruct;
        private readonly string _sourceTypeName;
        private readonly string _targetTypeName;

        public MapperClassGenerator(INamedTypeSymbol classSymbol, INamedTypeSymbol sourceType, INamedTypeSymbol targetType)
        {
            this._classSymbol = classSymbol;
            this._sourceType = sourceType;
            this._targetType = targetType;

            this._isSourceStruct = sourceType.IsValueType;
            this._isTargetStruct = targetType.IsValueType;

            this._sourceTypeName = sourceType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            this._targetTypeName = targetType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        }

        public string GetCode()
        {
            string classNamespace = this._classSymbol.ContainingNamespace?.ToDisplayString() ?? string.Empty;

            var usings = new HashSet<string> { "using System;", "using System.Diagnostics.CodeAnalysis;" };
            if (!string.IsNullOrEmpty(this._sourceType.ContainingNamespace?.ToString()))
            {
                usings.Add($"using {this._sourceType.ContainingNamespace};");
            }

            if (!string.IsNullOrEmpty(this._targetType.ContainingNamespace?.ToString()))
            {
                usings.Add($"using {this._targetType.ContainingNamespace};");
            }

            var sb = new StringBuilder();

            // Add using statements
            foreach (var use in usings)
            {
                sb.AppendLine(use);
            }

            var indent = new CodeIndent();
            if (!string.IsNullOrEmpty(classNamespace))
            {
                sb.AppendLine($"namespace {classNamespace}");
                sb.AppendLine($"{indent++}{{");
            }

            sb.AppendLine($"{indent}public partial class {this._classSymbol.Name} : global::SourceGenerator.IMapper");
            sb.AppendLine($"{indent++}{{");

            var buildTargetMethod = this.GenerateBuildTargetMethod(indent);
            sb.AppendLine(buildTargetMethod);

            var convertObject = this.GenerateConvertObjectMethod(indent);
            sb.AppendLine(convertObject);

            var convertMethod = GenerateConvertMethod(indent);
            sb.AppendLine(convertMethod);

            var mapObjectsMethod = this.GenerateMapObjectsMethod(indent);
            sb.AppendLine(mapObjectsMethod);
            
            var mapMethod = GenerateMapMethod(indent);
            sb.AppendLine(mapMethod);

            var canMap = GenerateCanMapMethod(indent);
            sb.AppendLine(canMap);

            
            sb.AppendLine($"{--indent}}}");

            if (!string.IsNullOrEmpty(classNamespace))
            {
                sb.AppendLine($"{--indent}}}");
            }

            return sb.ToString();

        }

        private string GenerateMapMethod(CodeIndent indent)
        {
            var sb = new StringBuilder();
            var sourceProperties = this._sourceType.GetMembers()
                .OfType<IPropertySymbol>()
                .Where(p => p.GetMethod != null); // Properties with getters

            var targetProperties = this._targetType.GetMembers()
                .OfType<IPropertySymbol>()
                .Where(p => p.SetMethod != null); // Properties with setters

            var commonProperties = sourceProperties
                .Join(targetProperties, sp => sp.Name, tp => tp.Name, (sp, tp) => new { sp, tp })
                .ToList();

            sb.AppendLine($"{indent}public void Map({this._sourceTypeName} source, {(this._isTargetStruct ? "ref " : "")}{this._targetTypeName} target)");
            sb.AppendLine($"{indent++}{{");
            if (!this._isSourceStruct)
            {
                sb.AppendLine($"{indent}if (source == null) {{ throw new ArgumentNullException(nameof(source)); }}");

            }
            if (!this._isTargetStruct)
            {
                sb.AppendLine($"{indent}if (target == null) {{ throw new ArgumentNullException(nameof(target)); }}");

            }

            foreach (var prop in commonProperties)
            {
                if (prop.sp.Type.Equals(prop.tp.Type, SymbolEqualityComparer.Default))
                {
                    sb.AppendLine($"{indent}target.{prop.tp.Name} = source.{prop.sp.Name};");
                }
            }

            sb.AppendLine($"{--indent}}}");
            return sb.ToString();
        }

        private string GenerateConvertMethod(CodeIndent indent)
        {
            var sb = new StringBuilder();
            if (!this._isSourceStruct && !this._isTargetStruct)
            {
                sb.AppendLine($"{indent}[return: NotNullIfNotNull(nameof(source))]");
            }

            sb.AppendLine($"{indent}public {(this._isTargetStruct ? this._targetTypeName : $"{this._targetTypeName}?")} Convert({(this._isSourceStruct ? this._sourceTypeName : $"{this._sourceTypeName}?")} source)");
            sb.AppendLine($"{indent++}{{");
            if (!this._isSourceStruct)
            {
                sb.AppendLine($"{indent}if (source == null) return " + (this._isTargetStruct ? "default;" : "null;"));
            }
            sb.AppendLine($"{indent}var result = BuildTarget(source);");

            sb.AppendLine($"{indent}this.Map(source, {(this._isTargetStruct ? "ref " : "")}result);");

            sb.AppendLine($"{indent}return result;");
            sb.AppendLine($"{--indent}}}");
            sb.AppendLine();

            return sb.ToString();
        }

        private string GenerateCanMapMethod(CodeIndent indent)
        {
            var code = new StringBuilder();
            code.AppendLine($"{indent}public bool CanMap(Type sourceType, Type targetType)");
            code.AppendLine($"{indent++}{{");
            code.AppendLine($"{indent}return sourceType == typeof({this._sourceTypeName}) && targetType == typeof({this._targetTypeName});");
            code.AppendLine($"{--indent}}}");
            return code.ToString();
        }

        private string GenerateMapObjectsMethod(CodeIndent indent)
        {
            var code = new StringBuilder();
            code.AppendLine($"{indent}public void Map(object source, object target)");
            code.AppendLine($"{indent++}{{");
            code.AppendLine($"{indent}if (source == null) {{ throw new ArgumentNullException(nameof(source)); }}");
            code.AppendLine($"{indent}if (target == null) {{ throw new ArgumentNullException(nameof(target)); }}");

            if (_isSourceStruct)
            {
                code.AppendLine($"{indent}var src = ({this._sourceTypeName})source;");
            }
            else
            {
                code.AppendLine($"{indent}var src = source as {this._sourceTypeName};");
                code.AppendLine($"{indent}if (src == null)");
                code.AppendLine($"{indent++}{{");
                code.AppendLine($"{indent}throw new InvalidOperationException($\"{{this.GetType().Name}} does not support mapping {{source.GetType().Name}} to {{target.GetType().Name}}\");");
                code.AppendLine($"{--indent}}}");
            }

            if (this._isTargetStruct)
            {
                code.AppendLine($"{indent}var tgt = ({this._targetTypeName})target;");
            }
            else
            {
                code.AppendLine($"{indent}var tgt = target as {this._targetTypeName};");
                code.AppendLine($"{indent}if (tgt == null)");
                code.AppendLine($"{indent++}{{");
                code.AppendLine($"{indent}throw new InvalidOperationException($\"{{this.GetType().Name}} does not support mapping {{source.GetType().Name}} to {{target.GetType().Name}}\");");
                code.AppendLine($"{--indent}}}");
            }

            code.AppendLine($"{indent}this.Map(src, tgt);");
            code.AppendLine($"{--indent}}}");
            return code.ToString();
        }

        private string GenerateBuildTargetMethod(CodeIndent indent)
        {
            bool hasBuildTarget = this._classSymbol.GetMembers().OfType<IMethodSymbol>()
                .Any(m => m.Name == "BuildTarget" && m.ReturnType.Equals(this._targetType, SymbolEqualityComparer.Default));

            var code = new StringBuilder();
            if (!hasBuildTarget)
            {
                code.AppendLine($"{indent}protected {this._targetTypeName} BuildTarget({this._sourceTypeName} source)");
                code.AppendLine($"{indent++}{{");
                code.AppendLine($"{indent}return new {this._targetTypeName}();");
                code.AppendLine($"{--indent}}}");
                code.AppendLine();
            }

            return code.ToString();
        }

        private string GenerateConvertObjectMethod(CodeIndent indent)
        {
            var code = new StringBuilder();
            code.AppendLine($"{indent}[return: NotNullIfNotNull(\"source\")]");
            code.AppendLine($"{indent}[return: MaybeNull]");
            code.AppendLine($"{indent}public object Convert([System.Diagnostics.CodeAnalysis.AllowNull] object source)");
            code.AppendLine($"{indent++}{{");
            code.AppendLine($"{indent}if (source == null) {{ return null; }}");

            if (_isSourceStruct)
            {
                code.AppendLine($"{indent}var src = ({this._sourceTypeName})source;");
            }
            else
            {
                code.AppendLine($"{indent}var src = source as {this._sourceTypeName};");
                code.AppendLine($"{indent}if (src == null)");
                code.AppendLine($"{indent++}{{");
                code.AppendLine($"{indent}throw new InvalidOperationException($\"{{this.GetType().Name}} does not support mapping from {{source.GetType().Name}}\");");
                code.AppendLine($"{--indent}}}");
            }

            code.AppendLine($"{indent}return this.Convert(src);");
            code.AppendLine($"{--indent}}}");

            return code.ToString();
        }


    }
}
