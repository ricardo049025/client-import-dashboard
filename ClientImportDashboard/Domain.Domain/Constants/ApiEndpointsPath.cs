namespace Domain.Domain.Constants;

public class ApiEndpointsPath
{
    public const string V1GetGenres = "/api/v1/Genres";

    public const string Albums = "/api/albums";
    public const string AlbumById = "/api/albums/{id:int}";
    public const string AlbumTracks = "/api/albums/{albumId:int}/tracks";
    public const string AlbumTracksBulkImport = "/api/albums/{albumId:int}/tracks/bulk-import";
    public const string TrackById = "/api/tracks/{id:int}";
}
