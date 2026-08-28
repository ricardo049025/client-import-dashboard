namespace Domain.Domain.DTOs.Classes;

public sealed class ParsedTrackRow
{
    public int TrackNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public int DurationSeconds { get; set; }
    public int GenreId { get; set; }
    public bool IsActive { get; set; }
}
