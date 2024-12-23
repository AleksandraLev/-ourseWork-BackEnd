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
        private readonly ILogger<TokenService> _logger;
        private readonly ITokenRepository _repository;
        public TokenService(ILogger<TokenService> logger, ITokenRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }
        public string GenerateAccessToken(User user, string deviceId)
        {
            try
            {
                _logger.LogInformation("Создаём AccessToken.");
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
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(5)),
                        signingCredentials: new SigningCredentials(JwtAuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
                return new JwtSecurityTokenHandler().WriteToken(jwt);
            }
            catch (Exception ex)
            {
                _logger.LogError("Ошибка при создании AccessToken:" + ex.Message);
                throw new GenerateAccessTokenException();
            }
        }
        public string GenerateRefreshToken(User user, string deviceId)
        {
            try
            {
                _logger.LogInformation("Создаём RefreshToken.");
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
            catch (Exception ex)
            {
                _logger.LogError("Ошибка при создании RefreshToken:" + ex.Message);
                throw new GenerateRefreshTokenException();
            }
        }
        public async Task CreateTokenAsync(string refreshToken, int userId, string deviceId)
        {
            _logger.LogInformation("Создаём Token (Обновляем AccessToken и RefreshToken).");
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
                _logger.LogInformation("Сохраняем новый RefreshToken.");
                await _repository.AddRefreshTokenAsync(token);
                await transaction.CommitAsync();
            }
            catch (TokenSavedFaledException)
            {
                _logger.LogError("Ошибка при сохранении RefreshToken.");
                await transaction.RollbackAsync();
                throw new TokenSavedFaledException();
            }
        }
        public async Task InvalidationTokenAsync(int userID, string deviceID)
        {
            try
            {
                _logger.LogInformation("Делаем RefreshToken не активным.");
                var token = await _repository.RevokeTokenAsync(userID, deviceID);
                if (token != null)
                {
                    token.RevokedAt = DateTime.UtcNow;
                    await _repository.SaveChangesAsync();
                }
            }
            catch (SavedFaledException ex)
            {
                _logger.LogError("Ошибка при дезактивации RefreshToken:" + ex.Message);
                throw new LogoutFaledException();
            }
            catch (Exception ex)
            {
                _logger.LogError("Ошибка при дезактивации RefreshToken:" + ex.Message);
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
