using Domain.Domain.DTOs.Responses;
using Domain.Domain.Interfaces.Repositories;
using Domain.Domain.Interfaces.Services;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Services.Main;

public class DashboardService(
    IBaseRepository<Album> albumRepository,
    IBaseRepository<Track> trackRepository,
    IBaseRepository<TrackImportHistory> trackImportHistoryRepository) : IDashboardService
{
    public async Task<DashboardSummaryResponse> GetDashboardSummaryAsync()
    {
        var totalAlbums = await albumRepository.GetCountAsync(value => true);
        var totalTracks = await trackRepository.GetCountAsync(value => true);

        var tracks = await trackRepository.GetByFiltersWithNoTrackingAsync(
            value => true,
            query => query.Include(track => track.Genre));

        var albumsByGenre = tracks
            .Where(track => track.Genre is not null && !string.IsNullOrWhiteSpace(track.Genre.Name))
            .GroupBy(track => track.Genre.Name)
            .Select(group => new DashboardGenreCountResponse
            {
                Genre = group.Key,
                AlbumCount = group.Select(track => track.AlbumId).Distinct().Count()
            })
            .OrderByDescending(value => value.AlbumCount)
            .ThenBy(value => value.Genre)
            .ToList();

        var recentImports = (await trackImportHistoryRepository.GetByFiltersWithNoTrackingAsync(
                value => true,
                query => query.Include(value => value.Album)))
            .OrderByDescending(value => value.ImportedAtUtc)
            .Take(10)
            .Select(value => new DashboardRecentImportResponse
            {
                AlbumId = value.AlbumId,
                AlbumTitle = value.Album?.Title ?? string.Empty,
                ImportedTracksCount = value.ImportedTracksCount,
                ImportedAtUtc = value.ImportedAtUtc
            })
            .ToList();

        return new DashboardSummaryResponse
        {
            TotalAlbums = totalAlbums,
            TotalTracks = totalTracks,
            AlbumsByGenre = albumsByGenre,
            RecentImports = recentImports
        };
    }
}
