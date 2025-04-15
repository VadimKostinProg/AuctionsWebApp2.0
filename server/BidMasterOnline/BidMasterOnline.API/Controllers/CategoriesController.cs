using BidMasterOnline.Application.Constants;
using BidMasterOnline.Application.DTO;
using BidMasterOnline.Application.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BidMasterOnline.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoriesService _categoriesService;

        public CategoriesController(ICategoriesService categoriesService)
        {
            _categoriesService = categoriesService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetAllCategories()
        {
            return Ok(await _categoriesService.GetAllCategoriesAsync());
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<CategoryDTO>> GetCategoryById([FromRoute] Guid id)
        {
            return Ok(await _categoriesService.GetCategoryByIdAsync(id));
        }

        [HttpGet("list")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<ListModel<CategoryDTO>>> GetCategoriesList(
            [FromQuery] SpecificationsDTO specifications)
        {
            return Ok(await _categoriesService.GetCategoriesListAsync(specifications));
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<string>> CreateNewCategory([FromBody] CreateCategoryDTO category)
        {
            await _categoriesService.CreateNewCategoryAsync(category);

            return Ok(new { Message = $"Category {category.Name} has been created successfully." });
        }

        [HttpPut]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<string>> UpdateCategory([FromBody] UpdateCategoryDTO category)
        {
            await _categoriesService.UpdateCategoryAsync(category);

            return Ok(new { Message = $"Category {category.Name} has been updated successfully."});
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<string>> DeleteCategory([FromRoute] Guid id)
        {
            await _categoriesService.DeleteCategoryAsync(id);

            return Ok(new { Message = "Category has been deleted succesffully." });
        }
    }
}
