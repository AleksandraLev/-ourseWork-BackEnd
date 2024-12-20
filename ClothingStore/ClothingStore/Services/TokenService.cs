using ClothingStore.Model;
using ClothingStore.Security;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ClothingStore.Services.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;
using ClothingStore.Repositories.Exeptions;
using ClothingStore.Repositories;
using System.Data;
using ClothingStore.DTOs;

namespace ClothingStore.Services
{
    public class TokenService : ITokenService
    {
        private readonly ITokenRepository _repository;
        public TokenService(ITokenRepository repository)
        {
            _repository = repository;
        }
        public string GenerateAccessToken(User user, string deviceId)
        {
            try
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.MobilePhone, user.PhoneNumber),
                    new Claim("DeviceId", deviceId),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                var jwt = new JwtSecurityToken(
                        issuer: JwtAuthOptions.ISSUER,
                        audience: JwtAuthOptions.AUDIENCE,
                        claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(1)),
                        signingCredentials: new SigningCredentials(JwtAuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
                return new JwtSecurityTokenHandler().WriteToken(jwt);
            }
            catch (Exception)
            {
                throw new GenerateAccessTokenException();
            }
        }
        public string GenerateRefreshToken(User user, string deviceId)
        {
            try
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.MobilePhone, user.PhoneNumber),
                    new Claim("DeviceId", deviceId),
                };

                var jwt = new JwtSecurityToken(
                        issuer: JwtAuthOptions.ISSUER,
                        audience: JwtAuthOptions.AUDIENCE,
                        claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromDays(30)),
                        signingCredentials: new SigningCredentials(JwtAuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
                return new JwtSecurityTokenHandler().WriteToken(jwt);
            }
            catch (Exception)
            {
                throw new GenerateRefreshTokenException();
            }
        }
        public async Task CreateTokenAsync(string refreshToken, int userId, string deviceId)
        {
            var token = new Token
            {
                UserId = userId,
                DeviceID = deviceId,
                RefreshToken = refreshToken,
                ExpiryDate = DateTime.UtcNow.AddDays(30),
                CreatedAt = DateTime.UtcNow,
                RevokedAt = null,
            };
            var transaction = await _repository.BeginTransactionAsync();
            try
            {
                await _repository.AddRefreshTokenAsync(token);
                await transaction.CommitAsync();
            }
            catch (TokenSavedFaledException)
            {
                await transaction.RollbackAsync();
                throw new TokenSavedFaledException();
            }
        }
        public async Task InvalidationTokenAsync(int userID, string deviceID)
        {
            try
            {
                var token = await _repository.RevokeTokenAsync(userID, deviceID);
                if (token != null)
                {
                    token.RevokedAt = DateTime.UtcNow;
                    await _repository.SaveChangesAsync();
                }
            }
            catch (SavedFaledException)
            {
                throw new LogoutFaledException();
            }
            catch (Exception)
            {
                throw new LogoutFaledException();
            }
        }
        public async Task<Token?> GetJwtTokenAsync(string refreshToken, string deviceId)
        {
            Token? token = await _repository.GetRefreshTokenAsync(refreshToken, deviceId);
            return token;
        }
    }
}
