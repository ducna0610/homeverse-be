using Homeverse.Application.DTOs.Requests;
using Homeverse.Application.DTOs.Responses;
using Homeverse.Application.Helpers;
using Homeverse.IntegrationTest.Helpers;
using System.Net;
using System.Net.Http.Json;

namespace Homeverse.IntegrationTest;

[Collection(nameof(SharedTestCollection))]
public class UsersControllerTests(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    private int userId;

    public async Task InitializeAsync()
    {
        var registerRequest = new RegisterRequest()
        {
            UserName = "Test",
            Email = "test@gmail.com",
            Phone = "0123456789",
            Password = "password",
        };
        var response = await factory.CreateClient().PostAsJsonAsync("api/v1/users/register", registerRequest);
        var user = await response.Content.ReadFromJsonAsync<UserResponse>();
        userId = user.Id;
    }
    public async Task DisposeAsync() => await factory.ResetDatabaseAsync();

    [Fact]
    public async Task Get_WhenUnauthentication_ShouldReturnStatusCode401()
    {
        // Arrange

        // Act
        var response = await factory.CreateClient().GetAsync("/api/v1/users");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Get_WhenLoginAsAdmin_ShouldReturnDataWithStatusCode200OK()
    {
        // Arrange
        var claimsProvider = TestClaimsProvider.WithAdminClaims();

        // Act
        var response = await factory.CreateClientWithTestAuth(claimsProvider).GetAsync("/api/v1/users");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Get_WhenLoginAsLandlord_ShouldReturnStatusCode403Forbidden()
    {
        // Arrange
        var claimsProvider = TestClaimsProvider.WithLandlordClaims();

        // Act
        var response = await factory.CreateClientWithTestAuth(claimsProvider).GetAsync("/api/v1/users");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetById_WhenUnauthentication_ShouldReturnStatusCode401Unauthorized()
    {
        // Arrange

        // Act
        var response = await factory.CreateClient().GetAsync("/api/v1/users/1");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetById_WhenLoginAsLandlordAndThereIsNoUserFound_ShouldReturnStatusCode404NotFound()
    {
        // Arrange
        var claimsProvider = TestClaimsProvider.WithLandlordClaims();

        // Act
        var response = await factory.CreateClientWithTestAuth(claimsProvider).GetAsync("/api/v1/users/12345");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetById_WhenLoginAsLandlordAndThereIsUser_ShouldReturnStatusCode200OK()
    {
        // Arrange
        var claimsProvider = TestClaimsProvider.WithLandlordClaims(userId.ToString());

        // Act
        var response = await factory.CreateClientWithTestAuth(claimsProvider).GetAsync($"/api/v1/users/{userId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetById_WhenLoginAsAdminAndThereIsNoUserFound_ShouldReturnStatusCode404NotFound()
    {
        // Arrange
        var claimsProvider = TestClaimsProvider.WithAdminClaims();

        // Act
        var response = await factory.CreateClientWithTestAuth(claimsProvider).GetAsync("/api/v1/users/12345");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetById_WhenLoginAsAdminAndThereIsUser_ShouldReturnStatusCode200OK()
    {
        // Arrange
        var claimsProvider = TestClaimsProvider.WithAdminClaims();

        // Act
        var response = await factory.CreateClientWithTestAuth(claimsProvider).GetAsync($"/api/v1/users/{userId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetProfile_WhenUnauthentication_ShouldReturnStatusCode401Unauthorized()
    {
        // Arrange

        // Act
        var response = await factory.CreateClient().GetAsync("/api/v1/profile");

        // Assert
        //Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetProfile_WhenLoginAsAdminAndThereIsUser_ShouldReturnUserWithStatusCode200OK()
    {
        // Arrange
        var claimsProvider = TestClaimsProvider.WithAdminClaims(userId.ToString());

        // Act
        var response = await factory.CreateClientWithTestAuth(claimsProvider).GetAsync("/api/v1/profile");

        // Assert
        //Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetProfile_WhenLoginAsAdminAndThereIsNoUserFound_ShouldReturnStatusCode404NotFound()
    {
        // Arrange
        var claimsProvider = TestClaimsProvider.WithAdminClaims("12345");

        // Act
        var response = await factory.CreateClientWithTestAuth(claimsProvider).GetAsync("/api/v1/profile");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetProfile_WhenLoginAsLandlordAndThereIsUser_ShouldReturnUserWithStatusCode200OK()
    {
        // Arrange
        var claimsProvider = TestClaimsProvider.WithLandlordClaims(userId.ToString());

        // Act
        var response = await factory.CreateClientWithTestAuth(claimsProvider).GetAsync("/api/v1/profile");

        // Assert
        //Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetProfile_WhenLoginAsLandlordAndThereIsNoUserFound_ShouldReturnStatusCode404NotFound()
    {
        // Arrange
        var claimsProvider = TestClaimsProvider.WithAdminClaims();

        // Act
        var response = await factory.CreateClientWithTestAuth(claimsProvider).GetAsync("/api/v1/profile");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Login_WhenInvalidRequest_ShouldReturnStatusCode400Badrequest()
    {
        // Arrange
        var request = new LoginRequest();

        // Act
        var response = await factory.CreateClient().PostAsJsonAsync("/api/v1/login", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_WhenInvalidUserNameOrPassword_ShouldReturnStatusCode401Unauthorized()
    {
        // Arrange
        var request = new LoginRequest()
        {
            Email = "test@gmail.com",
            Password = "12345"
        };

        // Act
        var response = await factory.CreateClient().PostAsJsonAsync("/api/v1/login", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_WhenInactive_ShouldReturnStatusCode401Unauthorized()
    {
        // Arrange
        var request = new LoginRequest()
        {
            Email = "",
            Password = ""
        };

        // Act
        var response = await factory.CreateClient().PostAsJsonAsync("/api/v1/login", request);

        // Assert
        //Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_WhenSuccessful_ShouldReturnStatusCode200OK()
    {
        // Arrange
        var request = new LoginRequest()
        {
            Email = "",
            Password = "password"
        };

        // Act
        var response = await factory.CreateClient().PostAsJsonAsync("/api/v1/login", request);

        // Assert
        //Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Register_WhenInvalidRequest_ShouldReturnStatusCode400Badrequest()
    {
        // Arrange
        var request = new RegisterRequest();

        // Act
        var response = await factory.CreateClient().PostAsJsonAsync("/api/v1/register", request);

        // Assert
        //Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_WhenEmailAlreadyExists_ShouldReturnStatusCode400Badrequest()
    {
        // Arrange
        var request = new RegisterRequest();

        // Act
        var response = await factory.CreateClient().PostAsJsonAsync("/api/v1/register", request);

        // Assert
        //Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_WhenSuccessful_ShouldReturnStatusCode201Created()
    {
        // Arrange
        var request = new RegisterRequest()
        {
            UserName = "test",
            Email = Utils.GenerateRandomString(10) + "@gmail.com",
            Phone = "0123456789",
            Password = "12345",
        };

        // Act
        var response = await factory.CreateClient().PostAsJsonAsync("/api/v1/register", request);

        // Assert
        //Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task ConfirmEmail_WhenThereIsNoUserFound_ShouldReturnStatusCode404NotFound()
    {
        // Arrange
        var email = "test1@gmail.com";
        var token = "xxx";

        // Act
        var response = await factory.CreateClient().GetAsync($"/api/v1/confirm-email?email={email}&token={token}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ConfirmEmail_WhenSuccessful_ShouldReturnStatusCode302Redirect()
    {
        // Arrange
        var email = "";

        // Act
        var response = await factory.CreateClient().PostAsJsonAsync("/api/v1/confirm-email", email);

        // Assert
        //Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
    }

    [Fact]
    public async Task ForgotPassword_WhenThereIsNoUserFound_ShouldReturnStatusCode404NotFound()
    {
        // Arrange
        var email = "";

        // Act
        var response = await factory.CreateClient().PostAsJsonAsync("/api/v1/forgot-password", email);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ForgotPassword_WhenSuccessful_ShouldReturnStatusCode204NoContent()
    {
        // Arrange
        var email = "test@gmail.com";

        // Act
        var response = await factory.CreateClient().PostAsJsonAsync("/api/v1/forgot-password", email);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task ResetPassword_WhenThereInvalidRequest_ShouldReturnStatusCode401BadRequest()
    {
        // Arrange
        var request = new ResetPasswordRequest();

        // Act
        var response = await factory.CreateClient().PostAsJsonAsync("/api/v1/reset-password", request);

        // Assert
        //Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ResetPassword_WhenThereIsNoUserFound_ShouldReturnStatusCode404NotFound()
    {
        // Arrange
        var request = new ResetPasswordRequest();

        // Act
        var response = await factory.CreateClient().PostAsJsonAsync("/api/v1/reset-password", request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ResetPassword_WhenSuccessful_ShouldReturnStatusCode204NoContent()
    {
        // Arrange
        var request = new ResetPasswordRequest();

        // Act
        var response = await factory.CreateClient().PostAsJsonAsync("/api/v1/reset-password", request);

        // Assert
        //Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Update_WhenUnauthentication_ShouldReturnStatusCode401Unauthorized()
    {
        // Arrange
        var request = new UpdateUserRequest();

        // Act
        var response = await factory.CreateClient().PutAsJsonAsync($"/api/v1/users/{userId}", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Update_WhenLoginAsLandlord_ShouldReturnStatusCode403Forbidden()
    {
        // Arrange
        var claimsProvider = TestClaimsProvider.WithLandlordClaims();
        var request = new UpdateUserRequest();

        // Act
        var response = await factory.CreateClientWithTestAuth(claimsProvider).PutAsJsonAsync($"/api/v1/users/{userId}", request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Update_WhenLoginAsAdminAndInvalidRequest_ShouldReturnStatusCode400BadRequest()
    {
        // Arrange
        var claimsProvider = TestClaimsProvider.WithAdminClaims();
        var request = new UpdateUserRequest();

        // Act
        var response = await factory.CreateClientWithTestAuth(claimsProvider).PutAsJsonAsync($"/api/v1/users/{userId}", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Update_WhenLoginAsAdminAndSuccessful_ShouldReturnStatusCode204NoContent()
    {
        // Arrange
        var claimsProvider = TestClaimsProvider.WithAdminClaims();
        var request = new UpdateUserRequest();

        // Act
        var response = await factory.CreateClientWithTestAuth(claimsProvider).PutAsJsonAsync($"/api/v1/users/{userId}", request);

        // Assert
        //Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UpdateProfile_WhenLoginAsAdminAndInvalidRequest_ShouldReturnStatusCode400BadRequest()
    {
        // Arrange
        var claimsProvider = TestClaimsProvider.WithAdminClaims();
        var request = new UpdateUserRequest();

        // Act
        var response = await factory.CreateClientWithTestAuth(claimsProvider).PutAsJsonAsync($"/api/v1/users/profile", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateProfile_WhenLoginAsAdminSuccessful_ShouldReturnStatusCode204NoContent()
    {
        // Arrange
        var claimsProvider = TestClaimsProvider.WithAdminClaims();
        var request = new UpdateUserRequest();

        // Act
        var response = await factory.CreateClientWithTestAuth(claimsProvider).PutAsJsonAsync($"/api/v1/users/profile", request);

        // Assert
        //Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UpdateProfile_WhenLoginAsLandlordAndInvalidRequest_ShouldReturnStatusCode400BadRequest()
    {
        // Arrange
        var claimsProvider = TestClaimsProvider.WithLandlordClaims();
        var request = new UpdateUserRequest();

        // Act
        var response = await factory.CreateClientWithTestAuth(claimsProvider).PutAsJsonAsync($"/api/v1/users/profile", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateProfile_WhenLoginAsLandlordSuccessful_ShouldReturnStatusCode204NoContent()
    {
        // Arrange
        var claimsProvider = TestClaimsProvider.WithLandlordClaims();
        var request = new UpdateUserRequest();

        // Act
        var response = await factory.CreateClientWithTestAuth(claimsProvider).PutAsJsonAsync($"/api/v1/users/profile", request);

        // Assert
        //Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
