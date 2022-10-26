using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.Models
{
    public class PaginatedCollection<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }

        public PaginatedCollection(IEnumerable<T> items, int totalCount, int pageNumber)
        {
            Items = items;
            TotalCount = totalCount;
            PageNumber = pageNumber;
        }
    }
}
