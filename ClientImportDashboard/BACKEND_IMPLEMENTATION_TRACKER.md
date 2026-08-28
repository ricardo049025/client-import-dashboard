# Backend Implementation Tracker - Client Import Dashboard

## 1) Current Solution Structure (latest)

### `Domain.Entities`
- Core entities: `Album`, `Track`, `Genre`
- EF Core context: `Contexts/ApiDbContext`
- Seed data: `Seeders/AppDbSeeder`

### `Domain.Domain`
- Constants:
  - `Constants/ApiEndpointsPath`
  - `Constants/HeadersMap`
- DTOs:
  - Requests: `UpsertAlbumRequest`, `UpsertTrackRequest`, `BulkImportTracksRequest`
  - Responses: `AlbumResponse`, `AlbumDetailResponse`, `TrackResponse`, `BulkImportRowResult`, `BulkImportTracksResult`
  - Classes: `ParsedTrackRow`
- Helpers:
  - `Helpers/UtilityHelper` (CSV header map + column extraction)
- Contracts:
  - `Interfaces/Services`: `IGenresService`, `IAlbumsService`, `ITracksService`
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
- `GET /api/albums` (search + genre filter)
- `GET /api/albums/{id}`
- `POST /api/albums`
- `PUT /api/albums/{id}`
- `DELETE /api/albums/{id}`

### Tracks
- `GET /api/albums/{albumId}/tracks` (genre + isActive filter)
- `POST /api/albums/{albumId}/tracks`
- `PUT /api/tracks/{id}`
- `DELETE /api/tracks/{id}`

### Bulk Import
- `POST /api/albums/{albumId}/tracks/bulk-import`
- Supports preview mode and import-valid-rows mode
- Validations implemented:
  - Required `title`
  - `trackNumber` integer + unique inside album
  - `durationSeconds` integer + greater than 0
  - `genre` supported
  - Duplicate title detection (existing album + CSV duplicate)

## 4) Repository Pattern Enhancements

`IBaseRepository<T>` / `BaseRepository<T>` include:
- `GetByFiltersAsync(filters, include)`
- `GetByFiltersWithNoTrackingAsync(filters, include)`
- `FindByFiltersAsync(filters, include)`
- `FindByFiltersWithNoTrackingAsync(filters, include)`

This allows loading related entities (tracks/genres) while preserving the repository pattern in services.

## 5) Current API Endpoint Constants

Defined in `Domain.Domain/Constants/ApiEndpointsPath.cs`:
- `/api/v1/Genres`
- `/api/albums`
- `/api/albums/{id:int}`
- `/api/albums/{albumId:int}/tracks`
- `/api/albums/{albumId:int}/tracks/bulk-import`
- `/api/tracks/{id:int}`

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
- [ ] Dashboard DTOs/contracts
- [ ] Dashboard endpoint
- [ ] Automated tests for services
- [ ] Automated tests for endpoints

## 7) Known Improvement Opportunities

- Standardize not-found behavior in repository methods (currently checks rely on default entity instances).
- Replace `ToLower()` comparisons with a consistent case-insensitive strategy.
- Add import history persistence to support the "Recent imports" dashboard card.

## 8) Update Log

- 2026-08-28: Initial tracker created.
- 2026-08-28: Albums/tracks/bulk import backend implemented.
- 2026-08-28: Generic repository include overloads added and services refactored toward repository-first approach.
- 2026-08-28: Tracker updated to reflect post-refactor structure and current completion state.