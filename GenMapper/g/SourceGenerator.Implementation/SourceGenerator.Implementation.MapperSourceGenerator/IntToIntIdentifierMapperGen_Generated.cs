using System;
using System.Diagnostics.CodeAnalysis;
using GenMapper;
namespace GenMapper
{
    public partial class IntToIntIdentifierMapperGen : global::SourceGenerator.IMapper
    {

        [return: NotNullIfNotNull("source")]
        [return: MaybeNull]
        public object Convert([System.Diagnostics.CodeAnalysis.AllowNull] object source)
        {
            if (source == null) { return null; }
            var src = (int)source;
            return this.Convert(src);
        }

        public global::GenMapper.IntIdentifier Convert(int source)
        {
            var result = BuildTarget(source);
            this.Map(source, ref result);
            return result;
        }


        public void Map(object source, object target)
        {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (target == null) { throw new ArgumentNullException(nameof(target)); }
            var src = (int)source;
            var tgt = (global::GenMapper.IntIdentifier)target;
            this.Map(src, tgt);
        }

        public void Map(int source, ref global::GenMapper.IntIdentifier target)
        {
        }

        public bool CanMap(Type sourceType, Type targetType)
        {
            return sourceType == typeof(int) && targetType == typeof(global::GenMapper.IntIdentifier);
        }

    }
}
