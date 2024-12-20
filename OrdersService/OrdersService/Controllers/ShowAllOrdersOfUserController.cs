using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrdersService.Repositories.Exceptions;
using OrdersService.Services;
using OrdersService.Services.Exceptions;

namespace OrdersService.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ShowAllOrdersOfUserController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public ShowAllOrdersOfUserController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [Authorize]
        [HttpGet("ShowAllOrdersOfUser")]
        public async Task<IActionResult> GetAllOrdersOfUser()
        {
            try
            {
                var orders = await _orderService.GetAllOrdersOfUser();
                return Ok(orders);
            }
            catch (SelectOrderException)
            {
                return BadRequest("Ошибка при получении информации о заказах.");
            }
        }
    }
}
