using ClothingStore.Model;
using Microsoft.EntityFrameworkCore.Storage;

namespace ClothingStore.Repositories
{
    public interface ITokenRepository
    {
        Task AddRefreshTokenAsync(Token token);
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task<Token?> RevokeTokenAsync(int userID, string deviceID);
        Task SaveChangesAsync();
        Task<Token?> GetRefreshTokenAsync(string refreshToken, string deviceID);
    }
}
