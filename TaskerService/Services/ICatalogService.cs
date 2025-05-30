using TaskerService.Dtos;

namespace TaskerService.Services
{
    public interface ICatalogService
    {
        Task<CategoryDto> GetCategory(int id);
    }
}
