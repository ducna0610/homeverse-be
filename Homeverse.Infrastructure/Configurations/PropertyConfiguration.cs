using Homeverse.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Homeverse.Infrastructure.Configurations;

public class PropertyConfiguration : IEntityTypeConfiguration<Property>
{
    public void Configure(EntityTypeBuilder<Property> builder)
    {
        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Price)
            .IsRequired()
            .HasColumnType("money");

        builder.Property(x => x.Area)
            .IsRequired();

        builder.Property(x => x.Address)
            .IsRequired();

        builder.Property(x => x.Description)
            .IsRequired();

        builder.Property(x => x.CategoryId)
            .IsRequired()
            .HasColumnType("tinyint");

        builder.Property(x => x.FurnishId)
            .IsRequired()
            .HasColumnType("tinyint");

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.HasOne(x => x.City)
            .WithMany(x => x.Properties)
            .HasForeignKey(x => x.CityId);

        builder.HasOne(x => x.User)
            .WithMany(x => x.Properties)
            .HasForeignKey(x => x.PostedBy);
    }
}
