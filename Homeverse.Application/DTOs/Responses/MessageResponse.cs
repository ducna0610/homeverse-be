namespace Homeverse.Application.DTOs.Responses;

public class MessageResponse : BaseResponse
{
    public string Content { get; set; }
    public UserResponse Sender { get; set; }
    public UserResponse Receiver { get; set; }
    public bool IsReaded { get; set; } = false;
}
