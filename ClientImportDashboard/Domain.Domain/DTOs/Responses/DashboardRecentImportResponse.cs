namespace Domain.Domain.DTOs.Responses;
public sealed class DashboardRecentImportResponse
{
    public int AlbumId { get; set; }
    public string AlbumTitle { get; set; } = string.Empty;
    public int ImportedTracksCount { get; set; }
    public DateTime ImportedAtUtc { get; set; }
}
