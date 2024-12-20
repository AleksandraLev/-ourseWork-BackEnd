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
    public class AddCategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public AddCategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        [Authorize(Roles = "admin")]
        [HttpPost("AddCategory")]
        public async Task<IActionResult> AddCategory([FromBody] CategoryDTO categoryDTO)
        {
            try
            {
                await _categoryService.CreateCategoryAsync(categoryDTO);
                return Ok("Категория успешно добавлена!");
            }
            catch (CategorySavedFaledException)
            {
                return BadRequest("Ошибка при добавлении категории.");
            }
            catch (CategoryExistsException)
            {
                return Conflict("Такая категрия уже существует.");
            }
        }
    }
}
