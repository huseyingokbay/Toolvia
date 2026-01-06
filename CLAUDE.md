# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**MultiTools (Toolvia)** is a collection of online developer/designer utilities built with a Vue.js 3 frontend and .NET 10 backend API. It provides encoding/decoding, hashing, format conversion, and generation tools.

## Tech Stack

### Frontend (`/frontend`)
- **Framework:** Vue.js 3 with TypeScript
- **Build Tool:** Vite 7
- **Styling:** TailwindCSS v4 with @tailwindcss/forms
- **State Management:** Pinia
- **Routing:** Vue Router
- **UI Components:** HeadlessUI, Heroicons
- **Utilities:** crypto-js, qrcode, js-base64, VueUse

### Backend (`/backend`)
- **Framework:** .NET 10, ASP.NET Core Web API
- **API Docs:** Scalar (OpenAPI)

## Common Commands

### Frontend
```bash
cd frontend
npm install          # Install dependencies
npm run dev          # Start dev server (http://localhost:5173)
npm run build        # Build for production (runs vue-tsc first)
npm run preview      # Preview production build
```

### Backend
```bash
cd backend
dotnet run           # Start API server (http://localhost:5000)
dotnet build         # Build project
dotnet publish       # Publish for production
```

## Project Structure

```
/frontend/src/
├── components/      # Reusable Vue components
├── views/           # Page components (including /views/tools/)
├── stores/          # Pinia stores (theme.ts, sidebar.ts)
├── router/          # Vue Router configuration
├── types/           # TypeScript type definitions
├── utils/           # Utility functions (hash.ts, etc.)
└── assets/          # Static assets

/backend/
├── Controllers/     # API controllers
├── Models/          # Data models
└── Program.cs       # Application entry point
```

## Development Notes

- Frontend uses TailwindCSS v4 with PostCSS integration
- Type checking is enforced via `vue-tsc` before builds
- The solution file `MultiTools.sln` is at project root for IDE support
- Backend uses Scalar for interactive API documentation
