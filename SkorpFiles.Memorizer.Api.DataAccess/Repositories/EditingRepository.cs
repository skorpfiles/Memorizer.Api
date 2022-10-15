using SkorpFiles.Memorizer.Api.Models.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.DataAccess.Repositories
{
    public class EditingRepository : RepositoryBase, IEditingRepository
    {
        public EditingRepository(ApplicationDbContext dbContext) : base(dbContext) { }
    }
}
