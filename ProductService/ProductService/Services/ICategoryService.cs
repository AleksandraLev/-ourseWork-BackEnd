using ProductsService.Model;
using ProductsService.DTOs;

namespace ProductsService.Services
{
    public interface ICategoryService
    {
        Task CreateCategoryAsync(CategoryDTO CategoryDTO);
        Task<Category> GetByNameAsync(string CategoryName);
        Task<List<CategoryDTO>> GetAllCategoryNamesAsync();
    }
}
