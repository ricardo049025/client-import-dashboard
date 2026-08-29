import { Button, Dialog, DialogActions, DialogContent, DialogTitle, Stack, TextField } from '@mui/material';
import { useEffect, useMemo, useState } from 'react';
import type { Album, UpsertAlbumRequest } from '../../api/types';

interface AlbumFormDialogProps {
  open: boolean;
  album?: Album | null;
  isLoading?: boolean;
  onClose: () => void;
  onSubmit: (payload: UpsertAlbumRequest) => void;
}

const toDateInput = (dateValue: string) => dateValue.slice(0, 10);

export const AlbumFormDialog = ({ open, album, isLoading = false, onClose, onSubmit }: AlbumFormDialogProps) => {
  const initialState = useMemo(
    () => ({
      title: album?.title ?? '',
      artistName: album?.artistName ?? '',
      releaseDate: album ? toDateInput(album.releaseDate) : '',
      coverImageUrl: album?.coverImageUrl ?? '',
    }),
    [album],
  );

  const [form, setForm] = useState(initialState);

  useEffect(() => {
    if (open) setForm(initialState);
  }, [initialState, open]);

  const handleSubmit = () => {
    onSubmit({
      title: form.title.trim(),
      artistName: form.artistName.trim(),
      releaseDate: form.releaseDate,
      coverImageUrl: form.coverImageUrl.trim(),
    });
  };

  return (
    <Dialog open={open} onClose={onClose} fullWidth maxWidth="sm">
      <DialogTitle>{album ? 'Edit Album' : 'Create Album'}</DialogTitle>
      <DialogContent>
        <Stack spacing={2} sx={{ mt: 1 }}>
          <TextField
            label="Title"
            value={form.title}
            onChange={(event) => setForm((current) => ({ ...current, title: event.target.value }))}
            required
          />
          <TextField
            label="Artist"
            value={form.artistName}
            onChange={(event) => setForm((current) => ({ ...current, artistName: event.target.value }))}
            required
          />
          <TextField
            label="Release Date"
            type="date"
            value={form.releaseDate}
            onChange={(event) => setForm((current) => ({ ...current, releaseDate: event.target.value }))}
            slotProps={{ inputLabel: { shrink: true } }}
            required
          />
          <TextField
            label="Cover URL"
            value={form.coverImageUrl}
            onChange={(event) => setForm((current) => ({ ...current, coverImageUrl: event.target.value }))}
          />
        </Stack>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose} disabled={isLoading}>
          Cancel
        </Button>
        <Button onClick={handleSubmit} variant="contained" disabled={isLoading || !form.title || !form.artistName || !form.releaseDate}>
          {album ? 'Save Changes' : 'Create'}
        </Button>
      </DialogActions>
    </Dialog>
  );
};
