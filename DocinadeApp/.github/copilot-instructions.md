# Copilot Instructions - RubricasApp.Web

## Project Overview
Educational rubrics management system for Costa Rica's Ministry of Education (MEP). ASP.NET Core 8 MVC application with custom permission-based authorization, automated seeding, and MEP regulatory compliance.

## Architecture

### Technology Stack
- **Framework**: ASP.NET Core 8.0 MVC
- **Database**: SQL Server (EF Core 8.0) - SQLite deprecated
- **Identity**: ASP.NET Core Identity with custom 168-permission system
- **Key Libraries**: EPPlus (Excel), QuestPDF (PDF), Rotativa (HTML→PDF), AutoMapper (DTOs)
- **Deployment**: Docker multi-stage build, Render.com hosting

### Directory Structure
```
Controllers/         # MVC controllers with permission attributes
Services/           # Business logic layer with interfaces
  ├── Academic/     # Student, groups, subjects
  ├── Identity/     # User management, IdentitySeederService
  ├── Permissions/  # 168-permission system
  ├── Audit/        # Operation logging
  └── Calificador/  # Grade book features
ViewModels/         # DTOs for presentation layer (separate from Models/)
Models/             # Domain entities (EF Core)
Data/               # RubricasDbContext (IdentityDbContext<ApplicationUser>)
Utils/              # Initializers (geographic data, groups, conduct)
Middleware/         # UTF8Middleware, PeriodoAcademicoMiddleware, RegistroAccesoMiddleware
Authorization/      # PermissionAuthorizationHandler, RequirePermissionAttribute
Areas/Admin/        # User/role management area
DB/                 # Production SQL scripts (DATOS_INICIALES_PRODUCCION.sql)
```

### Key Patterns
- **Service Layer**: All business logic in `Services/` with interface contracts
- **Repository Pattern**: Used in `Services/Academic/Repositories`
- **ViewModels**: Always use ViewModels (never Models) in controllers/views
- **Permission Checks**: Use `[RequirePermission("PermissionName")]` on controller actions
- **Areas**: Admin functionality isolated in `Areas/Admin/`

## Critical Workflows

### 1. Application Initialization (8-Stage Pipeline)
Located in `Program.cs` lines 230-360:
1. **SqlServerDatabaseInitializer**: Creates all tables from EF Core models
2. **DatabaseGroupsInitializer**: Grupo system tables
3. **CostaRicaGeographicInitializer**: 7 provinces, 82 cantons, 488 districts
4. **CrearTablaAuditoria**: Audit logging infrastructure
5. **ConductaSeedData**: 5 MEP fault types (REA 40862-V21 compliance)
6. **IdentitySeederService**: Auto-creates admin user (admin@rubricas.edu / Admin@2025!)
7. **PermissionService**: Seeds 168 permissions across modules
8. **Admin Verification**: Logs admin status and system readiness

**IMPORTANT**: Admin user is auto-created on first run. No manual SQL needed.

### 2. Permission System
- **168 permissions** across modules: Estudiantes, Grupos, Materias, Evaluaciones, etc.
- **Custom Authorization**: `PermissionAuthorizationHandler` validates claims
- **Usage**: `[RequirePermission("Ver.Estudiantes")]` on controller actions
- **Service**: `PermissionService.InitializeDefaultRolesAndPermissionsAsync()`

### 3. Database Context
- **DbContext**: `RubricasDbContext : IdentityDbContext<ApplicationUser>`
- **Connection**: Environment variable `ConnectionStrings__DefaultConnection`
- **Retry Strategy**: 5 attempts, 10s max delay, 60s timeout (see `Program.cs` lines 87-92)
- **SQL Server Only**: SQLite code paths deprecated, all configs use `UseSqlServer()`

### 4. Build & Deployment
```bash
# Local Development
dotnet build
dotnet run --urls http://localhost:18163

# Docker Build (multi-stage)
docker build -t rubricasapp .
docker run -p 10000:10000 -e ConnectionStrings__DefaultConnection="..." rubricasapp

# Render.com Deployment
- Branch: Publicar-render
- Environment Variables:
  * ASPNETCORE_ENVIRONMENT=Production
  * ConnectionStrings__DefaultConnection=Server=...;Database=...;
- Port: Dynamic via $PORT (default 10000)
```

### 5. Seeding Production Data
- **Auto-seeded**: Admin user, permissions, geographic data, conduct types
- **Manual Script**: `DB/DATOS_INICIALES_PRODUCCION.sql` (optional)
  - 12 materias (MAT, ESP, BIO, FIS, QUIM, ESTU, ING)
  - 4 grupos (11-A, 11-B, 10-A, 10-B)
  - 10 test students
  - 5 grade levels

## Project-Specific Conventions

### MEP Compliance Features
- **Conduct Module**: REA 40862-V21 fault tracking (`TipoFalta`, `BoletaConducta`, `NotaConducta`)
- **Attendance**: `Asistencia` and `Leccion` tables for daily tracking
- **Geographic Data**: Costa Rica provinces/cantons/districts for institutional context
- **Academic Periods**: `PeriodoAcademico` with active/inactive states

### Custom Middleware
1. **UTF8Middleware**: Ensures UTF-8 encoding for all responses
2. **PeriodoAcademicoMiddleware**: Injects active period into context
3. **RegistroAccesoMiddleware**: Logs all user access for audit

### Naming Conventions
- **Controllers**: Plural names (e.g., `EstudiantesController`, `EvaluacionesController`)
- **Services**: Singular with "Service" suffix (e.g., `EstudianteImportService`)
- **ViewModels**: Descriptive with context (e.g., `EstudianteListViewModel`, `EvaluacionCreateViewModel`)
- **Permissions**: Format `<Action>.<Module>` (e.g., `Ver.Estudiantes`, `Crear.Evaluacion`)

### Database Schema Notes
- **ApplicationUser**: Custom fields (Nombre, Apellidos, NumeroIdentificacion, Institucion, Activo)
- **Materias**: Columns are Codigo, Nombre, Creditos, Tipo, CicloSugerido (NOT NivelEducativo)
- **GruposEstudiantes**: Separate from EstudianteGrupos (bidirectional relationships)
- **InstrumentoRubrica**: Links evaluation instruments to rubrics

## Common Tasks

### Adding a New Controller
1. Inherit from `BaseController` (provides common utilities)
2. Add `[RequirePermission("...")]` attributes for authorization
3. Inject services via constructor (use interfaces)
4. Return ViewModels (never Models) to views

### Creating a New Service
1. Define interface in `Services/` or subdirectory
2. Implement with constructor injection for dependencies
3. Register in `Program.cs` as scoped/transient
4. Use `RubricasDbContext` for data access

### Adding Permissions
1. Define constant in `Models/Permissions/ApplicationPermissions.cs`
2. Add to module list in `PermissionService.GetAllPermissions()`
3. Restart app (auto-seeded on next initialization)
4. Use `[RequirePermission("New.Permission")]` on actions

### Troubleshooting
- **HTTP 500 on Render**: Check environment variables (ConnectionString, ASPNETCORE_ENVIRONMENT)
- **Admin Login Fails**: Verify IdentitySeederService logs, user is auto-created on startup
- **Database Errors**: Check retry strategy logs in Program.cs, validate ConnectionString
- **Permission Denied**: Verify user has role with permission claim

## Key Files Reference
- `Program.cs`: Application configuration and 8-stage initialization
- `IdentitySeederService.cs`: Auto-creates admin@rubricas.edu with SuperAdministrador role
- `RubricasDbContext.cs`: EF Core context with 70+ DbSets
- `PermissionService.cs`: 168-permission seeding and management
- `ApplicationPermissions.cs`: Permission constant definitions
- `RENDER_CONFIG.md`: Step-by-step Render.com deployment guide

## Integration Points
- **Email**: `IEmailService` abstraction (not yet implemented)
- **PDF Generation**: `IPdfService` using QuestPDF and Rotativa
- **Excel Export**: EPPlus and ClosedXML for grade book exports
- **Audit Logging**: `AuditLog` entity tracks all modifications
- **Session Management**: Active academic period stored in session

## Testing
- **Unit Tests**: `Tests/` directory (Playwright for E2E)
- **Test Admin**: admin@rubricas.edu / Admin@2025! (auto-created)
- **Test Students**: 10 students seeded via DATOS_INICIALES_PRODUCCION.sql

---

**TIP**: When working with this codebase, always check `Program.cs` initialization pipeline and `IdentitySeederService` for auto-seeding behavior. Most setup is automated—manual SQL scripts are optional supplements.
