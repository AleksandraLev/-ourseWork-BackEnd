using ClothingStore.Model;
using ClothingStore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using ClothingStore.Repositories.Exeptions;

namespace ClothingStore.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context) 
        {
            _context = context;
        }
        public async Task AddUserAsync(User user) // Уровень доступа к данным (Работа с базой данных)
        {
            await _context.Users.AddAsync(user);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new UserSavedFaledException("Не удалось добавить пользователя.");
            }
            catch (DbUpdateException)
            {
                throw new UserSavedFaledException("Не удалось добавить пользователя.");
            }
            catch (OperationCanceledException)
            {
                throw new UserSavedFaledException("Не удалось добавить пользователя.");
            }
        }
        public async Task<User> SelectUserByPhoneNumberAsync(string phoneNumber)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
        }
        public async Task<User> SelectUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
        public async Task<User> SelectUserByIdAsync(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }
        public async Task SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new UserSavedFaledException("Не удалось сохранить изменения.");
            }
            catch (DbUpdateException)
            {
                throw new UserSavedFaledException("Не удалось сохранить изменения.");
            }
            catch (OperationCanceledException)
            {
                throw new UserSavedFaledException("Не удалось сохранить изменения.");
            }
        }
    }
}
