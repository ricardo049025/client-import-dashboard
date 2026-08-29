# Backend Implementation Tracker - Client Import Dashboard

## 1) Current Solution Structure (latest)

### `Domain.Entities`
- Core entities: `Album`, `Track`, `Genre`, `TrackImportHistory`
- EF Core context: `Contexts/ApiDbContext`
- Seed data: `Seeders/AppDbSeeder`

### `Domain.Domain`
- Constants:
  - `Constants/ApiEndpointsPath`
  - `Constants/HeadersMap`
- DTOs:
  - Requests: `UpsertAlbumRequest`, `UpsertTrackRequest`, `BulkImportTracksRequest`
	- Responses: `AlbumResponse`, `AlbumDetailResponse`, `TrackResponse`, `BulkImportRowResult`, `BulkImportTracksResult`, `DashboardSummaryResponse`
  - Classes: `ParsedTrackRow`
- Helpers:
  - `Helpers/UtilityHelper` (CSV header map + column extraction)
- Contracts:
	- `Interfaces/Services`: `IGenresService`, `IAlbumsService`, `ITracksService`, `IDashboardService`
  - `Interfaces/Repositories`: `IBaseRepository<T>`

### `Infraestructure.Data`
- `Repositories/BaseRepository<T>`
  - Generic CRUD and query methods
  - Include-capable overloads for filtered list/single retrieval with and without tracking
- `Repositories/RepositoryRegistrationExtension`

### `Services.Main`
- `GenresService`
- `AlbumsService`
- `TracksService`
- `DashboardService`

### `webApi`
- `Program.cs` with DI for all services and InMemory DB seeding
- `ApiEndPoints.cs` with minimal API mappings

## 2) Architecture Status After Refactor

- Service layer now uses `IBaseRepository<T>` instead of direct DbContext access for most operations.
- Include-heavy reads are supported by generic repository include overloads.
- CSV parsing helper logic moved to `Domain.Domain/Helpers` and `Domain.Domain/Constants`.
- DTO mapping has started to be centralized (example: `TrackResponse.FromEntity`).

## 3) Implemented Backend Features

### Albums
- `GET /api/v1/albums` (search + genre filter)
- `GET /api/v1/albums/{id}`
- `POST /api/v1/albums`
- `PUT /api/v1/albums/{id}`
- `DELETE /api/v1/albums/{id}`

### Tracks
- `GET /api/v1/albums/{albumId}/tracks` (genre + isActive filter)
- `POST /api/v1/albums/{albumId}/tracks`
- `PUT /api/v1/tracks/{id}`
- `DELETE /api/v1/tracks/{id}`

### Bulk Import
- `POST /api/v1/albums/{albumId}/tracks/bulk-import`
- Supports preview mode and import-valid-rows mode
- Validations implemented:
  - Required `title`
  - `trackNumber` integer + unique inside album
  - `durationSeconds` integer + greater than 0
  - `genre` supported
  - Duplicate title detection (existing album + CSV duplicate)

### Dashboard
- `GET /api/v1/dashboard`
- Summary includes:
  - `totalAlbums`
  - `totalTracks`
  - `albumsByGenre` (distinct album count per genre from tracks)
	- `recentImports` (loaded from `TrackImportHistory`, latest first)

## 4) Repository Pattern Enhancements

`IBaseRepository<T>` / `BaseRepository<T>` include:
- `GetByFiltersAsync(filters, include)`
- `GetByFiltersWithNoTrackingAsync(filters, include)`
- `FindByFiltersAsync(filters, include)`
- `FindByFiltersWithNoTrackingAsync(filters, include)`
- `FindFirstOrDefaultAsync(filters)`
- `FindFirstOrDefaultAsync(filters, include)`
- `FindFirstOrDefaultWithNoTrackingAsync(filters)`
- `FindFirstOrDefaultWithNoTrackingAsync(filters, include)`

This allows loading related entities (tracks/genres) while preserving the repository pattern in services.

## 5) Current API Endpoint Constants

Defined in `Domain.Domain/Constants/ApiEndpointsPath.cs`:
- `/api/v1/Genres`
- `/api/v1/dashboard`
- `/api/v1/albums`
- `/api/v1/albums/{id:int}`
- `/api/v1/albums/{albumId:int}/tracks`
- `/api/v1/albums/{albumId:int}/tracks/bulk-import`
- `/api/v1/tracks/{id:int}`

## 6) Progress Checklist

- [x] InMemory seed data (albums/tracks/genres)
- [x] Generic repository base CRUD/query implementation
- [x] Include-capable generic repository overloads
- [x] Genres endpoint
- [x] Album DTOs/contracts
- [x] Track DTOs/contracts
- [x] Bulk import DTOs/contracts
- [x] CSV helper/constants extraction
- [x] Albums service + endpoints
- [x] Tracks service + endpoints
- [x] Bulk import preview + confirm valid rows
- [x] Dashboard DTOs/contracts
- [x] Dashboard endpoint

## 7) Known Improvement Opportunities

- Add persistence and endpoints for advanced import audit details (file name, source, user id).
- Add pagination for recent imports if history volume grows.
- Add optional uniqueness constraints at model configuration level for `Track` (`AlbumId + TrackNumber`, `AlbumId + Title`).

## 8) Update Log

- 2026-08-28: Initial tracker created.
- 2026-08-28: Albums/tracks/bulk import backend implemented.
- 2026-08-28: Generic repository include overloads added and services refactored toward repository-first approach.
- 2026-08-28: Tracker updated to reflect post-refactor structure and current completion state.
- 2026-08-28: Removed default-instance repository find APIs and standardized nullable first-or-default usage to eliminate not-found ambiguity.
- 2026-08-28: Confirmed import history is only written when bulk import confirmation actually inserts valid rows.
