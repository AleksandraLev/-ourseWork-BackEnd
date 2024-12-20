using ClothingStore.DTOs;
using ClothingStore.Services;
using ClothingStore.Services.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClothingStore.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class RefreshTokenController : ControllerBase
    {
        private readonly IUserService _userService;
        public RefreshTokenController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken(string refreshToken, string deviceId)
        {
            try
            {
                var jwtToken = await _userService.GetJwtTokenAsync(refreshToken, deviceId);
                return Ok($"Токен успешно обновлён!\nAccess Token: {jwtToken.AccessToken}\nRefresh Token: {jwtToken.RefreshToken}");
            }
            catch (TokenExistsFaledException)
            {
                return Unauthorized("Такой токен не найден.");
            }
        }
    }
}
