using Microsoft.AspNetCore.Mvc;
using ProductsService.DTOs;
using ProductsService.Services;
using ProductsService.Services.Exceptions;
using ProductsService.Model;
using Microsoft.AspNetCore.Authorization;


namespace ProductsService.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class SelectProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        public SelectProductsController(IProductService productService)
        {
            _productService = productService;
        }
        [HttpPost("ShowProductsOfCategory")]
        public async Task<IActionResult> ShowProductsOfCategory([FromBody] CategoryDTO categoryDTO)
        {
            try
            {
                List<ProductInfoDTO> namesOfProducts = await _productService.GetProductsOfCategory(categoryDTO);
                return Ok(namesOfProducts);
            }
            catch (CategoryNotFoundException e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPost("SelectProductsByPrice")]
        public async Task<IActionResult> SelectProductsByPrice([FromBody] MinAmdMaxPrisesDTO prisesDTO)
        {
            try
            {
                List<ProductInfoDTO> namesOfProducts = await _productService.GetProductsSelectedByPriseAsync(prisesDTO);
                return Ok(namesOfProducts);
            }
            catch (ProductNotFoundException e)
            {
                return BadRequest(e.Message);
            }
            catch(GetProductException e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPost("SelectProductsByArticleNumber")]
        public async Task<IActionResult> SelectProductsByArticleNumber(int Id)
        {
            try
            {
                Product product = await _productService.GetByIdAsync(Id);
                return Ok(product);
            }
            catch (ProductNotFoundException e)
            {
                return BadRequest(e.Message);
            }
            catch (GetProductException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
