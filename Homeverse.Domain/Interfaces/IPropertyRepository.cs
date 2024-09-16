using Homeverse.Domain.Entities;

namespace Homeverse.Domain.Interfaces;

public interface IPropertyRepository
{
    Task<IEnumerable<Property>> GetPropertiesAsync();
    Task<IEnumerable<Property>> GetAllPropertiesAsync();
    Task<IEnumerable<Property>> GetAllPropertiesForUserAsync(int userId);
    Task<Property> GetPropertyByIdAsync(int id);
    Task<IEnumerable<Photo>> GetPhotosByPropertyIdAsync(int id);
    Task AddPropertyAsync(Property property);
    Task UpdatePropertyAsync(Property property);
    Task DeletePropertyAsync(int id);
    Task AddPhotoAsync(Photo photo);
    Task<Photo> SetPrimaryPhotoAsync(string photoPublicId);
    Task DeletePhotoAsync(string photoPublicId);
    Task<IEnumerable<Property>> GetBookmarksAsync(int userId);
    Task AddBookmarkAsync(Bookmark bookmark);
    Task DeleteBookmarkAsync(int userId, int propId);
}