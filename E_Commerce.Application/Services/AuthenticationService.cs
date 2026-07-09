using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Authentications;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.Services
{
    internal class AuthenticationService : IAuthenticationService
    {
        private readonly IIdentityService identityService;
        private readonly ITokenService tokenService;

        public AuthenticationService(IIdentityService identityService , ITokenService tokenService)
        {
            this.identityService = identityService;
            this.tokenService = tokenService;
        }

        public async Task<Result<bool>> CheckEmailExistAsync(string email, CancellationToken ct = default)
        {
            return await identityService.EmailExistsAsync(email, ct);
        }

        public async Task<Result<UserDto>> GetCurrentUserAsync(string email, CancellationToken ct = default)
        {
            var userResult = await identityService.FindUserByEmailAsync(email, ct);

            var user = userResult.data;
            var rolesResult = await identityService.GetUserRoles(user.Email, ct);
            var token = tokenService.CreateToken(user.Id, user.UserName, user.Email, rolesResult.data);

            return new UserDto
            {
                Email = user.Email,
                DisplayName = user.DisplayName,
                Token = token
            };

        }

        public Task<Result<AddressDto>> GetUserAddressAsync(string email, CancellationToken ct = default)
        {
            return identityService.GetAddressByEmailAysnc(email, ct);
        }

        public async Task<Result<UserDto>> LoginAsync(LoginDto loginDto, CancellationToken ct = default)
        {
            // Get User By Email
            var userResult = await identityService.FindUserByEmailAsync(loginDto.Email, ct);
            if(!userResult.IsSuccess)
                return Result<UserDto>.Fail(userResult.Errors);

            // Check Password
            var passwordResult = await identityService.CheckPasswordAsync(loginDto.Email,loginDto.Password, ct);
            if(!passwordResult.IsSuccess)
                return Result<UserDto>.Fail(passwordResult.Errors);
            if (!passwordResult.data)
                return Result<UserDto>.Fail(Error.Unauthorized("Invalid email or password"));

            var user = userResult.data;
            var rolesResult = await identityService.GetUserRoles(user.Email, ct);
            var roles = rolesResult.IsSuccess ? rolesResult.data : new List<string>();
            var token =  tokenService.CreateToken(user.Id, user.UserName, user.Email,roles);
            return new UserDto
            {
                Email = loginDto.Email,
                DisplayName = userResult.data.DisplayName,
                Token = token
            };  
            // return Result + userDto


        }

        public async Task<Result<UserDto>> RegisterAsync(RegisterDto registerDto, CancellationToken ct = default)
        {
            var userResult = await identityService.CreateUserAsync(registerDto, ct);
            if (!userResult.IsSuccess)
            {
                return Result<UserDto>.Fail(userResult.Errors);
            }
            var user = userResult.data;
            var rolesResult = await identityService.GetUserRoles(user.Email, ct);
            var roles = rolesResult.IsSuccess ? rolesResult.data : new List<string>();
            var token = tokenService.CreateToken(user.Id, user.UserName, user.Email, roles);
            return Result<UserDto>.Ok(new UserDto()
            {
                Email = user.Email,
                DisplayName = user.DisplayName,
                Token = token
            });
          
        }

        public async Task<Result<AddressDto>> UpsertUserAddressAsync(string email, AddressDto addressDto, CancellationToken ct = default)
        {
            return await identityService.UpdateOrInsertUserAddressAsync(email, addressDto, ct);
        }
    }
}
