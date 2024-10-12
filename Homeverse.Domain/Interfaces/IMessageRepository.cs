using Homeverse.Domain.Entities;

namespace Homeverse.Domain.Interfaces;

public interface IMessageRepository
{
    Task<IEnumerable<Message>> GetMessageThreadAsync(int userId, int otherId);
    Task<Message> GetMessageByIdAsync(int id);
    Task ReadMessageThreadAsync(int userId, int otherId);
    Task AddMessageAsync(Message message);
}
