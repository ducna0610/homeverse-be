using Homeverse.Application.DTOs.Requests;
using Homeverse.Application.DTOs.Responses;
using Homeverse.IntegrationTest.Helpers;
using System.Net;
using System.Net.Http.Json;

namespace Homeverse.IntegrationTest;

[Collection("IntegrationTests")]
public class CitiesControllerTests(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    public Task InitializeAsync() => Task.CompletedTask;
    public async Task DisposeAsync() => await factory.ResetDatabaseAsync();


    [Fact]
    public async Task Get_WhenSuccessfull_ShouldReturnDataWithStatusCode200OK()
    {
        // Arrange

        // Act
        var response = await factory.CreateClient().GetAsync("/api/v1/cities");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private async Task<CityResponse> CreateCityAsync()
    {
        var claimsProvider = TestClaimsProvider.WithAdminClaims();
        var request = new CityRequest()
        {
            Name = "Test"
        };
        var httpResponse = await factory.CreateClientWithTestAuth(claimsProvider).PostAsJsonAsync("api/v1/cities", request);

        return await httpResponse.Content.ReadFromJsonAsync<CityResponse>();
    }
}
