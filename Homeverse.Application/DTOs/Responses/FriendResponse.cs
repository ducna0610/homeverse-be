namespace Homeverse.Application.DTOs.Responses;

public class FriendResponse : UserResponse
{
    public bool IsOnline { get; set; }
    public int MessageUnread { get; set; }
}
