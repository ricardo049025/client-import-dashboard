using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

[Table("Genres", Schema = "Music")]
public class Genre
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int Id { get; set; }
    
    [StringLength(50)]
    public required string Name { get; set; }

    public ICollection<Track> Tracks { get; set; } = new List<Track>();

}
