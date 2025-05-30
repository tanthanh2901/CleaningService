using CatalogService.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Extensions
{
    public static class PaginationExtensions
    {
        public static async Task<PaginationDto<T>> ToPaginatedListAsync<T>(
            this IQueryable<T> source,
            int pageNumber,
            int pageSize)
        {
            var totalCount = await source.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var items = await source
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginationDto<T>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                Items = items
            };
        }

        public static IQueryable<T> ApplyPagination<T>(
            this IQueryable<T> source,
            int pageNumber,
            int pageSize)
        {
            return source
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
        }
    }
} 