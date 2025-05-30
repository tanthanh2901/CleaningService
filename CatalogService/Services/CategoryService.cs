using AutoMapper;
using CatalogService.DbContexts;
using CatalogService.Dtos;
using CatalogService.Entities;
using CatalogService.Extensions;
using CatalogService.Interface;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly CatalogDbContext dbContext;
        private readonly ICategoryRepository categoryRepository;
        private readonly IMapper mapper;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper, CatalogDbContext dbContext)
        {
            this.categoryRepository = categoryRepository;
            this.mapper = mapper;
            this.dbContext = dbContext;
        }

        public async Task<CategoryForCreateAndUpdate> AddCategoryAsync(CategoryForCreateAndUpdate categoryDto)
        {
            var category =  mapper.Map<Category>(categoryDto);

            await categoryRepository.AddCategory(category);
            return categoryDto;
        }

        public async Task DeleteCategoryAsync(int categoryId)
        {
            var category = await categoryRepository.GetCategoryById(categoryId);

            await categoryRepository.DeleteCategory(category);
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {

            var categories = await categoryRepository.GetAllCategories();
            return mapper.Map<IEnumerable<CategoryDto>>(categories);
        }

        public async Task<CategoryDto> GetCategoryByIdAsync(int categoryId)
        {
            var category =  await categoryRepository.GetCategoryById(categoryId);
            return mapper.Map<CategoryDto>(category);
        }

        public async Task<PaginationDto<CategoryDto>> GetPaginatedCategories(int pageNumber, int pageSize)
        {
            var query = dbContext.Categories.AsQueryable();
            var result = await query.ToPaginatedListAsync(pageNumber, pageSize);

            var categoryDtos = mapper.Map<List<CategoryDto>>(result.Items);

            return new PaginationDto<CategoryDto>
            {
                Items = categoryDtos,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount,
                TotalPages = result.TotalPages
            };
        }

        public async Task UpdateCategoryAsync(int categoryId, CategoryForCreateAndUpdate categoryDto)
        {
            var category = mapper.Map<Category>(categoryDto);

            await categoryRepository.UpdateCategory(categoryId,category);
        }
    }
}
