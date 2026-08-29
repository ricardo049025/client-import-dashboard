import DownloadIcon from '@mui/icons-material/Download';
import UploadFileIcon from '@mui/icons-material/UploadFile';
import {
  Alert,
  Box,
  Button,
  Chip,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Divider,
  Stack,
  Typography,
} from '@mui/material';
import { DataGrid, type GridColDef } from '@mui/x-data-grid';
import { useMemo, useState, type ChangeEvent } from 'react';
import type { BulkImportTracksResult } from '../../api/types';

const CSV_TEMPLATE_CONTENT = [
  'trackNumber,title,durationSeconds,genre,isActive',
  '1,Neon Skyline,212,Pop,true',
  '2,Broken Circuit,198,Electronic,true',
  '3,Slow Motion Love,245,R&B,false',
  '4,Concrete Jungle,201,Hip-Hop,true',
  '5,Thunder Road,231,Rock,false',
  '6,Fuego Nocturno,207,Latin,true',
].join('\n');

interface BulkImportDialogProps {
  open: boolean;
  isLoading?: boolean;
  result?: BulkImportTracksResult | null;
  onClose: () => void;
  onPreview: (csvContent: string) => void;
  onImportValidRows: (csvContent: string) => void;
}

export const BulkImportDialog = ({
  open,
  isLoading = false,
  result,
  onClose,
  onPreview,
  onImportValidRows,
}: BulkImportDialogProps) => {
  const [csvContent, setCsvContent] = useState('');
  const [fileName, setFileName] = useState('');

  const columns = useMemo<GridColDef[]>(
    () => [
      { field: 'rowNumber', headerName: 'Row', width: 80 },
      { field: 'trackNumber', headerName: 'Track #', width: 90 },
      { field: 'title', headerName: 'Title', flex: 1, minWidth: 160 },
      { field: 'durationSeconds', headerName: 'Duration', width: 100 },
      { field: 'genre', headerName: 'Genre', width: 120 },
      {
        field: 'isActive',
        headerName: 'Active',
        width: 100,
        renderCell: (params) => (params.value ? 'Yes' : 'No'),
      },
      {
        field: 'status',
        headerName: 'Status',
        width: 120,
        renderCell: (params) =>
          params.row.isValid ? <Chip size="small" color="success" label="Valid" /> : <Chip size="small" color="error" label="Error" />,
      },
      {
        field: 'errors',
        headerName: 'Errors',
        flex: 2,
        minWidth: 250,
        renderCell: (params) => (
          <Typography variant="caption" color={params.row.isValid ? 'text.secondary' : 'error.main'}>
            {params.row.errors?.join(', ') || '-'}
          </Typography>
        ),
      },
    ],
    [],
  );

  const rows = (result?.rows ?? []).map((row) => ({
    id: row.rowNumber,
    ...row,
  }));

  const handleFileChange = async (event: ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (!file) return;

    setFileName(file.name);
    const content = await file.text();
    setCsvContent(content);
  };

  const handleDownloadTemplate = () => {
    const blob = new Blob([CSV_TEMPLATE_CONTENT], { type: 'text/csv;charset=utf-8;' });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = 'tracks-bulk-import-template.csv';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    URL.revokeObjectURL(url);
  };

  return (
    <Dialog open={open} onClose={onClose} maxWidth="lg" fullWidth>
      <DialogTitle>Bulk Import Tracks</DialogTitle>
      <DialogContent>
        <Stack spacing={2} sx={{ mt: 1 }}>
          <Stack direction="row" spacing={1}>
            <Button variant="outlined" component="label" startIcon={<UploadFileIcon />}>
              Upload CSV
              <input hidden type="file" accept=".csv,text/csv" onChange={handleFileChange} />
            </Button>
            <Button variant="text" startIcon={<DownloadIcon />} onClick={handleDownloadTemplate}>
              Download Template
            </Button>
          </Stack>

          {fileName ? (
            <Alert severity="info">{fileName} loaded. Click Preview to validate rows.</Alert>
          ) : (
            <Alert severity="warning">Select a CSV file with headers: trackNumber,title,durationSeconds,genre,isActive.</Alert>
          )}

          <Stack direction="row" spacing={1}>
            <Button variant="contained" disabled={!csvContent || isLoading} onClick={() => onPreview(csvContent)}>
              Preview Validation
            </Button>
            <Button
              variant="contained"
              color="success"
              disabled={!csvContent || !result || result.validRows === 0 || isLoading}
              onClick={() => onImportValidRows(csvContent)}
            >
              Import Valid Rows
            </Button>
          </Stack>

          {result ? (
            <>
              <Divider />
              <Stack direction="row" spacing={1}>
                <Chip label={`Total: ${result.totalRows}`} />
                <Chip color="success" label={`Valid: ${result.validRows}`} />
                <Chip color="error" label={`Invalid: ${result.invalidRows}`} />
                <Chip color="primary" label={`Imported: ${result.importedRows}`} />
              </Stack>
              <Box sx={{ height: 420 }}>
                <DataGrid columns={columns} rows={rows} disableRowSelectionOnClick pageSizeOptions={[5, 10, 25]} />
              </Box>
            </>
          ) : null}
        </Stack>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose}>Close</Button>
      </DialogActions>
    </Dialog>
  );
};
