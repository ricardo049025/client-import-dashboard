import AddIcon from '@mui/icons-material/Add';
import DeleteIcon from '@mui/icons-material/Delete';
import EditOutlinedIcon from '@mui/icons-material/EditOutlined';
import OpenInNewIcon from '@mui/icons-material/OpenInNew';
import {
  Button,
  Chip,
  IconButton,
  MenuItem,
  Paper,
  Stack,
  TextField,
  Tooltip,
} from '@mui/material';
import { DataGrid, type GridColDef } from '@mui/x-data-grid';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { useMemo, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { albumsApi } from '../api/albumsApi';
import { genresApi } from '../api/genresApi';
import { getApiErrorMessage } from '../api/httpClient';
import { queryKeys } from '../api/queryKeys';
import type { Album, UpsertAlbumRequest } from '../api/types';
import { AlbumFormDialog } from '../components/albums/AlbumFormDialog';
import { ErrorState, LoadingState } from '../components/common/AsyncState';
import { ConfirmDialog } from '../components/common/ConfirmDialog';
import { PageHeader } from '../components/common/PageHeader';
import { useNotification } from '../context/NotificationContext';

export const AlbumsPage = () => {
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const { notify } = useNotification();

  const [search, setSearch] = useState('');
  const [genre, setGenre] = useState('');
  const [isFormOpen, setIsFormOpen] = useState(false);
  const [editingAlbum, setEditingAlbum] = useState<Album | null>(null);
  const [albumToDelete, setAlbumToDelete] = useState<Album | null>(null);

  const albumsQuery = useQuery({
    queryKey: queryKeys.albums(search, genre),
    queryFn: () => albumsApi.getAll({ search, genre }),
  });

  const genresQuery = useQuery({
    queryKey: queryKeys.genres,
    queryFn: genresApi.getAll,
  });

  const refreshAfterMutation = async () => {
    await queryClient.invalidateQueries({ queryKey: queryKeys.albums(search, genre) });
    await queryClient.invalidateQueries({ queryKey: queryKeys.dashboard });
  };

  const createAlbumMutation = useMutation({
    mutationFn: (payload: UpsertAlbumRequest) => albumsApi.create(payload),
    onSuccess: async () => {
      notify('Album created.', 'success');
      setIsFormOpen(false);
      await refreshAfterMutation();
    },
    onError: (error) => notify(getApiErrorMessage(error), 'error'),
  });

  const updateAlbumMutation = useMutation({
    mutationFn: ({ id, payload }: { id: number; payload: UpsertAlbumRequest }) =>
      albumsApi.update(id, payload),
    onSuccess: async () => {
      notify('Album updated.', 'success');
      setIsFormOpen(false);
      setEditingAlbum(null);
      await refreshAfterMutation();
    },
    onError: (error) => notify(getApiErrorMessage(error), 'error'),
  });

  const deleteAlbumMutation = useMutation({
    mutationFn: (id: number) => albumsApi.delete(id),
    onSuccess: async () => {
      notify('Album deleted.', 'success');
      setAlbumToDelete(null);
      await refreshAfterMutation();
    },
    onError: (error) => notify(getApiErrorMessage(error), 'error'),
  });

  const columns = useMemo<GridColDef<Album>[]>(
    () => [
      { field: 'title', headerName: 'Title', flex: 1.2, minWidth: 180 },
      { field: 'artistName', headerName: 'Artist', flex: 1, minWidth: 160 },
      {
        field: 'releaseDate',
        headerName: 'Release Date',
        width: 130,
        valueFormatter: (value) => String(value).slice(0, 10),
      },
      { field: 'trackCount', headerName: 'Tracks', width: 90 },
      {
        field: 'genres',
        headerName: 'Genres',
        flex: 1,
        minWidth: 180,
        sortable: false,
        renderCell: (params) => (
          <Stack direction="row" spacing={0.5}>
            {(params.value as string[]).slice(0, 2).map((item) => (
              <Chip key={item} size="small" label={item} />
            ))}
          </Stack>
        ),
      },
      {
        field: 'actions',
        headerName: 'Actions',
        width: 140,
        sortable: false,
        filterable: false,
        renderCell: (params) => (
          <Stack direction="row" spacing={0.5}>
            <Tooltip title="Open album">
              <IconButton size="small" onClick={() => navigate(`/albums/${params.row.id}`)}>
                <OpenInNewIcon fontSize="small" />
              </IconButton>
            </Tooltip>
            <Tooltip title="Edit album">
              <IconButton
                size="small"
                onClick={() => {
                  setEditingAlbum(params.row);
                  setIsFormOpen(true);
                }}
              >
                <EditOutlinedIcon fontSize="small" />
              </IconButton>
            </Tooltip>
            <Tooltip title="Delete album">
              <IconButton size="small" color="error" onClick={() => setAlbumToDelete(params.row)}>
                <DeleteIcon fontSize="small" />
              </IconButton>
            </Tooltip>
          </Stack>
        ),
      },
    ],
    [navigate],
  );

  const genreOptions = genresQuery.data?.map((item) => item.name) ?? [];

  const handleSubmitAlbum = (payload: UpsertAlbumRequest) => {
    if (editingAlbum) {
      updateAlbumMutation.mutate({ id: editingAlbum.id, payload });
      return;
    }

    createAlbumMutation.mutate(payload);
  };

  return (
    <>
      <PageHeader
        title="Albums"
        subtitle="Browse albums, filter, and manage album records."
        actions={
          <Button
            variant="contained"
            startIcon={<AddIcon />}
            onClick={() => {
              setEditingAlbum(null);
              setIsFormOpen(true);
            }}
          >
            Create Album
          </Button>
        }
      />

      <Paper sx={{ p: 2, mb: 2 }}>
        <Stack direction="row" spacing={2}>
          <TextField
            label="Search albums"
            value={search}
            onChange={(event) => setSearch(event.target.value)}
            fullWidth
          />
          <TextField
            select
            label="Genre"
            value={genre}
            onChange={(event) => setGenre(event.target.value)}
            sx={{ minWidth: 220 }}
          >
            <MenuItem value="">All Genres</MenuItem>
            {genreOptions.map((item) => (
              <MenuItem key={item} value={item}>
                {item}
              </MenuItem>
            ))}
          </TextField>
        </Stack>
      </Paper>

      {albumsQuery.isLoading ? <LoadingState message="Loading albums..." /> : null}
      {albumsQuery.isError ? <ErrorState message={getApiErrorMessage(albumsQuery.error)} /> : null}

      <Paper sx={{ height: 580 }}>
        <DataGrid
          rows={albumsQuery.data ?? []}
          columns={columns}
          disableRowSelectionOnClick
          pageSizeOptions={[10, 25, 50]}
          loading={albumsQuery.isFetching}
        />
      </Paper>

      <AlbumFormDialog
        open={isFormOpen}
        album={editingAlbum}
        onClose={() => {
          setIsFormOpen(false);
          setEditingAlbum(null);
        }}
        onSubmit={handleSubmitAlbum}
        isLoading={createAlbumMutation.isPending || updateAlbumMutation.isPending}
      />

      <ConfirmDialog
        open={Boolean(albumToDelete)}
        title="Delete Album"
        message={`Delete album "${albumToDelete?.title ?? ''}"? This will remove all tracks in the album.`}
        confirmLabel="Delete"
        confirmColor="error"
        onCancel={() => setAlbumToDelete(null)}
        onConfirm={() => {
          if (albumToDelete) deleteAlbumMutation.mutate(albumToDelete.id);
        }}
        isLoading={deleteAlbumMutation.isPending}
      />
    </>
  );
};
