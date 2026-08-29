using Domain.Entities.Entities;

namespace Domain.Domain.DTOs.Responses;

public sealed class TrackResponse
{
    public int Id { get; set; }
    public int AlbumId { get; set; }
    public int TrackNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public int DurationSeconds { get; set; }
    public string Genre { get; set; } = string.Empty;
    public bool IsActive { get; set; }

    public static TrackResponse FromEntity(Track track) => new()
    {
        Id = track.Id,
        AlbumId = track.AlbumId,
        TrackNumber = track.TrackNumber,
        Title = track.Title,
        DurationSeconds = track.DurationSeconds,
        Genre = track.Genre.Name,
        IsActive = track.IsActive
    };
}
