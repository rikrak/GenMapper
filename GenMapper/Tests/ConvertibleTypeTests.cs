using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GenMapper.Tests;

[TestClass]
public class StructWithBuildOverrideTests
{
    [TestMethod]
    public void ToInt()
    {
        // arrange
        var target = new IntIdentifierToIntMapperGen();
        const int value = 1536;
        var source = new IntIdentifier(value);

        // act
        var actual = target.Convert(source);

        // assert
        actual.Should().Be(value);

    }

    [TestMethod]
    public void ToStruct()
    {
        // arrange
        var target = new IntToIntIdentifierMapperGen();
        const int value = 1536;

        // act
        var actual = target.Convert(value);

        // assert
        actual.Should().Be((IntIdentifier)value);
    }
}