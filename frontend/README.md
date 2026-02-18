# Medicine Finder Frontend

## Overview
Next.js 14 (App Router) frontend for the Medicine Finder system with real-time stock updates, JWT auth via httpOnly cookies, and a clean shadcn/ui design system.

## Prerequisites
- Node.js 18+
- Backend API running (ASP.NET Core)

## Setup
```bash
npm install
```

Create `.env.local` based on `.env.example` and set:
- `API_BASE_URL` (backend base URL)
- `NEXT_PUBLIC_SIGNALR_URL` (backend URL for SignalR)
- `JWT_SECRET` (must match backend `Jwt:SecretKey`)

Run dev server:
```bash
npm run dev
```

## Data Fetching Strategy
- **Server Components**: pharmacy details (`/pharmacies/[id]`) uses direct server fetch for SEO and fast paint.
- **Client Components**: search, login, admin, and reservations require interactivity and SignalR.
- **Next API routes** are used for auth and proxying backend calls with httpOnly cookies.

## Auth & RBAC
- JWT stored in httpOnly cookie (`mf_token`).
- `middleware.ts` verifies JWT and enforces role-based routing.
- Admin routes: `/admin/stock`.
- User routes: `/search`, `/pharmacies/*`, `/me/reservations`.

## Tradeoffs
- SignalR does not require token because StockHub is public. If you secure the hub later, add token negotiation.
- Proxy keeps JWT out of client JS; all API calls flow through `/api/proxy`.

## Tests
```bash
npm run test
```

Included tests:
- Search page renders results
- Reservation modal validates quantity

## Folder Structure
```
src/
  app/
    login/
    search/
    pharmacies/[id]/
    admin/stock/
    me/reservations/
    api/
  components/
  hooks/
  lib/
  types/
```
