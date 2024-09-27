using System.ComponentModel.DataAnnotations;

namespace Homeverse.Application.DTOs.Requests;

public class RegisterRequest
{
    public string UserName { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Password { get; set; }
}
