using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Entities;

[Table("Albums", Schema = "Music")]
public class Album
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;
    public string ArtistName { get; set; } = string.Empty;
    public DateOnly ReleaseDate { get; set; }
    public string? CoverImageUrl { get; set; }

    public ICollection<Track> Tracks { get; set; } = new List<Track>();
}