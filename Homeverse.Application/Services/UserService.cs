using AutoMapper;
using Hangfire;
using Homeverse.Application.DTOs.Requests;
using Homeverse.Application.DTOs.Responses;
using Homeverse.Application.Interfaces;
using Homeverse.Domain.Entities;
using Homeverse.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Web;

namespace Homeverse.Application.Services;

public interface IUserService
{
    Task<IEnumerable<UserResponse>> GetUsersAsync();
    Task<UserResponse> GetUserByIdAsync(int id);
    Task<UserResponse> GetProfileAsync();
    Task<UserResponse> Register(RegisterRequest request);
    Task ConfirmEmail(string email, string token);
    Task<UserResponse> Login(LoginRequest request);
    Task ForgotPassword(string email);
    Task ResetPassword(ResetPasswordRequest request);
    Task<UserResponse> UpdateUserAsync(int id, UpdateUserRequest request);
    Task<UserResponse> UpdateProfileAsync(UpdateUserRequest request);
    Task<bool> UserAlreadyExists(string email);
    Task AddConnection(string connectionId);
    Task DeleteConnection(string connectionId);
    Task<IEnumerable<int>> GetFriendIds();
    Task<IEnumerable<string>> GetConnectionIdsByUserId(int userId);
    Task<IEnumerable<FriendResponse>> GetFriends();
    Task<FriendResponse> GetFriend(int otherId);
    Task<IEnumerable<string>> GetFriendConnectionIds();
}

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;
    private readonly IMailService _mailService;
    private readonly IConfiguration _configuration;
    private readonly ICurrentUserService _currentUserService;
    private readonly IBackgroundJobClient _backgroundJobClient;

    public UserService
    (
        IUnitOfWork unitOfWork, 
        IMapper mapper,
        IUserRepository userRepository,
        IMailService mailService,
        IConfiguration configuration,
        ICurrentUserService currentUserService,
        IBackgroundJobClient backgroundJobClient
    )
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userRepository = userRepository;
        _mailService = mailService;
        _configuration = configuration;
        _currentUserService = currentUserService;
        _backgroundJobClient = backgroundJobClient;
    }

    public async Task<IEnumerable<UserResponse>> GetUsersAsync()
    {
        var users = await _userRepository.GetUsersAsync();

        return _mapper.Map<IEnumerable<UserResponse>>(users);
    }

    public async Task<UserResponse> GetUserByIdAsync(int id)
    {
        var user = await _userRepository.GetUserByIdAsync(id);

        return _mapper.Map<UserResponse>(user);
    }

    public async Task<UserResponse> GetProfileAsync()
    {
        var user = await _userRepository.GetUserByIdAsync(_currentUserService.UserId);

        return _mapper.Map<UserResponse>(user);
    }

    public async Task<UserResponse> Register(RegisterRequest request)
    {
        var user = _mapper.Map<User>(request);

        CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

        user.PasswordHash = passwordHash;
        user.PasswordSalt = passwordSalt;

        user.EmailVerifyToken = CreateRandomToken();

        await _unitOfWork.ExecuteTransactionAsync(async () =>
        {
            await _userRepository.AddUserAsync(user);
        });

        var apiUrl = _configuration.GetSection("UrlSettings:Api").Value;
        var link = $"{apiUrl}/api/v1/users/confirm-email?email={HttpUtility.UrlEncode(user.Email)}&token={user.EmailVerifyToken}";
        var content = $@"
<h3>Cảm ơn bạn đã đăng ký tài khoản tại Homeverse</h3>
<br>
<a href=""{link}"">Nhấn vào đây để xác minh email</a>
";
        _backgroundJobClient.Enqueue(() => _mailService.SendAsync(user.Email, "Xác minh email", content));

        return _mapper.Map<UserResponse>(user);
    }

    public async Task ConfirmEmail(string email, string token)
    {
        var user = await _userRepository.GetUserByEmailAsync(email);

        if (user.EmailVerifyToken == token)
        {
            user.IsActive = true;
        }

        await _unitOfWork.ExecuteTransactionAsync(async () =>
        {
            await _userRepository.UpdateUserAsync(user);
        });
    }

    public async Task<UserResponse> Login(LoginRequest request)
    {
        var user = await _userRepository.GetUserByEmailAsync(request.Email);
        if (user == null || !VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
        {
            user = new User();
        }

        return _mapper.Map<UserResponse>(user);
    }

    public async Task ForgotPassword(string email)
    {
        var user = await _userRepository.GetUserByEmailAsync(email);
        if (user != null)
        {
            user.PasswordResetToken = CreateRandomToken();
            user.ResetTokenExpire = DateTimeOffset.Now.AddDays(1);

            await _unitOfWork.ExecuteTransactionAsync(async () =>
            {
                await _userRepository.UpdateUserAsync(user);
            });

            var apiUrl = _configuration.GetSection("UrlSettings:Api").Value;
            var link = $"{apiUrl}/api/v1/users/reset-password";
            var content = $@"
<h3>Vui lòng nhập mật khẩu mới bên dưới</h3>
<br>
<form action=""{link}"" method=""post"">
    <input name=""email"" value=""{user.Email}"" type=""hidden"" />
    <input name=""token"" value=""{user.PasswordResetToken}"" type=""hidden"" />
    <input type=""text"" name=""password"" minlength=""8"" required />
    <br>
    <br>
    <button onclick=""alert(20)"" type=""submit"">Đổi mật khẩu</button>
</form>
<br>
* Lưu ý: 
<br>
Mật khẩu phải có đủ 8 kí tự
<br>
Form chỉ có thời hạn 24h và chỉ dùng được 1 lần.
";
            _backgroundJobClient.Enqueue(() => _mailService.SendAsync(user.Email, "Đổi mật khẩu", content));
        }
    }

    public async Task ResetPassword(ResetPasswordRequest request)
    {
        var user = await _userRepository.GetUserByEmailAsync(request.Email);

        if (user != null && user.PasswordResetToken == request.Token && user.ResetTokenExpire > DateTimeOffset.Now)
        {
            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            user.PasswordResetToken = null;
            user.ResetTokenExpire = null;

            await _unitOfWork.ExecuteTransactionAsync(async () =>
            {
                await _userRepository.UpdateUserAsync(user);
            });
        }
    }

    public async Task<UserResponse> UpdateUserAsync(int id, UpdateUserRequest request)
    {
        var user = await _userRepository.GetUserByIdAsync(id);
        user.Name = request.UserName;
        user.Phone = request.Phone;

        //
        if (request.NewPassword != null)
        {
            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
        }

        await _unitOfWork.ExecuteTransactionAsync(async () =>
        {
            await _userRepository.UpdateUserAsync(user);
        });

        return _mapper.Map<UserResponse>(user);
    }

    public async Task<UserResponse> UpdateProfileAsync(UpdateUserRequest request)
    {
        var user = await _userRepository.GetUserByIdAsync(_currentUserService.UserId);
        user.Name = request.UserName;
        user.Phone = request.Phone;

        //
        if (request.NewPassword != null)
        {
            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
        }

        await _unitOfWork.ExecuteTransactionAsync(async () =>
        {
            await _userRepository.UpdateUserAsync(user);
        });

        return _mapper.Map<UserResponse>(user);
    }

    public async Task<bool> UserAlreadyExists(string email)
    {
        var user = await _userRepository.GetUserByEmailAsync(email);

        return user != null;
    }

    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }

    private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512(passwordSalt))
        {
            var computedHash = hmac
                .ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            return computedHash.SequenceEqual(passwordHash);
        }
    }

    private string CreateRandomToken()
    {
        return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
    }

    public async Task AddConnection(string connectionId)
    {
        var connection = new Connection()
        {
            ConnectionId = connectionId,
            UserId = _currentUserService.UserId
        };

        await _userRepository.AddConnectionAsync(connection);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteConnection(string connectionId)
    {
        await _userRepository.DeleteConnectionAsync(connectionId);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<int>> GetFriendIds()
    {
        return await _userRepository.GetFriendIdsAsync(_currentUserService.UserId);
    }

    public async Task<IEnumerable<string>> GetConnectionIdsByUserId(int userId)
    {
        return await _userRepository.GetConnectionIdsByUserIdAsync(userId);
    }

    public async Task<IEnumerable<FriendResponse>> GetFriends()
    {
        var friends = await _userRepository.GetFriendsAsync(_currentUserService.UserId);

        return _mapper.Map<IEnumerable<FriendResponse>>(friends);
    }

    public async Task<FriendResponse> GetFriend(int otherId)
    {
        var friend = await _userRepository.GetFriendAsync(_currentUserService.UserId, otherId);

        return _mapper.Map<FriendResponse>(friend);
    }

    public async Task<IEnumerable<string>> GetFriendConnectionIds()
    {
        return await _userRepository.GetFriendConnectionIdsAsync(_currentUserService.UserId);
    }
}
