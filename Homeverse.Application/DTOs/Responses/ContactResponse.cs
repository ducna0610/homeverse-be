namespace Homeverse.Application.DTOs.Responses;

public class ContactResponse : BaseResponse
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Message { get; set; }
}
