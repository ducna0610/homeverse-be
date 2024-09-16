using Homeverse.Domain.Entities;
using Homeverse.Domain.Interfaces;
using Homeverse.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Homeverse.Infrastructure.Repositories;

public class ContactRepository : IContactRepository
{
    private readonly HomeverseDbContext _context;

    public ContactRepository(HomeverseDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Contact>> GetContactsAsync()
    {
        return await _context.Contacts.ToListAsync();
    }

    public async Task AddContactAsync(Contact contact)
    {
        await _context.Contacts.AddAsync(contact);
    }

    public async Task DeleteContactAsync(int id)
    {
        var contact = await _context.Contacts.FindAsync(id);
        _context.Contacts.Remove(contact);
    }
}
