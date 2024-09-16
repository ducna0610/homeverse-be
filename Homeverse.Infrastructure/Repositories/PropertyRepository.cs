using Homeverse.Domain.Entities;
using Homeverse.Domain.Interfaces;
using Homeverse.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Homeverse.Infrastructure.Repositories;

public class PropertyRepository : IPropertyRepository
{
    private readonly HomeverseDbContext _context;

    public PropertyRepository(HomeverseDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Property>> GetPropertiesAsync()
    {
        return await _context.Properties
                        .Include(p => p.Photos)
                        .Include(p => p.City)
                        .Include(p => p.User)
                        .Where(p => p.IsActive == true)
                        .ToListAsync();
    }

    public async Task<IEnumerable<Property>> GetAllPropertiesAsync()
    {
        return await _context.Properties
                        .Include(p => p.Photos)
                        .Include(p => p.City)
                        .Include(p => p.User)
                        .ToListAsync();
    }

    public async Task<IEnumerable<Property>> GetAllPropertiesForUserAsync(int userId)
    {
        return await _context.Properties
                        .Include(p => p.Photos)
                        .Include(p => p.City)
                        .Include(p => p.User)
                        .Where(p => p.User.Id == userId)
                        .ToListAsync();
    }

    public async Task<Property> GetPropertyByIdAsync(int id)
    {
        return await _context.Properties
                        .Include(p => p.User)
                        .Include(p => p.Photos)
                        .Include(p => p.City)
                        .Where(p => p.Id == id)
                        .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Photo>> GetPhotosByPropertyIdAsync(int id)
    {
        return await _context.Photos
                        .Where(p => p.PropertyId == id)
                        .ToListAsync();
    }

    public async Task AddPropertyAsync(Property property)
    {
        await _context.Properties.AddAsync(property);
    }

    public async Task AddPhotoAsync(Photo photo)
    {
        await _context.Photos.AddAsync(photo);
    }

    public async Task<Photo> SetPrimaryPhotoAsync(string photoPublicId)
    {
        var photo = await _context.Photos.FirstAsync(p => p.PublicId == photoPublicId);
        var currentPrimary = await _context.Photos.FirstAsync(x => x.PropertyId == photo.PropertyId && x.IsPrimary);
        currentPrimary.IsPrimary = false;
        photo.IsPrimary = true;

        _context.Photos.UpdateRange(photo, currentPrimary);
        return photo;
    }

    public async Task UpdatePropertyAsync(Property property)
    {
        _context.Properties.Update(property);
    }

    public async Task DeletePropertyAsync(int id)
    {
        var property = await _context.Properties.FirstOrDefaultAsync(p => p.Id == id);

        _context.Properties.Remove(property);
    }

    public async Task DeletePhotoAsync(string photoPublicId)
    {
        var photo = await _context.Photos.FirstAsync(x => x.PublicId == photoPublicId);

        _context.Photos.Remove(photo);
    }

    public async Task<IEnumerable<Property>> GetBookmarksAsync(int userId)
    {
        return await _context.Bookmarks
                        .Include(b => b.Property.City)
                        .Include(b => b.Property.User)
                        .Include(b => b.Property.Photos)
                        .Where(b => b.UserId == userId)
                        .Select(b => b.Property)
                        .ToListAsync();
    }

    public async Task AddBookmarkAsync(Bookmark bookmark)
    {
        await _context.Bookmarks.AddAsync(bookmark);
    }

    public async Task DeleteBookmarkAsync(int userId, int propId)
    {
        var bookmark = await _context.Bookmarks.FirstOrDefaultAsync(b => b.UserId == userId && b.PropertyId == propId);

        _context.Bookmarks.Remove(bookmark);
    }
}
