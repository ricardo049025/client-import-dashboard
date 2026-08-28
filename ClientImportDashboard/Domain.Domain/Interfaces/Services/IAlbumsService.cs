using Domain.Domain.DTOs.Requests;
using Domain.Domain.DTOs.Responses;

namespace Domain.Domain.Interfaces.Services;

public interface IAlbumsService
{
    Task<IEnumerable<AlbumResponse>> GetAlbumsAsync(string? search, string? genre);
    Task<AlbumDetailResponse?> GetAlbumByIdAsync(int albumId);
    Task<AlbumResponse> CreateAlbumAsync(UpsertAlbumRequest request);
    Task<AlbumResponse?> UpdateAlbumAsync(int albumId, UpsertAlbumRequest request);
    Task<bool> DeleteAlbumAsync(int albumId);
}
