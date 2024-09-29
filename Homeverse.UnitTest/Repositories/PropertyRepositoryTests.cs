using Homeverse.Domain.Entities;
using Homeverse.Infrastructure.Data;
using Homeverse.Infrastructure.Repositories;
using Homeverse.UnitTest.Mocks;
using Microsoft.EntityFrameworkCore;

namespace Homeverse.UnitTest.Repositories;

public class PropertyRepositoryTests
{
    private async Task<HomeverseDbContext> SeedDatabaseContext()
    {
        var context = MockDbContext.CreateMockDbContext();
        var city1 = new City
        {
            Id = 1,
            Name = "City1",
        };
        var user1 = new User
        {
            Id = 1,
            Name = "user1",
            Email = "user1@gmail.com",
            Phone = "0123456789",
            EmailVerifyToken = "",
            PasswordHash = Convert.FromBase64String(""),
            PasswordSalt = Convert.FromBase64String(""),
        };
        var property1 = new Property
        {
            Id = 1,
            Title = "Title1",
            IsActive = true,
            Address = "Address1",
            Description = "Description1",
            CityId = city1.Id,
            PostedBy = user1.Id,
        };
        var property2 = new Property
        {
            Id = 2,
            Title = "Title2",
            IsActive = true,
            Address = "Address2",
            Description = "Description2",
            CityId = city1.Id,
            PostedBy = user1.Id,
        };
        var photo1 = new Photo
        {
            Id = 1,
            IsPrimary = true,
            ImageUrl = "url",
            PublicId = "publicId1",
            PropertyId = property1.Id,
        };
        var photo2 = new Photo
        {
            Id = 2,
            IsPrimary = false,
            ImageUrl = "url",
            PublicId = "publicId2",
            PropertyId = property1.Id,
        };
        var bookmark1 = new Bookmark
        {
            PropertyId = property1.Id,
            UserId = user1.Id,
        };
        await context.Cities.AddAsync(city1);
        await context.Users.AddAsync(user1);
        await context.Properties.AddAsync(property1);
        await context.Properties.AddAsync(property2);
        await context.Photos.AddAsync(photo1);
        await context.Photos.AddAsync(photo2);
        await context.Bookmarks.AddAsync(bookmark1);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        return context;
    }

    [Fact]
    public async Task GetPropertiesAsync_WhenSuccessful_ShouldReturnProperties()
    {
        // Arrange
        var context = await SeedDatabaseContext();
        var sut = new PropertyRepository(context);

        // Act
        var actual = await sut.GetPropertiesAsync();

        // Assert
        Assert.IsAssignableFrom<IEnumerable<Property>>(actual);
        Assert.Equal(context.Properties.Where(x => x.IsActive == true).Count(), actual.Count());
    }

    [Fact]
    public async Task GetAllPropertiesAsync_WhenSuccessful_ShouldReturnProperties()
    {
        // Arrange
        var context = await SeedDatabaseContext();
        var sut = new PropertyRepository(context);

        // Act
        var actual = await sut.GetAllPropertiesAsync();

        // Assert
        Assert.IsAssignableFrom<IEnumerable<Property>>(actual);
        Assert.Equal(context.Properties.Count(), actual.Count());
    }

    [Fact]
    public async Task GetAllPropertiesForUserAsync_WhenSuccessful_ShouldReturnProperties()
    {
        // Arrange
        var userId = 1;
        var context = await SeedDatabaseContext();
        var sut = new PropertyRepository(context);

        // Act
        var actual = await sut.GetAllPropertiesForUserAsync(userId);

        // Assert
        Assert.IsAssignableFrom<IEnumerable<Property>>(actual);
        Assert.Equal(context.Properties.Where(x => x.PostedBy == userId).Count(), actual.Count());
    }

    [Fact]
    public async Task GetPropertyByIdAsync_WhenSuccessful_ShouldReturnProperty()
    {
        // Arrange
        var id = 1;
        var context = await SeedDatabaseContext();
        var sut = new PropertyRepository(context);

        // Act
        var actual = await sut.GetPropertyByIdAsync(id);

        // Assert
        Assert.IsAssignableFrom<Property>(actual);
    }

    [Fact]
    public async Task GetPhotosByPropertyIdAsync_WhenSuccessful_ShouldReturnPhotos()
    {
        // Arrange
        var id = 1;
        var context = await SeedDatabaseContext();
        var sut = new PropertyRepository(context);

        // Act
        var actual = await sut.GetPhotosByPropertyIdAsync(id);

        // Assert
        Assert.IsAssignableFrom<IEnumerable<Photo>>(actual);
    }

    [Fact]
    public async Task AddPropertyAsync_WhenSuccessful_ShouldAddPhoto()
    {
        // Arrange
        var property = new Property
        {
            Id = 3,
            Title = "test",
            IsActive = true,
            Address = "test",
            Description = "test",
            CityId = 1,
            PostedBy = 1,
        };
        var context = await SeedDatabaseContext();
        var sut = new PropertyRepository(context);

        // Act
        await sut.AddPropertyAsync(property);
        await context.SaveChangesAsync();

        // Assert
        Assert.NotNull(await context.Properties.FirstOrDefaultAsync(x => x.Title == property.Title));
    }

    [Fact]
    public async Task SetPrimaryPhotoAsync_WhenSuccessful_ShouldReturnPhoto()
    {
        // Arrange
        var photoPublicId = "publicId2";
        var context = await SeedDatabaseContext();
        var sut = new PropertyRepository(context);

        // Act
        await sut.SetPrimaryPhotoAsync(photoPublicId);
        await context.SaveChangesAsync();

        // Assert
        Assert.NotNull(await context.Photos.FirstOrDefaultAsync(x => x.PublicId == photoPublicId && x.IsPrimary == true));
    }

    [Fact]
    public async Task UpdatePropertyAsync_WhenSuccessful_ShouldUpdateProperty()
    {
        // Arrange
        var property = new Property
        {
            Id = 1,
            Title = "test",
            IsActive = true,
            Address = "test",
            Description = "test",
            CityId = 1,
            PostedBy = 1,
        };
        var context = await SeedDatabaseContext();
        var sut = new PropertyRepository(context);

        // Act
        await sut.UpdatePropertyAsync(property);
        await context.SaveChangesAsync();

        // Assert
        Assert.NotNull(await context.Properties.FirstOrDefaultAsync(x => x.Title == property.Title));
    }

    [Fact]
    public async Task DeletePropertyAsync_WhenSuccessful_ShouldDeletePropety()
    {
        // Arrange
        var id = 1;
        var context = await SeedDatabaseContext();
        var sut = new PropertyRepository(context);

        // Act
        await sut.DeletePropertyAsync(id);
        await context.SaveChangesAsync();

        // Assert
        Assert.Null(await context.Properties.FindAsync(id));
    }

    [Fact]
    public async Task AddPhotoAsync_WhenSuccessful_ShouldAddPhoto()
    {
        // Arrange
        var photo = new Photo
        {
            Id = 3,
            IsPrimary = true,
            ImageUrl = "url",
            PublicId = "publicId3",
            PropertyId = 1,
        };
        var context = await SeedDatabaseContext();
        var sut = new PropertyRepository(context);

        // Act
        await sut.AddPhotoAsync(photo);
        await context.SaveChangesAsync();

        // Assert
        Assert.NotNull(await context.Photos.FirstOrDefaultAsync(x => x.PublicId == photo.PublicId));
    }

    [Fact]
    public async Task DeletePhotoAsync_WhenSuccessful_ShouldDeletePhoto()
    {
        // Arrange
        var photoPublicId = "publicId1";
        var context = await SeedDatabaseContext();
        var sut = new PropertyRepository(context);

        // Act
        await sut.DeletePhotoAsync(photoPublicId);
        await context.SaveChangesAsync();

        // Assert
        Assert.Null(await context.Photos.FirstOrDefaultAsync(x => x.PublicId == photoPublicId));
    }

    [Fact]
    public async Task GetBookmarksAsync_WhenSuccessful_ShouldReturnBookmarks()
    {
        // Arrange
        var userId = 1;
        var context = await SeedDatabaseContext();
        var sut = new PropertyRepository(context);

        // Act
        var actual = await sut.GetBookmarksAsync(userId);

        // Assert
        Assert.IsAssignableFrom<IEnumerable<Property>>(actual);
        Assert.Equal(context.Bookmarks.Count(), actual.Count());
    }

    [Fact]
    public async Task AddBookmarkAsync_WhenSuccessful_ShouldDeleteBookmark()
    {
        // Arrange
        var bookmark = new Bookmark 
        {
            UserId = 1,
            PropertyId = 2,
        };
        var context = await SeedDatabaseContext();
        var sut = new PropertyRepository(context);

        // Act
        await sut.AddBookmarkAsync(bookmark);
        await context.SaveChangesAsync();

        // Assert
        Assert.NotNull(await context.Bookmarks.FirstOrDefaultAsync(x => x.UserId == bookmark.UserId && x.PropertyId == bookmark.PropertyId));
    }

    [Fact]
    public async Task DeleteBookmarkAsync_WhenSuccessful_ShouldDeleteBookmark()
    {
        // Arrange
        var userId = 1;
        var propId = 1;
        var context = await SeedDatabaseContext();
        var sut = new PropertyRepository(context);

        // Act
        await sut.DeleteBookmarkAsync(userId, propId);
        await context.SaveChangesAsync();

        // Assert
        Assert.Null(await context.Bookmarks.FirstOrDefaultAsync(x => x.UserId == userId && x.PropertyId == propId));
    }
}
