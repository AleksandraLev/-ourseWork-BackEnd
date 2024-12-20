using ClothingStore.Data;
using ClothingStore.Model;
using ClothingStore.Repositories.Exeptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
namespace ClothingStore.Repositories
{
    public class TokenRepository : ITokenRepository
    {

        private readonly AppDbContext _context;
        public TokenRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddRefreshTokenAsync(Token token)
        {
            try
            {
                await _context.Tokens.AddAsync(token);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new TokenSavedFaledException("Не удалось добавить токен.");
            }
            catch (DbUpdateException)
            {
                throw new TokenSavedFaledException("Не удалось добавить токен.");
            }
            catch (OperationCanceledException)
            {
                throw new TokenSavedFaledException("Не удалось добавить токен.");
            }
        }
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }
        public async Task<Token?> RevokeTokenAsync(int userID, string deviceID)
        {
            return await _context.Tokens.FirstOrDefaultAsync(rt => rt.user.Id == userID
            && rt.DeviceID == deviceID
            && rt.ExpiryDate > DateTime.UtcNow
            && rt.RevokedAt == null);
        }
        public async Task SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new SavedFaledException();
            }
        }
        public async Task<Token?> GetRefreshTokenAsync(string refreshToken, string deviceID)
        {
            return await _context.Tokens.FirstOrDefaultAsync(rt => rt.RefreshToken == refreshToken
                && rt.DeviceID == deviceID
                && rt.ExpiryDate > DateTime.UtcNow
                && rt.RevokedAt == null);
        }
    }
}
