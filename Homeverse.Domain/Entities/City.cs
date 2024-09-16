namespace Homeverse.Domain.Entities;

public class City : BaseEntity
{
    public string Name { get; set; }

    public virtual ICollection<Property> Properties { get; set; }
}
