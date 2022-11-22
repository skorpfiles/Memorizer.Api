using Autofac;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SkorpFiles.Memorizer.Api.DataAccess.DependencyInjection;
using SkorpFiles.Memorizer.Api.DataAccess.Extensions;
using SkorpFiles.Memorizer.Api.DataAccess.Mapping;
using SkorpFiles.Memorizer.Api.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.DataAccess.Tests
{
    public abstract class IntegrationTestsBase : IDisposable
    {
        protected ApplicationDbContext DbContext { get; private set; }
        protected ILifetimeScope Container { get; private set; }

        public IntegrationTestsBase()
        {
            ConfigureDependencies();
            ConfigureDbContext();
        }

        public Guid RegisterUser(Guid? nonstandardUserId=null)
        {
            Guid userId = nonstandardUserId ?? Guid.Parse("d661ebd5-5520-4739-ada8-390df5b3b696");
            string registerUserQuery = @$"insert into dbo.AspNetUsers (Id,Discriminator,UserName,Email,EmailConfirmed,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnabled,AccessFailedCount)
                                             values('{userId.ToAspNetUserIdString()}', 'IdentityUser','TestName','testname@email.com', 1, 0, 0, 0, 0)";
            DbContext.Users.FromSqlRaw(registerUserQuery);
            return userId;
        }

        public void Dispose()
        {
            DisposeDbContext();
            ResetDependencies();
        }

        private void ConfigureDependencies()
        {
        }

        private void ConfigureDbContext()
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

            IMapper mapper = mapperConfig.CreateMapper();

            containerBuilder.RegisterInstance(mapper);

            Container = containerBuilder.Build();
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
