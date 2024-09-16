using Homeverse.Domain.Entities;
using Homeverse.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Homeverse.Infrastructure.Data;

public class HomeverseDbContext : DbContext
{
    public HomeverseDbContext(DbContextOptions<HomeverseDbContext> options) : base(options) { }

    #region DbSet
    public DbSet<User> Users { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<Property> Properties { get; set; }
    public DbSet<Photo> Photos { get; set; }
    public DbSet<Bookmark> Bookmarks { get; set; }
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<Connection> Connections { get; set; }
    public DbSet<Message> Messages { get; set; }
    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new CityConfiguration());
        modelBuilder.ApplyConfiguration(new PropertyConfiguration());
        modelBuilder.ApplyConfiguration(new PhotoConfiguration());
        modelBuilder.ApplyConfiguration(new BookmarkConfiguration());
        modelBuilder.ApplyConfiguration(new ContactConfiguration());
        modelBuilder.ApplyConfiguration(new ConnectionConfiguration());
        modelBuilder.ApplyConfiguration(new MessageConfiguration());
    }
}
