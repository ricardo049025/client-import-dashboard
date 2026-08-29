import AlbumIcon from '@mui/icons-material/Album';
import LibraryMusicIcon from '@mui/icons-material/LibraryMusic';
import {
  Box,
  Card,
  CardContent,
  List,
  ListItem,
  ListItemText,
  Stack,
  Typography,
} from '@mui/material';
import { useQuery } from '@tanstack/react-query';
import { dashboardApi } from '../api/dashboardApi';
import { queryKeys } from '../api/queryKeys';
import { ErrorState, LoadingState } from '../components/common/AsyncState';
import { PageHeader } from '../components/common/PageHeader';
import { StatCard } from '../components/common/StatCard';

const formatUtcDateTime = (value: string) =>
  new Date(value).toLocaleString(undefined, {
    dateStyle: 'medium',
    timeStyle: 'short',
  });

export const DashboardPage = () => {
  const { data, isLoading, isError, error } = useQuery({
    queryKey: queryKeys.dashboard,
    queryFn: dashboardApi.getSummary,
  });

  const albumsByGenre = data?.albumsByGenre ?? [];
  const recentImports = data?.recentImports ?? [];

  return (
    <>
      <PageHeader title="Dashboard" subtitle="Overview of albums, tracks, genres, and recent imports." />

      {isLoading ? <LoadingState message="Loading dashboard..." /> : null}
      {isError ? <ErrorState message={(error as Error).message} /> : null}

      {data ? (
        <>
          <Box sx={{ mb: 2 }}>
            <Stack direction="row" spacing={2}>
            <Box sx={{ flex: 1 }}>
              <StatCard label="Total Albums" value={data.totalAlbums} icon={<AlbumIcon color="primary" />} />
            </Box>
            <Box sx={{ flex: 1 }}>
              <StatCard label="Total Tracks" value={data.totalTracks} icon={<LibraryMusicIcon color="secondary" />} />
            </Box>
            </Stack>
          </Box>

          <Stack direction="row" spacing={2}>
            <Box sx={{ flex: 1 }}>
              <Card>
                <CardContent>
                  <Typography variant="h6" gutterBottom>
                    Albums by Genre
                  </Typography>
                  <List dense disablePadding>
                    {albumsByGenre.length === 0 ? (
                      <ListItem>
                        <ListItemText primary="No genre data yet." />
                      </ListItem>
                    ) : (
                      albumsByGenre.map((item) => (
                        <ListItem key={item.genre} divider>
                          <ListItemText primary={item.genre} secondary={`${item.albumCount} album(s)`} />
                        </ListItem>
                      ))
                    )}
                  </List>
                </CardContent>
              </Card>
            </Box>

            <Box sx={{ flex: 1 }}>
              <Card>
                <CardContent>
                  <Typography variant="h6" gutterBottom>
                    Recent Imports
                  </Typography>
                  <List dense disablePadding>
                    {recentImports.length === 0 ? (
                      <ListItem>
                        <ListItemText primary="No imports yet." />
                      </ListItem>
                    ) : (
                      recentImports.map((item, index) => (
                        <ListItem key={`${item.albumId}-${item.importedAtUtc}-${index}`} divider>
                          <ListItemText
                            primary={item.albumTitle}
                            secondary={`${item.importedTracksCount} tracks · ${formatUtcDateTime(item.importedAtUtc)}`}
                          />
                        </ListItem>
                      ))
                    )}
                  </List>
                </CardContent>
              </Card>
            </Box>
          </Stack>
        </>
      ) : null}
    </>
  );
};
