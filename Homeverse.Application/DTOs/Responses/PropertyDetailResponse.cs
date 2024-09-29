namespace Homeverse.Application.DTOs.Responses;

public class PropertyDetailResponse : PropertyResponse
{
    public double Lat { get; set; }
    public double Lng { get; set; }
    public int CityId { get; set; }
    public int FurnishId { get; set; }
    public int CategoryId { get; set; }
    public bool IsActive { get; set; }
    public string Description { get; set; }
    public ICollection<PhotoResponse> Photos { get; set; }
}
