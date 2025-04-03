using System;
using System.Diagnostics.CodeAnalysis;
using GenMapper;
namespace GenMapper
{
    public partial class IntIdentifierToIntMapperGen : global::SourceGenerator.IMapper
    {

        [return: NotNullIfNotNull("source")]
        [return: MaybeNull]
        public object Convert([System.Diagnostics.CodeAnalysis.AllowNull] object source)
        {
            if (source == null) { return null; }
            var src = (global::GenMapper.IntIdentifier)source;
            return this.Convert(src);
        }

        public int Convert(global::GenMapper.IntIdentifier source)
        {
            var result = BuildTarget(source);
            this.Map(source, ref result);
            return result;
        }


        public void Map(object source, object target)
        {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (target == null) { throw new ArgumentNullException(nameof(target)); }
            var src = (global::GenMapper.IntIdentifier)source;
            var tgt = (int)target;
            this.Map(src, tgt);
        }

        public void Map(global::GenMapper.IntIdentifier source, ref int target)
        {
        }

        public bool CanMap(Type sourceType, Type targetType)
        {
            return sourceType == typeof(global::GenMapper.IntIdentifier) && targetType == typeof(int);
        }

    }
}
