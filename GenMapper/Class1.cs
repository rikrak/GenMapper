using SourceGenerator;

namespace GenMapper;

public class CountryDto
{
    public IntIdentifier Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class CountryViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

[Mapper(typeof(CountryDto), typeof(CountryViewModel))]
public partial class CountryDtoToCountryViewModelMapperGen
{
}