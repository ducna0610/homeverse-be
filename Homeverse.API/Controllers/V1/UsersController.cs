using Asp.Versioning;
using Homeverse.Application.DTOs.Requests;
using Homeverse.Application.DTOs.Responses;
using Homeverse.Application.Interfaces;
using Homeverse.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Homeverse.API.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;
    private readonly ICacheService _cacheService;

    public UsersController
    (
        ILogger<UsersController> logger, 
        IUserService userService, 
        IConfiguration configuration,
        ICacheService cacheService
    )
    {
        _logger = logger;
        _userService = userService;
        _configuration = configuration;
        _cacheService = cacheService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get()
    {
        try
        {
            var cacheData = await _cacheService.GetDataAsync<IEnumerable<UserResponse>>("users");
            if (cacheData != null)
            {
                return Ok(cacheData);
            }

            var response = await _userService.GetUsersAsync();
            if (response.Count() == 0)
            {
                return NotFound();
            }
            await _cacheService.SetDataAsync("users", response);

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError($"The method {nameof(UserService.GetUsersAsync)} caused an exception", ex);
        }

        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpGet]
    [Route("{id}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var cacheData = await _cacheService.GetDataAsync<IEnumerable<UserResponse>>("users");
            if (cacheData != null)
            {
                var filteredData = cacheData.FirstOrDefault(x => x.Id == id);
                if (filteredData != null)
                {
                    return Ok(filteredData);
                }
            }

            var response = await _userService.GetUserByIdAsync(id);

            if (response == null)
            {
                return NotFound();
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError($"The method {nameof(UserService.GetUserByIdAsync)} caused an exception", ex);
        }

        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpGet]
    [Route("profile")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetProfile()
    {
        try
        {
            var response = await _userService.GetProfileAsync();

            if (response == null)
            {
                return NotFound();
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError($"The method {nameof(UserService.GetProfileAsync)} caused an exception", ex);
        }

        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        try
        {
            var user = await _userService.Login(request);

            if (user.Id == 0)
            {
                return Unauthorized("Invalid user name or password");
            }

            var response = new TokenResponse();
            response.Token = CreateJWT(user);

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError($"The method {nameof(UserService.Login)} caused an exception", ex);
        }

        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        try
        {
            if (await _userService.UserAlreadyExists(request.Email))
            {
                return BadRequest("Email already exists, please try something else");
            }
            var response = await _userService.Register(request);
            await _cacheService.RemoveDataAsync("users");

            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError($"The method {nameof(UserService.Register)} caused an exception", ex);
        }

        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpGet("confirm-email")]
    public async Task<ActionResult> ConfirmEmail(string email, string token)
    {
        try
        {
            if (!(await _userService.UserAlreadyExists(email)))
            {
                return NotFound();
            }

            await _userService.ConfirmEmail(email, token);
            var frontendUrl = _configuration.GetSection("UrlSettings:Frontend").Value;

            return Redirect(frontendUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError($"The method {nameof(UserService.ConfirmEmail)} caused an exception", ex);
        }

        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpPost("forgot-password")]
    public async Task<ActionResult> ForgotPassword([FromForm] string email)
    {
        try
        {
            if (!(await _userService.UserAlreadyExists(email)))
            {
                return NotFound();
            }

            await _userService.ForgotPassword(email);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError($"The method {nameof(UserService.ForgotPassword)} caused an exception", ex);
        }

        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpPost("reset-password")]
    public async Task<ActionResult> ResetPassord([FromForm] ResetPasswordRequest request)
    {
        try
        {
            if (!(await _userService.UserAlreadyExists(request.Email)))
            {
                return NotFound();
            }

            await _userService.ResetPassword(request);
            var frontendUrl = _configuration.GetSection("UrlSettings:Frontend").Value;

            return Redirect(frontendUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError($"The method {nameof(UserService.ResetPassword)} caused an exception", ex);
        }

        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpPut]
    public async Task<ActionResult> Update(int id, UpdateUserRequest request)
    {
        try
        {
            var response = await _userService.UpdateUserAsync(id, request);
            await _cacheService.RemoveDataAsync("users");

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError($"The method {nameof(UserService.UpdateUserAsync)} caused an exception", ex);
        }

        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpPut("profile")]
    public async Task<ActionResult> UpdateProfile(UpdateUserRequest request)
    {
        try
        {
            var user = await _userService.Login(new LoginRequest() { Email = request.Email, Password = request.Password });
            if (user.Id == 0)
            {
                return Unauthorized("Invalid user name or password");
            }

            var response = await _userService.UpdateProfileAsync(request);
            await _cacheService.RemoveDataAsync("users");

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError($"The method {nameof(UserService.UpdateProfileAsync)} caused an exception", ex);
        }

        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    private string CreateJWT(UserResponse user)
    {
        var secretKey = _configuration.GetSection("JwtSettings:SecretKey").Value;
        var issuer = _configuration.GetSection("JwtSettings:Issuer").Value;
        var audience = _configuration.GetSection("JwtSettings:Audience").Value;
        var expiryDays = int.Parse(_configuration.GetSection("JwtSettings:ExpiryDays").Value);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        var claims = new Claim[] {
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var signingCredentials = new SigningCredentials(
                key, SecurityAlgorithms.HmacSha256Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = issuer,
            Audience = audience,
            Expires = DateTime.UtcNow.AddDays(expiryDays),
            Subject = new ClaimsIdentity(claims),
            SigningCredentials = signingCredentials,
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
