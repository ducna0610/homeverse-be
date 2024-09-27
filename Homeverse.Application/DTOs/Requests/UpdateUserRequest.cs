namespace Homeverse.Application.DTOs.Requests;

public class UpdateUserRequest
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Password { get; set; }
    public string NewPassword { get; set; }
}
