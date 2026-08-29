export interface Genre {
  id: number;
  name: string;
}

export interface Album {
  id: number;
  title: string;
  artistName: string;
  releaseDate: string;
  coverImageUrl?: string | null;
  trackCount: number;
  genres: string[];
}

export interface Track {
  id: number;
  albumId: number;
  trackNumber: number;
  title: string;
  durationSeconds: number;
  genre: string;
  isActive: boolean;
}

export interface AlbumDetail {
  id: number;
  title: string;
  artistName: string;
  releaseDate: string;
  coverImageUrl?: string | null;
  tracks: Track[];
}

export interface UpsertAlbumRequest {
  title: string;
  artistName: string;
  releaseDate: string;
  coverImageUrl?: string;
}

export interface UpsertTrackRequest {
  trackNumber: number;
  title: string;
  durationSeconds: number;
  genre: string;
  isActive: boolean;
}

export interface BulkImportTracksRequest {
  csvContent: string;
  importValidRows: boolean;
}

export interface BulkImportRowResult {
  rowNumber: number;
  trackNumber?: number | null;
  title: string;
  durationSeconds?: number | null;
  genre: string;
  isActive?: boolean | null;
  isValid: boolean;
  errors: string[];
}

export interface BulkImportTracksResult {
  previewOnly: boolean;
  totalRows: number;
  validRows: number;
  invalidRows: number;
  importedRows: number;
  rows: BulkImportRowResult[];
}

export interface DashboardGenreCount {
  genre: string;
  albumCount: number;
}

export interface DashboardRecentImport {
  albumId: number;
  albumTitle: string;
  importedTracksCount: number;
  importedAtUtc: string;
}

export interface DashboardSummary {
  totalAlbums: number;
  totalTracks: number;
  albumsByGenre: DashboardGenreCount[];
  recentImports: DashboardRecentImport[];
}
