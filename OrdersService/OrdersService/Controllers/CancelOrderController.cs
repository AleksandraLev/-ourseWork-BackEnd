using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrdersService.DTOs;
using OrdersService.Repositories.Exceptions;
using OrdersService.Services;
using OrdersService.Services.Exceptions;

namespace OrdersService.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class CancelOrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public CancelOrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [Authorize]
        [HttpPost("CancelOrder")]
        public async Task<IActionResult> CancelOrder(int orderNummber)
        {
            try
            {
                await _orderService.CancelOrderAsync(orderNummber);
                return Ok("Заказ отменён!");
            }
            catch (NoAccessRightsException)
            {
                return BadRequest("Не верный номер заказа! Попробуйте ещё раз.");
            }
            catch (OrderSavedFailedException)
            {
                return BadRequest("Ошибка при отмене заказа.");
            }
            catch(ChangeOrderStatusException)
            {
                return BadRequest("Ошибка при изменении статуса заказа.");
            }
            catch (OrderNotFoundException)
            {
                return BadRequest("Заказ с таким номером не найден.");
            }
        }
    }
}
