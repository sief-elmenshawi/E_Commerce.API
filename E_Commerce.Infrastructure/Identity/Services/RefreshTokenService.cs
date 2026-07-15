using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Security.Cryptography;
using System.Text;

namespace E_Commerce.Infrastructure.Identity.Services
{
    internal class RefreshTokenService : IRefreshTokenService
    {
        private readonly UserManager<ApplicationUser> userManager;

        public RefreshTokenService(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }
        public async Task<Result<string>> CreateAsync(string email, CancellationToken ct = default)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user is null)
            {
                return Result<string>.Fail(
                    Error.NotFound("UserNotFound", "User Not Found"));
            }

            var refreshToken = Convert.ToBase64String(
                RandomNumberGenerator.GetBytes(64));

            user.RefreshTokenHash = HashToken(refreshToken);
            user.RefreshTokenExpiresOn = DateTime.UtcNow.AddDays(7);

            var updateResult = await userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                var errors = updateResult.Errors
                    .Select(error => new Error(error.Code, error.Description))
                    .ToList();

                return Result<string>.Fail(errors);
            }

            return Result<string>.Ok(refreshToken);
        }

        public async Task<Result<string>> GetEmailAsync(string refreshToken, CancellationToken ct = default)
        {
            var refreshTokenHash = HashToken(refreshToken);

            var user = await userManager.Users.FirstOrDefaultAsync(
                user => user.RefreshTokenHash == refreshTokenHash, ct);

            if (user is null ||user.RefreshTokenExpiresOn is null || user.RefreshTokenExpiresOn <= DateTime.UtcNow)
            {
                return Result<string>.Fail(
                    Error.Unauthorized("InvalidRefreshToken", "The RefreshToken is invalid or expired.."));
            }

            return Result<string>.Ok(user.Email!);
        }

        public async Task<Result> RevokeAsync(string email, CancellationToken ct = default)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user is null)
            {
                return Result.Fail(
                    Error.NotFound("UserNotFound", "User not found."));
            }

            user.RefreshTokenHash = null;
            user.RefreshTokenExpiresOn = null;

            var updateResult = await userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                var errors = updateResult.Errors
                    .Select(error => new Error(error.Code, error.Description))
                    .ToList();

                return Result.Fail(errors);
            }

            return Result.OK();
        }
        private static string HashToken(string refreshToken)
        {
            var bytes = SHA256.HashData(
                Encoding.UTF8.GetBytes(refreshToken));

            return Convert.ToHexString(bytes);
        }
    }

}
