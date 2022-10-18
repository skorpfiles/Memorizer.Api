using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.DataAccess.Extensions
{
    internal static class QueryableExtensions
    {
        public static IQueryable<TSource> Page<TSource>(this IQueryable<TSource> source, int pageName, int pageSize)
        {
            return source.Skip((pageName - 1) * pageSize).Take(pageSize);
        }
    }
}
