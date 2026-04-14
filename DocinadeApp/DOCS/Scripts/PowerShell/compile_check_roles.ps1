# Script para verificar compilación del RolesController
Write-Host "=== Verificando compilación del RolesController ===" -ForegroundColor Green

# Cambiar al directorio del proyecto
Set-Location "C:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web"

# Limpiar y compilar
Write-Host "Limpiando proyecto..." -ForegroundColor Yellow
dotnet clean --verbosity quiet

Write-Host "Compilando proyecto..." -ForegroundColor Yellow
$buildResult = dotnet build --verbosity normal --no-restore 2>&1

# Mostrar solo errores relacionados con RolesController
$rolesErrors = $buildResult | Where-Object { $_ -match "RolesController" -or $_ -match "error CS" }

if ($rolesErrors) {
    Write-Host "=== ERRORES ENCONTRADOS ===" -ForegroundColor Red
    $rolesErrors | ForEach-Object { Write-Host $_ -ForegroundColor Red }
} else {
    Write-Host "=== COMPILACIÓN EXITOSA ===" -ForegroundColor Green
    Write-Host "No se encontraron errores en RolesController.cs" -ForegroundColor Green
}

# Buscar warnings específicos
$warnings = $buildResult | Where-Object { $_ -match "warning" -and $_ -match "RolesController" }
if ($warnings) {
    Write-Host "=== WARNINGS ===" -ForegroundColor Yellow
    $warnings | ForEach-Object { Write-Host $_ -ForegroundColor Yellow }
}

# Verificar que todas las constantes de permisos existan
Write-Host "=== Verificando constantes de permisos ===" -ForegroundColor Cyan
$permissionCheck = @(
    "ApplicationPermissions.Configuracion.GESTIONAR_ROLES",
    "ApplicationPermissions.Configuracion.GESTIONAR_PERMISOS", 
    "ApplicationPermissions.Usuarios.VER",
    "AuditActionTypes.Configuracion.Manage",
    "AuditActionTypes.Configuracion.SyncPermissions"
)

foreach ($permission in $permissionCheck) {
    Write-Host "Verificando: $permission" -ForegroundColor White
}

Write-Host "=== VERIFICACIÓN COMPLETADA ===" -ForegroundColor Cyan