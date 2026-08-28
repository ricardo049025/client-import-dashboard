using Microsoft.EntityFrameworkCore;

namespace Domain.Entities.Contexts;

public  class ApiDbContext: DbContext
{
    public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options) { }

    public DbSet<Track> Tracks { get; set; }
    public DbSet<Album> Albums { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<TrackImportHistory> TrackImportHistory { get; set; }

    //protected override void OnModelCreating(ModelBuilder modelBuilder)
    //{
    //    base.OnModelCreating(modelBuilder);
    //    SeedData(modelBuilder);

    //}

    //private static void SeedData(ModelBuilder modelBuilder)
    //{
    //    // Seed Genres
    //    modelBuilder.Entity<Genre>().HasData(
    //        new Genre { Id = 1, Name = "Rock" },
    //        new Genre { Id = 2, Name = "Pop" },
    //        new Genre { Id = 3, Name = "Jazz" }
    //    );
    //    // Seed Albums
    //    modelBuilder.Entity<Album>().HasData(
    //        new Album { Id = 1, Title = "Album 1", ArtistName = "Artist 1", ReleaseDate = new DateOnly(2020, 1, 1), CoverImageUrl = "https://placehold.co/400x400/1e293b/ffffff?text=Neon+Nights"},
    //        new Album { Id = 2, Title = "Album 2", ArtistName = "Artist 2", ReleaseDate = new DateOnly(2021, 2, 2), CoverImageUrl = "https://placehold.co/400x400/0f766e/ffffff?text=Coastal+Echoes" }
    //    );
    //    // Seed Tracks
    //    modelBuilder.Entity<Track>().HasData(
    //        new Track { Id = 1, TranckNumber = 1, Title = "Track 1", DurationSeconds = 180, IsActive = true, AlbumId = 1, GenreId = 1 },
    //        new Track { Id = 2, TranckNumber = 2, Title = "Track 2", DurationSeconds = 200, IsActive = true, AlbumId = 1, GenreId = 2 },
    //        new Track { Id = 3, TranckNumber = 3, Title = "Track 3", DurationSeconds = 220, IsActive = true, AlbumId = 2, GenreId = 3 }
    //    );
    //}
}
