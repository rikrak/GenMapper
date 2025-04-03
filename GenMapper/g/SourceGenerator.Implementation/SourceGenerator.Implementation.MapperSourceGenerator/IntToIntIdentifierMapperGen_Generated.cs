using System;
using System.Diagnostics.CodeAnalysis;
using GenMapper;
namespace GenMapper
{
public partial class IntToIntIdentifierMapperGen
{
    public global::GenMapper.IntIdentifier Convert(int source)
    {
        var result = BuildTarget(source);
        return result;
    }

    public void Map(int source, global::GenMapper.IntIdentifier target)
    {
    }
}
}
