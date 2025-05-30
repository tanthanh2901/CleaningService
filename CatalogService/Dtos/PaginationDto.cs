using CatalogService.Entities;

namespace CatalogService.Dtos
{
    public class PaginationDto<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
        public IEnumerable<T> Items { get; set; }
    }
} 