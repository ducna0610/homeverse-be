using Homeverse.Domain.Entities;
using Homeverse.Infrastructure.Data;
using Homeverse.Infrastructure.Repositories;
using Homeverse.UnitTest.Mocks;
using Microsoft.EntityFrameworkCore;

namespace Homeverse.UnitTest.Repositories;

public class CityRepositoryTests
{
    private async Task<HomeverseDbContext> SeedDatabaseContext()
    {
        var context = MockDbContext.CreateMockDbContext();
        var city1 = new City
        {
            Id = 1,
            Name = "City1",
        };
        var city2 = new City
        {
            Id = 2,
            Name = "City2",
        };
        await context.Cities.AddAsync(city1);
        await context.Cities.AddAsync(city2);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        return context;
    }

    [Fact]
    public async Task GetCitiesAsync_WhenSuccessful_ShouldReturnCities()
    {
        // Arrange
        var context = await SeedDatabaseContext();
        var sut = new CityRepository(context);

        // Act
        var actual = await sut.GetCitiesAsync();

        // Assert
        Assert.IsAssignableFrom<IEnumerable<City>>(actual); 
        Assert.Equal(context.Cities.Count(), actual.Count());
    }

    [Fact]
    public async Task GetCityByIdAsync_WhenSuccessful_ShouldReturnCity()
    {
        // Arrange
        var id = 1;
        var context = await SeedDatabaseContext();
        var sut = new CityRepository(context);

        // Act
        var actual = await sut.GetCityByIdAsync(id);

        // Assert
        Assert.IsType<City>(actual);
    }

    [Fact]
    public async Task AddCityAsync_WhenSuccessful_ShouldAddCity()
    {
        // Arrange
        var city = new City 
        { 
            Name = "test",
        };
        var context = await SeedDatabaseContext();
        var sut = new CityRepository(context);

        // Act
        await sut.AddCityAsync(city);
        await context.SaveChangesAsync();

        // Assert
        Assert.NotNull(await context.Cities.FirstOrDefaultAsync(x => x.Name == city.Name));
    }

    [Fact]
    public async Task UpdateCityAsync_WhenSuccessful_ShouldUpdateCity()
    {
        // Arrange
        var city = new City 
        { 
            Id = 1, 
            Name = "test",
        };
        var context = await SeedDatabaseContext();
        var sut = new CityRepository(context);

        // Act
        await sut.UpdateCityAsync(city);
        await context.SaveChangesAsync();

        // Assert
        Assert.NotNull(await context.Cities.FirstOrDefaultAsync(x => x.Name == city.Name));
    }

    [Fact]
    public async Task DeleteCityAsync_WhenSuccessful_ShouldDeleteCity()
    {
        // Arrange
        var id = 1;
        var context = await SeedDatabaseContext();
        var sut = new CityRepository(context);

        // Act
        await sut.DeleteCityAsync(id);
        await context.SaveChangesAsync();

        // Assert
        Assert.Null(await context.Cities.FindAsync(id));
    }
}
