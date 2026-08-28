namespace Domain.Domain.Constants;

public class ApiEndpointsPath
{
    public const string V1GetGenres = "/api/v1/Genres";
    public const string DashboardV1 = "/api/v1/dashboard";
    public const string AlbumsV1 = "/api/v1/albums";
    public const string AlbumByIdV1 = "/api/v1/albums/{id:int}";
    public const string AlbumTracksV1 = "/api/v1/albums/{albumId:int}/tracks";
    public const string AlbumTracksBulkImportV1 = "/api/v1/albums/{albumId:int}/tracks/bulk-import";
    public const string TrackByIdV1 = "/api/v1/tracks/{id:int}";
}
