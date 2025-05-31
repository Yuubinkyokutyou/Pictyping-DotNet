# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Pictyping is a .NET 8.0 reimplementation of a Rails typing game application, designed to gradually migrate from the existing Rails app while maintaining database compatibility and user sessions.

## Architecture

The codebase follows Clean Architecture with four main layers:

- **Pictyping.Core**: Domain entities, interfaces, DTOs (no external dependencies)
- **Pictyping.Infrastructure**: Data access with Entity Framework Core, Redis, external services
- **Pictyping.API**: ASP.NET Core Web API with JWT authentication and Google OAuth
- **Pictyping.Web**: React frontend with TypeScript, Vite, Redux Toolkit

## Development Commands

### API Development
```bash
cd src/Pictyping.API
dotnet watch run
```

### Frontend Development
```bash
cd src/Pictyping.Web
npm install
npm run dev
npm run build
npm run lint
```

### Full Environment
```bash
docker-compose up --build
```

### Testing
```bash
dotnet build Pictyping.sln
dotnet test
```

## Database & Migration

- Shares PostgreSQL database with existing Rails application
- Uses Entity Framework Core with snake_case mapping to match Rails conventions
- Migration script: `./scripts/migration/migrate-database.sh`
- All entities inherit from `BaseEntity` for consistent `created_at`/`updated_at` handling

## Authentication Architecture

- JWT tokens with Google OAuth integration
- Cross-domain session sharing between Rails and .NET apps via Redis
- CORS configured for `pictyping.com`, `new.pictyping.com`, and localhost development
- Cross-domain auth support in `CrossDomainAuth.tsx` for seamless user migration

## Access Points

- Frontend: http://localhost:3000
- API: http://localhost:5000
- Swagger: http://localhost:5000/swagger

## Key Implementation Notes

- Uses dependency injection for all services (registered in `Program.cs`)
- Entity Framework scaffolding generates models from existing Rails database schema
- Frontend Redux store manages authentication state and API communication
- Docker containerization supports both development and production deployment
- Nginx reverse proxy configuration for production routing