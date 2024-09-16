using Homeverse.Domain.Entities;

namespace Homeverse.Domain.Interfaces;

public interface IMessageRepository
{
    Task<IEnumerable<Message>> GetMessageThreadAsync(int userId, int otherId);
    Task ReadMessageThreadAsync(int userId, int otherId);
    Task AddMessageAsync(Message message);
}
