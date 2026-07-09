using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Domain.Contracts
{
    public interface ICacheRepository
    {
        Task<string?> GetAsync(string cacheKey,CancellationToken ct = default);

        Task SetAsync(string cacheKey,string cacheValue,TimeSpan? timeToLive = null,CancellationToken ct = default);
    }
}
