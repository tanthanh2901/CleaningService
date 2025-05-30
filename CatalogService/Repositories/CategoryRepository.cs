using CatalogService.DbContexts;
using CatalogService.Interface;
using CatalogService.Entities;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly CatalogDbContext dbContext;

        public CategoryRepository(CatalogDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Category> AddCategory(Category category)
        {
            await dbContext.Categories.AddAsync(category);
            await dbContext.SaveChangesAsync();
            return category;
        }

        public async Task DeleteCategory(Category category)
        {
            dbContext.Categories.Remove(category);
            await dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            return await dbContext.Categories.ToListAsync();
        }

        public async Task<Category> GetCategoryById(int categoryId)
        {
            return await dbContext.Categories.FirstOrDefaultAsync(x => x.CategoryId == categoryId);
        }

        public async Task UpdateCategory(int categoryId, Category category)
        {
            var existingCategory = await dbContext.Categories.FindAsync(categoryId);
            if (existingCategory == null)
                throw new ArgumentException("Category not found");

            existingCategory.Name = category.Name;

            await dbContext.SaveChangesAsync();
        }
    }
}
