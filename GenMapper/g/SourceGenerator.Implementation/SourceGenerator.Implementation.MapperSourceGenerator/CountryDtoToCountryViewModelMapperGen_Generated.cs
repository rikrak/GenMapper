using System;
using System.Diagnostics.CodeAnalysis;
using GenMapper;
namespace GenMapper
{
    public partial class CountryDtoToCountryViewModelMapperGen : global::SourceGenerator.IMapper
    {
        protected global::GenMapper.CountryViewModel BuildTarget(global::GenMapper.CountryDto source)
        {
            return new global::GenMapper.CountryViewModel();
        }


        [return: NotNullIfNotNull("source")]
        [return: MaybeNull]
        public object Convert([System.Diagnostics.CodeAnalysis.AllowNull] object source)
        {
            if (source == null) { return null; }
            var src = source as global::GenMapper.CountryDto;
            if (src == null)
            {
                throw new InvalidOperationException($"{this.GetType().Name} does not support mapping from {source.GetType().Name}");
            }
            return this.Convert(src);
        }

        [return: NotNullIfNotNull(nameof(source))]
        public global::GenMapper.CountryViewModel? Convert(global::GenMapper.CountryDto? source)
        {
            if (source == null) return null;
            var result = BuildTarget(source);
            this.Map(source, result);
            return result;
        }


        public void Map(object source, object target)
        {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (target == null) { throw new ArgumentNullException(nameof(target)); }
            var src = source as global::GenMapper.CountryDto;
            if (src == null)
            {
                throw new InvalidOperationException($"{this.GetType().Name} does not support mapping {source.GetType().Name} to {target.GetType().Name}");
            }
            var tgt = target as global::GenMapper.CountryViewModel;
            if (tgt == null)
            {
                throw new InvalidOperationException($"{this.GetType().Name} does not support mapping {source.GetType().Name} to {target.GetType().Name}");
            }
            this.Map(src, tgt);
        }

        public void Map(global::GenMapper.CountryDto source, global::GenMapper.CountryViewModel target)
        {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (target == null) { throw new ArgumentNullException(nameof(target)); }
            target.Name = source.Name;
        }

        public bool CanMap(Type sourceType, Type targetType)
        {
            return sourceType == typeof(global::GenMapper.CountryDto) && targetType == typeof(global::GenMapper.CountryViewModel);
        }

    }
}
