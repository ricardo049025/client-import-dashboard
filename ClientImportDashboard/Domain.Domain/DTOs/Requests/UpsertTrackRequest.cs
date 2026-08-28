namespace Domain.Domain.DTOs.Requests;

public sealed class UpsertTrackRequest
{
    public int TrackNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public int DurationSeconds { get; set; }
    public string Genre { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
