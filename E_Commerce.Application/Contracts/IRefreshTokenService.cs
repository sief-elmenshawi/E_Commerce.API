using E_Commerce.Application.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.Contracts
{
    public interface IRefreshTokenService
    {
        Task<Result<string>> CreateAsync(string email, CancellationToken ct = default); 

        Task<Result<string>> GetEmailAsync(string refreshToken,CancellationToken ct = default);

        Task<Result> RevokeAsync(string email,CancellationToken ct = default);
    }
}
