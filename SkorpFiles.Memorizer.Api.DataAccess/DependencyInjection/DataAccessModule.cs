using Autofac;
using Autofac.Core;
using SkorpFiles.Memorizer.Api.DataAccess.Repositories;
using System.Reflection;
using Module = Autofac.Module;

namespace SkorpFiles.Memorizer.Api.DataAccess.DependencyInjection
{
    public class DataAccessModule:Module
    {
        public DataAccessModule() : base() { }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<RepositoryBase>()
                .AsSelf()
                .WithParameter(new ResolvedParameter(
                (pi, ctx) => pi.Name == "dbContext",
                (pi, ctx) => ctx.ResolveKeyed<ApplicationDbContext>("DbContext")));

            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .AsImplementedInterfaces()
                .As<RepositoryBase>()
                .WithParameter(new ResolvedParameter(
                    (pi, ctx) => pi.Name == "dbContext",
                    (pi, ctx) => ctx.ResolveKeyed<ApplicationDbContext>("DbContext")));

            base.Load(builder);
        }
    }
}
