using ProductsService.Repositories;
using ProductsService.DTOs;
using ProductsService.Repositories.Exceptions;
using ProductsService.Services.Exceptions;
using ProductsService.Model;
using ProductsService.Services.Exceptions;


namespace ProductsService.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;
        public CategoryService(ICategoryRepository repository)
        {
            _repository = repository;
        }
        public async Task CreateCategoryAsync(CategoryDTO CategoryDTO)
        {
            var _category = await _repository.SelectCategoryByNameAsync(CategoryDTO.Name);
            if (_category != null)
            {
                throw new CategoryExistsException(nameof(CategoryDTO));
            }
            var category = new Category
            {
                Name = CategoryDTO.Name,
            };
            var transation = await _repository.BeginTransactionAsync();
            try
            {
                await _repository.AddCategoryAsync(category);
                await transation.CommitAsync();
            }
            catch (CategorySavedFaledException)
            {
                await transation.RollbackAsync();
                throw new CategorySavedFaledException();
            }
        }
        public async Task<Category> GetByNameAsync(string CategoryName)
        {
            return await _repository.SelectCategoryByNameAsync(CategoryName);
        }
        public async Task<List<CategoryDTO>> GetAllCategoryNamesAsync()
        {
            try
            {
                List<CategoryDTO> categoryNames = await _repository.GetCategoryNamesAsync();
                if(categoryNames == null)
                {
                    throw new CategoryNotFoundException("Тoвары не найдены.");
                }
                return categoryNames;
            }
            catch (CategoryNotFoundException)
            {
                throw new CategoryNotFoundException("Ошибка при получении списка категорий.");
            }
            catch (ArgumentNullException)
            {
                throw new CategoryIsNullException("Ошибка при получении списка категорий.");
            }
        }
    }
}
