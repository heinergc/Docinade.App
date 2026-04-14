# =====================================================================================
# Script PowerShell para ejecutar CreateAdminUser_SQLite.sql
# Sistema de Rúbricas Académicas
# =====================================================================================

param(
    [string]$DatabasePath = "RubricasDbNueva.db",
    [string]$ScriptPath = "Scripts\CreateAdminUser_SQLite.sql"
)

Write-Host "=== Configuración de Usuario Administrador ===" -ForegroundColor Green
Write-Host "Base de datos: $DatabasePath" -ForegroundColor Yellow
Write-Host "Script SQL: $ScriptPath" -ForegroundColor Yellow
Write-Host ""

# Verificar que existe SQLite
try {
    $sqliteVersion = sqlite3 -version
    Write-Host "SQLite encontrado: $sqliteVersion" -ForegroundColor Green
} catch {
    Write-Host "ERROR: SQLite3 no está instalado o no está en el PATH" -ForegroundColor Red
    Write-Host "Instalar desde: https://www.sqlite.org/download.html" -ForegroundColor Yellow
    exit 1
}

# Verificar que existe la base de datos
if (-not (Test-Path $DatabasePath)) {
    Write-Host "ERROR: Base de datos no encontrada: $DatabasePath" -ForegroundColor Red
    exit 1
}

# Verificar que existe el script
if (-not (Test-Path $ScriptPath)) {
    Write-Host "ERROR: Script SQL no encontrado: $ScriptPath" -ForegroundColor Red
    exit 1
}

Write-Host "Ejecutando script de configuración..." -ForegroundColor Cyan

try {
    # Método 1: Usar Get-Content con pipe
    Write-Host "Método: Get-Content | sqlite3" -ForegroundColor Gray
    
    $output = Get-Content $ScriptPath | sqlite3 $DatabasePath 2>&1
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Script ejecutado exitosamente!" -ForegroundColor Green
        Write-Host ""
        Write-Host "=== Salida del Script ===" -ForegroundColor Cyan
        $output | ForEach-Object { Write-Host $_ -ForegroundColor White }
    } else {
        Write-Host "❌ Error ejecutando el script" -ForegroundColor Red
        Write-Host "Código de salida: $LASTEXITCODE" -ForegroundColor Red
        Write-Host "Salida de error:" -ForegroundColor Red
        $output | ForEach-Object { Write-Host $_ -ForegroundColor Red }
    }
} catch {
    Write-Host "❌ Excepción ejecutando el script:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    
    # Intentar método alternativo
    Write-Host ""
    Write-Host "Intentando método alternativo..." -ForegroundColor Yellow
    
    try {
        Write-Host "Método: sqlite3 con .read" -ForegroundColor Gray
        $alternativeOutput = sqlite3 $DatabasePath ".read $ScriptPath" 2>&1
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✅ Script ejecutado exitosamente con método alternativo!" -ForegroundColor Green
            $alternativeOutput | ForEach-Object { Write-Host $_ -ForegroundColor White }
        } else {
            Write-Host "❌ También falló el método alternativo" -ForegroundColor Red
            $alternativeOutput | ForEach-Object { Write-Host $_ -ForegroundColor Red }
        }
    } catch {
        Write-Host "❌ Ambos métodos fallaron:" -ForegroundColor Red
        Write-Host $_.Exception.Message -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "=== Verificación Final ===" -ForegroundColor Cyan

# Verificar que el usuario fue configurado
try {
    $verificationQuery = @"
SELECT 
    'Usuario: ' || Email as Info
FROM AspNetUsers 
WHERE Email = 'admin@rubricas.edu'
UNION ALL
SELECT 
    'Roles: ' || COUNT(*) as Info
FROM AspNetUsers u
JOIN AspNetUserRoles ur ON u.Id = ur.UserId
WHERE u.Email = 'admin@rubricas.edu'
UNION ALL
SELECT 
    'Permisos: ' || COUNT(*) as Info
FROM AspNetUsers u
JOIN AspNetUserClaims uc ON u.Id = uc.UserId
WHERE u.Email = 'admin@rubricas.edu'
AND uc.ClaimType = 'permission';
"@

    $verification = echo $verificationQuery | sqlite3 $DatabasePath
    
    Write-Host "Verificación de configuración:" -ForegroundColor Green
    $verification | ForEach-Object { Write-Host "  $_" -ForegroundColor White }
    
} catch {
    Write-Host "No se pudo verificar la configuración: $($_.Exception.Message)" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "=== Script Completado ===" -ForegroundColor Green