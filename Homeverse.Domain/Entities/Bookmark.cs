namespace Homeverse.Domain.Entities;

public class Bookmark
{
    public int UserId { get; set; }
    public int PropertyId { get; set; }

    public virtual User User { get; set; }
    public virtual Property Property { get; set; }
}
