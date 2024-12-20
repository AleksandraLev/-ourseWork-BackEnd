using ProductsService.DTOs;
using ProductsService.Model;

namespace ProductsService.Services
{
    public interface IProductService
    {
        Task CreateProductAsync(AddProductDTO addProductDTO);
        Task<Product> GetByNameAsync(string ProductName);
        Task<List<ProductInfoDTO>> GetProductsOfCategory(CategoryDTO categoryDTO);
        Task ChangeProductNameAsync(ChangeProductNameDTO changeProductNameDTO);
        Task ChangePriceAsync(ChangePriceDTO changePriceDTO);
        Task ChangeDescriptionAsync(ChangeDescriptionDTO changeDescriptionDTO);
        Task ChangeCategoryOfProductAsync(ChangeCategoryOfProductDTO changeCategoryOfProductDTO);
        Task ChangeStockQuantityAsync(ChangeStockQuantityDTO changeStockQuantityDTO);
        Task<List<ProductInfoDTO>> GetAllProductNamesAsync();
        Task<List<ProductInfoDTO>> GetProductsSelectedByPriseAsync(MinAmdMaxPrisesDTO prisesDTO);
        Task<Product> GetByIdAsync(int Id);
    }
}
