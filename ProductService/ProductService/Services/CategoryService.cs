using ProductsService.Repositories;
using ProductsService.DTOs;
using ProductsService.Repositories.Exceptions;
using ProductsService.Services.Exceptions;
using ProductsService.Model;


namespace ProductsService.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ILogger<CategoryService> _logger;
        private readonly ICategoryRepository _repository;
        public CategoryService(ILogger<CategoryService> logger, ICategoryRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }
        public async Task CreateCategoryAsync(CategoryDTO categoryDTO)
        {
            _logger.LogInformation($"Создание новой категории товаров \"{categoryDTO.Name}\".");
            var _category = await _repository.SelectCategoryByNameAsync(categoryDTO.Name);
            if (_category != null)
            {
                _logger.LogWarning($"Категориия \"{categoryDTO.Name}\" уже существует.");
                throw new CategoryExistsException(nameof(categoryDTO));
            }
            var category = new Category
            {
                Name = categoryDTO.Name,
            };
            var transation = await _repository.BeginTransactionAsync();
            try
            {
                _logger.LogInformation("Сохранение новой категории товаров.");
                await _repository.AddCategoryAsync(category);
                await transation.CommitAsync();
            }
            catch (CategorySavedFaledException)
            {
                _logger.LogError("Ошибка при сохранении новой категории товаров.");
                await transation.RollbackAsync();
                throw new CategorySavedFaledException();
            }
        }
        public async Task<Category> GetByNameAsync(string CategoryName)
        {
            _logger.LogInformation("Получение категории товаров по названию.");
            return await _repository.SelectCategoryByNameAsync(CategoryName);
        }
        public async Task<List<CategoryDTO>> GetAllCategoryNamesAsync()
        {
            try
            {
                _logger.LogInformation("Получение списка всех категорий товаров.");
                List<CategoryDTO> categoryNames = await _repository.GetCategoryNamesAsync();
                if(categoryNames == null)
                {
                    throw new CategoryNotFoundException("Тoвары не найдены.");
                }
                return categoryNames;
            }
            catch (CategoryNotFoundException)
            {
                _logger.LogError("Ошибка при получении списка категорий.");
                throw new CategoryNotFoundException("Ошибка при получении списка категорий.");
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError("Ошибка при получении списка категорий: " + ex.Message);
                throw new CategoryIsNullException("Ошибка при получении списка категорий.");
            }
        }
    }
}
