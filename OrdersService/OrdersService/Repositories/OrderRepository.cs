using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using OrdersService.Data;
using OrdersService.Model;
using OrdersService.Repositories.Exceptions;

namespace OrdersService.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;
        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task CreateOrderAsync(Order order) // Уровень доступа к данным (Работа с базой данных)
        {
            await _context.Orders.AddAsync(order);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new OrderSavedFailedException("Не удалось создать заказ.");
            }
            catch (DbUpdateException)
            {
                //Console.WriteLine($"Не удалось создать заказ. Ошибка при сохранении: {ex.Message}");
                throw new OrderSavedFailedException("Не удалось создать заказ.");
            }
            catch (OperationCanceledException)
            {
                throw new OrderSavedFailedException("Не удалось создать заказ.");
            }
        }
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }
        public async Task<Order> SelectOrderByIdAsync(int orderId)
        {
            return await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
        }
        public async Task SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new OrderSavedFailedException("Не удалось изменить заказ.");
            }
            catch (DbUpdateException)
            {
                throw new OrderSavedFailedException("Не удалось изменить заказ.");
            }
            catch (OperationCanceledException)
            {
                throw new OrderSavedFailedException("Не удалось изменить заказ.");
            }
        }
        public async Task<List<Order>> SelectOrdersOfUserAsync(int userId)
        {
            return await _context.Orders.Where(o => o.UserId == userId).Select(o => new Order
            {
                Id = o.Id,
                UserId = o.UserId,
                OrderDate = o.OrderDate,
                Status = o.Status,
                ShippingAddress = o.ShippingAddress
            }).ToListAsync();
        }
    }
}
