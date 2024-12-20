using Microsoft.EntityFrameworkCore.Storage;
using ProductsService.DTOs;
using ProductsService.Model;

namespace ProductsService.Repositories
{
    public interface ICategoryRepository
    {
        Task AddCategoryAsync(Category category);
        Task<Category> SelectCategoryByNameAsync(string CategoryName);
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task<Category> SelectCategoryByIdAsync(int CategoryId);
        Task<List<CategoryDTO>> GetCategoryNamesAsync();
    }
}
