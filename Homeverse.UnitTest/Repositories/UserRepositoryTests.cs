using Homeverse.Domain.Entities;
using Homeverse.Infrastructure.Data;
using Homeverse.Infrastructure.Repositories;
using Homeverse.UnitTest.Mocks;
using Microsoft.EntityFrameworkCore;

namespace Homeverse.UnitTest.Repositories;

public class UserRepositoryTests
{
    private async Task<HomeverseDbContext> SeedDatabaseContext()
    {
        var context = MockDbContext.CreateMockDbContext();
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
        user1.Connections = new List<Connection>();
        user1.Connections.Add(new Connection
        {
            ConnectionId = "xxx"
        });
        var user2 = new User 
        { 
            Id = 2, 
            Name = "user2", 
            Email = "user2@gmail.com", 
            Phone = "0987654321", 
            EmailVerifyToken = "", 
            PasswordHash = Convert.FromBase64String(""), 
            PasswordSalt = Convert.FromBase64String(""),
        };
        var user3 = new User
        {
            Id = 3,
            Name = "user3",
            Email = "user3@gmail.com",
            Phone = "0246813579",
            EmailVerifyToken = "",
            PasswordHash = Convert.FromBase64String(""),
            PasswordSalt = Convert.FromBase64String(""),
        };
        var message1 = new Message
        {
            Id = 1,
            SenderId = user1.Id,
            Sender = user1,
            ReceiverId = user2.Id,
            Receiver = user2,
            Content = "Hi",
        };
        await context.Users.AddAsync(user1);
        await context.Users.AddAsync(user2);
        await context.Users.AddAsync(user3);
        await context.Messages.AddAsync(message1);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        return context;
    }

    [Fact]
    public async Task GetUsersAsync_WhenSuccessful_ShouldReturnUsers()
    {
        // Arrange
        var context = await SeedDatabaseContext();
        var sut = new UserRepository(context);

        // Act
        var actual = await sut.GetUsersAsync();

        // Assert
        Assert.IsAssignableFrom<IEnumerable<User>>(actual);
        Assert.Equal(context.Users.Count(), actual.Count());
    }

    [Fact]
    public async Task GetUserByIdAsync_WhenSuccessful_ShouldReturnUser()
    {
        // Arrange
        var id = 1;
        var context = await SeedDatabaseContext();
        var sut = new UserRepository(context);

        // Act
        var actual = await sut.GetUserByIdAsync(id);

        // Assert
        Assert.IsType<User>(actual);
    }

    [Fact]
    public async Task GetUserByEmailAsync_WhenSuccessful_ShouldReturnUser()
    {
        // Arrange
        var email = "user1@gmail.com";
        var context = await SeedDatabaseContext();
        var sut = new UserRepository(context);

        // Act
        var actual = await sut.GetUserByEmailAsync(email);

        // Assert
        Assert.IsType<User>(actual);
    }

    [Fact]
    public async Task AddUserAsync_WhenSuccessful_ShouldAddUser()
    {
        // Arrange
        var user = new User 
        { 
            Name = "test", 
            Email = "test@gmail.com", 
            Phone = "0102030405", 
            EmailVerifyToken = "", 
            PasswordHash = Convert.FromBase64String(""), 
            PasswordSalt = Convert.FromBase64String(""),
        };
        var context = await SeedDatabaseContext();
        var sut = new UserRepository(context);

        // Act
        await sut.AddUserAsync(user);
        await context.SaveChangesAsync();

        // Assert
        Assert.NotNull(await context.Users.FirstOrDefaultAsync(x => x.Name == user.Name));
    }

    [Fact]
    public async Task UpdateUserAsync_WhenSuccessful_ShouldUpdateUser()
    {
        // Arrange
        var user = new User 
        { 
            Id = 1, 
            Name = "test", 
            Email = "test@gmail.com", 
            Phone = "0102030405", 
            EmailVerifyToken = "", 
            PasswordHash = Convert.FromBase64String(""), 
            PasswordSalt = Convert.FromBase64String(""),
        };
        var context = await SeedDatabaseContext();
        var sut = new UserRepository(context);

        // Act
        await sut.UpdateUserAsync(user);
        await context.SaveChangesAsync();

        // Assert
        Assert.NotNull(await context.Users.FirstOrDefaultAsync(x => x.Name == user.Name));
    }

    [Fact]
    public async Task GetConnectionIdsByUserIdAsync_WhenSuccessful_ShouldReturnConnectionIds()
    {
        // Arrange
        var userId = 1;
        var context = await SeedDatabaseContext();
        var sut = new UserRepository(context);

        // Act
        var actual = await sut.GetConnectionIdsByUserIdAsync(userId);

        // Assert
        Assert.IsAssignableFrom<IEnumerable<string>>(actual);
    }
    
    [Fact]
    public async Task GetFriendsAsync_WhenSuccessful_ShouldReturnFriends()
    {
        // Arrange
        var userId = 1;
        var context = await SeedDatabaseContext();
        var sut = new UserRepository(context);

        // Act
        var actual = await sut.GetFriendsAsync(userId);

        // Assert
        Assert.IsAssignableFrom<IEnumerable<User>>(actual);
        Assert.NotNull(actual.Where(x => x.Id == 2));
    }

    [Fact]
    public async Task GetFriendAsync_WhenSuccessful_ShouldReturnFriend()
    {
        // Arrange
        var userId = 1;
        var otherId = 2;
        var context = await SeedDatabaseContext();
        var sut = new UserRepository(context);

        // Act
        var actual = await sut.GetFriendAsync(userId, otherId);

        // Assert
        Assert.IsAssignableFrom<User>(actual);
        Assert.NotNull(actual);
    }

    [Fact]
    public async Task GetFriendConnectionIdsAsync_WhenSuccessful_ShouldReturnConnectionIds()
    {
        // Arrange
        var userId = 2;
        var context = await SeedDatabaseContext();
        var sut = new UserRepository(context);

        // Act
        var actual = await sut.GetFriendConnectionIdsAsync(userId);

        // Assert
        Assert.IsAssignableFrom<IEnumerable<string>>(actual);
        Assert.NotNull(actual);
    }

    [Fact]
    public async Task AddConnectionAsync_WhenSuccessful_ShouldAddConnectionId()
    {
        // Arrange
        var connection = new Connection
        {
            ConnectionId = "test",
            UserId = 1,
        };
        var context = await SeedDatabaseContext();
        var sut = new UserRepository(context);

        // Act
        await sut.AddConnectionAsync(connection);
        await context.SaveChangesAsync();

        // Assert
        Assert.NotNull((await context.Users.FindAsync(connection.UserId)).Connections.FirstOrDefault(c => c.ConnectionId == connection.ConnectionId, null));
    }

    [Fact]
    public async Task DeleteConnectionAsync_WhenSuccessful_ShouldDeleteConnectionId()
    {
        // Arrange
        var connectionId = "xxx";
        var context = await SeedDatabaseContext();
        var sut = new UserRepository(context);

        // Act
        await sut.DeleteConnectionAsync(connectionId);
        await context.SaveChangesAsync();

        // Assert
        Assert.Null(await context.Connections.Where(x => x.ConnectionId == connectionId).FirstOrDefaultAsync());
    }
}
