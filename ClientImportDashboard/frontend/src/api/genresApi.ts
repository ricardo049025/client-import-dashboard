import { httpClient } from './httpClient';
import type { Genre } from './types';

export const genresApi = {
  getAll: async (): Promise<Genre[]> => {
    const response = await httpClient.get<Genre[]>('/api/v1/Genres');
    return response.data;
  },
};
