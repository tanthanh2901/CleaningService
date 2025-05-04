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

        public async Task DeleteCategory(int categoryId)
        {
            var category = await this.GetCategoryById(categoryId);
            dbContext.Categories.Remove(category);
            await dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            return await dbContext.Categories.ToListAsync();
        }

        public async Task<Category> GetCategoryById(int categoryId)
        {
            return await dbContext.Categories.Where(x => x.CategoryId == categoryId).FirstOrDefaultAsync();
        }

        public async Task UpdateCategory(Category category)
        {
            try
            {
                dbContext.Entry(category).State = EntityState.Modified;
                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new InvalidOperationException("Concurrency issue occurred while updating", ex);
            }
        }
    }
}
