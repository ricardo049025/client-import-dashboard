import { httpClient } from './httpClient';
import type {
  BulkImportTracksRequest,
  BulkImportTracksResult,
  Track,
  UpsertTrackRequest,
} from './types';

export const tracksApi = {
  getByAlbum: async (
    albumId: number,
    params: { genre?: string; isActive?: boolean },
  ): Promise<Track[]> => {
    const response = await httpClient.get<Track[]>(`/api/v1/albums/${albumId}/tracks`, {
      params: {
        genre: params.genre || undefined,
        isActive: params.isActive,
      },
    });

    return response.data;
  },

  create: async (albumId: number, payload: UpsertTrackRequest): Promise<Track> => {
    const response = await httpClient.post<Track>(`/api/v1/albums/${albumId}/tracks`, payload);
    return response.data;
  },

  update: async (trackId: number, payload: UpsertTrackRequest): Promise<Track> => {
    const response = await httpClient.put<Track>(`/api/v1/tracks/${trackId}`, payload);
    return response.data;
  },

  delete: async (trackId: number): Promise<void> => {
    await httpClient.delete(`/api/v1/tracks/${trackId}`);
  },

  bulkImport: async (
    albumId: number,
    payload: BulkImportTracksRequest,
  ): Promise<BulkImportTracksResult> => {
    const response = await httpClient.post<BulkImportTracksResult>(
      `/api/v1/albums/${albumId}/tracks/bulk-import`,
      payload,
    );
    return response.data;
  },
};
