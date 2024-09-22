using AutoFixture;
using Homeverse.Application.Services;

namespace Homeverse.UnitTest.Services;

public class EnumServiceTests
{
    private readonly Fixture _fixture;
    private readonly IEnumService _sut;

    public EnumServiceTests()
    {
        _fixture = new Fixture();
        _sut = new EnumService();
    }

    [Fact]
    public async Task GetCaegoryEnum_WhenSuccessful_ShouldReturnCategories()
    {
        // Arrange
        var response = _fixture.Create<IEnumerable<KeyValuePair<int, string>>>().ToList();

        // Act
        var actual = _sut.GetCaegoryEnum();

        // Assert
        Assert.IsAssignableFrom<IEnumerable<KeyValuePair<int, string>>>(actual);
    }

    [Fact]
    public async Task GetFurnishEnum_WhenSuccessful_ShouldReturnFurnishes()
    {
        // Arrange
        var response = _fixture.Create<IEnumerable<KeyValuePair<int, string>>>().ToList();

        // Act
        var actual = _sut.GetFurnishEnum();

        // Assert
        Assert.IsAssignableFrom<IEnumerable<KeyValuePair<int, string>>>(actual);
    }
}
