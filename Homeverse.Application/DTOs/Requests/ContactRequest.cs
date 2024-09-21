using System.ComponentModel.DataAnnotations;

namespace Homeverse.Application.DTOs.Requests;

public class ContactRequest
{
    [Required]
    public string Name { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    public string Phone { get; set; }
    [Required]
    public string Message { get; set; }
}
