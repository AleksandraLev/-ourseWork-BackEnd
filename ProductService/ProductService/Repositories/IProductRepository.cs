using Microsoft.EntityFrameworkCore.Storage;
using ProductsService.DTOs;
using ProductsService.Model;

namespace ProductsService.Repositories
{
    public interface IProductRepository
    {
        Task AddProductAsync(Product product);
        Task<Product> SelectProductByNameAsync(string ProductName);
        Task<Product> SelectProductByIdAsync(int ProductId);
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task<List<ProductInfoDTO>> GetProductNamesByCategoryNameAsync(string categoryName);
        Task SaveChangesAsync();
        Task<List<ProductInfoDTO>> GetAllProductNamesAsync();
        Task<List<ProductInfoDTO>> SelectProductsByPriceAsync(MinAmdMaxPrisesDTO prisesDTO);
    }
}
