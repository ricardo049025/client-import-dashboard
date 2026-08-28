using Domain.Domain.Interfaces.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Infraestructure.Data.Repositories;

public static class RepositoryRegistrationExtension
{
    /// <summary>
    /// Adds the various repositories to the service collection.
    /// </summary>
    /// <param name="serviceCollection">The service collection to add the DbContexts</param>
    public static IServiceCollection RegisterRepositories(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        return serviceCollection;
    }
}
