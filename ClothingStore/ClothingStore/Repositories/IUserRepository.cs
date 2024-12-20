using ClothingStore.Model;
using Microsoft.EntityFrameworkCore.Storage;

namespace ClothingStore.Repositories // Уровень доступа к данным (Работа с базой данных)
{
    public interface IUserRepository
    {
        Task AddUserAsync(User user);
        Task<User> SelectUserByPhoneNumberAsync(string phoneNumber);
        Task<User> SelectUserByEmailAsync(string email);
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task SaveChangesAsync();
        Task<User> SelectUserByIdAsync(int id);
        //Task<Token?> GetRefreshTokenAsync(string refreshToken, string deviceID);
        //Task DeleteRefreshTokenAsync(string refreshToken);
    }
}
