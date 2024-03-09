namespace SkorpFiles.Memorizer.Api.DataAccess.Repositories
{
    public class RepositoryBase(ApplicationDbContext dbContext)
    {
        protected private readonly ApplicationDbContext DbContext = dbContext;
    }
}
