namespace Domain.Domain.DTOs.Responses;

public sealed class BulkImportTracksResult
{
    public bool PreviewOnly { get; set; }
    public int TotalRows { get; set; }
    public int ValidRows { get; set; }
    public int InvalidRows { get; set; }
    public int ImportedRows { get; set; }
    public IReadOnlyCollection<BulkImportRowResult> Rows { get; set; } = [];
}
