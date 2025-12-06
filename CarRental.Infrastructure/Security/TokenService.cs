using CarRental.Core.Entities;
using CarRental.Core.Interfaces.Services;
using CarRental.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CarRental.Infrastructure.Security
{
    /// <summary>
    /// JWT token generation and validation service
    /// </summary>
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _settings;
        private readonly IDateTimeProvider _dateTimeProvider;

        public TokenService(IOptions<JwtSettings> settings, IDateTimeProvider dateTimeProvider)
        {
            _settings = settings.Value;
            _dateTimeProvider = dateTimeProvider;
        }

        /// <summary>
        /// Generates a JWT token for the specified user
        /// </summary>
        public string GenerateToken(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new(JwtRegisteredClaimNames.Name, user.FullName ?? string.Empty),
                new("username", user.Username ?? string.Empty),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            // Add role claim based on user type
            if (user.Employee != null)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Employee"));
                if (!string.IsNullOrEmpty(user.Employee.Position))
                {
                    claims.Add(new Claim("position", user.Employee.Position));
                }
            }
            else if (user.Client != null)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Client"));
            }
            else
            {
                claims.Add(new Claim(ClaimTypes.Role, "User"));
            }

            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                notBefore: _dateTimeProvider.UtcNow,
                expires: _dateTimeProvider.UtcNow.AddMinutes(_settings.ExpirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Validates a JWT token
        /// </summary>
        public bool ValidateToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = securityKey,
                    ValidateIssuer = true,
                    ValidIssuer = _settings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _settings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero // No tolerance for token expiration
                }, out _);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

