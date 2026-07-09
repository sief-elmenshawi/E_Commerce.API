using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Authentications;
using E_Commerce.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Infrastructure.Identity.Services
{
    internal class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> userManager;

        public IdentityService(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }
        public async Task<Result<bool>> CheckPasswordAsync(string email, string password, CancellationToken ct = default)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Result<bool>.Fail(Error.NotFound("User not found", $"User with email {email} not found"));
            }
            else
                return await userManager.CheckPasswordAsync(user, password);
        }

        public async Task<Result<IdentityUserResult>> CreateUserAsync(RegisterDto registerDto, CancellationToken ct = default)
        {
            var user = new ApplicationUser()
            {
                Email = registerDto.Email,
                UserName = registerDto.UserName,
                DisplayName = registerDto.DisplayName,
                PhoneNumber = registerDto.PhoneNumber
            };
            var result = await userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => new Error(e.Code, e.Description)).ToList();
                return Result<IdentityUserResult>.Fail(errors);
            }
            
            return Result<IdentityUserResult>.Ok(new IdentityUserResult(user.Id, user.DisplayName, user.Email, user.UserName));
        }

        public async Task<Result<bool>> EmailExistsAsync(string email, CancellationToken ct = default)
        {
            return await userManager.FindByEmailAsync(email) is not null;
        }

        public async Task<Result<IdentityUserResult>> FindUserByEmailAsync(string email, CancellationToken ct = default)
        {
            var user =  await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Result<IdentityUserResult>.Fail(Error.NotFound("User not found", $"User with email {email} not found"));
            }
            else
            return Result<IdentityUserResult>.Ok(new IdentityUserResult(user.Id, user.DisplayName, user.Email, user.UserName));
        }

        public async Task<Result<AddressDto>> GetAddressByEmailAysnc(string email, CancellationToken ct = default)
        {
            var user = await userManager.Users.Include(u => u.Address).FirstOrDefaultAsync(u => u.Email == email, ct);
            
            if(user?.Address  == null)
                return Result<AddressDto>.Fail(Error.NotFound("Address not found", $"Address for user with email {email} not found"));
            
            var address = user.Address;
            return new AddressDto
            {
                City = address.City,
                Street = address.Street,
                Country = address.Country,
                FirstName = address.FirstName,
                LastName = address.LastName
            };
        }

        public async Task<Result<IReadOnlyList<string>>> GetUserRoles(string email, CancellationToken ct = default)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
                   return Error.NotFound("User not found", $"User with email {email} not found");

            var roles = await userManager.GetRolesAsync(user);

            return roles.ToList();
        }

        public async Task<Result<AddressDto>> UpdateOrInsertUserAddressAsync(string email, AddressDto addressDto, CancellationToken ct = default)
        {
            var user = await userManager.Users.Include(u => u.Address).FirstOrDefaultAsync(u => u.Email == email, ct);

            if(user?.Address == null)
            {
                // Insert new address
                user.Address = new Address
                {
                    City = addressDto.City,
                    Street = addressDto.Street,
                    Country = addressDto.Country,
                    FirstName = addressDto.FirstName,
                    LastName = addressDto.LastName
                };
            }
            else
            {
                // Update existing address
                user.Address.City = addressDto.City;
                user.Address.Street = addressDto.Street;
                user.Address.Country = addressDto.Country;
                user.Address.FirstName = addressDto.FirstName;
                user.Address.LastName = addressDto.LastName;
            }

            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => new Error(e.Code, e.Description)).ToList();
                return Result<AddressDto>.Fail(errors);
            }

            return Result<AddressDto>.Ok(addressDto);
        }
    }
}
