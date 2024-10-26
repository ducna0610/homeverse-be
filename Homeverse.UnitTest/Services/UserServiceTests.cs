using AutoFixture;
using AutoMapper;
using FakeItEasy;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Homeverse.Application.DTOs.Requests;
using Homeverse.Application.DTOs.Responses;
using Homeverse.Application.Interfaces;
using Homeverse.Application.Services;
using Homeverse.Domain.Entities;
using Homeverse.Domain.Interfaces;
using Homeverse.Infrastructure.Data;
using Homeverse.UnitTest.Mocks;
using Microsoft.Extensions.Configuration;

namespace Homeverse.UnitTest.Services;

public class UserServiceTests
{
    private readonly Fixture _fixture;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMailService _mailService;
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly IConfiguration _configuration;
    private readonly IUserService _sut;

    public UserServiceTests()
    {
        _fixture = new Fixture(); 
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        var context = MockDbContext.CreateMockDbContext();
        _unitOfWork = new UnitOfWork(context);
        _mapper = A.Fake<IMapper>();
        _userRepository = A.Fake<IUserRepository>();
        _mailService = A.Fake<IMailService>();
        _backgroundJobClient = A.Fake<IBackgroundJobClient>();
        _configuration = A.Fake<IConfiguration>();
        _currentUserService = A.Fake<ICurrentUserService>();
        _sut = new UserService(_unitOfWork, _mapper, _userRepository, _mailService, _configuration, _currentUserService, _backgroundJobClient);
    }

    [Fact]
    public async Task GetUsersAsync_WhenSuccessful_ShouldReturnUsers()
    {
        // Arrange
        var users = _fixture.CreateMany<User>(3).ToList();
        var response = _fixture.CreateMany<UserResponse>(3).ToList();
        A.CallTo(() => _userRepository.GetUsersAsync()).Returns(users);
        A.CallTo(() => _mapper.Map<IEnumerable<UserResponse>>(A<IEnumerable<User>>._)).Returns(response);

        // Act
        var actual = await _sut.GetUsersAsync();

        // Assert
        A.CallTo(() => _userRepository.GetUsersAsync()).MustHaveHappenedOnceExactly();
        Assert.IsAssignableFrom<IEnumerable<UserResponse>>(actual);
        Assert.Equal(users.Count(), actual.Count());
    }

    [Fact]
    public async Task GetUserByIdAsync_WhenSuccessful_ShouldReturnUser()
    {
        // Arrange
        var id = _fixture.Create<int>();
        var user = _fixture.Create<User>();
        var response = _fixture.Create<UserResponse>();
        A.CallTo(() => _userRepository.GetUserByIdAsync(A<int>._)).Returns(user);
        A.CallTo(() => _mapper.Map<UserResponse>(A<User>._)).Returns(response);

        // Act
        var actual = await _sut.GetUserByIdAsync(id);

        // Assert
        A.CallTo(() => _userRepository.GetUserByIdAsync(A<int>._)).MustHaveHappenedOnceExactly();
        Assert.IsAssignableFrom<UserResponse>(actual);
    }

    [Fact]
    public async Task GetProfileAsync_WhenSuccessful_ShouldReturnUser()
    {
        // Arrange
        var user = _fixture.Create<User>();
        var response = _fixture.Create<UserResponse>();
        A.CallTo(() => _userRepository.GetUserByIdAsync(A<int>._)).Returns(user);
        A.CallTo(() => _mapper.Map<UserResponse>(A<User>._)).Returns(response);

        // Act
        var actual = await _sut.GetProfileAsync();

        // Assert
        A.CallTo(() => _userRepository.GetUserByIdAsync(A<int>._)).MustHaveHappenedOnceExactly();
        Assert.IsAssignableFrom<UserResponse>(actual);
    }

    [Fact]
    public async Task Register_WhenSuccessful_ShouldReturnUser()
    {
        // Arrange
        var request = _fixture.Create<RegisterRequest>();
        var user = _fixture.Create<User>();
        var response = _fixture.Create<UserResponse>();
        A.CallTo(() => _mapper.Map<User>(A<RegisterRequest>._)).Returns(user);
        A.CallTo(() => _mapper.Map<UserResponse>(A<User>._)).Returns(response);
        A.CallTo(() => _backgroundJobClient.Create(A<Job>._, A<EnqueuedState>._)).Returns("");

        // Act
        var actual = await _sut.Register(request);

        // Assert
        A.CallTo(() => _userRepository.AddUserAsync(A<User>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _backgroundJobClient 
            .Create(A<Job>.That.Matches(job => job.Method.Name == "SendAsync"), A<EnqueuedState>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ConfirmEmail_WhenSuccessful_ShouldReturnUsers()
    {
        // Arrange
        var email = _fixture.Create<string>();
        var token = _fixture.Create<string>();
        var user = _fixture.Create<User>();
        A.CallTo(() => _userRepository.GetUserByEmailAsync(A<string>._)).Returns(user);

        // Act
        await _sut.ConfirmEmail(email, token);

        // Assert
        A.CallTo(() => _userRepository.GetUserByEmailAsync(A<string>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _userRepository.UpdateUserAsync(A<User>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Login_WhenSuccessful_ShouldReturnUser()
    {
        // Arrange
        var request = _fixture.Create<LoginRequest>();
        var user = _fixture.Create<User>();
        var response = _fixture.Create<UserResponse>();
        A.CallTo(() => _userRepository.GetUserByEmailAsync(A<string>._)).Returns(user);
        A.CallTo(() => _mapper.Map<UserResponse>(A<User>._)).Returns(response);

        // Act
        var actual = await _sut.Login(request);

        // Assert
        A.CallTo(() => _userRepository.GetUserByEmailAsync(A<string>._)).MustHaveHappenedOnceExactly();
        Assert.IsAssignableFrom<UserResponse>(actual);
    }

    [Fact]
    public async Task ForgotPassword_WhenSuccessful_ShouldSendMail()
    {
        // Arrange
        var email = _fixture.Create<string>();
        var user = _fixture.Create<User>();
        A.CallTo(() => _userRepository.GetUserByEmailAsync(A<string>._)).Returns(user);

        // Act
        await _sut.ForgotPassword(email);

        // Assert
        A.CallTo(() => _userRepository.GetUserByEmailAsync(A<string>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _userRepository.UpdateUserAsync(A<User>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _backgroundJobClient
            .Create(A<Job>.That.Matches(job => job.Method.Name == "SendAsync"), A<EnqueuedState>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ResetPassword_WhenSuccessful_ShouldSendMail()
    {
        // Arrange
        var request = _fixture.Create<ResetPasswordRequest>();
        var user = _fixture.Create<User>();
        user.PasswordResetToken = request.Token;
        user.ResetTokenExpire = DateTime.UtcNow.AddMinutes(1);
        A.CallTo(() => _userRepository.GetUserByEmailAsync(A<string>._)).Returns(user);

        // Act
        await _sut.ResetPassword(request);

        // Assert
        A.CallTo(() => _userRepository.GetUserByEmailAsync(A<string>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _userRepository.UpdateUserAsync(A<User>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task UpdateUserAsync_WhenSuccessful_ShouldSendMail()
    {
        // Arrange
        var id = _fixture.Create<int>();
        var request = _fixture.Create<UpdateUserRequest>();
        var user = _fixture.Create<User>();
        var response = _fixture.Create<UserResponse>();
        A.CallTo(() => _userRepository.GetUserByIdAsync(A<int>._)).Returns(user);
        A.CallTo(() => _mapper.Map<UserResponse>(A<User>._)).Returns(response);

        // Act
        var actual = await _sut.UpdateUserAsync(id, request);

        // Assert
        A.CallTo(() => _userRepository.UpdateUserAsync(A<User>._)).MustHaveHappenedOnceExactly();
        Assert.IsAssignableFrom<UserResponse>(actual);
    }

    [Fact]
    public async Task UpdateProfileAsync_WhenSuccessful_ShouldReturnUser()
    {
        // Arrange
        var id = _fixture.Create<int>();
        var request = _fixture.Create<UpdateUserRequest>();
        var user = _fixture.Create<User>();
        var response = _fixture.Create<UserResponse>();
        A.CallTo(() => _userRepository.GetUserByIdAsync(A<int>._)).Returns(user);
        A.CallTo(() => _mapper.Map<UserResponse>(A<User>._)).Returns(response);

        // Act
        var actual = await _sut.UpdateProfileAsync(request);

        // Assert
        A.CallTo(() => _userRepository.UpdateUserAsync(A<User>._)).MustHaveHappenedOnceExactly();
        Assert.IsAssignableFrom<UserResponse>(actual);
    }

    [Fact]
    public async Task UserAlreadyExists_WhenSuccessful_ShouldReturnUser()
    {
        // Arrange
        var email = _fixture.Create<string>();
        var user = _fixture.Create<User>();
        A.CallTo(() => _userRepository.GetUserByEmailAsync(A<string>._)).Returns(user);

        // Act
        var actual = await _sut.UserAlreadyExists(email);

        // Assert
        A.CallTo(() => _userRepository.GetUserByEmailAsync(A<string>._)).MustHaveHappenedOnceExactly();
        Assert.IsAssignableFrom<bool>(actual);
        Assert.Equal(true, actual);
    }

    [Fact]
    public async Task AddConnection_WhenSuccessful_ShouldReturnUser()
    {
        // Arrange
        var connection = _fixture.Create<Connection>();

        // Act
        await _sut.AddConnection(connection.ConnectionId);

        // Assert
        A.CallTo(() => _userRepository.AddConnectionAsync(A<Connection>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task DeleteConnection_WhenSuccessful_ShouldReturnUser()
    {
        // Arrange
        var connectionId = _fixture.Create<string>();

        // Act
        await _sut.DeleteConnection(connectionId);

        // Assert
        A.CallTo(() => _userRepository.DeleteConnectionAsync(A<string>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task GetFriendIds_WhenSuccessful_ShouldReturnUser()
    {
        // Arrange

        // Act
        var actual = await _sut.GetFriendIds();

        // Assert
        A.CallTo(() => _userRepository.GetFriendIdsAsync(A<int>._)).MustHaveHappenedOnceExactly();
        Assert.IsAssignableFrom<IEnumerable<int>>(actual);
    }

    [Fact]
    public async Task GetConnectionIdsByUserId_WhenSuccessful_ShouldReturnUser()
    {
        // Arrange
        var userId = _fixture.Create<int>();

        // Act
        var actual = await _sut.GetConnectionIdsByUserId(userId);

        // Assert
        A.CallTo(() => _userRepository.GetConnectionIdsByUserIdAsync(A<int>._)).MustHaveHappenedOnceExactly();
        Assert.IsAssignableFrom<IEnumerable<string>>(actual);
    }

    [Fact]
    public async Task GetFriends_WhenSuccessful_ShouldReturnUser()
    {
        // Arrange
        var users = _fixture.CreateMany<User>(3);
        var response = _fixture.CreateMany<FriendResponse>(3);
        A.CallTo(() => _userRepository.GetFriendsAsync(A<int>._)).Returns(users);
        A.CallTo(() => _mapper.Map<IEnumerable<FriendResponse>>(A<IEnumerable<User>>._)).Returns(response);

        // Act
        var actual = await _sut.GetFriends();

        // Assert
        A.CallTo(() => _userRepository.GetFriendsAsync(A<int>._)).MustHaveHappenedOnceExactly();
        Assert.IsAssignableFrom<IEnumerable<FriendResponse>>(actual);
        Assert.Equal(users.Count(), actual.Count());
    }

    [Fact]
    public async Task GetFriend_WhenSuccessful_ShouldReturnUser()
    {
        // Arrange
        var otherId = _fixture.Create<int>();
        var user = _fixture.Create<User>();
        var response = _fixture.Create<FriendResponse>();
        A.CallTo(() => _userRepository.GetFriendAsync(A<int>._, A<int>._)).Returns(user);
        A.CallTo(() => _mapper.Map<FriendResponse>(A<User>._)).Returns(response);

        // Act
        var actual = await _sut.GetFriend(otherId);

        // Assert
        A.CallTo(() => _userRepository.GetFriendAsync(A<int>._, A<int>._)).MustHaveHappenedOnceExactly();
        Assert.IsAssignableFrom<FriendResponse>(actual);
    }

    [Fact]
    public async Task GetFriendConnectionIds_WhenSuccessful_ShouldReturnUser()
    {
        // Arrange
        var response = _fixture.CreateMany<string>(3);
        A.CallTo(() => _userRepository.GetFriendConnectionIdsAsync(A<int>._)).Returns(response);

        // Act
        var actual = await _sut.GetFriendConnectionIds();

        // Assert
        A.CallTo(() => _userRepository.GetFriendConnectionIdsAsync(A<int>._)).MustHaveHappenedOnceExactly();
        Assert.IsAssignableFrom<IEnumerable<string>>(actual);
    }
}
