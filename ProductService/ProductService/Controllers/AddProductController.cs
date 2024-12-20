using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductsService.DTOs;
using ProductsService.Repositories.Exceptions;
using ProductsService.Services;
using ProductsService.Services.Exceptions;


namespace ProductsService.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AddProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public AddProductController(IProductService productService)
        {
            _productService = productService;
        }
        [Authorize(Roles = "admin")]
        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProduct([FromBody] AddProductDTO addProductDTO)
        {
            try
            {
                await _productService.CreateProductAsync(addProductDTO);
                return Ok("Товар успешно добавлен!");
            }
            catch (ProductSavedFaledException)
            {
                return BadRequest("Ошибка при добавлении товара.");
            }
            catch (CategoryNotFoundException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
