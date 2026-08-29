import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import CloudUploadIcon from '@mui/icons-material/CloudUpload';
import DeleteIcon from '@mui/icons-material/Delete';
import EditOutlinedIcon from '@mui/icons-material/EditOutlined';
import QueueMusicIcon from '@mui/icons-material/QueueMusic';
import {
  Avatar,
  Box,
  Button,
  Chip,
  IconButton,
  MenuItem,
  Paper,
  Stack,
  TextField,
  Tooltip,
  Typography,
} from '@mui/material';
import { DataGrid, type GridColDef } from '@mui/x-data-grid';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { useMemo, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { albumsApi } from '../api/albumsApi';
import { genresApi } from '../api/genresApi';
import { getApiErrorMessage } from '../api/httpClient';
import { queryKeys } from '../api/queryKeys';
import { tracksApi } from '../api/tracksApi';
import type { BulkImportTracksResult, Track, UpsertTrackRequest } from '../api/types';
import { ErrorState, LoadingState } from '../components/common/AsyncState';
import { ConfirmDialog } from '../components/common/ConfirmDialog';
import { PageHeader } from '../components/common/PageHeader';
import { TrackFormDialog } from '../components/tracks/TrackFormDialog';
import { BulkImportDialog } from '../components/tracks/BulkImportDialog';
import { useNotification } from '../context/NotificationContext';

const isActiveFilterOptions = [
  { value: 'all', label: 'All' },
  { value: 'active', label: 'Active' },
  { value: 'inactive', label: 'Inactive' },
] as const;

export const AlbumDetailPage = () => {
  const navigate = useNavigate();
  const { albumId } = useParams<{ albumId: string }>();
  const parsedAlbumId = Number(albumId);
  const queryClient = useQueryClient();
  const { notify } = useNotification();

  const [genreFilter, setGenreFilter] = useState('');
  const [isActiveFilter, setIsActiveFilter] = useState<(typeof isActiveFilterOptions)[number]['value']>('all');
  const [isTrackFormOpen, setIsTrackFormOpen] = useState(false);
  const [editingTrack, setEditingTrack] = useState<Track | null>(null);
  const [trackToDelete, setTrackToDelete] = useState<Track | null>(null);
  const [isImportDialogOpen, setIsImportDialogOpen] = useState(false);
  const [importResult, setImportResult] = useState<BulkImportTracksResult | null>(null);

  const genresQuery = useQuery({
    queryKey: queryKeys.genres,
    queryFn: genresApi.getAll,
  });

  const albumQuery = useQuery({
    queryKey: queryKeys.albumDetail(parsedAlbumId),
    queryFn: () => albumsApi.getById(parsedAlbumId),
    enabled: Number.isFinite(parsedAlbumId),
  });

  const tracksQuery = useQuery({
    queryKey: queryKeys.tracks(parsedAlbumId, genreFilter, isActiveFilter),
    queryFn: () =>
      tracksApi.getByAlbum(parsedAlbumId, {
        genre: genreFilter || undefined,
        isActive:
          isActiveFilter === 'all' ? undefined : isActiveFilter === 'active',
      }),
    enabled: Number.isFinite(parsedAlbumId),
  });

  const refreshData = async () => {
    await queryClient.invalidateQueries({ queryKey: queryKeys.albumDetail(parsedAlbumId) });
    await queryClient.invalidateQueries({ queryKey: queryKeys.tracks(parsedAlbumId, genreFilter, isActiveFilter) });
    await queryClient.invalidateQueries({ queryKey: ['albums'] });
    await queryClient.invalidateQueries({ queryKey: queryKeys.dashboard });
  };

  const createTrackMutation = useMutation({
    mutationFn: (payload: UpsertTrackRequest) => tracksApi.create(parsedAlbumId, payload),
    onSuccess: async () => {
      notify('Track added.', 'success');
      setIsTrackFormOpen(false);
      await refreshData();
    },
    onError: (error) => notify(getApiErrorMessage(error), 'error'),
  });

  const updateTrackMutation = useMutation({
    mutationFn: ({ trackId, payload }: { trackId: number; payload: UpsertTrackRequest }) =>
      tracksApi.update(trackId, payload),
    onSuccess: async () => {
      notify('Track updated.', 'success');
      setEditingTrack(null);
      setIsTrackFormOpen(false);
      await refreshData();
    },
    onError: (error) => notify(getApiErrorMessage(error), 'error'),
  });

  const deleteTrackMutation = useMutation({
    mutationFn: (trackId: number) => tracksApi.delete(trackId),
    onSuccess: async () => {
      notify('Track deleted.', 'success');
      setTrackToDelete(null);
      await refreshData();
    },
    onError: (error) => notify(getApiErrorMessage(error), 'error'),
  });

  const previewImportMutation = useMutation({
    mutationFn: (csvContent: string) =>
      tracksApi.bulkImport(parsedAlbumId, {
        csvContent,
        importValidRows: false,
      }),
    onSuccess: (data) => {
      setImportResult(data);
      notify('Preview generated.', 'info');
    },
    onError: (error) => notify(getApiErrorMessage(error), 'error'),
  });

  const importValidRowsMutation = useMutation({
    mutationFn: (csvContent: string) =>
      tracksApi.bulkImport(parsedAlbumId, {
        csvContent,
        importValidRows: true,
      }),
    onSuccess: async (data) => {
      setImportResult(data);
      notify(`${data.importedRows} tracks imported.`, 'success');
      await refreshData();
    },
    onError: (error) => notify(getApiErrorMessage(error), 'error'),
  });

  const columns = useMemo<GridColDef<Track>[]>(
    () => [
      { field: 'trackNumber', headerName: '#', width: 70 },
      { field: 'title', headerName: 'Title', flex: 1.3, minWidth: 180 },
      { field: 'durationSeconds', headerName: 'Duration (s)', width: 120 },
      { field: 'genre', headerName: 'Genre', width: 130 },
      {
        field: 'isActive',
        headerName: 'Active',
        width: 100,
        renderCell: (params) =>
          params.value ? <Chip color="success" size="small" label="Yes" /> : <Chip color="default" size="small" label="No" />,
      },
      {
        field: 'actions',
        headerName: 'Actions',
        width: 120,
        sortable: false,
        filterable: false,
        renderCell: (params) => (
          <Stack direction="row" spacing={0.5}>
            <Tooltip title="Edit track">
              <IconButton
                size="small"
                onClick={() => {
                  setEditingTrack(params.row);
                  setIsTrackFormOpen(true);
                }}
              >
                <EditOutlinedIcon fontSize="small" />
              </IconButton>
            </Tooltip>
            <Tooltip title="Delete track">
              <IconButton size="small" color="error" onClick={() => setTrackToDelete(params.row)}>
                <DeleteIcon fontSize="small" />
              </IconButton>
            </Tooltip>
          </Stack>
        ),
      },
    ],
    [],
  );

  const genreOptions = genresQuery.data?.map((item) => item.name) ?? [];

  if (!Number.isFinite(parsedAlbumId)) {
    return <ErrorState message="Invalid album id." />;
  }

  return (
    <>
      <PageHeader
        title={albumQuery.data?.title ?? 'Album'}
        subtitle={albumQuery.data ? `${albumQuery.data.artistName} · ${albumQuery.data.releaseDate.slice(0, 10)}` : undefined}
        actions={
          <Button variant="text" startIcon={<ArrowBackIcon />} onClick={() => navigate('/albums')}>
            Back to Albums
          </Button>
        }
      />

      {albumQuery.isLoading ? <LoadingState message="Loading album..." /> : null}
      {albumQuery.isError ? <ErrorState message={getApiErrorMessage(albumQuery.error)} /> : null}

      {albumQuery.data ? (
        <Paper sx={{ p: 2, mb: 2 }}>
          <Stack direction="row" spacing={2}>
            <Avatar
              variant="rounded"
              src={albumQuery.data.coverImageUrl || undefined}
              alt={albumQuery.data.title}
              sx={{ width: 104, height: 104 }}
            >
              {albumQuery.data.title[0]}
            </Avatar>
            <Box>
              <Typography variant="h5">{albumQuery.data.title}</Typography>
              <Typography color="text.secondary">{albumQuery.data.artistName}</Typography>
            </Box>
          </Stack>
        </Paper>
      ) : null}

      <Paper sx={{ p: 2, mb: 2 }}>
        <Stack direction="row" spacing={2}>
          <TextField
            select
            label="Genre"
            value={genreFilter}
            onChange={(event) => setGenreFilter(event.target.value)}
            sx={{ minWidth: 220 }}
          >
            <MenuItem value="">All Genres</MenuItem>
            {genreOptions.map((item) => (
              <MenuItem key={item} value={item}>
                {item}
              </MenuItem>
            ))}
          </TextField>

          <TextField
            select
            label="Active"
            value={isActiveFilter}
            onChange={(event) =>
              setIsActiveFilter(event.target.value as (typeof isActiveFilterOptions)[number]['value'])
            }
            sx={{ minWidth: 220 }}
          >
            {isActiveFilterOptions.map((item) => (
              <MenuItem key={item.value} value={item.value}>
                {item.label}
              </MenuItem>
            ))}
          </TextField>

          <Stack direction="row" spacing={1} sx={{ ml: 'auto' }}>
            <Button
              variant="contained"
              startIcon={<QueueMusicIcon />}
              onClick={() => {
                setEditingTrack(null);
                setIsTrackFormOpen(true);
              }}
            >
              Add Track
            </Button>
            <Button
              variant="outlined"
              startIcon={<CloudUploadIcon />}
              onClick={() => {
                setImportResult(null);
                setIsImportDialogOpen(true);
              }}
            >
              Bulk Import Tracks
            </Button>
          </Stack>
        </Stack>
      </Paper>

      {tracksQuery.isLoading ? <LoadingState message="Loading tracks..." /> : null}
      {tracksQuery.isError ? <ErrorState message={getApiErrorMessage(tracksQuery.error)} /> : null}

      <Paper sx={{ height: 560 }}>
        <DataGrid
          rows={tracksQuery.data ?? []}
          columns={columns}
          disableRowSelectionOnClick
          pageSizeOptions={[10, 25, 50]}
          loading={tracksQuery.isFetching}
        />
      </Paper>

      <TrackFormDialog
        open={isTrackFormOpen}
        track={editingTrack}
        genres={genreOptions}
        onClose={() => {
          setEditingTrack(null);
          setIsTrackFormOpen(false);
        }}
        onSubmit={(payload) => {
          if (editingTrack) {
            updateTrackMutation.mutate({ trackId: editingTrack.id, payload });
            return;
          }

          createTrackMutation.mutate(payload);
        }}
        isLoading={createTrackMutation.isPending || updateTrackMutation.isPending}
      />

      <ConfirmDialog
        open={Boolean(trackToDelete)}
        title="Delete Track"
        message={`Delete track "${trackToDelete?.title ?? ''}"?`}
        confirmLabel="Delete"
        confirmColor="error"
        onCancel={() => setTrackToDelete(null)}
        onConfirm={() => {
          if (trackToDelete) deleteTrackMutation.mutate(trackToDelete.id);
        }}
        isLoading={deleteTrackMutation.isPending}
      />

      <BulkImportDialog
        open={isImportDialogOpen}
        onClose={() => setIsImportDialogOpen(false)}
        onPreview={(csvContent) => previewImportMutation.mutate(csvContent)}
        onImportValidRows={(csvContent) => importValidRowsMutation.mutate(csvContent)}
        result={importResult}
        isLoading={previewImportMutation.isPending || importValidRowsMutation.isPending}
      />
    </>
  );
};
