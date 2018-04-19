using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace connections.Helpers
{
    public class PagedList<T> : List<T>
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public PagedList(List<T> items, int count, int pageNumber, int pagesize)
        {
            this.TotalCount = count;
            this.PageSize = pageNumber;
            this.CurrentPage = pagesize;
            this.TotalPages = (int)Math.Ceiling(count / (double)pageNumber);
            this.AddRange(items);

        }

        public static async Task<PagedList<T>> createPagedList(IQueryable<T> source, int pageSize, int pageNumber)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedList<T>(items, count, pageSize, pageNumber);

        }
    }
}