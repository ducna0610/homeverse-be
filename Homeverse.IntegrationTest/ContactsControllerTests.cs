using Homeverse.Application.DTOs.Requests;
using Homeverse.IntegrationTest.Helpers;
using System.Net;
using System.Net.Http.Json;

namespace Homeverse.IntegrationTest;

[Collection(nameof(SharedTestCollection))]
public class ContactsControllerTests(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    public async Task InitializeAsync()
    {
        var claimsProvider = TestClaimsProvider.WithAdminClaims();
        var contactRequest = new ContactRequest()
        {
            Email = "test@gmail.com",
            Message = "test",
            Name = "Test",
            Phone = "0123456789"
        };
        await factory.CreateClientWithTestAuth(claimsProvider).PostAsJsonAsync("api/v1/contacts", contactRequest);
    }
    public async Task DisposeAsync() => await factory.ResetDatabaseAsync();

    [Fact]
    public async Task Get_WhenUnauthentication_ShouldReturnStatusCode401()
    {
        // Arrange

        // Act
        var response = await factory.CreateClient().GetAsync("/api/v1/contacts");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Get_WhenLoginAsAdmin_ShouldReturnDataWithStatusCode200OK()
    {
        // Arrange
        var claimsProvider = TestClaimsProvider.WithAdminClaims();

        // Act
        var response = await factory.CreateClientWithTestAuth(claimsProvider).GetAsync("/api/v1/contacts");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Get_WhenLoginAsLandlord_ShouldReturnStatusCode403Forbidden()
    {
        // Arrange
        var claimsProvider = TestClaimsProvider.WithLandlordClaims();

        // Act
        var response = await factory.CreateClientWithTestAuth(claimsProvider).GetAsync("/api/v1/contacts");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetById_WhenThereIsNoContactFound_ShouldReturnStatusCode404NotFound()
    {
        // Arrange

        // Act
        var response = await factory.CreateClient().GetAsync("/api/v1/contacts/12345");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetById_WhenThereIsContact_ShouldReturnStatusCode200OK()
    {
        // Arrange

        // Act
        var response = await factory.CreateClient().GetAsync("/api/v1/contacts/1");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Add_WhenInvalidRequest_ShouldReturnStatusCode400BadRequest()
    {
        // Arrange
        var request = new ContactRequest();

        // Act
        var response = await factory.CreateClient().PostAsJsonAsync("api/v1/contacts", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Add_WhenSuceessful_ShouldReturnStatusCode201Created()
    {
        // Arrange
        var request = new ContactRequest()
        {
            Email = "test@gmail.com",
            Message = "test",
            Name = "Test",
            Phone = "0123456789"
        };

        // Act
        var response = await factory.CreateClient().PostAsJsonAsync("api/v1/contacts", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
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
        var response = await factory.CreateClientWithTestAuth(claimsProvider).DeleteAsync("api/v1/contacts/1");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_WhenLoginAsLandlord_ShouldReturnStatusCode403Forbidden()
    {
        // Arrange
        var claimsProvider = TestClaimsProvider.WithLandlordClaims();

        // Act
        var response = await factory.CreateClientWithTestAuth(claimsProvider).DeleteAsync("api/v1/contacts/12345");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
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
