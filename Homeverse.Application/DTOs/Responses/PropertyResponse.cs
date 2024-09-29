namespace Homeverse.Application.DTOs.Responses;

public class PropertyResponse : BaseResponse
{
    public string Title { get; set; }
    public decimal Price { get; set; }
    public int Area { get; set; }
    public string City { get; set; }
    public string Address { get; set; }
    public string Category { get; set; }
    public string Furnish { get; set; }
    public string ImageUrl { get; set; }
    public int NumberImage { get; set; }
    public UserResponse PostedBy { get; set; }
}
