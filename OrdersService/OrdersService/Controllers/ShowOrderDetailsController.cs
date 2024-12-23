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
    public class ShowOrderDetailsController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public ShowOrderDetailsController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [Authorize]
        [HttpPost("ShowOrderDetails")]
        public async Task<IActionResult> GetOrderDetails(int orderNummber)
        {
            try
            {
                //Dictionary<string, string> orderInfo = await _orderService.GetOrderDetailsAsync(orderNummber);
                var orderInfo = await _orderService.GetOrderDetailsAsync(orderNummber);
                return Ok(orderInfo);
            }
            catch (NoAccessRightsException)
            {
                return BadRequest("Не верный номер заказа! Попробуйте ещё раз.");
            }
            catch (SelectOrderException)
            {
                return BadRequest("Ошибка при полученни данных о заказе.");
            }
            catch (OrderNotFoundException)
            {
                return BadRequest("Заказ с таким номером не найден.");
            }
        }
    }
}
