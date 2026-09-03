# Client Import Dashboard

A full-stack application for managing music **albums** and **tracks**, with CSV **bulk import** capabilities and an analytics **dashboard**. The backend is a .NET 10 minimal API following a clean, layered architecture, and the frontend is a React + TypeScript + Vite single-page application.

## Overview

Client Import Dashboard lets you:

- Manage albums (create, read, update, delete, search and filter by genre).
- Manage tracks within albums (create, update, delete, filter by genre and active state).
- Bulk import tracks from CSV with a preview mode and an import-valid-rows mode, including rich validation.
- View a dashboard summary with total albums, total tracks, albums grouped by genre, and recent import history.

## Tech Stack

- **Backend:** .NET 10, Minimal APIs, Entity Framework Core (InMemory provider), C#
- **Frontend:** React, TypeScript, Vite
- **Deployment:** Render (API, via Docker) and Vercel (frontend)

## Solution Structure

The solution (`ClientImportDashboard/ClientImportDashboard.slnx`) is organized into layered projects:

| Project | Responsibility |
| --- | --- |
| `Domain.Entities` | Core entities (`Album`, `Track`, `Genre`, `TrackImportHistory`), EF Core `ApiDbContext`, and `AppDbSeeder`. |
| `Domain.Domain` | Constants (`ApiEndpointsPath`, `HeadersMap`), DTOs (requests/responses), helpers (`UtilityHelper`), and contracts (service and repository interfaces). |
| `Infraestructure.Data` | Generic `BaseRepository<T>` with CRUD, query, and include-capable overloads, plus repository DI registration. |
| `Services.Main` | Business logic services: `GenresService`, `AlbumsService`, `TracksService`, `DashboardService`. |
| `webApi` | `Program.cs` (DI + InMemory DB seeding) and `ApiEndPoints.cs` (minimal API mappings). |

### Architecture Notes

- The service layer uses `IBaseRepository<T>` instead of direct `DbContext` access for most operations.
- Include-heavy reads are supported through generic repository include overloads (e.g., loading related tracks/genres) while preserving the repository pattern.
- CSV parsing logic lives in `Domain.Domain/Helpers` and `Domain.Domain/Constants`.
- DTO mapping is centralized where possible (e.g., `TrackResponse.FromEntity`).

## API Endpoints

Endpoint paths are defined in `Domain.Domain/Constants/ApiEndpointsPath.cs`.

### Albums

- `GET /api/v1/albums` — list albums (search + genre filter)
- `GET /api/v1/albums/{id}` — album detail
- `POST /api/v1/albums` — create album
- `PUT /api/v1/albums/{id}` — update album
- `DELETE /api/v1/albums/{id}` — delete album

### Tracks

- `GET /api/v1/albums/{albumId}/tracks` — list tracks (genre + isActive filter)
- `POST /api/v1/albums/{albumId}/tracks` — create track
- `PUT /api/v1/tracks/{id}` — update track
- `DELETE /api/v1/tracks/{id}` — delete track

### Bulk Import

- `POST /api/v1/albums/{albumId}/tracks/bulk-import` — preview or import valid rows

Validations include:

- Required `title`
- `trackNumber` integer and unique within the album
- `durationSeconds` integer and greater than 0
- Supported `genre`
- Duplicate title detection (against existing album tracks and within the CSV)

Import history is written only when a bulk import confirmation actually inserts valid rows.

### Dashboard

- `GET /api/v1/dashboard` — summary including:
  - `totalAlbums`
  - `totalTracks`
  - `albumsByGenre` (distinct album count per genre from tracks)
  - `recentImports` (from `TrackImportHistory`, latest first)

### Genres

- `GET /api/v1/Genres`

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/)
- [Node.js](https://nodejs.org/) (for the frontend)

### Run the Backend

```bash
dotnet build ClientImportDashboard/ClientImportDashboard.slnx
dotnet run --project ClientImportDashboard/webApi
```

The API uses an EF Core InMemory database that is seeded with sample albums, tracks, and genres on startup.

### Run the Frontend

```bash
cd ClientImportDashboard/frontend
npm install
npm run dev
```

Set the API base URL for the frontend via the `VITE_API_BASE_URL` environment variable.

## Deployment

This is a monorepo with the backend deployed to Render and the frontend to Vercel.

- **Backend (Render):** `ClientImportDashboard/webApi` — deployed via Docker (`webApi/Dockerfile`) and the `render.yaml` blueprint. Configure allowed origins with the `CORS_ALLOWED_ORIGINS` environment variable (comma-separated) or the `Cors:AllowedOrigins` appsettings section.
- **Frontend (Vercel):** set the project root to `ClientImportDashboard/frontend`, set `VITE_API_BASE_URL` to the Render API URL. A SPA rewrite is included in `frontend/vercel.json` to prevent 404s on deep links.

See [`ClientImportDashboard/DEPLOYMENT_GUIDE.md`](ClientImportDashboard/DEPLOYMENT_GUIDE.md) for full deployment instructions.

## Additional Documentation

- [Backend Implementation Tracker](ClientImportDashboard/BACKEND_IMPLEMENTATION_TRACKER.md) — detailed backend implementation status and history.
- [Deployment Guide](ClientImportDashboard/DEPLOYMENT_GUIDE.md) — step-by-step deployment to Render and Vercel.