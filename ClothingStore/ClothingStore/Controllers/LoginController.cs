using ClothingStore.DTOs;
using ClothingStore.Model;
using ClothingStore.Services;
using ClothingStore.Services.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace ClothingStore.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class LoginController : ControllerBase
    {
        private readonly IUserService _userService;
        public LoginController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn([FromBody] SignInDTO signInDTO)
        {
            try
            {
                var jwtToken = await _userService.AuthenticationAsync(signInDTO);
                return Ok($"Вы вошли в аккаункт!\nAccess Token: {jwtToken.AccessToken}\nRefresh Token: {jwtToken.RefreshToken}");
            }
            catch (UserNotFoundException)
            {
                return Unauthorized("Пользователь с таким номером телефона не найден.");
            }
            catch (InvalidPasswordException)
            {
                return Unauthorized("Неверный пароль!");
            }            
        }
    }
}
