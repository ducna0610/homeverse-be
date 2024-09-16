using Homeverse.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Homeverse.Infrastructure.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasOne(x => x.Sender)
            .WithMany(x => x.MessagesSent)
            .HasForeignKey(x => x.SenderId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Receiver)
            .WithMany(x => x.MessagseReceived)
            .HasForeignKey(x => x.ReceiverId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
