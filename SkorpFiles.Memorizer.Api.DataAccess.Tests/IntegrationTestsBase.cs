using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SkorpFiles.Memorizer.Api.DataAccess.DependencyInjection;
using SkorpFiles.Memorizer.Api.DataAccess.Extensions;
using SkorpFiles.Memorizer.Api.DataAccess.Mapping;

namespace SkorpFiles.Memorizer.Api.DataAccess.Tests
{
    public abstract class IntegrationTestsBase : IDisposable
    {
        protected ApplicationDbContext DbContext { get; private set; }
        protected IMapper Mapper { get; private set; }
        protected IServiceProvider ServiceProvider { get; private set; }

        public IntegrationTestsBase()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkSqlServer()
                .BuildServiceProvider();

            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();

            var configurationBuilder = new ConfigurationBuilder().AddUserSecrets("a8ce15b5-6722-46fe-9f32-95ceee83f5be");
            var configuration = configurationBuilder.Build();

            builder.UseSqlServer(configuration["DatabaseConnectionString"])
                    .UseInternalServiceProvider(serviceProvider);

            DbContext = new ApplicationDbContext(builder.Options);
            DbContext.Database.EnsureDeleted();
            DbContext.Database.Migrate();

            var services = new ServiceCollection();
            services.AddRepositories();

            var opt = new DbContextOptionsBuilder<ApplicationDbContext>();
            opt.UseSqlServer(configuration["DatabaseConnectionString"]);

            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new DataAccessMappingProfile());
            });

            Mapper = mapperConfig.CreateMapper();

            services.AddScoped(services => mapperConfig.CreateMapper());

            ServiceProvider = services.BuildServiceProvider();
        }

        public async Task RegisterUserAsync()
        {
            await DbContext.Users.AddAsync(new IdentityUser { Id = Constants.DefaultUserId.ToAspNetUserIdString()!, UserName = "TestLogin" });
            await DbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            DisposeDbContext();
            GC.SuppressFinalize(this);
        }

        private void DisposeDbContext()
        {
            DbContext.Database.EnsureDeleted();
        }
    }
}
