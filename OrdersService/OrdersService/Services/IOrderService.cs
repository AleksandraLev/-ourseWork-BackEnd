using OrdersService.DTOs;
using OrdersService.Model;

namespace OrdersService.Services
{
    public interface IOrderService
    {
        Task CreateOrderAsync(CreateOrderDTO createOrderDTO);
        Task CancelOrderAsync(int orderNummber);
        //Task<Dictionary<string, string>> GetOrderDetailsAsync(int orderNummber);
        Task<AllOrderInfoDTO> GetOrderDetailsAsync(int orderNummber);
        Task<List<Order>> GetAllOrdersOfUser();
    }
}
