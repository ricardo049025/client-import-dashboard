namespace Domain.Domain.DTOs.Responses;

public sealed class DashboardGenreCountResponse
{
    public string Genre { get; set; } = string.Empty;
    public int AlbumCount { get; set; }
}
