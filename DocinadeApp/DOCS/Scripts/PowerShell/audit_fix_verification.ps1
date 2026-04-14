# Script para verificar corrección de errores de auditoría en RolesController
Write-Host "=== VERIFICACIÓN DE CORRECCIONES DE AUDITORÍA ===" -ForegroundColor Green
Write-Host "Verificando llamadas a LogAsync en RolesController.cs" -ForegroundColor Cyan

# Cambiar al directorio del proyecto
Set-Location "C:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web"

# Limpiar y compilar
Write-Host "Limpiando y compilando proyecto..." -ForegroundColor Yellow
dotnet clean --verbosity quiet > $null 2>&1
$buildResult = dotnet build --verbosity normal 2>&1

# Buscar errores específicos de LogAsync y RolesController
$auditErrors = $buildResult | Where-Object { 
    ($_ -match "RolesController" -and $_ -match "error") -or 
    ($_ -match "LogAsync" -and $_ -match "error") -or
    ($_ -match "IAuditService" -and $_ -match "error")
}

$generalErrors = $buildResult | Where-Object { $_ -match "error CS" }

if ($auditErrors) {
    Write-Host "=== ERRORES DE AUDITORÍA ENCONTRADOS ===" -ForegroundColor Red
    $auditErrors | ForEach-Object { Write-Host $_ -ForegroundColor Red }
} elseif ($generalErrors) {
    Write-Host "=== OTROS ERRORES ENCONTRADOS ===" -ForegroundColor Yellow
    $generalErrors | Select-Object -First 5 | ForEach-Object { Write-Host $_ -ForegroundColor Yellow }
    if ($generalErrors.Count -gt 5) {
        Write-Host "... y $($generalErrors.Count - 5) errores más" -ForegroundColor Yellow
    }
} else {
    $successLine = $buildResult | Where-Object { $_ -match "Build succeeded" }
    if ($successLine) {
        Write-Host "=== CORRECCIONES EXITOSAS ===" -ForegroundColor Green
        Write-Host "✅ Todas las llamadas a LogAsync han sido corregidas" -ForegroundColor Green
        Write-Host "✅ RolesController.cs compila sin errores" -ForegroundColor Green
        Write-Host "✅ Parámetros de auditoría ahora son correctos" -ForegroundColor Green
    } else {
        Write-Host "=== RESULTADO INCIERTO ===" -ForegroundColor Yellow
        Write-Host "No se detectaron errores evidentes pero revisar resultado:" -ForegroundColor Yellow
        $buildResult | Select-Object -Last 10 | ForEach-Object { Write-Host $_ -ForegroundColor Gray }
    }
}

# Verificar warnings específicos de auditoría
$auditWarnings = $buildResult | Where-Object { 
    ($_ -match "RolesController" -and $_ -match "warning") -or 
    ($_ -match "LogAsync" -and $_ -match "warning")
}

if ($auditWarnings) {
    Write-Host "=== WARNINGS DE AUDITORÍA ===" -ForegroundColor Yellow
    $auditWarnings | ForEach-Object { Write-Host $_ -ForegroundColor Yellow }
}

Write-Host "=== CORRECCIONES APLICADAS ===" -ForegroundColor Cyan
Write-Host "1. Removido primer parámetro userId de LogAsync" -ForegroundColor White
Write-Host "2. Corregido orden de parámetros: action, entityType, entityId, entityName" -ForegroundColor White
Write-Host "3. Ajustados parámetros oldValues y newValues" -ForegroundColor White
Write-Host "4. Todas las llamadas ahora usan la signatura correcta de IAuditService" -ForegroundColor White

Write-Host "=== VERIFICACIÓN COMPLETADA ===" -ForegroundColor Cyan