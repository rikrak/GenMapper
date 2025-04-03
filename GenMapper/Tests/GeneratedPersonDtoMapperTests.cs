using System.Drawing.Text;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GenMapper.Tests;

[TestClass]
public class GeneratedPersonDtoMapperTests
{
    private PersonDtoToPersonViewModelMapperGen _target = null!;

    [TestInitialize]
    public void Setup()
    {
        this._target = new PersonDtoToPersonViewModelMapperGen();
    }

    [TestMethod]
    public void ShouldConvert()
    {
        // arrange
        var source = new PersonDto()
        {
            Id = 456,
            Name = "James Hetfield"
        };

        // act
        var actual = this._target.Convert(source);

        // assert
        actual.Should().NotBeNull();
        actual.Should().BeEquivalentTo(source);
    }

}