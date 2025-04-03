using SourceGenerator;

namespace GenMapper;

[Mapper(typeof(PersonDto), typeof(PersonViewModel))]
public partial class PersonDtoToPersonViewModelMapperGen{}

[Mapper(typeof(int), typeof(IntIdentifier))]
public partial class IntToIntIdentifierMapperGen
{
    public IntIdentifier BuildTarget(int value) => new IntIdentifier(value);
}

[Mapper(typeof(IntIdentifier), typeof(int))]
public partial class IntIdentifierToIntMapperGen
{
    public int BuildTarget(IntIdentifier value) => (int)value;
}