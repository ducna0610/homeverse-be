﻿using Homeverse.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Homeverse.Infrastructure.Services;

public class CacheService : ICacheService
{

    private readonly IDistributedCache _cache;

    public CacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<T> GetDataAsync<T>(string key)
    {
        var data = await _cache.GetStringAsync(key);
        if (string.IsNullOrEmpty(data))
        {
            return default;
        }

        return JsonConvert.DeserializeObject<T>(data);
    }

    public async Task SetDataAsync<T>(string key, T value)
    {
        var options = new DistributedCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7)
        };

        await _cache.SetStringAsync(key, JsonConvert.SerializeObject(value), options);
    }

    public async Task RemoveDataAsync(string key)
    {
        await _cache.RemoveAsync(key);
    }
}