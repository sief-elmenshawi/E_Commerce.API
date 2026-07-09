using E_Commerce.Application.Contracts;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace E_Commerce.Infrastructure.Identity.Services
{
    internal class TokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;
        public TokenService(IOptions<JwtSettings> jwtOptions)
        {
            _jwtSettings = jwtOptions.Value;
        }
        public string CreateToken(string userId, string userName, string email, IReadOnlyList<string> roles)
        {

            // Claim
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Email, email),
            };
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));


            // signing credentials [secret key, algorithm]
            var secKey = _jwtSettings.SecretKey;    
            if (string.IsNullOrWhiteSpace(secKey))
                throw new InvalidOperationException("Secretkey is not configured.");

            if (secKey.Length < 32)
                throw new InvalidOperationException("Secretkey is Too Short");
            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secKey));
            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
               issuer: _jwtSettings.Issuer,
               audience: _jwtSettings.Audience,
               claims: claims,
               expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
               signingCredentials: credential
            );
            return new JwtSecurityTokenHandler().WriteToken(token);


        }
    }


    public class JwtSettings
    {
        public string SecretKey { get; set; } = default!;
        public int ExpirationMinutes { get; set; }
        public string Issuer { get; set; } = default!;
        public string Audience { get; set; } = default!;
    }
}
