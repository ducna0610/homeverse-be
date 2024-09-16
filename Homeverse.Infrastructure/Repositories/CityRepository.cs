using Homeverse.Domain.Entities;
using Homeverse.Domain.Interfaces;
using Homeverse.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Homeverse.Infrastructure.Repositories;

public class CityRepository : ICityRepository
{
    private readonly HomeverseDbContext _context;

    public CityRepository(HomeverseDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<City>> GetCitiesAsync()
    {
        return await _context.Cities.ToListAsync();
    }

    public async Task<City> GetCityByIdAsync(int id)
    {
        return await _context.Cities.FindAsync(id);
    }

    public async Task AddCityAsync(City city)
    {
        await _context.Cities.AddAsync(city);
    }

    public async Task UpdateCityAsync(City city)
    {
        _context.Cities.Update(city);
    }

    public async Task DeleteCityAsync(int id)
    {
        var city = await _context.Cities.FindAsync(id);
        _context.Cities.Remove(city);
    }
}
