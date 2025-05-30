using CatalogService.Dtos;

namespace CatalogService.Interface
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryDto> GetCategoryByIdAsync(int categoryId);
        Task<CategoryForCreateAndUpdate> AddCategoryAsync(CategoryForCreateAndUpdate categoryDto);
        Task UpdateCategoryAsync(int categoryId, CategoryForCreateAndUpdate categoryDto);
        Task DeleteCategoryAsync(int categoryId);
        Task<PaginationDto<CategoryDto>> GetPaginatedCategories(int pageNumber, int pageSize);

    }
}
