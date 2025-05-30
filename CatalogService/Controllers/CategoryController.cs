using AutoMapper;
using CatalogService.Dtos;
using CatalogService.Entities;
using CatalogService.Interface;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Controllers
{
    [Route("api/categories")]
    [ApiController]

    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Category>>> Get()
        //{
        //    var result = await categoryRepository.GetAllCategories();
        //    return Ok(result);
        //}

        [HttpGet]
        public async Task<ActionResult<PaginationDto<Category>>> GetPaginatedCategories(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 50) pageSize = 50; // Limit maximum page size

            var result = await categoryService.GetPaginatedCategories(pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("{categoryId}")]
        public async Task<ActionResult<Category>> GetDetails(int categoryId)
        {
            var result = await categoryService.GetCategoryByIdAsync(categoryId);
            return Ok(result);
        }

        [HttpPut("{categoryId}")]
        public async Task<ActionResult<int>> Update(int categoryId, CategoryForCreateAndUpdate categoryForUpdate)
        {
            var categoryToUpdate = await categoryService.GetCategoryByIdAsync(categoryId);
            if (categoryToUpdate == null)
            {
                return NotFound();
            }

            await categoryService.UpdateCategoryAsync(categoryId, categoryForUpdate);

            return Ok();
        }

        [HttpDelete("{categoryId}")]
        public async Task<ActionResult> Delete(int categoryId)
        {
            var serviceToDelete = await categoryService.GetCategoryByIdAsync(categoryId);
            if (serviceToDelete == null)
            {
                return NotFound();
            }

            await categoryService.DeleteCategoryAsync(categoryId);

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<int>> Create(CategoryForCreateAndUpdate categoryDto)
        {
            await categoryService.AddCategoryAsync(categoryDto);

            return Created();
        }
    }
}
