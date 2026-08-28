namespace Domain.Domain.DTOs.Requests;

public sealed class UpsertAlbumRequest
{
    public string Title { get; set; } = string.Empty;
    public string ArtistName { get; set; } = string.Empty;
    public DateOnly ReleaseDate { get; set; }
    public string? CoverImageUrl { get; set; }
}
