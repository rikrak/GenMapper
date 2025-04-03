using System.Diagnostics.CodeAnalysis;

namespace GenMapper;

public class PersonDtoToPersonViewModelMapper
{
    public PersonViewModel BuildTarget(PersonDto source)
    {
        return new PersonViewModel();
    }

    [return: NotNullIfNotNull(nameof(source))]
    public PersonViewModel? Convert(PersonDto? source)
    {
        if (source == null)
        {
            return null;
        }
        var result = BuildTarget(source);

        result.Id = source.Id;
        result.Name = source.Name;

        return result;
    }

    public void Map(PersonDto source, PersonViewModel target)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(target);

        target.Id = source.Id;
        target.Name = source.Name;
    }
}