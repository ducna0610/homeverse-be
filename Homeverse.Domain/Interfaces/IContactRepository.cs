using Homeverse.Domain.Entities;

namespace Homeverse.Domain.Interfaces;

public interface IContactRepository
{
    Task<IEnumerable<Contact>> GetContactsAsync();
    Task AddContactAsync(Contact contact);
    Task DeleteContactAsync(int id);
}
