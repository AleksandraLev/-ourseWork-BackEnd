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
        private readonly IOrderRepository _repository;
        private readonly IOrderItemService _orderItemServise;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public OrderService(IOrderRepository repository, IOrderItemService orderItemService, IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _orderItemServise = orderItemService;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task CreateOrderAsync(CreateOrderDTO createOrderDTO)
        {
            int userId = int.Parse(_httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            Order order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                ShippingAddress = createOrderDTO.ShippingAddress
            };
            var transation = await _repository.BeginTransactionAsync();
            try
            {
                await _repository.CreateOrderAsync(order);
                Console.WriteLine($"OrderId: {order.Id}");
                await transation.CommitAsync();
                foreach (var item in createOrderDTO.Items)
                {
                    await _orderItemServise.CreateOrderItemAsync(item, order.Id);
                }
            }
            catch
            {
                await transation.RollbackAsync();
                throw new OrderSavedFailedException();
            }
        }
        public async Task CancelOrderAsync(int orderNummber)
        {
            try
            {
                Order order = await _repository.SelectOrderByIdAsync(orderNummber);
                if (order == null)
                {
                    throw new OrderNotFoundException();
                }
                else if (order.UserId != int.Parse(_httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value))
                {
                    throw new NoAccessRightsException();
                }
                else
                {
                    var transation = await _repository.BeginTransactionAsync(); // убрать
                    try
                    {
                        order.CanceleStatus();
                        await _repository.SaveChangesAsync();
                        await transation.CommitAsync();
                    }
                    catch (OrderSavedFailedException)
                    {
                        await transation.RollbackAsync();
                        throw new OrderSavedFailedException();
                    }
                }
            }
            catch(ArgumentNullException)
            {
                throw new ChangeOrderStatusException();
            }
            catch(OperationCanceledException)
            {
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
            try
            {
                Order order = await _repository.SelectOrderByIdAsync(orderNummber);
                if (order.UserId != int.Parse(_httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value))
                {
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
            catch (NoAccessRightsException)
            {
                throw new NoAccessRightsException();
            }
            catch (ArgumentNullException)
            {
                throw new SelectOrderException();
            }
            catch (OperationCanceledException)
            {
                throw new SelectOrderException();
            }
        }
        public async Task<List<Order>> GetAllOrdersOfUser()
        {
            try
            {
                int userId = int.Parse(_httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                return await _repository.SelectOrdersOfUserAsync(userId);
            }
            catch (ArgumentNullException)
            {
                throw new SelectOrderException();
            }
            catch (OperationCanceledException)
            {
                throw new SelectOrderException();
            }
        }
    }
}
