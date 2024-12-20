using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ProductsService.Data;
using ProductsService.DTOs;
using ProductsService.Model;
using ProductsService.Repositories.Exceptions;

namespace ProductsService.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;
        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddCategoryAsync(Category category) // Уровень доступа к данным (Работа с базой данных)
        {
            await _context.Category.AddAsync(category);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new CategorySavedFaledException("Не удалось добавить категорию.");
            }
            catch (DbUpdateException)
            {
                throw new CategorySavedFaledException("Не удалось добавить категорию.");
            }
            catch (OperationCanceledException)
            {
                throw new CategorySavedFaledException("Не удалось добавить категорию.");
            }
        }
        public async Task<Category> SelectCategoryByNameAsync(string CategoryName)
        {
            return await _context.Category.FirstOrDefaultAsync(c => c.Name == CategoryName);
        }
        public async Task<Category> SelectCategoryByIdAsync(int CategoryId)
        {
            return await _context.Category.FirstOrDefaultAsync(c => c.Id == CategoryId);
        }
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }
        public async Task<List<CategoryDTO>> GetCategoryNamesAsync()
        {
            return await _context.Category.Select(c => new CategoryDTO
            {
                Name = c.Name
            }).ToListAsync();
        }
    }
}
