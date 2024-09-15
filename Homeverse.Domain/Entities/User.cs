using Homeverse.Domain.Enums;

namespace Homeverse.Domain.Entities;

public class User : BaseEntity
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public RoleEnum Role { get; set; } = RoleEnum.Landlord;
    public string EmailVerifyToken { get; set; }
    public string? PasswordResetToken { get; set; }
    public DateTimeOffset? ResetTokenExpire { get; set; }
    public bool IsActive { get; set; } = false;
    public bool IsDeleted { get; set; } = false;
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }

    public virtual ICollection<Property> Properties { get; set; }
    public virtual ICollection<Bookmark> Bookmarks { get; set; }
    public virtual ICollection<Message> MessagesSent { get; set; }
    public virtual ICollection<Message> MessagseReceived { get; set; }
    public virtual ICollection<Connection> Connections { get; set; }
}
