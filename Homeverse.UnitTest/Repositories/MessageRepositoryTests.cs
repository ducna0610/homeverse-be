using Homeverse.Domain.Entities;
using Homeverse.Infrastructure.Data;
using Homeverse.Infrastructure.Repositories;
using Homeverse.UnitTest.Mocks;
using Microsoft.EntityFrameworkCore;

namespace Homeverse.UnitTest.Repositories;

public class MessageRepositoryTests
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
            Content = "message1",
            SenderId = user1.Id,
            Sender = user1,
            ReceiverId = user2.Id,
            Receiver = user2,
            IsReaded = true,
        };
        var message2 = new Message
        {
            Id = 2,
            Content = "message2",
            SenderId = user1.Id,
            Sender = user1,
            ReceiverId = user2.Id,
            Receiver = user2,
        };
        await context.Users.AddAsync(user1);
        await context.Users.AddAsync(user2);
        await context.Users.AddAsync(user3);
        await context.Messages.AddAsync(message1);
        await context.Messages.AddAsync(message2);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        return context;
    }

    [Fact]
    public async Task GetMessageThreadAsync_WhenSuccessful_ShouldReturnMessages()
    {
        // Arrange
        var userId = 1;
        var otherId = 2;
        var context = await SeedDatabaseContext();
        var sut = new MessageRepository(context);

        // Act
        var actual = await sut.GetMessageThreadAsync(userId, otherId);

        // Assert
        Assert.IsAssignableFrom<IEnumerable<Message>>(actual);
        Assert.Equal(context.Messages.Where(x => (x.SenderId == userId && x.ReceiverId == otherId) || (x.SenderId == otherId && x.ReceiverId == userId)).Count(), actual.Count());
    }

    [Fact]
    public async Task GetMessageByIdAsync_WhenSuccessful_ShouldReturnMessage()
    {
        // Arrange
        var id = 1;
        var context = await SeedDatabaseContext();
        var sut = new MessageRepository(context);

        // Act
        var actual = await sut.GetMessageByIdAsync(id);

        // Assert
        Assert.IsType<Message>(actual);
    }

    [Fact]
    public async Task AddMessageAsync_WhenSuccessful_ShouldAddMessage()
    {
        // Arrange
        var message = new Message
        {
            Content = "test",
            SenderId = 1,
            ReceiverId = 2,
        };
        var context = await SeedDatabaseContext();
        var sut = new MessageRepository(context);

        // Act
        await sut.AddMessageAsync(message);
        await context.SaveChangesAsync();

        // Assert
        Assert.NotNull(await context.Messages.FirstOrDefaultAsync(x => x.Content == message.Content 
                                                                    && x.SenderId == message.SenderId 
                                                                    && x.ReceiverId == message.ReceiverId));
    }

    [Fact(Skip = "These new extensions only work on relational providers. And InMemory is not a relational provider")]
    public async Task ReadMessageThreadAsync_WhenSuccessful_ShouldReadMessage()
    {
        // Arrange
        var userId = 2;
        var otherId = 1;
        var context = await SeedDatabaseContext();
        var sut = new MessageRepository(context);

        // Act
        await sut.ReadMessageThreadAsync(userId, otherId);

        // Assert
        Assert.True(context.Messages.Where(x => x.SenderId == otherId && x.ReceiverId == userId).All(x => x.IsReaded == true));
    }
}
