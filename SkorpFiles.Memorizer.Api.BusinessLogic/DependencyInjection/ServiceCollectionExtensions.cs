using Microsoft.Extensions.DependencyInjection;
using SkorpFiles.Memorizer.Api.Models.Interfaces.BusinessLogic;

namespace SkorpFiles.Memorizer.Api.BusinessLogic.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBusinessLogic(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IAccountLogic, AccountLogic>();
            serviceCollection.AddScoped<IEditingLogic, EditingLogic>();
            serviceCollection.AddScoped<ITrainingLogic, TrainingLogic>();
            return serviceCollection;
        }
    }
}
