using Domain.Domain.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Services.Main;

public static class SeviceRegistrationExtension
{
    public static IServiceCollection RegisterServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IGenresService, GenresService>();
        serviceCollection.AddScoped<IAlbumsService, AlbumsService>();
        serviceCollection.AddScoped<ITracksService, TracksService>();
        serviceCollection.AddScoped<IDashboardService, DashboardService>();
        return serviceCollection;
    }
}
