using OrdersService.DTOs;
using OrdersService.Model;

namespace OrdersService.Services
{
    public interface IOrderItemService
    {
        Task CreateOrderItemAsync(CreateOrderItemDTO createOrderItemDTO, int OrderId);
        Task<List<OrderItem>> GetOrderItemsAsync(int orderId);
    }
}
