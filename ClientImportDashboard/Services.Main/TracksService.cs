using Domain.Domain.Constants;
using Domain.Domain.DTOs.Classes;
using Domain.Domain.DTOs.Requests;
using Domain.Domain.DTOs.Responses;
using Domain.Domain.Helpers;
using Domain.Domain.Interfaces.Repositories;
using Domain.Domain.Interfaces.Services;
using Domain.Entities.Entities;
using Microsoft.EntityFrameworkCore;

namespace Services.Main;

public class TracksService(
    IBaseRepository<Track> trackRepository,
    IBaseRepository<Album> albumRepository,
    IBaseRepository<Genre> genreRepository,
    IBaseRepository<TrackImportHistory> trackImportHistoryRepository) : ITracksService
{
    public async Task<IEnumerable<TrackResponse>> GetTracksByAlbumAsync(int albumId, string? genre, bool? isActive)
    {
        var tracks = await trackRepository.GetByFiltersWithNoTrackingAsync(
            track => track.AlbumId == albumId,
            query => query.Include(track => track.Genre));

        var query = tracks.AsQueryable();

        if (!string.IsNullOrWhiteSpace(genre))
        {
            var genreTerm = genre.Trim();
            query = query.Where(track => track.Genre.Name == genreTerm);
        }

        if (isActive.HasValue) query = query.Where(track => track.IsActive == isActive.Value);
        
        return query
            .OrderBy(track => track.TrackNumber)
            .Select(track => TrackResponse.FromEntity(track))
            .ToList();
    }

    public async Task<TrackResponse?> CreateTrackAsync(int albumId, UpsertTrackRequest request)
    {
        var albumExists = await albumRepository.GetCountAsync(album => album.Id == albumId) > 0;
        if (!albumExists) return null;

        var normalizedTitle = request.Title.Trim();
        if (string.IsNullOrWhiteSpace(normalizedTitle)) throw new ArgumentException("Title is required.");
        if (request.DurationSeconds <= 0)throw new ArgumentException("DurationSeconds must be greater than zero.");

        var genre = await genreRepository.FindFirstOrDefaultWithNoTrackingAsync(value => value.Name == request.Genre.Trim());
        if (genre is null) throw new ArgumentException("Genre is not supported.");

        var hasTrackNumberConflict = await trackRepository.GetCountAsync(track =>track.AlbumId == albumId && track.TrackNumber == request.TrackNumber) > 0;
        if (hasTrackNumberConflict) throw new InvalidOperationException("Track number must be unique within the album.");

        var albumTracks = await trackRepository.GetByFiltersWithNoTrackingAsync(track => track.AlbumId == albumId);
        var hasTitleConflict = albumTracks.Any(track => string.Equals(track.Title, normalizedTitle, StringComparison.OrdinalIgnoreCase));
        if (hasTitleConflict) throw new InvalidOperationException("Track title must be unique within the album.");

        var trackEntity = new Track
        {
            AlbumId = albumId,
            TrackNumber = request.TrackNumber,
            Title = normalizedTitle,
            DurationSeconds = request.DurationSeconds,
            GenreId = genre.Id,
            IsActive = request.IsActive
        };

        await trackRepository.AddAsync(trackEntity);

        return new TrackResponse
        {
            Id = trackEntity.Id,
            AlbumId = trackEntity.AlbumId,
            TrackNumber = trackEntity.TrackNumber,
            Title = trackEntity.Title,
            DurationSeconds = trackEntity.DurationSeconds,
            Genre = genre.Name,
            IsActive = trackEntity.IsActive
        };
    }

    public async Task<TrackResponse?> UpdateTrackAsync(int trackId, UpsertTrackRequest request)
    {
        var track = await trackRepository.FindFirstOrDefaultAsync(value => value.Id == trackId);
        if (track is null) return null;

        var normalizedTitle = request.Title.Trim();
        if (string.IsNullOrWhiteSpace(normalizedTitle)) throw new ArgumentException("Title is required.");

        if (request.DurationSeconds <= 0) throw new ArgumentException("DurationSeconds must be greater than zero.");

        var genre = await genreRepository.FindFirstOrDefaultWithNoTrackingAsync(value => value.Name == request.Genre.Trim());
        if (genre is null) throw new ArgumentException("Genre is not supported.");

        var hasTrackNumberConflict = await trackRepository.GetCountAsync(value => value.Id != trackId && value.AlbumId == track.AlbumId && value.TrackNumber == request.TrackNumber) > 0;
        if (hasTrackNumberConflict) throw new InvalidOperationException("Track number must be unique within the album.");

        var albumTracks = await trackRepository.GetByFiltersWithNoTrackingAsync(value =>
            value.Id != trackId && value.AlbumId == track.AlbumId);
        var hasTitleConflict = albumTracks.Any(value => string.Equals(value.Title, normalizedTitle, StringComparison.OrdinalIgnoreCase));
        if (hasTitleConflict) throw new InvalidOperationException("Track title must be unique within the album.");

        track.TrackNumber = request.TrackNumber;
        track.Title = normalizedTitle;
        track.DurationSeconds = request.DurationSeconds;
        track.GenreId = genre.Id;
        track.IsActive = request.IsActive;

        await trackRepository.UpdateAsync(track);

        return new TrackResponse
        {
            Id = track.Id,
            AlbumId = track.AlbumId,
            TrackNumber = track.TrackNumber,
            Title = track.Title,
            DurationSeconds = track.DurationSeconds,
            Genre = genre.Name,
            IsActive = track.IsActive
        };
    }

    public async Task<bool> DeleteTrackAsync(int trackId)
    {
        var track = await trackRepository.FindFirstOrDefaultAsync(value => value.Id == trackId);
        if (track is null) return false;

        await trackRepository.DeleteAsync(track);
        return true;
    }

    public async Task<BulkImportTracksResult?> BulkImportTracksAsync(int albumId, BulkImportTracksRequest request)
    {
        var albumExists = await albumRepository.GetCountAsync(album => album.Id == albumId) > 0;
        if (!albumExists) return null;

        var genres = (await genreRepository.GetAllWithNoTrackingAsync()).ToList();
        var genreByName = genres.ToDictionary(value => value.Name, StringComparer.OrdinalIgnoreCase);

        var existingTracks = (await trackRepository.GetByFiltersWithNoTrackingAsync(track => track.AlbumId == albumId)).ToList();

        var existingTrackNumbers = existingTracks.Select(track => track.TrackNumber).ToHashSet();
        var existingTitles = existingTracks.Select(track => track.Title).ToHashSet(StringComparer.OrdinalIgnoreCase);

        var rows = new List<BulkImportRowResult>();
        var validRows = new List<ParsedTrackRow>();

        var lines = request.CsvContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (lines.Length == 0)
            return new BulkImportTracksResult
            {
                PreviewOnly = !request.ImportValidRows,
                TotalRows = 0,
                ValidRows = 0,
                InvalidRows = 0,
                ImportedRows = 0,
                Rows = []
            };
        

        var headers = lines[0].Split(',', StringSplitOptions.TrimEntries);
        var headerMap = UtilityHelper.BuildHeaderMap(headers);
        var hasExpectedHeaders = headerMap.ContainsKey(HeadersMap.TrackNumber)
            && headerMap.ContainsKey(HeadersMap.Title)
            && headerMap.ContainsKey(HeadersMap.Description)
            && headerMap.ContainsKey(HeadersMap.Genre)
            && headerMap.ContainsKey(HeadersMap.IsActive);

        if (!hasExpectedHeaders) throw new ArgumentException("CSV headers must include trackNumber,title,durationSeconds,genre,isActive.");

        var seenTrackNumbers = new HashSet<int>();
        var seenTitles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (var i = 1; i < lines.Length; i++)
        {
            var rowNumber = i + 1;
            var row = lines[i].Split(',', StringSplitOptions.TrimEntries);
            var errors = new List<string>();

            int? trackNumber = null;
            int? durationSeconds = null;
            bool? isActive = null;

            var title = UtilityHelper.ReadColumnValue(row, headerMap, HeadersMap.Title);
            var genreName = UtilityHelper.ReadColumnValue(row, headerMap, HeadersMap.Genre);
            var trackNumberText = UtilityHelper.ReadColumnValue(row, headerMap, HeadersMap.TrackNumber);
            var durationSecondsText = UtilityHelper.ReadColumnValue(row, headerMap, HeadersMap.Description);
            var isActiveText = UtilityHelper.ReadColumnValue(row, headerMap, HeadersMap.IsActive);

            if (string.IsNullOrWhiteSpace(title)) errors.Add("Title is required.");

            if (!int.TryParse(trackNumberText, out var parsedTrackNumber)) 
                errors.Add("TrackNumber must be a valid integer.");
            
            else
            {
                trackNumber = parsedTrackNumber;

                if (existingTrackNumbers.Contains(parsedTrackNumber))
                    errors.Add("Track number already exists in this album.");
                else if (!seenTrackNumbers.Add(parsedTrackNumber))
                    errors.Add("Track number is duplicated in CSV.");
            }

            if (!int.TryParse(durationSecondsText, out var parsedDurationSeconds))
                errors.Add("DurationSeconds must be a valid integer.");
            else
            {
                durationSeconds = parsedDurationSeconds;
                if (parsedDurationSeconds <= 0) errors.Add("DurationSeconds must be greater than zero.");
            }

            if (!bool.TryParse(isActiveText, out var parsedIsActive))
                errors.Add("IsActive must be true or false.");
            else
                isActive = parsedIsActive;
            
            if (string.IsNullOrWhiteSpace(genreName) || !genreByName.ContainsKey(genreName)) errors.Add("Genre is not supported.");

            if (!string.IsNullOrWhiteSpace(title))
                if (existingTitles.Contains(title))
                    errors.Add("Title already exists in this album.");
                else if (!seenTitles.Add(title))
                    errors.Add("Title is duplicated in CSV.");

            var isValid = errors.Count == 0;
            rows.Add(new BulkImportRowResult
            {
                RowNumber = rowNumber,
                TrackNumber = trackNumber,
                Title = title,
                DurationSeconds = durationSeconds,
                Genre = genreName,
                IsActive = isActive,
                IsValid = isValid,
                Errors = errors
            });

            if (isValid)
            {
                validRows.Add(new ParsedTrackRow
                {
                    TrackNumber = trackNumber!.Value,
                    Title = title,
                    DurationSeconds = durationSeconds!.Value,
                    GenreId = genreByName[genreName].Id,
                    IsActive = isActive!.Value
                });
            }
        }

        var importedRows = 0;
        if (request.ImportValidRows && validRows.Count > 0)
        {
            var entities = validRows.Select(value => new Track
            {
                AlbumId = albumId,
                TrackNumber = value.TrackNumber,
                Title = value.Title,
                DurationSeconds = value.DurationSeconds,
                GenreId = value.GenreId,
                IsActive = value.IsActive
            });

            await trackRepository.AddRangeAsync(entities);
            importedRows = validRows.Count;

            await trackImportHistoryRepository.AddAsync(new TrackImportHistory
            {
                AlbumId = albumId,
                ImportedTracksCount = importedRows,
                ImportedAtUtc = DateTime.UtcNow
            });
        }

        return new BulkImportTracksResult
        {
            PreviewOnly = !request.ImportValidRows,
            TotalRows = rows.Count,
            ValidRows = rows.Count(value => value.IsValid),
            InvalidRows = rows.Count(value => !value.IsValid),
            ImportedRows = importedRows,
            Rows = rows
        };
    }
   
}
