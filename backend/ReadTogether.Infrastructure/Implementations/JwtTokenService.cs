using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using ReadTogether.Domain.Configuration;
using ReadTogether.Domain.Entities;
using ReadTogether.Infrastructure.Dtos;
using ReadTogether.Infrastructure.Interfaces;

namespace ReadTogether.Infrastructure.Implementations
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly JwtConfiguration jwtConfiguration;

        public JwtTokenService(IOptions<JwtConfiguration> options)
        {
            this.jwtConfiguration = options.Value;
        }
        public JwtTokenDto GenerateToken(ApplicationUser user)
        {
            // Token will be signed using symmetric encryption shared key 
            var key = Encoding.UTF8.GetBytes(jwtConfiguration.Key);
            var securityKey = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var handler = new JsonWebTokenHandler { SetDefaultTimesOnTokenCreation = false };

            // Configures Token Payload

            var claims = new ClaimsIdentity(
                [
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                    new Claim(JwtRegisteredClaimNames.Name, user.UserName ?? ""),
                    new Claim("profileCompleted", user.ProfileCompleted.ToString()),
                    new Claim(ClaimTypes.Role, "User") // TODO: Change once roles are implemented.
                ]);

            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = jwtConfiguration.Issuer,
                Subject = claims,
                Audience = jwtConfiguration.Audience,
                Expires = DateTime.UtcNow.AddHours(jwtConfiguration.ExpiresInHours),
                SigningCredentials = credentials
            };

            // Creates and signs the token
            var token = handler.CreateToken(descriptor);
            Console.WriteLine($"Generated JWT Token {token}");
            return new JwtTokenDto
            {
                Token = token,
                ExpiresAt = (DateTime)descriptor.Expires
            };
        }
    }
}