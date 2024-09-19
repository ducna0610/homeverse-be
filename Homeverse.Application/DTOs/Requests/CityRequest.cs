using System.ComponentModel.DataAnnotations;

namespace Homeverse.Application.DTOs.Requests;

public class CityRequest
{
    [Required]
    [MinLength(2)]
    [MaxLength(30)]
    public string Name { get; set; }
}
