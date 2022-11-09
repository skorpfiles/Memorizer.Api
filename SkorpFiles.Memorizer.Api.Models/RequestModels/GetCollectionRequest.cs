using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.Models.RequestModels
{
    public class GetCollectionRequest
    {
        public virtual int PageNumber { get; set; } = 1;
        public virtual int PageSize { get; set; } = 50;
    }
}
