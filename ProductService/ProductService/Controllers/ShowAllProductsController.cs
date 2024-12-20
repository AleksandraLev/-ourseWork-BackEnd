using Microsoft.AspNetCore.Mvc;
using ProductsService.DTOs;
using ProductsService.Services;
using ProductsService.Services.Exceptions;

namespace ProductsService.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ShowAllProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        public ShowAllProductsController(IProductService productService)
        {
            _productService = productService;
        }
        [HttpGet("ShowAllProducts")]
        public async Task<IActionResult> ShowAllProducts()
        {
            try
            {
                List<ProductInfoDTO> namesOfProducts = await _productService.GetAllProductNamesAsync();
                return Ok(namesOfProducts);
            }
            catch (ProductNotFoundException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
