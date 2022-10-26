using Microsoft.EntityFrameworkCore;
using SkorpFiles.Memorizer.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.DataAccess.Extensions
{
    internal static class QueryableExtensions
    {
        public static IQueryable<TSource> Page<TSource>(this IQueryable<TSource> source, int pageNumber, int pageSize)
        {
            return source.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }

        public static async Task<PaginatedCollection<TSource>> ToPaginatedCollectionAsync<TSource>(this IQueryable<TSource> source, int pageNumber, int pageSize)
        {
            var totalCount = await source.CountAsync();
            var page = source.Page(pageNumber, pageSize);
            return new PaginatedCollection<TSource>(await page.ToListAsync(), totalCount, pageNumber);
        }
    }
}
