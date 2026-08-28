using Domain.Domain.DTOs.Requests;
using Domain.Domain.DTOs.Responses;

namespace Domain.Domain.Interfaces.Services;

public interface ITracksService
{
    Task<IEnumerable<TrackResponse>> GetTracksByAlbumAsync(int albumId, string? genre, bool? isActive);
    Task<TrackResponse?> CreateTrackAsync(int albumId, UpsertTrackRequest request);
    Task<TrackResponse?> UpdateTrackAsync(int trackId, UpsertTrackRequest request);
    Task<bool> DeleteTrackAsync(int trackId);
    Task<BulkImportTracksResult?> BulkImportTracksAsync(int albumId, BulkImportTracksRequest request);
}
