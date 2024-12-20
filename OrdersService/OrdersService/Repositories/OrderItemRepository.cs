using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using OrdersService.Data;
using OrdersService.Model;
using OrdersService.Repositories.Exceptions;

namespace OrdersService.Repositories
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly AppDbContext _context;
        public OrderItemRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task CreateOrderItemAsync(OrderItem orderItm) // Уровень доступа к данным (Работа с базой данных)
        {
            await _context.OrderItems.AddAsync(orderItm);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new OrderItemSavedFailedException("Не удалось создать часть заказа.");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Не удалось создать часть заказа. Ошибка БД {ex.Message}");
                throw new OrderItemSavedFailedException("Не удалось создать часть заказа.");
            }
            catch (OperationCanceledException)
            {
                throw new OrderItemSavedFailedException("Не удалось создать часть заказа.");
            }
        }
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }
        public async Task<OrderItem> SelectOrderByIdAsync(int orderItemId)
        {
            return await _context.OrderItems.FirstOrDefaultAsync(oi => oi.Id == orderItemId);
        }
        public async Task<List<OrderItem>> SelectItemsOfOrderAsync(int orderId)
        {
            return await _context.OrderItems.Where(oi => oi.OrderId == orderId).Select(oi => new OrderItem
            {
                Id = oi.Id,
                OrderId = orderId,
                ProductId = oi.ProductId,
                Quantity = oi.Quantity,
                Price = oi.Price
            }).ToListAsync();
        }
    }
}
