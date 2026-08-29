import { httpClient } from './httpClient';
import type { Album, AlbumDetail, UpsertAlbumRequest } from './types';

export const albumsApi = {
  getAll: async (params: { search?: string; genre?: string }): Promise<Album[]> => {
    const response = await httpClient.get<Album[]>('/api/v1/albums', {
      params: {
        search: params.search || undefined,
        genre: params.genre || undefined,
      },
    });

    return response.data;
  },

  getById: async (id: number): Promise<AlbumDetail> => {
    const response = await httpClient.get<AlbumDetail>(`/api/v1/albums/${id}`);
    return response.data;
  },

  create: async (payload: UpsertAlbumRequest): Promise<Album> => {
    const response = await httpClient.post<Album>('/api/v1/albums', payload);
    return response.data;
  },

  update: async (id: number, payload: UpsertAlbumRequest): Promise<Album> => {
    const response = await httpClient.put<Album>(`/api/v1/albums/${id}`, payload);
    return response.data;
  },

  delete: async (id: number): Promise<void> => {
    await httpClient.delete(`/api/v1/albums/${id}`);
  },
};
