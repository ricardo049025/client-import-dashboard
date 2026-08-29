import {
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  FormControlLabel,
  MenuItem,
  Stack,
  Switch,
  TextField,
} from '@mui/material';
import { useEffect, useMemo, useState } from 'react';
import type { Track, UpsertTrackRequest } from '../../api/types';

interface TrackFormDialogProps {
  open: boolean;
  track?: Track | null;
  genres: string[];
  isLoading?: boolean;
  onClose: () => void;
  onSubmit: (payload: UpsertTrackRequest) => void;
}

export const TrackFormDialog = ({
  open,
  track,
  genres,
  isLoading = false,
  onClose,
  onSubmit,
}: TrackFormDialogProps) => {
  const initialState = useMemo(
    () => ({
      trackNumber: track?.trackNumber ?? 1,
      title: track?.title ?? '',
      durationSeconds: track?.durationSeconds ?? 180,
      genre: track?.genre ?? genres[0] ?? '',
      isActive: track?.isActive ?? true,
    }),
    [genres, track],
  );

  const [form, setForm] = useState(initialState);

  useEffect(() => {
    if (open) setForm(initialState);
  }, [initialState, open]);

  const handleSubmit = () => {
    onSubmit({
      trackNumber: Number(form.trackNumber),
      title: form.title.trim(),
      durationSeconds: Number(form.durationSeconds),
      genre: form.genre,
      isActive: form.isActive,
    });
  };

  return (
    <Dialog open={open} onClose={onClose} fullWidth maxWidth="sm">
      <DialogTitle>{track ? 'Edit Track' : 'Add Track'}</DialogTitle>
      <DialogContent>
        <Stack spacing={2} sx={{ mt: 1 }}>
          <TextField
            label="Track Number"
            type="number"
            value={form.trackNumber}
            onChange={(event) =>
              setForm((current) => ({ ...current, trackNumber: Number(event.target.value) }))
            }
            slotProps={{ htmlInput: { min: 1 } }}
            required
          />
          <TextField
            label="Title"
            value={form.title}
            onChange={(event) => setForm((current) => ({ ...current, title: event.target.value }))}
            required
          />
          <TextField
            label="Duration (seconds)"
            type="number"
            value={form.durationSeconds}
            onChange={(event) =>
              setForm((current) => ({ ...current, durationSeconds: Number(event.target.value) }))
            }
            slotProps={{ htmlInput: { min: 1 } }}
            required
          />
          <TextField
            label="Genre"
            select
            value={form.genre}
            onChange={(event) => setForm((current) => ({ ...current, genre: event.target.value }))}
            required
          >
            {genres.map((genre) => (
              <MenuItem key={genre} value={genre}>
                {genre}
              </MenuItem>
            ))}
          </TextField>
          <FormControlLabel
            control={
              <Switch
                checked={form.isActive}
                onChange={(event) =>
                  setForm((current) => ({ ...current, isActive: event.target.checked }))
                }
              />
            }
            label="Active"
          />
        </Stack>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose} disabled={isLoading}>
          Cancel
        </Button>
        <Button
          onClick={handleSubmit}
          variant="contained"
          disabled={isLoading || !form.title || !form.genre || form.trackNumber <= 0 || form.durationSeconds <= 0}
        >
          {track ? 'Save Changes' : 'Add Track'}
        </Button>
      </DialogActions>
    </Dialog>
  );
};
