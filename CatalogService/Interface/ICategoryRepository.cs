using CatalogService.Entities;

namespace CatalogService.Interface
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllCategories();
        Task<Category> GetCategoryById(int categoryId);
        Task<Category> AddCategory(Category category);
        Task UpdateCategory(Category category);
        Task DeleteCategory(int categoryId);

    }
}
