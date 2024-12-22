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
                    Console.WriteLine("Ошибка при сохранении элемента заказа: " + ex.Message);
                    throw;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine("Произошла ошибка при обработке: " + ex.Message);
                }
            }
            else
            {
                Console.WriteLine("Невозможно обработать заказ, так как один или несколько товаров недоступны.");
            }
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
