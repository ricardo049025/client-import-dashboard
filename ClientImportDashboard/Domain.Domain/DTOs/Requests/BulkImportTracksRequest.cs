namespace Domain.Domain.DTOs.Requests;

public sealed class BulkImportTracksRequest
{
    public string CsvContent { get; set; } = string.Empty;
    public bool ImportValidRows { get; set; }
}
