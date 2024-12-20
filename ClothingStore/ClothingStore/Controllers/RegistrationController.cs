using ClothingStore.Services;
using ClothingStore.Model;
using Microsoft.AspNetCore.Mvc;
using ClothingStore.Repositories.Exeptions;
using ClothingStore.DTOs;

namespace ClothingStore.Controllers // Слой представлния
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class RegistrationController : ControllerBase
    {
        private readonly IUserService _userService;
        public RegistrationController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost("Registration/user")]
        public async Task<IActionResult> SignUp([FromBody] RegistrationDTO registrationDTO)
        {
            try
            {
                await _userService.CreateUserAsync(registrationDTO);
                return Ok("Регистрация прошла успешно!");
            }
            catch (UserSavedFaledException)
            {
                return BadRequest("Ошибка при регистрации.");
            }
            catch (PhoneNumberExistsException)
            {
                return Conflict("Пользователь с таким номером телефона уже существует.");
            }
            catch (EmailExistsException)
            {
                return Conflict("Пользователь с такой почтой уже существует.");
            }
        }
        [HttpPost("Registration/admin")]
        public async Task<IActionResult> SignUpAdmin([FromBody] RegistrationDTO registrationDTO)
        {
            try
            {
                await _userService.CreateAdminAsync(registrationDTO);
                return Ok("Регистрация прошла успешно!");
            }
            catch (UserSavedFaledException)
            {
                return BadRequest("Ошибка при регистрации.");
            }
            catch (PhoneNumberExistsException)
            {
                return Conflict("Пользователь с таким номером телефона уже существует.");
            }
            catch (EmailExistsException)
            {
                return Conflict("Пользователь с такой почтой уже существует.");
            }
        }
    }
}
