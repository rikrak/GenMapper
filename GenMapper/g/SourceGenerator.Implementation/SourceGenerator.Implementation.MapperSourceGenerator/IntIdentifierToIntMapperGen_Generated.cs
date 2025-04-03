using System;
using System.Diagnostics.CodeAnalysis;
using GenMapper;
namespace GenMapper
{
public partial class IntIdentifierToIntMapperGen
{
    public int Convert(global::GenMapper.IntIdentifier source)
    {
        var result = BuildTarget(source);
        return result;
    }

    public void Map(global::GenMapper.IntIdentifier source, int target)
    {
    }
}
}
