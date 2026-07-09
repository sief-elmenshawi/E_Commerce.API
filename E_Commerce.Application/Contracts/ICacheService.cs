using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.Contracts
{
    public interface ICacheService
    {
        Task<string?> GetDataAsync(string cacheKey,CancellationToken ct = default);

        Task SetDataAsync(string cacheKey, object cacheValue, TimeSpan? timeToLive = null, CancellationToken ct = default);

    }
}
