using OrdersService.DTOs;
using OrdersService.Model;

namespace OrdersService.Services
{
    public interface IOrderItemService
    {
        //Task CreateOrderItemAsync(CreateOrderItemDTO createOrderItemDTO, int OrderId);
        Task CreateOrderItemsAsync(List<CreateOrderItemDTO> createOrderItems, int orderId);
        Task<List<OrderItem>> GetOrderItemsAsync(int orderId);
        Task WorkWithProductInfoAsync(List<KafkaProductDTO> kafkaProductDTO);
    }
}
