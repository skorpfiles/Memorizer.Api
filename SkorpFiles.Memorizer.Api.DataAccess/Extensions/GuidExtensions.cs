using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.DataAccess.Extensions
{
    public static class GuidExtensions
    {
        public static string ToAspNetUserIdString(this Guid source)
        {
            return source.ToString().ToLowerInvariant();
        }
    }
}
