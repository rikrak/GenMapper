using System;
using System.Diagnostics.CodeAnalysis;
using GenMapper;
namespace GenMapper
{
public partial class PersonDtoToPersonViewModelMapperGen
{
    protected global::GenMapper.PersonViewModel BuildTarget(global::GenMapper.PersonDto source)
    {
        return new global::GenMapper.PersonViewModel();
    }

    [return: NotNullIfNotNull(nameof(source))]
    public global::GenMapper.PersonViewModel? Convert(global::GenMapper.PersonDto? source)
    {
        if (source == null) return null;
        var result = BuildTarget(source);
        result.Id = source.Id;
        result.Name = source.Name;
        return result;
    }

    public void Map(global::GenMapper.PersonDto source, global::GenMapper.PersonViewModel target)
    {
        if (source == null) { throw new ArgumentNullException(nameof(source)); }
        if (target == null) { throw new ArgumentNullException(nameof(target)); }
        target.Id = source.Id;
        target.Name = source.Name;
    }
}
}
