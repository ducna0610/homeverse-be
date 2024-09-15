namespace Homeverse.Domain.Entities;

public class Photo
{
    public int Id { get; set; }
    public string PublicId { get; set; }
    public string ImageUrl { get; set; }
    public bool IsPrimary { get; set; } = false;
    public int PropertyId { get; set; }

    public virtual Property Property { get; set; }
}
