using ClothingStore.DTOs;
using ClothingStore.Services;
using ClothingStore.Services.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClothingStore.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ChangeController : ControllerBase
    {
        private readonly IUserService _userService;
        public ChangeController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize]
        [HttpPatch("ChangeName")]
        public async Task<IActionResult> ChangeName([FromBody] ChangeNameDTO changeNameDTO)
        {
            try
            {
                await _userService.ChangeNameAsync(changeNameDTO);
                return Ok("Данные успешно изменены!");
            }
            catch (UserNotFoundException)
            {
                return Unauthorized("Пользователь с таким номером телефона не найден.");
            }
            catch (ChangeToSameDataException)
            {
                return Unauthorized("Вы пытаетесь изменить имя на то же самое!");
            }
        }

        [Authorize]
        [HttpPatch("ChangePhone")]
        public async Task<IActionResult> ChangePhoneNumber([FromBody] ChangePhoneNumberDTO changePhoneNumberDTO)
        {
            try
            {
                await _userService.ChangePhoneNumberAsync(changePhoneNumberDTO);
                return Ok("Данные успешно изменены!");
            }
            catch (UserNotFoundException)
            {
                return Unauthorized("Пользователь с таким номером телефона не найден.");
            }
            catch (ChangeToSameDataException)
            {
                return Unauthorized("Вы пытаетесь изменить номер телефона на тот же самый!");
            }
        }

        [Authorize]
        [HttpPatch("ChangeEmail")]
        public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailDTO changeEmailDTO)
        {
            try
            {
                await _userService.ChangeEmailAsync(changeEmailDTO);
                return Ok("Данные успешно изменены!");
            }
            catch (UserNotFoundException)
            {
                return Unauthorized("Пользователь с таким номером телефона не найден.");
            }
            catch (ChangeToSameDataException)
            {
                return Unauthorized("Вы пытаетесь изменить почту на ту же самую!");
            }
        }

        [Authorize]
        [HttpPatch("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO changePasswordDTO)
        {
            try
            {
                await _userService.ChangePasswordAsync(changePasswordDTO);
                return Ok("Данные успешно изменены!");
            }
            catch (UserNotFoundException)
            {
                return Unauthorized("Пользователь с таким номером телефона не найден.");
            }
            catch (ChangeToSameDataException)
            {
                return Unauthorized("Вы пытаетесь изменить номер пароль на тот же самый!");
            }
        }
    }
}
