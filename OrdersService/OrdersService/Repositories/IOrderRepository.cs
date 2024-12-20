using Microsoft.EntityFrameworkCore.Storage;
using OrdersService.Model;

namespace OrdersService.Repositories
{
    public interface IOrderRepository
    {
        Task CreateOrderAsync(Order order);
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task<Order> SelectOrderByIdAsync(int orderId);
        Task SaveChangesAsync();
        Task<List<Order>> SelectOrdersOfUserAsync(int userId);
    }
}
