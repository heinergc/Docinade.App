# ================================================================
# SCRIPT DE MIGRACIÓN DE SQLITE A SQL SERVER
# ================================================================

param(
    [switch]$CreateMigration = $false,
    [switch]$UpdateDatabase = $false,
    [switch]$DropDatabase = $false,
    [switch]$ShowHelp = $false
)

if ($ShowHelp) {
    Write-Host "?? SCRIPT DE MIGRACIÓN A SQL SERVER" -ForegroundColor Cyan
    Write-Host "===================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Uso:"
    Write-Host "  .\MigrarSqlServer.ps1 -CreateMigration    # Crear nueva migración"
    Write-Host "  .\MigrarSqlServer.ps1 -UpdateDatabase     # Aplicar migraciones a BD"
    Write-Host "  .\MigrarSqlServer.ps1 -DropDatabase       # Eliminar BD (cuidado!)"
    Write-Host "  .\MigrarSqlServer.ps1 -ShowHelp           # Mostrar esta ayuda"
    Write-Host ""
    Write-Host "Pasos recomendados:"
    Write-Host "  1. Ejecutar con -CreateMigration primero"
    Write-Host "  2. Revisar los archivos de migración generados"
    Write-Host "  3. Ejecutar con -UpdateDatabase para aplicar"
    exit 0
}

Write-Host "?? MIGRACIÓN A SQL SERVER EXPRESS" -ForegroundColor Green
Write-Host "=======================================" -ForegroundColor Green

# Verificar que estamos en el directorio correcto
if (-not (Test-Path "RubricasApp.Web.csproj")) {
    Write-Host "? Error: Este script debe ejecutarse desde el directorio del proyecto" -ForegroundColor Red
    Write-Host "   Navegue a: src\RubricasApp.Web\" -ForegroundColor Yellow
    exit 1
}

# Función para verificar la conexión a SQL Server
function Test-SqlServerConnection {
    try {
        Write-Host "?? Verificando conexión a SQL Server..." -ForegroundColor Yellow
        
        # Intentar conectar usando dotnet ef
        $result = dotnet ef dbcontext info --no-build 2>&1
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "? Conexión a SQL Server exitosa" -ForegroundColor Green
            return $true
        } else {
            Write-Host "? Error de conexión a SQL Server:" -ForegroundColor Red
            Write-Host $result -ForegroundColor Red
            return $false
        }
    }
    catch {
        Write-Host "? Error verificando conexión: $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
}

# Función para crear respaldo de SQLite
function Backup-SqliteDatabase {
    if (Test-Path "RubricasDb.db") {
        $backupName = "RubricasDb_backup_$(Get-Date -Format 'yyyyMMdd_HHmmss').db"
        Copy-Item "RubricasDb.db" $backupName
        Write-Host "?? Backup de SQLite creado: $backupName" -ForegroundColor Green
    }
}

# Crear nueva migración
if ($CreateMigration) {
    Write-Host "?? Creando nueva migración para SQL Server..." -ForegroundColor Cyan
    
    # Verificar conexión primero
    if (-not (Test-SqlServerConnection)) {
        Write-Host "? No se puede conectar a SQL Server. Verifique la configuración." -ForegroundColor Red
        exit 1
    }
    
    # Crear backup de SQLite
    Backup-SqliteDatabase
    
    # Generar nombre de migración con timestamp
    $migrationName = "MigracionSqlServer_$(Get-Date -Format 'yyyyMMdd_HHmmss')"
    
    Write-Host "?? Generando migración: $migrationName" -ForegroundColor Yellow
    
    try {
        dotnet ef migrations add $migrationName --verbose
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "? Migración creada exitosamente: $migrationName" -ForegroundColor Green
            Write-Host ""
            Write-Host "?? Archivos generados en: Data/Migrations/" -ForegroundColor Cyan
            Write-Host "?? Revise los archivos antes de aplicar la migración" -ForegroundColor Yellow
            Write-Host ""
            Write-Host "? Siguiente paso: .\MigrarSqlServer.ps1 -UpdateDatabase" -ForegroundColor Green
        } else {
            Write-Host "? Error creando migración" -ForegroundColor Red
            exit 1
        }
    }
    catch {
        Write-Host "? Error: $($_.Exception.Message)" -ForegroundColor Red
        exit 1
    }
}

# Aplicar migraciones a la base de datos
if ($UpdateDatabase) {
    Write-Host "??? Aplicando migraciones a SQL Server..." -ForegroundColor Cyan
    
    # Verificar conexión primero
    if (-not (Test-SqlServerConnection)) {
        Write-Host "? No se puede conectar a SQL Server. Verifique la configuración." -ForegroundColor Red
        exit 1
    }
    
    Write-Host "?? Ejecutando: dotnet ef database update" -ForegroundColor Yellow
    
    try {
        dotnet ef database update --verbose
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "? Base de datos actualizada exitosamente" -ForegroundColor Green
            Write-Host ""
            Write-Host "?? SQL Server Express configurado correctamente" -ForegroundColor Green
            Write-Host "?? Servidor: SCPDTIC16584\SQLEXPRESS" -ForegroundColor Cyan
            Write-Host "?? Base de datos: RubricasDb" -ForegroundColor Cyan
            Write-Host ""
            Write-Host "?? Siguiente paso: Ejecutar la aplicación" -ForegroundColor Green
            Write-Host "   dotnet run" -ForegroundColor White
        } else {
            Write-Host "? Error aplicando migraciones" -ForegroundColor Red
            exit 1
        }
    }
    catch {
        Write-Host "? Error: $($_.Exception.Message)" -ForegroundColor Red
        exit 1
    }
}

# Eliminar base de datos (para empezar desde cero)
if ($DropDatabase) {
    Write-Host "?? ADVERTENCIA: Eliminando base de datos SQL Server" -ForegroundColor Red
    Write-Host "   Esto eliminará TODOS los datos permanentemente" -ForegroundColor Red
    
    $confirm = Read-Host "¿Está seguro? Escriba 'SI' para confirmar"
    
    if ($confirm -eq "SI") {
        Write-Host "??? Eliminando base de datos..." -ForegroundColor Yellow
        
        try {
            dotnet ef database drop --force --verbose
            
            if ($LASTEXITCODE -eq 0) {
                Write-Host "? Base de datos eliminada" -ForegroundColor Green
                Write-Host "?? Ahora puede ejecutar -CreateMigration y -UpdateDatabase" -ForegroundColor Yellow
            } else {
                Write-Host "? Error eliminando base de datos" -ForegroundColor Red
            }
        }
        catch {
            Write-Host "? Error: $($_.Exception.Message)" -ForegroundColor Red
        }
    } else {
        Write-Host "? Operación cancelada" -ForegroundColor Yellow
    }
}

# Si no se especifica ningún parámetro, mostrar estado
if (-not $CreateMigration -and -not $UpdateDatabase -and -not $DropDatabase) {
    Write-Host "?? ESTADO ACTUAL DE LA MIGRACIÓN" -ForegroundColor Cyan
    Write-Host "==================================" -ForegroundColor Cyan
    
    # Verificar conexión
    if (Test-SqlServerConnection) {
        Write-Host "? Conexión a SQL Server: OK" -ForegroundColor Green
    } else {
        Write-Host "? Conexión a SQL Server: FALLO" -ForegroundColor Red
    }
    
    # Verificar migraciones
    Write-Host ""
    Write-Host "?? Migraciones disponibles:" -ForegroundColor Yellow
    try {
        dotnet ef migrations list --no-build
    }
    catch {
        Write-Host "? Error listando migraciones" -ForegroundColor Red
    }
    
    Write-Host ""
    Write-Host "?? Comandos disponibles:" -ForegroundColor Cyan
    Write-Host "  .\MigrarSqlServer.ps1 -ShowHelp" -ForegroundColor White
    Write-Host "  .\MigrarSqlServer.ps1 -CreateMigration" -ForegroundColor White
    Write-Host "  .\MigrarSqlServer.ps1 -UpdateDatabase" -ForegroundColor White
}

Write-Host ""
Write-Host "?? Script completado" -ForegroundColor Green