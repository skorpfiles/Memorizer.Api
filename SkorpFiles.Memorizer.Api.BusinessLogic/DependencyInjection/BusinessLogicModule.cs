using Autofac;
using SkorpFiles.Memorizer.Api.Interfaces.BusinessLogic;

namespace SkorpFiles.Memorizer.Api.BusinessLogic.DependencyInjection
{
    public class BusinessLogicModule:Module
    {
        public BusinessLogicModule() : base() { }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AccountLogic>()
                .As<IAccountLogic>();

            base.Load(builder);
        }
    }
}
