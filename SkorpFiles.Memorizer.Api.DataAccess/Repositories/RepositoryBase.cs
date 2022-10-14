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
