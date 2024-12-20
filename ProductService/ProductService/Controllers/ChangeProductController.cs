using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductsService.Services;
using ProductsService.DTOs;
using ProductsService.Services.Exceptions;
using ProductsService.Repositories.Exceptions;

namespace ProductsService.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ChangeProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ChangeProductController(IProductService productService)
        {
            _productService = productService;
        }
        [Authorize(Roles = "admin")]
        [HttpPatch("ChangeProductName")]
        public async Task<IActionResult> ChangeProductNameService([FromBody] ChangeProductNameDTO changeProductNameDTO)
        {
            try
            {
                await _productService.ChangeProductNameAsync(changeProductNameDTO);
                return Ok("Данные успешно изменены!");
            }
            catch (ProductNotFoundException e)
            {
                return Unauthorized(e.Message);
            }
            catch (ChangeToSameDataException)
            {
                return Unauthorized("Вы пытаетесь изменить название на то же самое!");
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPatch("ChangePrice")]
        public async Task<IActionResult> ChangePrice([FromBody] ChangePriceDTO changePriceDTO)
        {
            try
            {
                await _productService.ChangePriceAsync(changePriceDTO);
                return Ok("Данные успешно изменены!");
            }
            catch (ProductNotFoundException e)
            {
                return Unauthorized(e.Message);
            }
            catch (ChangeToSameDataException)
            {
                return Unauthorized("Вы пытаетесь изменить цену на ту же самую!");
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPatch("ChangeDescription")]
        public async Task<IActionResult> ChangeDescription([FromBody] ChangeDescriptionDTO changeDescriptionDTO)
        {
            try
            {
                await _productService.ChangeDescriptionAsync(changeDescriptionDTO);
                return Ok("Данные успешно изменены!");
            }
            catch (ProductNotFoundException e)
            {
                return Unauthorized(e.Message);
            }
            catch (ChangeToSameDataException)
            {
                return Unauthorized("Вы пытаетесь изменить описание товара на то же самое!");
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPatch("ChangeCategoryOfProduct")]
        public async Task<IActionResult> ChangeCategoryOfProduct([FromBody] ChangeCategoryOfProductDTO changeCategoryOfProductDTO)
        {
            try
            {
                await _productService.ChangeCategoryOfProductAsync(changeCategoryOfProductDTO);
                return Ok("Данные успешно изменены!");
            }
            catch (ProductNotFoundException e)
            {
                return Unauthorized(e.Message);
            }
            catch (ChangeToSameDataException)
            {
                return Unauthorized("Вы пытаетесь категорию товара на ту же самую!");
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPatch("ChangeStockQuantity")]
        public async Task<IActionResult> ChangeStockQuantity([FromBody] ChangeStockQuantityDTO changeStockQuantityDTO)
        {
            try
            {
                await _productService.ChangeStockQuantityAsync(changeStockQuantityDTO);
                return Ok("Данные успешно изменены!");
            }
            catch (ProductNotFoundException e)
            {
                return Unauthorized(e.Message);
            }
            catch (ChangeToSameDataException)
            {
                return Unauthorized("Вы пытаетесь изменить количество товара на то же самое!");
            }
        }
    }
}
