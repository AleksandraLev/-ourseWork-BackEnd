using OrdersService.DTOs;
using OrdersService.Repositories;
using OrdersService.Model;
using OrdersService.Repositories.Exceptions;
using OrdersService.Services.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace OrdersService.Services
{
    public class OrderService : IOrderService
    {
        private readonly ILogger<OrderService> _logger;
        private readonly IOrderRepository _repository;
        private readonly IOrderItemService _orderItemServise;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public OrderService(ILogger<OrderService> logger,IOrderRepository repository, IOrderItemService orderItemService, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _repository = repository;
            _orderItemServise = orderItemService;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task CreateOrderAsync(CreateOrderDTO createOrderDTO)
        {
            _logger.LogInformation("Начало создания заказа.");
            int userId = int.Parse(_httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            _logger.LogInformation($"ID пользователя: {userId}", userId);
            Order order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                ShippingAddress = createOrderDTO.ShippingAddress
            };
            var transation = await _repository.BeginTransactionAsync();
            try
            {
                _logger.LogInformation("Сохраняем данные о заказе.");
                await _repository.CreateOrderAsync(order);
                //Console.WriteLine($"OrderId: {order.Id}");
                await transation.CommitAsync();
                await _orderItemServise.CreateOrderItemsAsync(createOrderDTO.Items, order.Id);
                /*foreach (var item in createOrderDTO.Items)
                {
                    await _orderItemServise.CreateOrderItemAsync(item, order.Id);
                }*/
                await ProcessOrderAsync(order);
            }
            catch (Exception ex)
            {
                _logger.LogError("Ошибка при сохранении данных о заказе:" + ex.Message);
                await transation.RollbackAsync();
                throw new OrderSavedFailedException();
            }
            _logger.LogInformation("Данные о заказе успешно сохранены.");
        }
        public async Task ProcessOrderAsync(Order order)
        {
            _logger.LogInformation($"Смена статуса заказа на \"Processing\" (Обрабатывается)."); 
            _logger.LogInformation($"Номер заказа (ID) \"{order.Id}\".", order.Id);
            order.ProcessStatus();
            var transation = await _repository.BeginTransactionAsync();
            try
            {
                _logger.LogInformation("Сохраняем данные о заказе.");
                await _repository.SaveChangesAsync();
                await transation.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Ошибка при изменении статуса заказа на \"Processing\":" + ex.Message);
                await transation.RollbackAsync();
                throw new ChangeOrderStatusException();
            }
            _logger.LogInformation("Данные о заказе успешно сохранены.");
        }
        public async Task ShipOrderAsync(int orderId)
        {
            _logger.LogInformation("Смена статуса заказа на \"Shipped\" (Отправлен).");
            _logger.LogInformation($"Номер заказа (ID) \"{orderId}\".", orderId);
            Order order = await _repository.SelectOrderByIdAsync(orderId);
            order.ShipStatus();
            var transation = await _repository.BeginTransactionAsync();
            try
            {
                _logger.LogInformation("Сохраняем данные о заказе.");
                await _repository.SaveChangesAsync();
                await transation.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Ошибка при изменении статуса заказа на \"Shipped\"^" + ex.Message);
                await transation.RollbackAsync();
                throw new ChangeOrderStatusException();
            }
            _logger.LogInformation("Данные о заказе успешно сохранены.");
        }
        public async Task AfterProcessOrderAsync(List<KafkaProductDTO> kafkaProductDTO)
        {
            if (kafkaProductDTO == null || !kafkaProductDTO.Any())
            {
                _logger.LogWarning("Список товаров пуст.");
                return;
            }
            foreach (var item in kafkaProductDTO)
            {
                if (!item.ProductExist)
                    Console.WriteLine($"Товар \"{item.ProductId}\" не найден.");
                else if (item.Quantity == 1)
                    Console.WriteLine($"Товара нет на складе");
                else
                    Console.WriteLine($"Товар отсутствует на складе в запрашиваемом колличестве.");
            }
            Order order = await _repository.SelectOrderByIdAsync(kafkaProductDTO.First().OrderId);
            _logger.LogInformation("Смена статуса заказа на \"Canceled\" (Отменён).");
            order.CanceleStatus();
            var transation = await _repository.BeginTransactionAsync();
            try
            {
                _logger.LogInformation("Сохраняем данные о заказе.");
                await _repository.SaveChangesAsync();
                await transation.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Ошибка при изменении статуса заказа на \"Canceled\":" + ex.Message);
                await transation.RollbackAsync();
                throw new ChangeOrderStatusException();
            }
            _logger.LogInformation("Заказ отменён.");
        }
        public async Task CancelOrderAsync(int orderNummber)
        {
            _logger.LogInformation($"Номер заказа (ID) \"{orderNummber}\".", orderNummber);
            try
            {
                Order order = await _repository.SelectOrderByIdAsync(orderNummber);
                if (order == null)
                {
                    _logger.LogWarning("Объект заказа пуст.");
                    throw new OrderNotFoundException();
                }
                else if (order.UserId != int.Parse(_httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value))
                {
                    _logger.LogWarning("Попытка отменить заказ другого пользователя.");
                    throw new NoAccessRightsException();
                }
                else
                {
                    var transation = await _repository.BeginTransactionAsync(); // убрать
                    try
                    {
                        _logger.LogInformation("Отменяем заказ.");
                        order.CanceleStatus();
                        await _repository.SaveChangesAsync();
                        await transation.CommitAsync();
                    }
                    catch (OrderSavedFailedException ex)
                    {
                        _logger.LogError("Ошибка при изменении статуса заказа на \"Canceled\":" + ex.Message);
                        await transation.RollbackAsync();
                        throw new OrderSavedFailedException();
                    }
                    _logger.LogInformation("Заказ отменён.");
                }
            }
            catch(ArgumentNullException ex)
            {
                _logger.LogError("Ошибка при изменении статуса заказа на \"Canceled\":" + ex.Message);
                throw new ChangeOrderStatusException();
            }
            catch(OperationCanceledException ex)
            {
                _logger.LogError("Ошибка при изменении статуса заказа на \"Canceled\":" + ex.Message);
                throw new ChangeOrderStatusException();
            }
        }
        //public async Task<Dictionary<string, string>> GetOrderDetailsAsync(int orderNummber)
        //{
        //    try
        //    {
        //        Order order = await _repository.SelectOrderByIdAsync(orderNummber);
        //        AllOrderInfoDTO orderInfo = new AllOrderInfoDTO
        //        {
        //            Id = order.Id,
        //            UserId = order.UserId,
        //            OrderDate = order.OrderDate,
        //            Status = order.Status,
        //            ShippingAddress = order.ShippingAddress,
        //            AllItems = await _orderItemServise.GetOrderItemsAsync(orderNummber)
        //        };
        //        return orderInfo.GetOrderInfo();
        //    }
        //    catch (ArgumentNullException)
        //    {
        //        throw new SelectOrderException();
        //    }
        //    catch (OperationCanceledException)
        //    {
        //        throw new SelectOrderException();
        //    }
        //}
        public async Task<AllOrderInfoDTO> GetOrderDetailsAsync(int orderNummber)
        {
            _logger.LogInformation("Получение подробной информации о заказе.");
            try
            {
                Order order = await _repository.SelectOrderByIdAsync(orderNummber);
                if (order == null)
                {
                    _logger.LogWarning("Объект заказа пуст.");
                    throw new OrderNotFoundException();
                }
                else if (order.UserId != int.Parse(_httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value))
                {
                    _logger.LogWarning("Попытка получить информацию о заказе другого пользователя.");
                    throw new NoAccessRightsException();
                }
                AllOrderInfoDTO orderInfo = new AllOrderInfoDTO
                {
                    Id = order.Id,
                    UserId = order.UserId,
                    //UserId = int.Parse(_httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value),
                    OrderDate = order.OrderDate,
                    Status = order.Status,
                    ShippingAddress = order.ShippingAddress,
                    AllItems = await _orderItemServise.GetOrderItemsAsync(orderNummber)
                };
                return orderInfo;
            }
            catch (NoAccessRightsException ex)
            {
                _logger.LogError("Ошибка при получении подробной информации о заказе:" + ex.Message);
                throw new NoAccessRightsException();
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError("Ошибка при получении подробной информации о заказе:" + ex.Message);
                throw new SelectOrderException();
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError("Ошибка при получении подробной информации о заказе:" + ex.Message);
                throw new SelectOrderException();
            }
        }
        public async Task<List<Order>> GetAllOrdersOfUser()
        {
            _logger.LogInformation("Получение информации о всех заказах.");
            try
            {
                int userId = int.Parse(_httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                return await _repository.SelectOrdersOfUserAsync(userId);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError("Ошибка при получении информации о всех заказах:" + ex.Message);
                throw new SelectOrderException();
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError("Ошибка при получении информации о всех заказах:" + ex.Message);
                throw new SelectOrderException();
            }
        }
    }
}
