namespace Homeverse.Domain.Entities;

public class Connection
{
    public int UserId { get; set; }
    public string ConnectionId { get; set; }

    public virtual User User { get; set; }
}
