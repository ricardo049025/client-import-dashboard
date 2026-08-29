# Deployment Guide

## Recommended structure

Keep this monorepo structure as-is:

- Backend (Render): `ClientImportDashboard/webApi`
- Frontend (Vercel): `ClientImportDashboard/frontend`

No need to move frontend outside this workspace.

## 1) Deploy API to Render

### Option A: Blueprint (recommended)

- Render file included: `ClientImportDashboard/render.yaml`
- Docker file included: `ClientImportDashboard/webApi/Dockerfile`

In Render:

1. Create new Blueprint and point to this repository.
2. Ensure Render reads `ClientImportDashboard/render.yaml`.
3. Set env var manually:
   - `CORS_ALLOWED_ORIGINS=https://your-vercel-domain.vercel.app`
   - If needed, include multiple origins comma-separated.

### CORS notes

The API now supports origins from either:

- `Cors:AllowedOrigins` section in appsettings, or
- `CORS_ALLOWED_ORIGINS` environment variable (comma-separated)

## 2) Deploy frontend to Vercel

Project root directory in Vercel should be:

- `ClientImportDashboard/frontend`

Set env var in Vercel:

- `VITE_API_BASE_URL=https://your-render-api.onrender.com`

A SPA rewrite is included in:

- `ClientImportDashboard/frontend/vercel.json`

This prevents 404 on deep links like `/albums/1`.

## 3) Local production-like checks

### Backend

- Build: `dotnet build ClientImportDashboard.slnx`

### Frontend

From `ClientImportDashboard/frontend`:

- Build: `npm run build`

## 4) Post-deploy quick verification

- Open frontend URL.
- Confirm dashboard data loads from Render API.
- Test create album, add track, and bulk import preview/import.
- Confirm no CORS errors in browser console.
