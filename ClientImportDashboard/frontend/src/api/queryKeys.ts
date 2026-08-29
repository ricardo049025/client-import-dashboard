export const queryKeys = {
  genres: ['genres'] as const,
  dashboard: ['dashboard'] as const,
  albums: (search: string, genre: string) => ['albums', search, genre] as const,
  albumDetail: (albumId: number) => ['albumDetail', albumId] as const,
  tracks: (albumId: number, genre: string, isActive: string) =>
    ['tracks', albumId, genre, isActive] as const,
};
