using Homeverse.Domain.Entities;
using Homeverse.Domain.Interfaces;
using Homeverse.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Homeverse.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly HomeverseDbContext _context;

    public UserRepository(HomeverseDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<User>> GetUsersAsync()
    {
        return await _context.Users.Include(x => x.Properties).OrderByDescending(x => x.CreatedAt).ToListAsync();
    }

    public async Task<User> GetUserByIdAsync(int id)
    {
        return await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task AddUserAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }

    public async Task UpdateUserAsync(User user)
    {
        _context.Users.Update(user);
    }

    public async Task<IEnumerable<string>> GetConnectionIdsByUserIdAsync(int userId)
    {
        return await _context.Connections.Where(x => x.UserId == userId).Select(x => x.ConnectionId).ToListAsync();
    }

    public async Task<IEnumerable<int>> GetFriendIdsAsync(int userId)
    {
        var senderIds = await _context.Messages
                        .Where(m => m.ReceiverId == userId)
                        .Select(x => x.SenderId).ToListAsync();
        var receiverIds = await _context.Messages
                        .Where(m => m.SenderId == userId)
                        .Select(x => x.ReceiverId).ToListAsync();

        return senderIds.Union(receiverIds)
                        .Where(x => x != userId)
                        .ToList();
    }

    public async Task<IEnumerable<User>> GetFriendsAsync(int userId)
    {
        var friendIds = await GetFriendIdsAsync(userId);
        return await _context.Users
                        .Include(x => x.Connections)
                        .Include(u => u.MessagesSent.Where(m => m.ReceiverId == userId))
                        .Where(x => friendIds.Contains(x.Id))
                        .ToListAsync();
    }

    public async Task<User> GetFriendAsync(int userId, int otherId)
    {
        return await _context.Users
                        .Include(x => x.Connections)
                        .Include(u => u.MessagesSent.Where(m => m.ReceiverId == otherId))
                        .Where(x => x.Id == userId)
                        .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<string>> GetFriendConnectionIdsAsync(int userId)
    {
        var friendIds = await GetFriendIdsAsync(userId);
        return await _context.Connections
                        .Where(c => friendIds.Contains(c.UserId))
                        .Select(x => x.ConnectionId)
                        .ToListAsync();
    }

    public async Task AddConnectionAsync(Connection connection)
    {
        await _context.Connections.AddAsync(connection);
    }

    public async Task DeleteConnectionAsync(string connectionId)
    {
        var connection = await _context.Connections.Where(x => x.ConnectionId == connectionId).FirstOrDefaultAsync();
        _context.Connections.Remove(connection);
    }
}
