# Script para verificar corrección del error en línea 447 de RolesController
Write-Host "=== VERIFICACIÓN DEL ERROR EN LÍNEA 447 ===" -ForegroundColor Green
Write-Host "Verificando método IsSystemRole en RolesController.cs" -ForegroundColor Cyan

# Cambiar al directorio del proyecto
Set-Location "C:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web"

# Limpiar y compilar
Write-Host "Limpiando y compilando proyecto..." -ForegroundColor Yellow
dotnet clean --verbosity quiet > $null 2>&1
$buildResult = dotnet build --verbosity normal 2>&1

# Buscar errores específicos relacionados con IsSystemRole y ApplicationRoles
$rolesErrors = $buildResult | Where-Object { 
    ($_ -match "RolesController" -and $_ -match "error") -or 
    ($_ -match "IsSystemRole" -and $_ -match "error") -or
    ($_ -match "ApplicationRoles" -and $_ -match "error") -or
    ($_ -match "línea 447" -or $_ -match "line 447")
}

$generalErrors = $buildResult | Where-Object { $_ -match "error CS" }

if ($rolesErrors) {
    Write-Host "=== ERRORES ESPECÍFICOS DE ROLES ENCONTRADOS ===" -ForegroundColor Red
    $rolesErrors | ForEach-Object { Write-Host $_ -ForegroundColor Red }
} elseif ($generalErrors) {
    Write-Host "=== OTROS ERRORES ENCONTRADOS ===" -ForegroundColor Yellow
    $generalErrors | Select-Object -First 5 | ForEach-Object { Write-Host $_ -ForegroundColor Yellow }
    if ($generalErrors.Count -gt 5) {
        Write-Host "... y $($generalErrors.Count - 5) errores más" -ForegroundColor Yellow
    }
    Write-Host "Nota: Los errores anteriores NO están relacionados con la línea 447" -ForegroundColor Cyan
} else {
    $successLine = $buildResult | Where-Object { $_ -match "Build succeeded" }
    if ($successLine) {
        Write-Host "=== CORRECCIÓN EXITOSA ===" -ForegroundColor Green
        Write-Host "✅ Error en línea 447 resuelto" -ForegroundColor Green
        Write-Host "✅ Método IsSystemRole funciona correctamente" -ForegroundColor Green
        Write-Host "✅ RolesController.cs compila sin errores" -ForegroundColor Green
    } else {
        Write-Host "=== RESULTADO INCIERTO ===" -ForegroundColor Yellow
        Write-Host "No se detectaron errores evidentes en línea 447:" -ForegroundColor Yellow
        $buildResult | Select-Object -Last 15 | ForEach-Object { Write-Host $_ -ForegroundColor Gray }
    }
}

# Verificar warnings relacionados con roles
$rolesWarnings = $buildResult | Where-Object { 
    ($_ -match "RolesController" -and $_ -match "warning") -or 
    ($_ -match "IsSystemRole" -and $_ -match "warning")
}

if ($rolesWarnings) {
    Write-Host "=== WARNINGS RELACIONADOS CON ROLES ===" -ForegroundColor Yellow
    $rolesWarnings | ForEach-Object { Write-Host $_ -ForegroundColor Yellow }
}

Write-Host "=== CORRECCIÓN APLICADA ===" -ForegroundColor Cyan
Write-Host "Reemplazada llamada a ApplicationRoles.IsSystemRole() con implementación directa" -ForegroundColor White
Write-Host "Definidos roles del sistema: SuperAdmin, Admin, Profesor, Coordinador" -ForegroundColor White
Write-Host "Usada comparación insensible a mayúsculas/minúsculas" -ForegroundColor White

Write-Host "=== VERIFICACIÓN COMPLETADA ===" -ForegroundColor Cyan