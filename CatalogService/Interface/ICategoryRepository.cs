using CatalogService.Entities;

namespace CatalogService.Interface
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllCategories();
        Task<Category> GetCategoryById(int categoryId);
        Task<Category> AddCategory(Category category);
        Task UpdateCategory(int categoryId, Category category);
        Task DeleteCategory(Category category);
    }
}
