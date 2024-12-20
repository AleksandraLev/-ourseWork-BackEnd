using OrdersService.DTOs;
using OrdersService.Messaging;
using OrdersService.Model;
using OrdersService.Repositories;
using OrdersService.Repositories.Exceptions;
using OrdersService.Services.Exceptions;

namespace OrdersService.Services
{
    public class OrderItemService : IOrderItemService
    {
        private readonly IOrderItemRepository _repository;
        private readonly IKafkaProduserService _kafkaProduserService;
        public OrderItemService(IOrderItemRepository repository, IKafkaProduserService kafkaProduserService)
        {
            _repository = repository;
            _kafkaProduserService = kafkaProduserService;
        }
        public async Task CreateOrderItemAsync(CreateOrderItemDTO createOrderItemDTO, int orderId)
        {
            OrderItem orderItem = new OrderItem
            {
                OrderId = orderId,
                ProductId = createOrderItemDTO.ProductId,
                Quantity = createOrderItemDTO.Quantity,
                Price = createOrderItemDTO.Price,
            };
            //Console.WriteLine($"orderItem.OrderId: {orderItem.OrderId}");
            var transation = await _repository.BeginTransactionAsync();
            try
            {
                await _repository.CreateOrderItemAsync(orderItem);
                await transation.CommitAsync();
            }
            catch (OrderItemSavedFailedException)
            {
                await transation.RollbackAsync();
                throw new OrderItemSavedFailedException();
            }
            await _kafkaProduserService.SendMessageAsync("orderIten-added", orderItem);
        }
        public async Task<List<OrderItem>> GetOrderItemsAsync(int orderId)
        {
            try
            {
                return await _repository.SelectItemsOfOrderAsync(orderId);
            }
            catch(ArgumentNullException)
            {
                throw new SelectOrderItemException();
            }
            catch(OperationCanceledException)
            {
                throw new SelectOrderItemException();
            }
        }
    }
}
