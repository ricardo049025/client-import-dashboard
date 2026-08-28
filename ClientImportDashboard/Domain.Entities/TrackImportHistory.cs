using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

[Table("TrackImportHistory", Schema = "Music")]
public class TrackImportHistory
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int Id { get; set; }

    public int AlbumId { get; set; }

    [ForeignKey(nameof(AlbumId))]
    public Album Album { get; set; } = null!;

    public int ImportedTracksCount { get; set; }

    public DateTime ImportedAtUtc { get; set; }
}
