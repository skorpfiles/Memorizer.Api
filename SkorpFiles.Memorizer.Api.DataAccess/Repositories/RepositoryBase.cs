using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.DataAccess.Repositories
{
    public class RepositoryBase
    {
        protected private readonly ApplicationDbContext DbContext;
        public RepositoryBase(ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
        }
    }
}
