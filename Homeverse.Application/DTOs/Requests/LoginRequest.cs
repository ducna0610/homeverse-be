using System.ComponentModel.DataAnnotations;

namespace Homeverse.Application.DTOs.Requests;

public class LoginRequest
{
    [EmailAddress]
    public string Email { get; set; }
    public string Password { get; set; }
}
