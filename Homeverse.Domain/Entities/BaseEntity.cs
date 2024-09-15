namespace Homeverse.Domain.Entities;

public class BaseEntity
{
    public int Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public BaseEntity()
    {
        CreatedAt = UpdatedAt = DateTimeOffset.UtcNow;
    }
}
