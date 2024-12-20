using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using ProductsService.Data;
using ProductsService.Model;
using ProductsService.Repositories.Exceptions;
using ProductsService.DTOs;

namespace ProductsService.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;
        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddProductAsync(Product product) // Уровень доступа к данным (Работа с базой данных)
        {
            await _context.Product.AddAsync(product);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new ProductSavedFaledException("Не удалось добавить товар.");
            }
            catch (DbUpdateException)
            {
                throw new ProductSavedFaledException("Не удалось добавить товар.");
            }
            catch (OperationCanceledException)
            {
                throw new ProductSavedFaledException("Не удалось добавить товар.");
            }
        }
        public async Task<Product> SelectProductByNameAsync(string ProductName)
        {
            return await _context.Product.FirstOrDefaultAsync(p => p.Name == ProductName);
        }
        public async Task<Product> SelectProductByIdAsync(int ProductId)
        {
            return await _context.Product.FirstOrDefaultAsync(p => p.Id == ProductId);
        }
        public async Task<List<ProductInfoDTO>> GetProductNamesByCategoryNameAsync(string categoryName)
        {
            return await _context.Product.Where(p => p.Category.Name == categoryName).Select(p => new ProductInfoDTO {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price
            }).ToListAsync();
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
                throw new ProductSavedFaledException("Не удалось сохранить изменения.");
            }
            catch (DbUpdateException)
            {
                throw new ProductSavedFaledException("Не удалось сохранить изменения.");
            }
            catch (OperationCanceledException)
            {
                throw new ProductSavedFaledException("Не удалось сохранить изменения.");
            }
        }
        public async Task<List<ProductInfoDTO>> GetAllProductNamesAsync()
        {
            return await _context.Product.Select(p => new ProductInfoDTO
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price
            }).ToListAsync();
        }
        public async Task<List<ProductInfoDTO>> SelectProductsByPriceAsync(MinAmdMaxPrisesDTO prisesDTO)
        {
            return await _context.Product.Where(p => p.Price >= prisesDTO.MinPrice && p.Price <= prisesDTO.MaxPrice).Select(p => new ProductInfoDTO
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price
            }).ToListAsync();
        }
    }
}
