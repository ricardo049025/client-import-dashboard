using Domain.Domain.Constants;
using Domain.Domain.DTOs.Requests;
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
        #region Dashboard Endpoints
        app.MapGet(ApiEndpointsPath.DashboardV1, async (IDashboardService dashboardService) =>
            Results.Ok(await dashboardService.GetDashboardSummaryAsync()));
        #endregion

        #region Genres Endpoints
        app.MapGet(ApiEndpointsPath.V1GetGenres, async (IGenresService genresService) => Results.Ok(await genresService.GetAllGenresAsync()));
        #endregion

        #region Albums Endpoints
        app.MapGet(ApiEndpointsPath.AlbumsV1, async (string? search, string? genre, IAlbumsService albumsService) => Results.Ok(await albumsService.GetAlbumsAsync(search, genre)));

        app.MapGet(ApiEndpointsPath.AlbumByIdV1, async (int id, IAlbumsService albumsService) =>
        {
            var album = await albumsService.GetAlbumByIdAsync(id);
            return album is null ? Results.NotFound() : Results.Ok(album);
        });

        app.MapPost(ApiEndpointsPath.AlbumsV1, async (UpsertAlbumRequest request, IAlbumsService albumsService) =>
        {
            if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.ArtistName)) return Results.BadRequest("Title and ArtistName are required.");
            var created = await albumsService.CreateAlbumAsync(request);
            return Results.Created($"/api/albums/{created.Id}", created);
        });

        app.MapPut(ApiEndpointsPath.AlbumByIdV1, async (int id, UpsertAlbumRequest request, IAlbumsService albumsService) =>
        {
            if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.ArtistName)) return Results.BadRequest("Title and ArtistName are required.");
            var updated = await albumsService.UpdateAlbumAsync(id, request);
            return updated is null ? Results.NotFound() : Results.Ok(updated);
        });

        app.MapDelete(ApiEndpointsPath.AlbumByIdV1, async (int id, IAlbumsService albumsService) =>
        {
            var deleted = await albumsService.DeleteAlbumAsync(id);
            return deleted ? Results.NoContent() : Results.NotFound();
        });

        #endregion

        #region Trancks EndPoints
        app.MapGet(ApiEndpointsPath.AlbumTracksV1, async (int albumId, string? genre, bool? isActive, ITracksService tracksService) => Results.Ok(await tracksService.GetTracksByAlbumAsync(albumId, genre, isActive)));

        app.MapPost(ApiEndpointsPath.AlbumTracksV1, async (int albumId, UpsertTrackRequest request, ITracksService tracksService) =>
        {
            try
            {
                var created = await tracksService.CreateTrackAsync(albumId, request);
                return created is null ? Results.NotFound("Album not found.") : Results.Created($"/api/tracks/{created.Id}", created);
            }
            catch (ArgumentException ex) { return Results.BadRequest(ex.Message); }
            catch (InvalidOperationException ex) { return Results.BadRequest(ex.Message); }
        });

        app.MapPut(ApiEndpointsPath.TrackByIdV1, async (int id, UpsertTrackRequest request, ITracksService tracksService) =>
        {
            try
            {
                var updated = await tracksService.UpdateTrackAsync(id, request);
                return updated is null ? Results.NotFound() : Results.Ok(updated);
            }
            catch (ArgumentException ex) { return Results.BadRequest(ex.Message); }
            catch (InvalidOperationException ex) { return Results.BadRequest(ex.Message); }
        });

        app.MapDelete(ApiEndpointsPath.TrackByIdV1, async (int id, ITracksService tracksService) =>
        {
            var deleted = await tracksService.DeleteTrackAsync(id);
            return deleted ? Results.NoContent() : Results.NotFound();
        });

        app.MapPost(ApiEndpointsPath.AlbumTracksBulkImportV1, async (int albumId, BulkImportTracksRequest request, ITracksService tracksService) =>
        {
            try
            {
                var result = await tracksService.BulkImportTracksAsync(albumId, request);
                return result is null ? Results.NotFound("Album not found.") : Results.Ok(result);
            }
            catch (ArgumentException ex) { return Results.BadRequest(ex.Message); }
        });
        #endregion
    }
}
