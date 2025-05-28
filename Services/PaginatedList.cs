using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class PaginatedList<T>
    {
        public List<T> Items { get; }
        public int TotalCount { get; }
        public int PageIndex { get; }
        public int PageSize { get; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            Items = items ?? throw new ArgumentNullException(nameof(items));
            TotalCount = count;
            PageIndex = pageIndex;
            PageSize = pageSize;
        }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        // Valfritt: du kan lägga till en statisk hjälpfunktion om du vill skapa listan direkt från en IQueryable
        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await Task.Run(() => source.Count());
            var items = await Task.Run(() =>
                source.Skip((pageIndex - 1) * pageSize)
                      .Take(pageSize)
                      .ToList());

            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
