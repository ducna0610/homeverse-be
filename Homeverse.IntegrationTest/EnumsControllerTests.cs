using Homeverse.IntegrationTest.Helpers;
using System.Net;

namespace Homeverse.IntegrationTest;

[Collection(nameof(SharedTestCollection))]
public class EnumsControllerTests(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    public Task InitializeAsync() => Task.CompletedTask;
    public async Task DisposeAsync() => await factory.ResetDatabaseAsync();

    [Fact]
    public async Task GetCategoryEnum_WhenThereAreCategories_ShouldReturnDataWithStatusCode200OK()
    {
        // Arrange

        // Act
        var response = await factory.CreateClient().GetAsync("/api/v1/enums/category");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetFurnishEnum_WhenThereAreFurnishes_ShouldReturnDataWithStatusCode200OK()
    {
        // Arrange

        // Act
        var response = await factory.CreateClient().GetAsync("/api/v1/enums/furnish");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
