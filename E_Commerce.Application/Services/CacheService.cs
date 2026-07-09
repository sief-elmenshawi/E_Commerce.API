using E_Commerce.Application.Contracts;
using E_Commerce.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace E_Commerce.Application.Services
{
    internal class CacheService : ICacheService
    {
        private readonly ICacheRepository cacheRepository;

        public CacheService(ICacheRepository cacheRepository)
        {
            this.cacheRepository = cacheRepository;
        }
        public async Task<string?> GetDataAsync(string cacheKey, CancellationToken ct = default)
        {
            return await cacheRepository.GetAsync(cacheKey, ct);
        }

        public async Task SetDataAsync(string cacheKey, object cacheValue, TimeSpan? timeToLive = null, CancellationToken ct = default)
        {
            var json = JsonSerializer.Serialize(cacheValue, new JsonSerializerOptions 
            { 
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase, 
            });
            await cacheRepository.SetAsync(cacheKey, json, timeToLive, ct);
        }
    }
}
