using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

[Table("Tracks", Schema = "Music")]
public class Track
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int Id { get; set; }

    public int TrackNumber { get; set; }
    
    [StringLength(100)]
    public required string Title { get; set; }

    public int DurationSeconds { set; get; }

    public bool IsActive { get; set; }

    public int AlbumId { get; set; } = 0;
    
    [ForeignKey(nameof(AlbumId))]
    public Album Album { get; set; } = null!;

    public int GenreId { get; set; } = 0;

    [ForeignKey(nameof(GenreId))]
    public virtual Genre Genre { get; set; } = null!;

}
