namespace Homeverse.Application.Interfaces;

public interface ICacheService
{
    Task<T> GetDataAsync<T>(string key);

    Task SetDataAsync<T>(string key, T value);

    Task RemoveDataAsync(string key);
}
