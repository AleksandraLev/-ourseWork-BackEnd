using ClothingStore.DTOs;
using ClothingStore.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ClothingStore.Services.Exceptions;

namespace ClothingStore.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class LogoutController : ControllerBase
    {
        private readonly IUserService _userService;
        public LogoutController(IUserService userService)
        {
            _userService = userService;
        }
        [Authorize]
        [HttpPost("SignOut")]
        public async Task<IActionResult> NewSignOut()
        {
            try
            {
                await _userService.LogoutAsync();
                return Ok("Вы вышли из аккаунта!");
            }
            catch (LogoutFaledException)
            {
                return BadRequest("Возникла ошибка!.");
            }
        }
    }
}
