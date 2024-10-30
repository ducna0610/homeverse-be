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
                        .Include(x => x.Photos)
                        .Include(x => x.City)
                        .Include(x => x.User)
                        .Where(x => x.IsActive == true)
                        .OrderByDescending(x => x.CreatedAt)
                        .ToListAsync();
    }

    public async Task<IEnumerable<Property>> GetAllPropertiesAsync()
    {
        return await _context.Properties
                        .Include(x => x.Photos)
                        .Include(x => x.City)
                        .Include(x => x.User)
                        .OrderByDescending(x => x.CreatedAt)
                        .ToListAsync();
    }

    public async Task<IEnumerable<Property>> GetAllPropertiesForUserAsync(int userId)
    {
        return await _context.Properties
                        .Include(x => x.Photos)
                        .Include(x => x.City)
                        .Include(x => x.User)
                        .Where(p => p.User.Id == userId)
                        .OrderByDescending(x => x.CreatedAt)
                        .ToListAsync();
    }

    public async Task<Property> GetPropertyByIdAsync(int id)
    {
        return await _context.Properties
                        .Include(x => x.User)
                        .Include(x => x.Photos)
                        .Include(x => x.City)
                        .Where(x => x.Id == id)
                        .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Photo>> GetPhotosByPropertyIdAsync(int id)
    {
        return await _context.Photos
                        .Where(x => x.PropertyId == id)
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
        var photo = await _context.Photos.FirstAsync(x => x.PublicId == photoPublicId);
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
        var property = await _context.Properties.FirstOrDefaultAsync(x => x.Id == id);

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
                        .Include(x => x.Property.City)
                        .Include(x => x.Property.User)
                        .Include(x => x.Property.Photos)
                        .Where(x => x.UserId == userId)
                        .Where (x => x.Property.IsActive == true)
                        .Select(x => x.Property)
                        .ToListAsync();
    }

    public async Task AddBookmarkAsync(Bookmark bookmark)
    {
        await _context.Bookmarks.AddAsync(bookmark);
    }

    public async Task DeleteBookmarkAsync(int userId, int propId)
    {
        var bookmark = await _context.Bookmarks.FirstOrDefaultAsync(x => x.UserId == userId && x.PropertyId == propId);

        _context.Bookmarks.Remove(bookmark);
    }
}
