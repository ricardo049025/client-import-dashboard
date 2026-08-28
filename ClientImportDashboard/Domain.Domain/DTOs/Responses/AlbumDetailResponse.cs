namespace Domain.Domain.DTOs.Responses;

public sealed class AlbumDetailResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ArtistName { get; set; } = string.Empty;
    public DateOnly ReleaseDate { get; set; }
    public string? CoverImageUrl { get; set; }
    public IReadOnlyCollection<TrackResponse> Tracks { get; set; } = [];
}
