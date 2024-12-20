using ClothingStore.DTOs;
using ClothingStore.Model;

namespace ClothingStore.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user, string deviceId);
        string GenerateRefreshToken(User user, string deviceId);
        Task CreateTokenAsync(string refreshToken, int id, string deviceId);
        Task InvalidationTokenAsync(int userID, string deviceID);
        Task<Token?> GetJwtTokenAsync(string refreshToken, string deviceId);
    }
}
