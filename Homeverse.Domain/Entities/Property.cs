using Homeverse.Domain.Enums;

namespace Homeverse.Domain.Entities;

public class Property : BaseEntity
{
    public string Title { get; set; }
    public decimal Price { get; set; }
    public int Area { get; set; }
    public string Address { get; set; }
    public double Lat { get; set; }
    public double Lng { get; set; }
    public string Description { get; set; }
    public CategoryEnum CategoryId { get; set; } = CategoryEnum.Rent;
    public FurnishEnum FurnishId { get; set; } = FurnishEnum.None;
    public bool IsActive { get; set; } = false;
    public int CityId { get; set; }
    public int PostedBy { get; set; }

    public virtual City City { get; set; }
    public virtual User User { get; set; }
    public virtual ICollection<Bookmark> Bookmarks { get; set; }
    public virtual ICollection<Photo> Photos { get; set; }
}
