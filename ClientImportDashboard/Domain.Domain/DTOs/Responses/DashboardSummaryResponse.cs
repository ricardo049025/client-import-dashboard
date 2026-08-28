namespace Domain.Domain.DTOs.Responses;

public sealed class DashboardSummaryResponse
{
    public int TotalAlbums { get; set; }
    public int TotalTracks { get; set; }
    public IReadOnlyCollection<DashboardGenreCountResponse> AlbumsByGenre { get; set; } = [];
    public IReadOnlyCollection<DashboardRecentImportResponse> RecentImports { get; set; } = [];
}
