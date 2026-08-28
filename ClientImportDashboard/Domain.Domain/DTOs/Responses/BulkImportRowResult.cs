namespace Domain.Domain.DTOs.Responses;

public sealed class BulkImportRowResult
{
    public int RowNumber { get; set; }
    public int? TrackNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public int? DurationSeconds { get; set; }
    public string Genre { get; set; } = string.Empty;
    public bool? IsActive { get; set; }
    public bool IsValid { get; set; }
    public IReadOnlyCollection<string> Errors { get; set; } = [];
}
