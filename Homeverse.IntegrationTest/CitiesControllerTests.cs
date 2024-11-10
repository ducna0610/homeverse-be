using Homeverse.Application.DTOs.Requests;
using Homeverse.Application.DTOs.Responses;
using Homeverse.Application.Helpers;
using Homeverse.IntegrationTest.Helpers;
using MimeKit.Utils;
using System.Net;
using System.Net.Http.Json;

namespace Homeverse.IntegrationTest;

[Collection(nameof(SharedTestCollection))]
public class CitiesControllerTests(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    private int cityId;
    public async Task InitializeAsync()
    {
        var claimsProvider = TestClaimsProvider.WithAdminClaims();
        var cityRequest = new CityRequest()
        {
            Name = Utils.GenerateRandomString(10),
        };

        var response = await factory.CreateClientWithTestAuth(claimsProvider).PostAsJsonAsync("api/v1/cities", cityRequest);
        var city = await response.Content.ReadFromJsonAsync<CityResponse>();
        cityId = city.Id;
    }

    public async Task DisposeAsync() => await factory.ResetDatabaseAsync();


    [Fact]
    public async Task Get_WhenThereAreCities_ShouldReturnDataWithStatusCode200OK()
    {
        // Arrange

        // Act
        var response = await factory.CreateClient().GetAsync("/api/v1/cities");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetById_WhenUnauthentication_ShouldReturnWithStatusCode401Unauthorized()
    {
        // Arrange

        // Act
        var response = await factory.CreateClient().GetAsync("/api/v1/cities/12345");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetById_WhenLoginAsLandlord_ShouldReturnWithStatusCode403Forbidden()
    {
        // Arrange
        var claimsProvider = TestClaimsProvider.WithLandlordClaims();

        // Act
        var response = await factory.CreateClientWithTestAuth(claimsProvider).GetAsync("/api/v1/cities/12345");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetById_WhenLoginAsAdminAndThereIsCity_ShouldReturnDataWithStatusCode200OK()
    {
        // Arrange
        var claimsProvider = TestClaimsProvider.WithAdminClaims();

        // Act
        var response = await factory.CreateClientWithTestAuth(claimsProvider).GetAsync($"/api/v1/cities/{cityId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetById_WhenLoginAsAdminAndThereIsNoCityFound_ShouldReturnStatusCode404NotFound()
    {
        // Arrange
        var claimsProvider = TestClaimsProvider.WithAdminClaims();

        // Act
        var response = await factory.CreateClientWithTestAuth(claimsProvider).GetAsync("/api/v1/cities/12345");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Add_WhenUnauthentication_ShouldReturnStatusCode401Unauthorized()
    {
        // Arrange
        var request = new CityRequest();

        // Act
        var response = await factory.CreateClient().PostAsJsonAsync("api/v1/cities", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Add_WhenLoginAsLandlord_ShouldReturnStatusCode403Forbidden()
    {
        // Arrange
        var claimsProvider = TestClaimsProvider.WithLandlordClaims();
        var request = new CityRequest();

        // Act
        var response = await factory.CreateClientWithTestAuth(claimsProvider).PostAsJsonAsync("api/v1/cities", request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Add_WhenLoginAsAdminAndInvalidRequest_ShouldReturnStatusCode400BadRequest()
    {
        // Arrange
        var claimsProvider = TestClaimsProvider.WithAdminClaims();
        var request = new CityRequest();

        // Act
        var response = await factory.CreateClientWithTestAuth(claimsProvider).PostAsJsonAsync("api/v1/cities", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Add_WhenLoginAsAdminAndSuceessful_ShouldReturnStatusCode201Created()
    {
        // Arrange
        var claimsProvider = TestClaimsProvider.WithAdminClaims();
        var request = new CityRequest()
        {
            Name = "Test1"
        };

        // Act
        var response = await factory.CreateClientWithTestAuth(claimsProvider).PostAsJsonAsync("api/v1/cities", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Update_WhenUnauthentication_ShouldReturnStatusCode401Unauthorized()
    {
        // Arrange
        var request = new CityRequest();

        // Act
        var response = await factory.CreateClient().PutAsJsonAsync("api/v1/cities/12345", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Update_WhenLoginAsLandlord_ShouldReturnStatusCode403Forbidden()
    {
        // Arrange
        var claimsProvider = TestClaimsProvider.WithLandlordClaims();
        var request = new CityRequest();

        // Act
        var response = await factory.CreateClientWithTestAuth(claimsProvider).PutAsJsonAsync("api/v1/cities/12345", request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Update_WhenLoginAsAdminAndInvalidRequest_ShouldReturnStatusCode400BadRequest()
    {
        // Arrange
        var claimsProvider = TestClaimsProvider.WithAdminClaims();
        var request = new CityRequest();

        // Act
        var response = await factory.CreateClientWithTestAuth(claimsProvider).PutAsJsonAsync("api/v1/cities/12345", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Update_WhenLoginAsAdminAndSuceessful_ShouldReturnStatusCode200OK()
    {
        // Arrange
        var claimsProvider = TestClaimsProvider.WithAdminClaims();
        var request = new CityRequest()
        {
            Name = "Test2"
        };

        // Act
        var response = await factory.CreateClientWithTestAuth(claimsProvider).PutAsJsonAsync($"api/v1/cities/{cityId}", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Delete_WhenUnauthentication_ShouldReturnStatusCode401Unauthorized()
    {
        // Arrange

        // Act
        var response = await factory.CreateClient().DeleteAsync("api/v1/cities/12345");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Delete_WhenLoginAsAdminAndContactNotFound_ShouldReturnStatusCode404NotFound()
    {
        // Arrange
        var claimsProvider = TestClaimsProvider.WithAdminClaims();

        // Act
        var response = await factory.CreateClientWithTestAuth(claimsProvider).DeleteAsync("api/v1/contacts/12345");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_WhenLoginAsAdminAndSuccessful_ShouldReturnStatusCode204NoContent()
    {
        // Arrange
        var claimsProvider = TestClaimsProvider.WithAdminClaims();

        // Act
        var response = await factory.CreateClientWithTestAuth(claimsProvider).DeleteAsync("api/v1/cities/1");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_WhenLoginAsLandlord_ShouldReturnStatusCode403Forbidden()
    {
        // Arrange
        var claimsProvider = TestClaimsProvider.WithLandlordClaims();

        // Act
        var response = await factory.CreateClientWithTestAuth(claimsProvider).DeleteAsync("api/v1/cities/12345");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
