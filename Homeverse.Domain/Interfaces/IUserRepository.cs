using Homeverse.Domain.Entities;

namespace Homeverse.Domain.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetUsersAsync();
    Task<User> GetUserByIdAsync(int id);
    Task<User> GetUserByEmailAsync(string email);
    Task AddUserAsync(User user);
    Task UpdateUserAsync(User user);
    Task<IEnumerable<string>> GetConnectionIdsByUserIdAsync(int userId);
    Task<IEnumerable<User>> GetFriendsAsync(int userId);
    Task<User> GetFriendAsync(int userId, int otherId);
    Task<IEnumerable<int>> GetFriendIdsAsync(int userId);
    Task<IEnumerable<string>> GetFriendConnectionIdsAsync(int userId);
    Task AddConnectionAsync(Connection connection);
    Task DeleteConnectionAsync(string connectionId);
}
