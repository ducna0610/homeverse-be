using Homeverse.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Homeverse.Infrastructure.Configurations;

public class ContactConfiguration : IEntityTypeConfiguration<Contact>
{
    public void Configure(EntityTypeBuilder<Contact> builder)
    {
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(x => x.Phone)
            .IsRequired()
            .HasMaxLength(11);

        builder.Property(x => x.Message)
            .IsRequired();
    }
}
