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
        private readonly ILogger<OrderItemService> _logger;
        private readonly IOrderItemRepository _repository;
        private readonly IKafkaProduserService _kafkaProduserService;
        public OrderItemService(ILogger<OrderItemService> logger, IOrderItemRepository repository, IKafkaProduserService kafkaProduserService)
        {
            _logger = logger;
            _repository = repository;
            _kafkaProduserService = kafkaProduserService;
        }
        /*public async Task CreateOrderItemAsync(CreateOrderItemDTO createOrderItemDTO, int orderId)
        {
            OrderItem orderItem = new OrderItem
            {
                OrderId = orderId,
                ProductId = createOrderItemDTO.ProductId,
                Quantity = createOrderItemDTO.Quantity,
            };
            KafkaOrderItemDTO kafkaOrderItemDTO = new KafkaOrderItemDTO
            {
                //Id = orderItem.Id,
                OrderId = orderItem.OrderId,
                ProductId = createOrderItemDTO.ProductId,
                Quantity = createOrderItemDTO.Quantity,
            };
            await _kafkaProduserService.SendMessageAsync("orderItem-added", kafkaOrderItemDTO); // Доделать
        }*/
        public async Task CreateOrderItemsAsync(List<CreateOrderItemDTO> createOrderItems, int orderId)
        {
            _logger.LogInformation("Создание части заказа.");
            List<KafkaOrderItemDTO> kafkaOrderItemDTOs = createOrderItems.Select(item => new KafkaOrderItemDTO
            {
                OrderId = orderId,
                ProductId = item.ProductId,
                Quantity = item.Quantity
            }).ToList();

            await _kafkaProduserService.SendMessageAsync("orderItems-added", kafkaOrderItemDTOs);
        }
        public async Task WorkWithProductInfoAsync(List<KafkaProductDTO> kafkaProductDTO)
        {
            if (kafkaProductDTO != null && kafkaProductDTO.All(item => item.ProductExist && item.QuantityExist))
            {
                var transaction = await _repository.BeginTransactionAsync();

                try
                {
                    _logger.LogInformation($"Обрабатывам части заказа по номером: {kafkaProductDTO[0].OrderId}");
                    foreach (var item in kafkaProductDTO)
                    {
                        OrderItem orderItem = new OrderItem
                        {
                            OrderId = item.OrderId,
                            ProductId = item.ProductId,
                            Quantity = item.Quantity,
                            Price = item.Price,
                        };
                        await _repository.CreateOrderItemAsync(orderItem);
                    }
                    await transaction.CommitAsync();
                }
                catch (OrderItemSavedFailedException ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError("Ошибка при сохранении части заказа: " + ex.Message);
                    throw;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError("Произошла ошибка при обработке: " + ex.Message);
                }
            }
            else
            {
                _logger.LogWarning("Невозможно обработать заказ, так как один или несколько товаров недоступны.");
            }
        }

        public async Task<List<OrderItem>> GetOrderItemsAsync(int orderId)
        {
            try
            {
                _logger.LogInformation($"Получаем часть заказа по номером: {orderId}");
                return await _repository.SelectItemsOfOrderAsync(orderId);
            }
            catch(ArgumentNullException ex)
            {
                _logger.LogError("Произошла ошибка при получении части заказа: " + ex.Message);
                throw new SelectOrderItemException();
            }
            catch(OperationCanceledException ex)
            {
                _logger.LogError("Произошла ошибка при получении части заказа: " + ex.Message);
                throw new SelectOrderItemException();
            }
        }
    }
}
