using Microsoft.EntityFrameworkCore.Storage;
using OrdersService.Model;

namespace OrdersService.Repositories
{
    public interface IOrderItemRepository
    {
        Task CreateOrderItemAsync(OrderItem orderItm);
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task<OrderItem> SelectOrderByIdAsync(int orderItemId);
        Task<List<OrderItem>> SelectItemsOfOrderAsync(int orderId);
    }
}
