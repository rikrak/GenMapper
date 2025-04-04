﻿using System;
using System.Diagnostics.CodeAnalysis;
using GenMapper;
namespace GenMapper
{
    public partial class PersonDtoToPersonViewModelMapperGen : global::SourceGenerator.IMapper
    {
        protected global::GenMapper.PersonViewModel BuildTarget(global::GenMapper.PersonDto source)
        {
            return new global::GenMapper.PersonViewModel();
        }


        [return: NotNullIfNotNull("source")]
        [return: MaybeNull]
        public object Convert([System.Diagnostics.CodeAnalysis.AllowNull] object source)
        {
            if (source == null) { return null; }
            var src = source as global::GenMapper.PersonDto;
            if (src == null)
            {
                throw new InvalidOperationException($"{this.GetType().Name} does not support mapping from {source.GetType().Name}");
            }
            return this.Convert(src);
        }

        [return: NotNullIfNotNull(nameof(source))]
        public global::GenMapper.PersonViewModel? Convert(global::GenMapper.PersonDto? source)
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
            var src = source as global::GenMapper.PersonDto;
            if (src == null)
            {
                throw new InvalidOperationException($"{this.GetType().Name} does not support mapping {source.GetType().Name} to {target.GetType().Name}");
            }
            var tgt = target as global::GenMapper.PersonViewModel;
            if (tgt == null)
            {
                throw new InvalidOperationException($"{this.GetType().Name} does not support mapping {source.GetType().Name} to {target.GetType().Name}");
            }
            this.Map(src, tgt);
        }

        public void Map(global::GenMapper.PersonDto source, global::GenMapper.PersonViewModel target)
        {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (target == null) { throw new ArgumentNullException(nameof(target)); }
            target.Id = source.Id;
            target.Name = source.Name;
        }

        public bool CanMap(Type sourceType, Type targetType)
        {
            return sourceType == typeof(global::GenMapper.PersonDto) && targetType == typeof(global::GenMapper.PersonViewModel);
        }

    }
}
