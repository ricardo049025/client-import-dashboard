using Domain.Entities.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities.Seeders;

public static class AppDbSeeder
{
    public static async Task SeedAsync(ApiDbContext context)
    {
        // Prevent duplicate seed data every time the API starts.
        if (await context.Albums.AnyAsync()) return;

        var genres = new List<Genre> { new() { Name = "Pop" },new() { Name = "Rock" },new() { Name = "Hip-Hop" },new() { Name = "R&B" },new() { Name = "Electronic" },new() { Name = "Latin" }};
        context.Genres.AddRange(genres);
        await context.SaveChangesAsync();

        var genreByName = genres.ToDictionary(genre => genre.Name);

        var album1 = new Album { Title = "Neon Nights",ArtistName = "Luna Reyes",ReleaseDate = new DateOnly(2025, 6, 14),CoverImageUrl = "https://i.scdn.co/image/ab67616d00001e02a0ad79db81c0f95bf4c0c6ea" };
        var album2 = new Album {Title = "Coastal Echoes",ArtistName = "The Blue Lines",ReleaseDate = new DateOnly(2024, 11, 8),CoverImageUrl = "https://www.extrememusic.com/albums/3655" };
        var album3 = new Album {Title = "Ritmo del Sol",ArtistName = "María Sol",ReleaseDate = new DateOnly(2026, 2, 20),CoverImageUrl = "https://www.deezer.com/es/album/606003252" };

        context.Albums.AddRange(album1, album2, album3);
        await context.SaveChangesAsync();

        var tracks = new List<Track> 
        {   new() { AlbumId = album1.Id, GenreId = genreByName["Pop"].Id, TrackNumber = 1, Title = "Midnight Drive", DurationSeconds = 224, IsActive = false },
            new() { AlbumId = album1.Id, GenreId = genreByName["Electronic"].Id, TrackNumber = 2, Title = "Electric Heart", DurationSeconds = 198, IsActive = false },
            new() { AlbumId = album1.Id, GenreId = genreByName["R&B"].Id, TrackNumber = 3, Title = "After Hours", DurationSeconds = 245, IsActive = true },
            new() { AlbumId = album2.Id, GenreId = genreByName["Rock"].Id, TrackNumber = 1, Title = "Waves Don't Wait", DurationSeconds = 231, IsActive = false },
            new() { AlbumId = album2.Id, GenreId = genreByName["Rock"].Id, TrackNumber = 2, Title = "Golden Horizon", DurationSeconds = 211, IsActive = false },
            new() { AlbumId = album3.Id, GenreId = genreByName["Latin"].Id, TrackNumber = 1, Title = "Bajo el Sol", DurationSeconds = 207, IsActive = false },
            new() { AlbumId = album3.Id, GenreId = genreByName["Latin"].Id, TrackNumber = 2, Title = "Sin Miedo", DurationSeconds = 219, IsActive = false } 
        };
        context.Tracks.AddRange(tracks);
        await context.SaveChangesAsync();
    }
}
