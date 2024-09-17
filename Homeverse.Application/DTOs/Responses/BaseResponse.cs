namespace Homeverse.Application.DTOs.Responses;

public class BaseResponse
{
    public int Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
