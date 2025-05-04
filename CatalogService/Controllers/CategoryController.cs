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
        private readonly ICategoryRepository categoryRepository;
        private readonly IMapper mapper;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> Get()
        {
            var result = await categoryRepository.GetAllCategories();
            return Ok(result);
        }

        [HttpGet("{categoryId}")]
        public async Task<ActionResult<IEnumerable<Category>>> GetDetails(int categoryId)
        {
            var result = await categoryRepository.GetCategoryById(categoryId);
            return Ok(result);
        }

        [HttpPut("{categoryId}")]
        public async Task<ActionResult<int>> Update(int categoryId, CategoryForUpdate categoryForUpdate)
        {
            var categoryToUpdate = await categoryRepository.GetCategoryById(categoryId);
            if (categoryToUpdate == null)
            {
                return NotFound();
            }

            mapper.Map(categoryForUpdate, categoryToUpdate, typeof(CategoryForUpdate), typeof(Category));

            await categoryRepository.UpdateCategory(categoryToUpdate);

            return Ok();
        }

        [HttpDelete("{categoryId}")]
        public async Task<ActionResult> Delete(int categoryId)
        {
            var serviceToDelete = await categoryRepository.GetCategoryById(categoryId);
            if (serviceToDelete == null)
            {
                return NotFound();
            }

            await categoryRepository.DeleteCategory(categoryId);

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<int>> Create(CategoryDto categoryDto)
        {
            var category = mapper.Map<Category>(categoryDto);

            await categoryRepository.AddCategory(category);

            return Created();
        }
    }
}
