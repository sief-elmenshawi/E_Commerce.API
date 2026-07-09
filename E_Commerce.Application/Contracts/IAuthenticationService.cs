using E_Commerce.Application.Common;
using E_Commerce.Application.DTOs.Authentications;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.Contracts
{
    public interface IAuthenticationService
    {
        // Login
        // Email + Password => Token , Email , DisplayName

        Task<Result<UserDto>> LoginAsync(LoginDto loginDto,CancellationToken ct = default); 
        Task<Result<UserDto>> RegisterAsync(RegisterDto registerDto,CancellationToken ct = default);

        Task<Result<bool>> CheckEmailExistAsync(string email, CancellationToken ct = default);
        Task<Result<UserDto>> GetCurrentUserAsync(string email, CancellationToken ct = default);
        Task<Result<AddressDto>> GetUserAddressAsync(string email, CancellationToken ct = default);
        Task<Result<AddressDto>> UpsertUserAddressAsync(string email, AddressDto addressDto, CancellationToken ct = default);

    }
}
