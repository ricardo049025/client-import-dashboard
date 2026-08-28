using Domain.Domain.DTOs.Constants;
using Domain.Domain.Interfaces.Services;
namespace webApi;

public static class ApiEndPoints
{
    public static void ConfigureApiEndpoints(this WebApplication webApplication)
    {
        if (!webApplication.Environment.IsDevelopment() && webApplication.Environment.EnvironmentName != "Local") webApplication.UseHttpsRedirection();
        ConfigureEndpoints(webApplication);
    }

    private static void ConfigureEndpoints(WebApplication app)
    {
        //for Genres endpoints  
        app.MapGet(ApiEndpointsPath.V1GetGenres, async (IGenresService genresService) => Results.Ok(await genresService.GetAllGenresAsync()));

    }
}
