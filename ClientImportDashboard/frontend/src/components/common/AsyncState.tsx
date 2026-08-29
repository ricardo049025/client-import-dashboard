import { Alert, Box, CircularProgress, Typography } from '@mui/material';

export const LoadingState = ({ message = 'Loading...' }: { message?: string }) => (
  <Box sx={{ py: 6, textAlign: 'center' }}>
    <CircularProgress size={28} sx={{ mb: 2 }} />
    <Typography color="text.secondary">{message}</Typography>
  </Box>
);

export const ErrorState = ({ message }: { message: string }) => (
  <Alert severity="error" sx={{ mb: 2 }}>
    {message}
  </Alert>
);
