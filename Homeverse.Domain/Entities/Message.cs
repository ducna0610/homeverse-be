namespace Homeverse.Domain.Entities;

public class Message : BaseEntity
{
    public string Content { get; set; }
    public int SenderId { get; set; }
    public int ReceiverId { get; set; }
    public bool IsReaded { get; set; } = false;

    public virtual User Sender { get; set; }
    public virtual User Receiver { get; set; }
}
