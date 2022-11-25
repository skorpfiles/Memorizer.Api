using Autofac;
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
        protected ILifetimeScope Container { get; private set; }

        protected IMapper Mapper { get; private set; }

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

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule(new DataAccessModule());
            var opt = new DbContextOptionsBuilder<ApplicationDbContext>();
            opt.UseSqlServer(configuration["DatabaseConnectionString"]);
            containerBuilder.RegisterInstance(new ApplicationDbContext(opt.Options)).Keyed<ApplicationDbContext>("DbContext");

            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new DataAccessMappingProfile());
            });

            Mapper = mapperConfig.CreateMapper();

            containerBuilder.RegisterInstance(Mapper);

            Container = containerBuilder.Build();
        }

        public async Task RegisterUserAsync()
        {
            await DbContext.Users.AddAsync(new IdentityUser { Id= Constants.DefaultUserId.ToAspNetUserIdString(), UserName = "TestLogin" });
            await DbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            DisposeDbContext();
            ResetDependencies();
        }

        private void DisposeDbContext()
        {
            DbContext.Database.EnsureDeleted();
        }

        private void ResetDependencies()
        {
            if (Container != null)
                Container.Dispose();
        }
    }
}
