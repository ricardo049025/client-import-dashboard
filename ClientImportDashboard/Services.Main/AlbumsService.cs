using Domain.Domain.DTOs.Requests;
using Domain.Domain.DTOs.Responses;
using Domain.Domain.Interfaces.Repositories;
using Domain.Domain.Interfaces.Services;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Services.Main;

public class AlbumsService(IBaseRepository<Album> albumRepository, IBaseRepository<Track> trackRepository) : IAlbumsService
{
    public async Task<IEnumerable<AlbumResponse>> GetAlbumsAsync(string? search, string? genre)
    {
        var albums = await albumRepository.GetByFiltersWithNoTrackingAsync(value => true, query => query.Include(album => album.Tracks).ThenInclude(track => track.Genre));
        var query = albums.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(album =>
                album.Title.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                album.ArtistName.Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(genre))
        {
            var genreTerm = genre.Trim();
            query = query.Where(album => album.Tracks.Any(track => track.Genre.Name == genreTerm));
        }

        return query
            .OrderBy(album => album.Title)
            .Select(album => ToAlbumResponse(album))
            .ToList();
    }

    public async Task<AlbumDetailResponse?> GetAlbumByIdAsync(int albumId)
    {
        var album = await albumRepository.FindFirstOrDefaultWithNoTrackingAsync(x => x.Id == albumId,y => y
                                         .Include(value => value.Tracks)
                                         .ThenInclude(track => track.Genre));

        if (album is null) return null;

        return new AlbumDetailResponse
        {
            Id = album.Id,
            Title = album.Title,
            ArtistName = album.ArtistName,
            ReleaseDate = album.ReleaseDate,
            CoverImageUrl = album.CoverImageUrl,
            Tracks = album.Tracks
                .OrderBy(track => track.TrackNumber)
                .Select(ToTrackResponse)
                .ToList()
        };
    }

    public async Task<AlbumResponse> CreateAlbumAsync(UpsertAlbumRequest request)
    {
        var entity = new Album
        {
            Title = request.Title.Trim(),
            ArtistName = request.ArtistName.Trim(),
            ReleaseDate = request.ReleaseDate,
            CoverImageUrl = string.IsNullOrWhiteSpace(request.CoverImageUrl) ? null : request.CoverImageUrl.Trim()
        };

        await albumRepository.AddAsync(entity);

        return ToAlbumResponse(entity);
    }

    public async Task<AlbumResponse?> UpdateAlbumAsync(int albumId, UpsertAlbumRequest request)
    {
        var album = await albumRepository.FindFirstOrDefaultAsync(x => x.Id == albumId);

        if (album is null) return null;

        album.Title = request.Title.Trim();
        album.ArtistName = request.ArtistName.Trim();
        album.ReleaseDate = request.ReleaseDate;
        album.CoverImageUrl = string.IsNullOrWhiteSpace(request.CoverImageUrl) ? null : request.CoverImageUrl.Trim();

        await albumRepository.UpdateAsync(album);
        return ToAlbumResponse(album);
    }

    public async Task<bool> DeleteAlbumAsync(int albumId)
    {
        var album = await albumRepository.FindFirstOrDefaultAsync(
            value => value.Id == albumId,
            query => query.Include(value => value.Tracks));

        if (album is null) return false;
        if (album.Tracks.Count > 0) await trackRepository.DeleteRangeAsync(album.Tracks);

        await albumRepository.DeleteAsync(album);

        return true;
    }

    private static AlbumResponse ToAlbumResponse(Album album) => new()
    {
        Id = album.Id,
        Title = album.Title,
        ArtistName = album.ArtistName,
        ReleaseDate = album.ReleaseDate,
        CoverImageUrl = album.CoverImageUrl,
        TrackCount = album.Tracks.Count,
        Genres = album.Tracks
            .Select(track => track.Genre.Name)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(name => name)
            .ToList()
    };

    private static TrackResponse ToTrackResponse(Track track) => new()
    {
        Id = track.Id,
        AlbumId = track.AlbumId,
        TrackNumber = track.TrackNumber,
        Title = track.Title,
        DurationSeconds = track.DurationSeconds,
        Genre = track.Genre.Name,
        IsActive = track.IsActive
    };
}
