using Homeverse.Application.DTOs.Requests;
using Homeverse.Application.DTOs.Responses;
using Homeverse.Application.Helpers;
using Homeverse.Domain.Enums;
using Homeverse.IntegrationTest.Helpers;
using System.Net;
using System.Net.Http.Json;

namespace Homeverse.IntegrationTest;

[Collection(nameof(SharedTestCollection))]
public class PropertiesControllerTests(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    private int userId;
    private int cityId;
    private int propertyId;
    public async Task InitializeAsync()
    {
        var registerRequest = new RegisterRequest()
        {
            UserName = "Test",
            Email = Utils.GenerateRandomString(10) + "@gmail.com",
            Phone = "0123456789",
            Password = "password",
        };
        var response = await factory.CreateClient().PostAsJsonAsync("api/v1/users/register", registerRequest);
        var user = await response.Content.ReadFromJsonAsync<UserResponse>();
        userId = user.Id;

        var claimsProvider = TestClaimsProvider.WithAdminClaims(userId.ToString());
        var cityRequest = new CityRequest()
        {
            Name = Utils.GenerateRandomString(10)
        };
        response = await factory.CreateClientWithTestAuth(claimsProvider).PostAsJsonAsync("api/v1/cities", cityRequest);
        var city = await response.Content.ReadFromJsonAsync<CityResponse>();
        cityId = city.Id;

        var client = factory.CreateClientWithTestAuth(claimsProvider);
        var propertyRequest = new MultipartFormDataContent();
        propertyRequest.Add(new StringContent("Title"), "Title");
        propertyRequest.Add(new StringContent("Address"), "Address");
        propertyRequest.Add(new StringContent("Description"), "Description");
        propertyRequest.Add(new StringContent("20"), "Area");
        propertyRequest.Add(new StringContent((CategoryEnum.Rent).ToString()), "Category");
        propertyRequest.Add(new StringContent((FurnishEnum.Full).ToString()), "Furnish");
        propertyRequest.Add(new StringContent("0"), "Lat");
        propertyRequest.Add(new StringContent("0"), "Lng");
        propertyRequest.Add(new StringContent(cityId.ToString()), "CityId");

        response = await client.PostAsync("api/v1/properties", propertyRequest);
        var property = await response.Content.ReadFromJsonAsync<PropertyResponse>();
        propertyId = property.Id;
    }
    public new async Task DisposeAsync() => await factory.ResetDatabaseAsync();

    [Fact]
    public async Task GetAll_WhenUnauthentication_ShouldReturnStatusCode401()
    {
        // Arrange
        await DisposeAsync();

        // Act
        var response = await factory.CreateClient().GetAsync("/api/v1/properties");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_WhenLoginAsAdmin_ShouldReturnDataWithStatusCode200OK()
    {
        // Arrange
        var claimsProvider = TestClaimsProvider.WithAdminClaims();

        // Act
        var response = await factory.CreateClientWithTestAuth(claimsProvider).GetAsync("/api/v1/properties");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_WhenLoginAsLandlord_ShouldReturnStatusCode403Forbidden()
    {
        // Arrange
        var claimsProvider = TestClaimsProvider.WithLandlordClaims();

        // Act
        var response = await factory.CreateClientWithTestAuth(claimsProvider).GetAsync("/api/v1/properties");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetForUser_WhenUnauthentication_ShouldReturnStatusCode401()
    {
        // Arrange

        // Act
        var response = await factory.CreateClient().GetAsync("/api/v1/properties/user");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetForUser_WhenLoginAsAdmin_ShouldReturnDataWithStatusCode200OK()
    {
        // Arrange
        var claimsProvider = TestClaimsProvider.WithAdminClaims();

        // Act
        var response = await factory.CreateClientWithTestAuth(claimsProvider).GetAsync("/api/v1/properties/user");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetForUser_WhenLoginAsLandlord_ShouldReturnStatusCode200OK()
    {
        // Arrange
        var claimsProvider = TestClaimsProvider.WithLandlordClaims();

        // Act
        var response = await factory.CreateClientWithTestAuth(claimsProvider).GetAsync("/api/v1/properties/user");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetActive_WhenThereAreProperties_ShouldReturnStatusCode200OK()
    {
        // Arrange

        // Act
        var response = await factory.CreateClient().GetAsync("/api/v1/properties/list");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetById_WhenThereIsProperty_ShouldReturnStatusCode200OK()
    {
        // Arrange

        // Act
        var response = await factory.CreateClient().GetAsync($"/api/v1/properties/detail/{propertyId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetById_WhenThereIsNoPropertyFound_ShouldReturnStatusCode404NotFound()
    {
        // Arrange

        // Act
        var response = await factory.CreateClient().GetAsync("/api/v1/properties/detail/12345");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Add_WhenUnauthentication_ShouldReturnStatusCode401Unauthorized()
    {
        // Arrange
        var request = new MultipartFormDataContent();

        // Act
        var response = await factory.CreateClient().PostAsync("api/v1/properties", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Add_WhenLoginAsLandlordAndInvalidRequest_ShouldReturnStatusCode400BadRequest()
    {
        // Arrange
        var claimsProvider = TestClaimsProvider.WithLandlordClaims();
        var request = new MultipartFormDataContent();

        // Act
        var response = await factory.CreateClientWithTestAuth(claimsProvider).PostAsync("api/v1/properties", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Add_WhenLoginAsLandlordAndSuceessful_ShouldReturnStatusCode201Created()
    {
        // Arrange
        var claimsProvider = TestClaimsProvider.WithLandlordClaims(userId.ToString());
        var request = new MultipartFormDataContent();
        request.Add(new StringContent("Title"), "Title");
        request.Add(new StringContent("Address"), "Address");
        request.Add(new StringContent("Description"), "Description");
        request.Add(new StringContent("20"), "Area");
        request.Add(new StringContent((CategoryEnum.Rent).ToString()), "Category");
        request.Add(new StringContent((FurnishEnum.Full).ToString()), "Furnish");
        request.Add(new StringContent("0"), "Lat");
        request.Add(new StringContent("0"), "Lng");
        request.Add(new StringContent(cityId.ToString()), "CityId");

        // Act
        var response = await factory.CreateClientWithTestAuth(claimsProvider).PostAsync("api/v1/properties", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }


    [Fact]
    public async Task Add_WhenLoginAsAdminAndInvalidRequest_ShouldReturnStatusCode400BadRequest()
    {
        // Arrange
        var claimsProvider = TestClaimsProvider.WithAdminClaims();
        var request = new MultipartFormDataContent();

        // Act
        var response = await factory.CreateClientWithTestAuth(claimsProvider).PostAsync("api/v1/properties", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Add_WhenLoginAsAdminAndSuceessful_ShouldReturnStatusCode201Created()
    {
        // Arrange
        var claimsProvider = TestClaimsProvider.WithAdminClaims(userId.ToString());
        var request = new MultipartFormDataContent();
        request.Add(new StringContent("Title"), "Title");
        request.Add(new StringContent("Address"), "Address");
        request.Add(new StringContent("Description"), "Description");
        request.Add(new StringContent("20"), "Area");
        request.Add(new StringContent((CategoryEnum.Rent).ToString()), "Category");
        request.Add(new StringContent((FurnishEnum.Full).ToString()), "Furnish");
        request.Add(new StringContent("0"), "Lat");
        request.Add(new StringContent("0"), "Lng");
        request.Add(new StringContent(cityId.ToString()), "CityId");

        // Act
        var response = await factory.CreateClientWithTestAuth(claimsProvider).PostAsync("api/v1/properties", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Update_WhenUnauthentication_ShouldReturnStatusCode401Unauthorized()
    {
        // Arrange
        var request = new MultipartFormDataContent();

        // Act
        var response = await factory.CreateClient().PutAsync("api/v1/properties/12345", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Update_WhenLoginAsLandlordAndInvalidRequest_ShouldReturnStatusCode400BadRequest()
    {
        // Arrange
        var claimsProvider = TestClaimsProvider.WithLandlordClaims();
        var request = new MultipartFormDataContent();

        // Act
        var response = await factory.CreateClientWithTestAuth(claimsProvider).PutAsync("api/v1/properties/12345", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Update_WhenLoginAsLandlordAndSuceessful_ShouldReturnStatusCode200OK()
    {
        // Arrange
        var claimsProvider = TestClaimsProvider.WithLandlordClaims();
        var request = new MultipartFormDataContent();
        request.Add(new StringContent("Title"), "Title");
        request.Add(new StringContent("Address"), "Address");
        request.Add(new StringContent("Description"), "Description");
        request.Add(new StringContent("20"), "Area");
        request.Add(new StringContent((CategoryEnum.Rent).ToString()), "Category");
        request.Add(new StringContent((FurnishEnum.Full).ToString()), "Furnish");
        request.Add(new StringContent("0"), "Lat");
        request.Add(new StringContent("0"), "Lng");
        request.Add(new StringContent(cityId.ToString()), "CityId");

        // Act
        var response = await factory.CreateClientWithTestAuth(claimsProvider).PutAsJsonAsync($"api/v1/properties/{propertyId}", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }


    [Fact]
    public async Task Update_WhenLoginAsAdminAndInvalidRequest_ShouldReturnStatusCode400BadRequest()
    {
        // Arrange
        var claimsProvider = TestClaimsProvider.WithAdminClaims();
        var request = new MultipartFormDataContent();

        // Act
        var response = await factory.CreateClientWithTestAuth(claimsProvider).PutAsync("api/v1/properties/12345", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Update_WhenLoginAsAdminAndSuceessful_ShouldReturnStatusCode200OK()
    {
        // Arrange
        var claimsProvider = TestClaimsProvider.WithAdminClaims();
        var request = new MultipartFormDataContent();
        request.Add(new StringContent("Title"), "Title");
        request.Add(new StringContent("Address"), "Address");
        request.Add(new StringContent("Description"), "Description");
        request.Add(new StringContent("20"), "Area");
        request.Add(new StringContent((CategoryEnum.Rent).ToString()), "Category");
        request.Add(new StringContent((FurnishEnum.Full).ToString()), "Furnish");
        request.Add(new StringContent("0"), "Lat");
        request.Add(new StringContent("0"), "Lng");
        request.Add(new StringContent(cityId.ToString()), "CityId");

        // Act
        var response = await factory.CreateClientWithTestAuth(claimsProvider).PutAsync($"api/v1/properties/{propertyId}", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Delete_WhenLoginAsAdminAndSuccessful_ShouldReturnStatusCode204NoContent()
    {
        // Arrange
        var claimsProvider = TestClaimsProvider.WithAdminClaims();

        // Act
        var response = await factory.CreateClientWithTestAuth(claimsProvider).DeleteAsync($"api/v1/properties/{propertyId}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        InitializeAsync();
    }

    [Fact]
    public async Task Delete_WhenLoginAsLandlord_ShouldReturnStatusCode204NoContent()
    {
        // Arrange
        var claimsProvider = TestClaimsProvider.WithLandlordClaims();

        // Act
        var response = await factory.CreateClientWithTestAuth(claimsProvider).DeleteAsync($"api/v1/properties/{propertyId}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        InitializeAsync();
    }

    [Fact]
    public async Task Delete_WhenUnauthentication_ShouldReturnStatusCode401Unauthorized()
    {
        // Arrange

        // Act
        var response = await factory.CreateClient().DeleteAsync("api/v1/contacts/12345");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
