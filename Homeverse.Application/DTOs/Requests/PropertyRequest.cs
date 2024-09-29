using Homeverse.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Homeverse.Application.DTOs.Requests;

public class PropertyRequest
{
    [Required]
    public string Title { get; set; } = string.Empty;
    [Required]
    public decimal Price { get; set; }
    [Required]
    public int Area { get; set; }
    [Required]
    public double Lat { get; set; }
    [Required]
    public double Lng { get; set; }
    [Required]
    public string Address { get; set; } = string.Empty;
    [Required]
    public string Description { get; set; } = string.Empty;
    [Required]
    public CategoryEnum Category { get; set; }
    [Required]
    public FurnishEnum Furnish { get; set; }
    //[Required]
    public bool IsActive { get; set; }
    [Required]
    public int CityId { get; set; }
    public ICollection<IFormFile> Images { get; set; } = new List<IFormFile>();
}
