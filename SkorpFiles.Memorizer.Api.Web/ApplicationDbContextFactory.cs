using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SkorpFiles.Memorizer.Api.DataAccess;

namespace SkorpFiles.Memorizer.Api.Web
{

    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer();
            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}