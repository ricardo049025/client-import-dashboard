import LibraryMusicIcon from '@mui/icons-material/LibraryMusic';
import SpaceDashboardOutlinedIcon from '@mui/icons-material/SpaceDashboardOutlined';
import { AppBar, Box, Button, Container, Stack, Toolbar, Typography } from '@mui/material';
import { NavLink, Outlet } from 'react-router-dom';

const links = [
  { to: '/', label: 'Dashboard', icon: <SpaceDashboardOutlinedIcon fontSize="small" /> },
  { to: '/albums', label: 'Albums', icon: <LibraryMusicIcon fontSize="small" /> },
];

export const AppShell = () => {
  return (
    <Box sx={{ minHeight: '100vh', bgcolor: 'background.default' }}>
      <AppBar position="static" elevation={0} sx={{ borderBottom: '1px solid', borderColor: 'divider' }}>
        <Toolbar sx={{ gap: 2 }}>
          <Typography variant="h6" sx={{ flexGrow: 1, fontWeight: 700 }}>
            Client Import Dashboard
          </Typography>
          <Stack direction="row" spacing={1}>
            {links.map((link) => (
              <Button
                key={link.to}
                component={NavLink}
                to={link.to}
                startIcon={link.icon}
                color="inherit"
                sx={{
                  '&.active': {
                    bgcolor: 'rgba(255,255,255,0.15)',
                  },
                }}
              >
                {link.label}
              </Button>
            ))}
          </Stack>
        </Toolbar>
      </AppBar>
      <Container maxWidth="xl" sx={{ py: 3 }}>
        <Outlet />
      </Container>
    </Box>
  );
};
