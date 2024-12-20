using ProductsService.DTOs;
using ProductsService.Model;
using ProductsService.Services.Exceptions;
using ProductsService.Repositories;
using ProductsService.Repositories.Exceptions;

namespace ProductsService.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly ICategoryService _categoryService;
        public ProductService(IProductRepository repository, ICategoryService categoryService)
        {
            _repository = repository;
            _categoryService = categoryService;
        }
        public async Task CreateProductAsync(AddProductDTO addProductDTO)
        {
            //var _product = await _repository.SelectProductByNameAsync(addProductDTO.Name);
            //if (_product != null)
            //{
            //    throw new ProductExistsException(nameof(addProductDTO));
            //}
            Category category = await _categoryService.GetByNameAsync(addProductDTO.CategoryName);
            int categoryId;
            if (category == null)
            {
                throw new CategoryNotFoundException(addProductDTO.CategoryName);
            }
            else
            {
                categoryId = category.Id;
                var product = new Product
                {
                    Name = addProductDTO.Name,
                    Price = addProductDTO.Price,
                    Description = addProductDTO.Description,
                    CategoryId = categoryId,
                    StockQuantity = addProductDTO.StockQuantity,
                };
                var transation = await _repository.BeginTransactionAsync();
                try
                {
                    await _repository.AddProductAsync(product);
                    await transation.CommitAsync();
                }
                catch (ProductSavedFaledException)
                {
                    await transation.RollbackAsync();
                    throw new ProductSavedFaledException();
                }
            }
        }
        public async Task<Product> GetByNameAsync(string ProductName)
        {
            return await _repository.SelectProductByNameAsync(ProductName);
        }
        public async Task<List<ProductInfoDTO>> GetProductsOfCategory(CategoryDTO categoryDTO)
        {
            if (await _categoryService.GetByNameAsync(categoryDTO.Name) == null)
            {
                throw new CategoryNotFoundException(categoryDTO.Name);
            }
            else
            {
                return await _repository.GetProductNamesByCategoryNameAsync(categoryDTO.Name);
            }
        }
        public async Task ChangeProductNameAsync(ChangeProductNameDTO changeProductNameDTO)
        {
            var product = await _repository.SelectProductByIdAsync(changeProductNameDTO.Id);
            if (product == null)
            {
                throw new ProductNotFoundException(changeProductNameDTO.Id.ToString());
            }
            else
            {
                if (product.Name == changeProductNameDTO.Name)
                {
                    throw new ChangeToSameDataException();
                }
                else
                {
                    var transation = await _repository.BeginTransactionAsync();
                    try
                    {
                        product.Name = changeProductNameDTO.Name;
                        await _repository.SaveChangesAsync();
                        await transation.CommitAsync();
                    }
                    catch (ProductSavedFaledException)
                    {
                        await transation.RollbackAsync();
                        throw new ProductSavedFaledException();
                    }
                }
            }
        }
        public async Task ChangePriceAsync(ChangePriceDTO changePriceDTO)
        {
            var product = await _repository.SelectProductByIdAsync(changePriceDTO.Id);
            if (product == null)
            {
                throw new ProductNotFoundException(changePriceDTO.Id.ToString());
            }
            else
            {
                if (product.Price == changePriceDTO.Price)
                {
                    throw new ChangeToSameDataException();
                }
                else
                {
                    var transation = await _repository.BeginTransactionAsync();
                    try
                    {
                        product.Price = changePriceDTO.Price;
                        await _repository.SaveChangesAsync();
                        await transation.CommitAsync();
                    }
                    catch (ProductSavedFaledException)
                    {
                        await transation.RollbackAsync();
                        throw new ProductSavedFaledException();
                    }
                }
            }
        }
        public async Task ChangeDescriptionAsync(ChangeDescriptionDTO changeDescriptionDTO)
        {
            var product = await _repository.SelectProductByIdAsync(changeDescriptionDTO.Id);
            if (product == null)
            {
                throw new ProductNotFoundException(changeDescriptionDTO.Id.ToString());
            }
            else
            {
                if (product.Description == changeDescriptionDTO.Description)
                {
                    throw new ChangeToSameDataException();
                }
                else
                {
                    var transation = await _repository.BeginTransactionAsync();
                    try
                    {
                        product.Description = changeDescriptionDTO.Description;
                        await _repository.SaveChangesAsync();
                        await transation.CommitAsync();
                    }
                    catch (ProductSavedFaledException)
                    {
                        await transation.RollbackAsync();
                        throw new ProductSavedFaledException();
                    }
                }
            }
        }
        public async Task ChangeCategoryOfProductAsync(ChangeCategoryOfProductDTO changeCategoryOfProductDTO)
        {
            var product = await _repository.SelectProductByIdAsync(changeCategoryOfProductDTO.Id);
            if (product == null)
            {
                throw new ProductNotFoundException(changeCategoryOfProductDTO.Id.ToString());
            }
            else
            {
                Category category = await _categoryService.GetByNameAsync(changeCategoryOfProductDTO.CategoryName);
                if (category == null)
                {
                    throw new CategoryNotFoundException(changeCategoryOfProductDTO.CategoryName);
                }
                else
                {
                    if (product.CategoryId == category.Id)
                    {
                        throw new ChangeToSameDataException();
                    }
                    var transation = await _repository.BeginTransactionAsync();
                    try
                    {
                        product.CategoryId = category.Id;
                        await _repository.SaveChangesAsync();
                        await transation.CommitAsync();
                    }
                    catch (ProductSavedFaledException)
                    {
                        await transation.RollbackAsync();
                        throw new ProductSavedFaledException();
                    }
                }
            }
        }
        public async Task ChangeStockQuantityAsync(ChangeStockQuantityDTO changeStockQuantityDTO)
        {
            var product = await _repository.SelectProductByIdAsync(changeStockQuantityDTO.Id);
            if (product == null)
            {
                throw new ProductNotFoundException(changeStockQuantityDTO.Id.ToString());
            }
            else
            {
                if (product.StockQuantity == changeStockQuantityDTO.StockQuantity)
                {
                    throw new ChangeToSameDataException();
                }
                else
                {
                    var transation = await _repository.BeginTransactionAsync();
                    try
                    {
                        product.StockQuantity = changeStockQuantityDTO.StockQuantity;
                        await _repository.SaveChangesAsync();
                        await transation.CommitAsync();
                    }
                    catch (ProductSavedFaledException)
                    {
                        await transation.RollbackAsync();
                        throw new ProductSavedFaledException();
                    }
                }
            }
        }
        public async Task<List<ProductInfoDTO>> GetAllProductNamesAsync()
        {
            try
            {
                List<ProductInfoDTO> productNames = await _repository.GetAllProductNamesAsync();
                if (productNames == null)
                {
                    throw new ProductNotFoundException("Тoвары не найдены.");
                }
                return productNames;
            }
            catch (ProductNotFoundException)
            {
                throw new ProductNotFoundException("Ошибка при получении списка товаров.");
            }
            catch (ArgumentNullException)
            {
                throw new GetProductException("Ошибка при получении списка товаров.");
            }
        }
        public async Task<List<ProductInfoDTO>> GetProductsSelectedByPriseAsync(MinAmdMaxPrisesDTO prisesDTO)
        {
            try
            {
                List<ProductInfoDTO> productNames = await _repository.SelectProductsByPriceAsync(prisesDTO);
                if (productNames == null)
                {
                    throw new ProductNotFoundException("Тoвары не найдены.");
                }
                return productNames;
            }
            catch (ProductNotFoundException)
            {
                throw new ProductNotFoundException("Ошибка при получении списка товаров.");
            }
            catch (ArgumentNullException)
            {
                throw new GetProductException("Ошибка при получении списка товаров.");
            }
        }
        public async Task<Product> GetByIdAsync(int Id)
        {
            try
            {
                Product product = await _repository.SelectProductByIdAsync(Id);
                if (product == null)
                {
                    throw new ProductNotFoundException(Id.ToString());
                }
                return await _repository.SelectProductByIdAsync(Id);
            }
            catch (ArgumentNullException)
            {
                throw new GetProductException("Ошибка при получении товара.");
            }
            catch (OperationCanceledException)
            {
                throw new GetProductException("Ошибка при получении товара.");
            }
        }
    }
}
