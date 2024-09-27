using Homeverse.Domain.Enums;

namespace Homeverse.Application.DTOs.Responses;

public class UserResponse : BaseResponse
{
    public string Name { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public RoleEnum Role { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
}
