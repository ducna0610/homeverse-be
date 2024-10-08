﻿using Homeverse.Domain.Entities;
using Homeverse.Infrastructure.Data;
using Homeverse.Infrastructure.Repositories;
using Homeverse.UnitTest.Mocks;
using Microsoft.EntityFrameworkCore;

namespace Homeverse.UnitTest.Repositories;

public class ContactRepositoryTests
{
    private async Task<HomeverseDbContext> SeedDatabaseContext()
    {
        var context = MockDbContext.CreateMockDbContext();
        var contact1 = new Contact
        {
            Id = 1,
            Name = "Duc senpai",
            Email = "contact1@gmail.com",
            Phone = "0123456789",
            Message = "Hello",
        };
        var contact2 = new Contact
        {
            Id = 2,
            Name = "Duc oppa",
            Email = "contact2@gmail.com",
            Phone = "0987654321",
            Message = "Hi",
        };
        await context.Contacts.AddAsync(contact1);
        await context.Contacts.AddAsync(contact2);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        return context;
    }

    [Fact]
    public async Task GetContactsAsync_WhenSuccessful_ShouldReturnContacts()
    {
        // Arrange
        var context = await SeedDatabaseContext();
        var sut = new ContactRepository(context);

        // Act
        var actual = await sut.GetContactsAsync();

        // Assert
        Assert.IsAssignableFrom<IEnumerable<Contact>>(actual);
        Assert.Equal(context.Contacts.Count(), actual.Count());
    }

    [Fact]
    public async Task GetContactByIdAsync_WhenSuccessful_ShouldReturnContact()
    {
        // Arrange
        var id = 1;
        var context = await SeedDatabaseContext();
        var sut = new ContactRepository(context);

        // Act
        var actual = await sut.GetContactByIdAsync(id);

        // Assert
        Assert.IsType<Contact>(actual);
    }

    [Fact]
    public async Task AddContactAsync_WhenSuccessful_ShouldAddContact()
    {
        // Arrange
        var contact = new Contact 
        { 
            Name = "test", 
            Email = "test@gmail.com", 
            Phone = "0123456789", 
            Message = "test" 
        };
        var context = await SeedDatabaseContext();
        var sut = new ContactRepository(context);

        // Act
        await sut.AddContactAsync(contact);
        await context.SaveChangesAsync();

        // Assert
        Assert.NotNull(await context.Contacts.FirstOrDefaultAsync(x => x.Name == contact.Name));
    }

    [Fact]
    public async Task DeleteContactAsync_WhenSuccessful_ShouldUpdateContact()
    {
        // Arrange
        var id = 1;
        var context = await SeedDatabaseContext();
        var sut = new ContactRepository(context);

        // Act
        await sut.DeleteContactAsync(id);
        await context.SaveChangesAsync();

        // Assert
        Assert.Null(await context.Contacts.FindAsync(id));
    }
}
