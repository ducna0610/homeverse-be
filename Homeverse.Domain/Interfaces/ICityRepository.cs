using Homeverse.Domain.Entities;

namespace Homeverse.Domain.Interfaces;

public interface ICityRepository
{
    Task<IEnumerable<City>> GetCitiesAsync();
    Task<City> GetCityByIdAsync(int id);
    Task AddCityAsync(City city);
    Task UpdateCityAsync(City city);
    Task DeleteCityAsync(int id);
}