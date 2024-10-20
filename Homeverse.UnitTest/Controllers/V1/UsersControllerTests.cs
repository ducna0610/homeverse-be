using AutoFixture;
using FakeItEasy;
using Homeverse.API.Controllers.V1;
using Homeverse.Application.DTOs.Requests;
using Homeverse.Application.DTOs.Responses;
using Homeverse.Application.Interfaces;
using Homeverse.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Homeverse.UnitTest.Controllers.V1;

public class UsersControllerTests
{
    private readonly Fixture _fixture;
    private readonly ILogger<UsersController> _logger;
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;
    private readonly ICacheService _cacheService;
    private readonly UsersController _sut;

    public UsersControllerTests()
    {
        _fixture = new Fixture();
        _logger = A.Fake<ILogger<UsersController>>();
        _userService = A.Fake<IUserService>();
        _cacheService = A.Fake<ICacheService>();
        _configuration = A.Fake<IConfiguration>();
        _sut = new UsersController(_logger, _userService, _configuration, _cacheService);
    }

    [Fact]
    public async Task Get_WhenThereIsCacheData_ShouldReturnCitiesWithStatusCode200OK()
    {
        // Arrange
        var cacheData = _fixture.CreateMany<UserResponse>(3).ToList();
        A.CallTo(() => _cacheService.GetDataAsync<IEnumerable<UserResponse>>("users")).Returns(cacheData);

        // Act
        var actual = await _sut.Get();

        // Assert
        A.CallTo(() => _cacheService.GetDataAsync<IEnumerable<UserResponse>>("users")).MustHaveHappenedOnceExactly();
        var actionResult = Assert.IsType<OkObjectResult>(actual);
        var result = Assert.IsAssignableFrom<IEnumerable<UserResponse>>(actionResult.Value);
        Assert.Equal(cacheData.Count(), result.Count());
    }

    [Fact]
    public async Task Get_WhenThereAreUsers_ShouldReturnUsersWithStatusCode200OK()
    {
        // Arrange
        var cacheData = (IEnumerable<UserResponse>)null;
        var response = _fixture.CreateMany<UserResponse>(3).ToList();
        A.CallTo(() => _cacheService.GetDataAsync<IEnumerable<UserResponse>>("users")).Returns(cacheData);
        A.CallTo(() => _userService.GetUsersAsync()).Returns(response);

        // Act
        var actual = await _sut.Get();

        // Assert
        A.CallTo(() => _cacheService.GetDataAsync<IEnumerable<UserResponse>>("users")).MustHaveHappenedOnceExactly();
        A.CallTo(() => _userService.GetUsersAsync()).MustHaveHappenedOnceExactly();
        A.CallTo(() => _cacheService.SetDataAsync<IEnumerable<UserResponse>>("users", response)).MustHaveHappenedOnceExactly();
        var actionResult = Assert.IsType<OkObjectResult>(actual);
        var result = Assert.IsAssignableFrom<IEnumerable<UserResponse>>(actionResult.Value);
        Assert.Equal(response.Count(), result.Count());
    }

    [Fact]
    public async Task Get_WhenThereIsUnhandledException_ShouldReturnStatusCode500InternalServerErrorAndLogAnException()
    {
        // Arrange
        var cacheData = (IEnumerable<UserResponse>)null;
        A.CallTo(() => _cacheService.GetDataAsync<IEnumerable<UserResponse>>("users")).Returns(cacheData);
        A.CallTo(() => _userService.GetUsersAsync()).Throws<Exception>();

        // Act
        var actual = await _sut.Get() as StatusCodeResult;

        // Assert
        A.CallTo(() => _cacheService.GetDataAsync<IEnumerable<UserResponse>>("users")).MustHaveHappenedOnceExactly();
        A.CallTo(() => _userService.GetUsersAsync()).MustHaveHappenedOnceExactly();
        A.CallTo(_logger).Where(
                call => call.Method.Name == "Log"
                && call.GetArgument<LogLevel>(0) == LogLevel.Error)
            .MustHaveHappened(1, Times.Exactly);
        Assert.Equal(StatusCodes.Status500InternalServerError, actual.StatusCode);
    }

    [Fact]
    public async Task GetById_WhenThereIsUser_ShouldReturnUserWithStatusCode200OK()
    {
        // Arrange
        var cacheData = _fixture.CreateMany<UserResponse>(3).ToList();
        var id = _fixture.Create<int>();
        var response = _fixture.Create<UserResponse>();
        A.CallTo(() => _cacheService.GetDataAsync<IEnumerable<UserResponse>>("users")).Returns(cacheData);
        A.CallTo(() => _userService.GetUserByIdAsync(A<int>._)).Returns(response);

        // Act
        var actual = await _sut.GetById(id);

        // Assert
        A.CallTo(() => _cacheService.GetDataAsync<IEnumerable<UserResponse>>("users")).MustHaveHappenedOnceExactly();
        A.CallTo(() => _userService.GetUserByIdAsync(A<int>._)).MustHaveHappenedOnceExactly();
        var actionResult = Assert.IsType<OkObjectResult>(actual);
        Assert.IsType<UserResponse>(actionResult.Value);
    }

    [Fact]
    public async Task GetById_WhenThereIsNoUserFound_ShouldReturnStatusCode404NotFound()
    {
        // Arrange
        var cacheData = _fixture.CreateMany<UserResponse>(3).ToList();
        var id = _fixture.Create<int>();
        var response = (UserResponse)null;
        A.CallTo(() => _cacheService.GetDataAsync<IEnumerable<UserResponse>>("users")).Returns(cacheData);
        A.CallTo(() => _userService.GetUserByIdAsync(A<int>._)).Returns(response);

        // Act
        var actual = await _sut.GetById(id);

        // Assert
        A.CallTo(() => _cacheService.GetDataAsync<IEnumerable<UserResponse>>("users")).MustHaveHappenedOnceExactly();
        A.CallTo(() => _userService.GetUserByIdAsync(A<int>._)).MustHaveHappenedOnceExactly();
        Assert.IsType<NotFoundResult>(actual);
    }

    [Fact]
    public async Task GetById_WhenThereIsUnhandledException_ShouldReturnStatusCode500InternalServerErrorAndLogAnException()
    {
        // Arrange
        var cacheData = _fixture.CreateMany<UserResponse>(3).ToList();
        var id = _fixture.Create<int>();
        A.CallTo(() => _cacheService.GetDataAsync<IEnumerable<UserResponse>>("users")).Returns(cacheData);
        A.CallTo(() => _userService.GetUserByIdAsync(A<int>._)).Throws<Exception>();

        // Act
        var actual = await _sut.GetById(id) as StatusCodeResult;

        // Assert
        A.CallTo(() => _cacheService.GetDataAsync<IEnumerable<UserResponse>>("users")).MustHaveHappenedOnceExactly();
        A.CallTo(() => _userService.GetUserByIdAsync(A<int>._)).MustHaveHappenedOnceExactly();
        A.CallTo(_logger).Where(
                call => call.Method.Name == "Log"
                && call.GetArgument<LogLevel>(0) == LogLevel.Error)
            .MustHaveHappened(1, Times.Exactly);
        Assert.Equal(StatusCodes.Status500InternalServerError, actual.StatusCode);
    }

    [Fact]
    public async Task GetProfile_WhenThereIsUser_ShouldReturnUserWithStatusCode200OK()
    {
        // Arrange
        var response = _fixture.Create<UserResponse>();
        A.CallTo(() => _userService.GetProfileAsync()).Returns(response);

        // Act
        var actual = await _sut.GetProfile();

        // Assert
        A.CallTo(() => _userService.GetProfileAsync()).MustHaveHappenedOnceExactly();
        var actionResult = Assert.IsType<OkObjectResult>(actual);
        Assert.IsType<UserResponse>(actionResult.Value);
    }

    [Fact]
    public async Task GetProfile_WhenThereIsNoUserFound_ShouldReturnStatusCode404NotFound()
    {
        // Arrange
        var response = (UserResponse)null;
        A.CallTo(() => _userService.GetProfileAsync()).Returns(response);

        // Act
        var actual = await _sut.GetProfile();

        // Assert
        A.CallTo(() => _userService.GetProfileAsync()).MustHaveHappenedOnceExactly();
        Assert.IsType<NotFoundResult>(actual);
    }

    [Fact]
    public async Task GetProfile_WhenThereIsUnhandledException_ShouldReturnStatusCode500InternalServerErrorAndLogAnException()
    {
        // Arrange
        A.CallTo(() => _userService.GetProfileAsync()).Throws<Exception>();

        // Act
        var actual = await _sut.GetProfile() as StatusCodeResult;

        // Assert
        A.CallTo(() => _userService.GetProfileAsync()).MustHaveHappenedOnceExactly();
        A.CallTo(_logger).Where(
                call => call.Method.Name == "Log"
                && call.GetArgument<LogLevel>(0) == LogLevel.Error)
            .MustHaveHappened(1, Times.Exactly);
        Assert.Equal(StatusCodes.Status500InternalServerError, actual.StatusCode);
    }

    [Fact]
    public async Task Login_WhenSuccessful_ShouldReturnTokenWithStatusCode200OK()
    {
        // Arrange
        var request = _fixture.Create<LoginRequest>();
        var user = _fixture.Create<UserResponse>();
        var secretKey = A.Fake<IConfigurationSection>();
        A.CallTo(() => secretKey.Value).Returns(_fixture.Create<string>());
        var expiryDays = A.Fake<IConfigurationSection>();
        A.CallTo(() => expiryDays.Value).Returns("7");
        var issuer = A.Fake<IConfigurationSection>();
        A.CallTo(() => issuer.Value).Returns("Homeverse");
        var audience = A.Fake<IConfigurationSection>();
        A.CallTo(() => audience.Value).Returns("Homeverse");
        A.CallTo(() => _configuration.GetSection("JwtSettings:SecretKey")).Returns(secretKey);
        A.CallTo(() => _configuration.GetSection("JwtSettings:ExpiryDays")).Returns(expiryDays);
        A.CallTo(() => _configuration.GetSection("JwtSettings:Issuer")).Returns(issuer);
        A.CallTo(() => _configuration.GetSection("JwtSettings:Audience")).Returns(audience);
        A.CallTo(() => _userService.Login(A<LoginRequest>._)).Returns(user);

        // Act
        var actual = await _sut.Login(request);

        // Assert
        A.CallTo(() => _userService.Login(A<LoginRequest>._)).MustHaveHappenedOnceExactly();
        var actionResult = Assert.IsType<OkObjectResult>(actual);
        Assert.IsType<TokenResponse>(actionResult.Value);
    }

    [Fact]
    public async Task Login_WhenThereIsNoUserFound_ShouldReturnStatus401Unauthorized()
    {
        // Arrange
        var request = _fixture.Create<LoginRequest>();
        var user = new UserResponse();
        A.CallTo(() => _userService.Login(A<LoginRequest>._)).Returns(user);

        // Act
        var actual = await _sut.Login(request);

        // Assert
        A.CallTo(() => _userService.Login(A<LoginRequest>._)).MustHaveHappenedOnceExactly();
        Assert.IsType<UnauthorizedObjectResult>(actual);
    }

    [Fact]
    public async Task Login_WhenThereIsUnhandledException_ShouldReturnStatusCode500InternalServerErrorAndLogAnException()
    {
        // Arrange
        var request = _fixture.Create<LoginRequest>();
        A.CallTo(() => _userService.Login(A<LoginRequest>._)).Throws<Exception>();

        // Act
        var actual = await _sut.Login(request) as StatusCodeResult;

        // Assert
        A.CallTo(() => _userService.Login(A<LoginRequest>._)).MustHaveHappenedOnceExactly();
        A.CallTo(_logger).Where(
                call => call.Method.Name == "Log"
                && call.GetArgument<LogLevel>(0) == LogLevel.Error)
            .MustHaveHappened(1, Times.Exactly);
        Assert.Equal(StatusCodes.Status500InternalServerError, actual.StatusCode);
    }

    [Fact]
    public async Task Register_WhenSuccessful_ShouldReturnStatusCode201Created()
    {
        // Arrange
        var request = _fixture.Create<RegisterRequest>();
        var response = _fixture.Create<UserResponse>();
        A.CallTo(() => _userService.UserAlreadyExists(A<string>._)).Returns(false);
        A.CallTo(() => _userService.Register(A<RegisterRequest>._)).Returns(response);

        // Act
        var actual = await _sut.Register(request);

        // Assert
        A.CallTo(() => _cacheService.RemoveDataAsync("users")).MustHaveHappenedOnceExactly();
        A.CallTo(() => _userService.Register(A<RegisterRequest>._)).MustHaveHappenedOnceExactly();
        var actionResult = Assert.IsType<CreatedAtActionResult>(actual);
        Assert.IsType<UserResponse>(actionResult.Value);
    }

    [Fact]
    public async Task Register_WhenUserAlreadyExists_ShouldReturnStatusCode400BadRequest()
    {
        // Arrange
        var request = _fixture.Create<RegisterRequest>();
        A.CallTo(() => _userService.UserAlreadyExists(A<string>._)).Returns(true);

        // Act
        var actual = await _sut.Register(request);

        // Assert
        var actionResult = Assert.IsType<BadRequestObjectResult>(actual);
    }

    [Fact]
    public async Task Register_WhenThereIsUnhandledException_ShouldReturnStatusCode500InternalServerErrorAndLogAnException()
    {
        // Arrange
        var request = _fixture.Create<RegisterRequest>();
        A.CallTo(() => _userService.UserAlreadyExists(A<string>._)).Returns(false);
        A.CallTo(() => _userService.Register(A<RegisterRequest>._)).Throws<Exception>();

        // Act
        var actual = await _sut.Register(request) as StatusCodeResult;

        // Assert
        A.CallTo(() => _userService.Register(A<RegisterRequest>._)).MustHaveHappenedOnceExactly();
        A.CallTo(_logger).Where(
                call => call.Method.Name == "Log"
                && call.GetArgument<LogLevel>(0) == LogLevel.Error)
            .MustHaveHappened(1, Times.Exactly);
        Assert.Equal(StatusCodes.Status500InternalServerError, actual.StatusCode);
    }

    [Fact]
    public async Task Update_WhenSuccessful_ShouldReturnUserWithStatusCode200OK()
    {
        // Arrange
        var id = _fixture.Create<int>();
        var request = _fixture.Create<UpdateUserRequest>();
        var response = _fixture.Create<UserResponse>();
        A.CallTo(() => _userService.UpdateUserAsync(A<int>._, A<UpdateUserRequest>._)).Returns(response);

        // Act
        var actual = await _sut.Update(id, request);

        // Assert
        A.CallTo(() => _userService.UpdateUserAsync(A<int>._, A<UpdateUserRequest>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _cacheService.RemoveDataAsync("users")).MustHaveHappenedOnceExactly();
        var actionResult = Assert.IsType<OkObjectResult>(actual);
        Assert.IsType<UserResponse>(actionResult.Value);
    }

    [Fact]
    public async Task Update_WhenThereIsUnhandledException_ShouldReturnStatusCode500InternalServerErrorAndLogAnException()
    {
        // Arrange
        var id = _fixture.Create<int>();
        var request = _fixture.Create<UpdateUserRequest>();
        A.CallTo(() => _userService.UpdateUserAsync(A<int>._, A<UpdateUserRequest>._)).Throws<Exception>();

        // Act
        var actual = await _sut.Update(id, request) as StatusCodeResult;

        // Assert
        A.CallTo(() => _userService.UpdateUserAsync(A<int>._, A<UpdateUserRequest>._)).MustHaveHappenedOnceExactly();
        A.CallTo(_logger).Where(
                call => call.Method.Name == "Log"
                && call.GetArgument<LogLevel>(0) == LogLevel.Error)
            .MustHaveHappened(1, Times.Exactly);
        Assert.Equal(StatusCodes.Status500InternalServerError, actual.StatusCode);
    }

    [Fact]
    public async Task UpdateProfile_WhenSuccessful_ShouldReturnUserWithStatusCode200OK()
    {
        // Arrange
        var user = _fixture.Create<UserResponse>();
        var request = _fixture.Create<UpdateUserRequest>();
        var response = _fixture.Create<UserResponse>();
        A.CallTo(() => _userService.Login(A<LoginRequest>._)).Returns(user);
        A.CallTo(() => _userService.UpdateProfileAsync(A<UpdateUserRequest>._)).Returns(response);

        // Act
        var actual = await _sut.UpdateProfile(request);

        // Assert
        A.CallTo(() => _userService.UpdateProfileAsync(A<UpdateUserRequest>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _cacheService.RemoveDataAsync("users")).MustHaveHappenedOnceExactly();
        var actionResult = Assert.IsType<OkObjectResult>(actual);
        Assert.IsType<UserResponse>(actionResult.Value);
    }

    [Fact]
    public async Task UpdateProfile_WhenUnauthorized_ShouldReturnUserWithStatusCode401Unauthorized()
    {
        // Arrange
        var user = new UserResponse();
        var request = _fixture.Create<UpdateUserRequest>();
        A.CallTo(() => _userService.Login(A<LoginRequest>._)).Returns(user);

        // Act
        var actual = await _sut.UpdateProfile(request);

        // Assert
        var actionResult = Assert.IsType<UnauthorizedObjectResult>(actual);
    }

    [Fact]
    public async Task UpdateProfile_WhenThereIsUnhandledException_ShouldReturnStatusCode500InternalServerErrorAndLogAnException()
    {
        // Arrange
        var user = _fixture.Create<UserResponse>();
        var request = _fixture.Create<UpdateUserRequest>();
        A.CallTo(() => _userService.Login(A<LoginRequest>._)).Returns(user);
        A.CallTo(() => _userService.UpdateProfileAsync(A<UpdateUserRequest>._)).Throws<Exception>();

        // Act
        var actual = await _sut.UpdateProfile(request) as StatusCodeResult;

        // Assert
        A.CallTo(() => _userService.UpdateProfileAsync(A<UpdateUserRequest>._)).MustHaveHappenedOnceExactly();
        A.CallTo(_logger).Where(
                call => call.Method.Name == "Log"
                && call.GetArgument<LogLevel>(0) == LogLevel.Error)
            .MustHaveHappened(1, Times.Exactly);
        Assert.Equal(StatusCodes.Status500InternalServerError, actual.StatusCode);
    }

    [Fact]
    public async Task ConfirmEmail_WhenSuccessful_ShouldReturnRedirect()
    {
        // Arrange
        var email = _fixture.Create<string>();
        var token = _fixture.Create<string>();
        A.CallTo(() => _userService.UserAlreadyExists(A<string>._)).Returns(true);
        var frontendUrl = A.Fake<IConfigurationSection>();
        A.CallTo(() => frontendUrl.Value).Returns(_fixture.Create<string>());
        A.CallTo(() => _configuration.GetSection("UrlSettings:Frontend")).Returns(frontendUrl);

        // Act
        var actual = await _sut.ConfirmEmail(email, token);

        // Assert
        A.CallTo(() => _userService.UserAlreadyExists(A<string>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _userService.ConfirmEmail(A<string>._, A<string>._)).MustHaveHappenedOnceExactly();
        Assert.IsType<RedirectResult>(actual);
    }

    [Fact]
    public async Task ConfirmEmail_WhenThereIsNoUserFound_ShouldReturnStatusCode404NotFound()
    {
        // Arrange
        var email = _fixture.Create<string>();
        var token = _fixture.Create<string>();
        A.CallTo(() => _userService.UserAlreadyExists(A<string>._)).Returns(false);

        // Act
        var actual = await _sut.ConfirmEmail(email, token);

        // Assert
        A.CallTo(() => _userService.UserAlreadyExists(A<string>._)).MustHaveHappenedOnceExactly();
        Assert.IsType<NotFoundResult>(actual);
    }

    [Fact]
    public async Task ConfirmEmail_WhenThereIsUnhandledException_ShouldReturnStatusCode500InternalServerErrorAndLogAnException()
    {
        // Arrange
        var email = _fixture.Create<string>();
        var token = _fixture.Create<string>();
        A.CallTo(() => _userService.UserAlreadyExists(A<string>._)).Returns(true);
        A.CallTo(() => _userService.ConfirmEmail(A<string>._, A<string>._)).Throws<Exception>();

        // Act
        var actual = await _sut.ConfirmEmail(email, token) as StatusCodeResult;

        // Assert
        A.CallTo(() => _userService.ConfirmEmail(A<string>._, A<string>._)).MustHaveHappenedOnceExactly();
        A.CallTo(_logger).Where(
                call => call.Method.Name == "Log"
                && call.GetArgument<LogLevel>(0) == LogLevel.Error)
            .MustHaveHappened(1, Times.Exactly);
        Assert.Equal(StatusCodes.Status500InternalServerError, actual.StatusCode);
    }

    [Fact]
    public async Task ForgotPassword_WhenSuccessful_ShouldReturnStatusCode204NoContent()
    {
        // Arrange
        var email = _fixture.Create<string>();
        A.CallTo(() => _userService.UserAlreadyExists(A<string>._)).Returns(true);

        // Act
        var actual = await _sut.ForgotPassword(email);

        // Assert
        A.CallTo(() => _userService.UserAlreadyExists(A<string>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _userService.ForgotPassword(A<string>._)).MustHaveHappenedOnceExactly();
        Assert.IsType<NoContentResult>(actual);
    }

    [Fact]
    public async Task ForgotPassword_WhenThereIsNoUserFound_ShouldReturnStatusCode404NotFound()
    {
        // Arrange
        var email = _fixture.Create<string>();
        A.CallTo(() => _userService.UserAlreadyExists(A<string>._)).Returns(false);

        // Act
        var actual = await _sut.ForgotPassword(email);

        // Assert
        A.CallTo(() => _userService.UserAlreadyExists(A<string>._)).MustHaveHappenedOnceExactly();
        Assert.IsType<NotFoundResult>(actual);
    }

    [Fact]
    public async Task ForgotPassword_WhenThereIsUnhandledException_ShouldReturnStatusCode500InternalServerErrorAndLogAnException()
    {
        // Arrange
        var email = _fixture.Create<string>();
        A.CallTo(() => _userService.UserAlreadyExists(A<string>._)).Returns(true);
        A.CallTo(() => _userService.ForgotPassword(A<string>._)).Throws<Exception>();

        // Act
        var actual = await _sut.ForgotPassword(email) as StatusCodeResult;

        // Assert
        A.CallTo(() => _userService.ForgotPassword(A<string>._)).MustHaveHappenedOnceExactly();
        A.CallTo(_logger).Where(
                call => call.Method.Name == "Log"
                && call.GetArgument<LogLevel>(0) == LogLevel.Error)
            .MustHaveHappened(1, Times.Exactly);
        Assert.Equal(StatusCodes.Status500InternalServerError, actual.StatusCode);
    }

    [Fact]
    public async Task ResetPassword_WhenSuccessful_ShouldReturnRedirect()
    {
        // Arrange
        var request = _fixture.Create<ResetPasswordRequest>();
        A.CallTo(() => _userService.UserAlreadyExists(A<string>._)).Returns(true);
        var frontendUrl = A.Fake<IConfigurationSection>();
        A.CallTo(() => frontendUrl.Value).Returns(_fixture.Create<string>());
        A.CallTo(() => _configuration.GetSection("UrlSettings:Frontend")).Returns(frontendUrl);

        // Act
        var actual = await _sut.ResetPassord(request);

        // Assert
        A.CallTo(() => _userService.UserAlreadyExists(A<string>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _userService.ResetPassword(A<ResetPasswordRequest>._)).MustHaveHappenedOnceExactly();
        Assert.IsType<RedirectResult>(actual);
    }

    [Fact]
    public async Task ResetPassword_WhenThereIsNoUserFound_ShouldReturnStatusCode404NotFound()
    {
        // Arrange
        var request = _fixture.Create<ResetPasswordRequest>();
        A.CallTo(() => _userService.UserAlreadyExists(A<string>._)).Returns(false);

        // Act
        var actual = await _sut.ResetPassord(request);

        // Assert
        A.CallTo(() => _userService.UserAlreadyExists(A<string>._)).MustHaveHappenedOnceExactly();
        Assert.IsType<NotFoundResult>(actual);
    }

    [Fact]
    public async Task ResetPassword_WhenThereIsUnhandledException_ShouldReturnStatusCode500InternalServerErrorAndLogAnException()
    {
        // Arrange
        var request = _fixture.Create<ResetPasswordRequest>();
        A.CallTo(() => _userService.UserAlreadyExists(A<string>._)).Returns(true);
        A.CallTo(() => _userService.ResetPassword(A<ResetPasswordRequest>._)).Throws<Exception>();

        // Act
        var actual = await _sut.ResetPassord(request) as StatusCodeResult;

        // Assert
        A.CallTo(() => _userService.ResetPassword(A<ResetPasswordRequest>._)).MustHaveHappenedOnceExactly();
        A.CallTo(_logger).Where(
                call => call.Method.Name == "Log"
                && call.GetArgument<LogLevel>(0) == LogLevel.Error)
            .MustHaveHappened(1, Times.Exactly);
        Assert.Equal(StatusCodes.Status500InternalServerError, actual.StatusCode);
    }
}
