using E_Commerce.Application.Common;
using E_Commerce.Application.DTOs.Authentications;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.Contracts
{
    public interface IIdentityService
    {
        Task<Result<IdentityUserResult>> FindUserByEmailAsync(string email , CancellationToken ct = default);
        Task<Result<bool>> CheckPasswordAsync(string email, string password, CancellationToken ct = default);
        Task<Result<IdentityUserResult>> CreateUserAsync( RegisterDto registerDto, CancellationToken ct = default);
        Task<Result<IReadOnlyList<string>>> GetUserRoles(string email, CancellationToken ct = default);

        Task<Result<bool>> EmailExistsAsync(string email, CancellationToken ct = default);
        Task<Result<AddressDto>> GetAddressByEmailAysnc(string email, CancellationToken ct = default);
        Task<Result<AddressDto>> UpdateOrInsertUserAddressAsync(string email, AddressDto addressDto, CancellationToken ct = default);

    }
}
