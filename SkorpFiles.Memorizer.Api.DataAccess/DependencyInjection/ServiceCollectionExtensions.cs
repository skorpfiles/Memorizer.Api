using Microsoft.Extensions.DependencyInjection;
using SkorpFiles.Memorizer.Api.DataAccess.Repositories;
using SkorpFiles.Memorizer.Api.Models.Interfaces.DataAccess;

namespace SkorpFiles.Memorizer.Api.DataAccess.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IAccountRepository, AccountRepository>();
            serviceCollection.AddScoped<IEditingRepository, EditingRepository>();
            serviceCollection.AddScoped<ITrainingRepository, TrainingRepository>();
            return serviceCollection;
        }
    }
}