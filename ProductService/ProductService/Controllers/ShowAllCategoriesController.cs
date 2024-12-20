using Microsoft.AspNetCore.Mvc;
using ProductsService.DTOs;
using ProductsService.Services.Exceptions;
using ProductsService.Services;

namespace ProductsService.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ShowAllCategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public ShowAllCategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        [HttpGet("ShowAllCategories")]
        public async Task<IActionResult> ShowAllCategories()
        {
            try
            {
                List<CategoryDTO> categoryNames = await _categoryService.GetAllCategoryNamesAsync();
                return Ok(categoryNames);
            }
            catch (CategoryNotFoundException e)
            {
                return BadRequest(e.Message);
            }
            catch(CategoryIsNullException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
