# Script para verificar compilación final después de todas las correcciones
Write-Host "=== VERIFICACIÓN FINAL DE COMPILACIÓN ===" -ForegroundColor Green
Write-Host "Verificando: RolesController, AdminController, PermissionService y ApplicationRoles" -ForegroundColor Cyan

# Cambiar al directorio del proyecto
Set-Location "C:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web"

# Limpiar y restaurar
Write-Host "Limpiando y restaurando proyecto..." -ForegroundColor Yellow
dotnet clean --verbosity quiet
dotnet restore --verbosity quiet

# Compilar
Write-Host "Compilando proyecto..." -ForegroundColor Yellow
$buildResult = dotnet build --verbosity normal 2>&1

# Analizar resultados
$errors = $buildResult | Where-Object { $_ -match "error CS" }
$warnings = $buildResult | Where-Object { $_ -match "warning CS" }
$successLine = $buildResult | Where-Object { $_ -match "Build succeeded" }

if ($errors) {
    Write-Host "=== ERRORES ENCONTRADOS ===" -ForegroundColor Red
    $errors | ForEach-Object { Write-Host $_ -ForegroundColor Red }
    
    # Mostrar errores específicos de los archivos corregidos
    $specificErrors = $buildResult | Where-Object { 
        $_ -match "RolesController|AdminController|PermissionService|ApplicationRoles" 
    }
    if ($specificErrors) {
        Write-Host "=== ERRORES EN ARCHIVOS ESPECÍFICOS ===" -ForegroundColor Magenta
        $specificErrors | ForEach-Object { Write-Host $_ -ForegroundColor Magenta }
    }
} elseif ($successLine) {
    Write-Host "=== COMPILACIÓN EXITOSA ===" -ForegroundColor Green
    Write-Host "✅ Todos los errores han sido corregidos" -ForegroundColor Green
    Write-Host "✅ RolesController.cs - OK" -ForegroundColor Green
    Write-Host "✅ AdminController.cs - OK" -ForegroundColor Green  
    Write-Host "✅ PermissionService.cs - OK" -ForegroundColor Green
    Write-Host "✅ ApplicationRoles.cs - OK" -ForegroundColor Green
} else {
    Write-Host "=== RESULTADO DESCONOCIDO ===" -ForegroundColor Yellow
    Write-Host "La compilación no mostró errores evidentes, pero revise el resultado completo:" -ForegroundColor Yellow
    $buildResult | ForEach-Object { Write-Host $_ -ForegroundColor Gray }
}

if ($warnings) {
    Write-Host "=== WARNINGS (No críticos) ===" -ForegroundColor Yellow
    $warnings | Select-Object -First 10 | ForEach-Object { Write-Host $_ -ForegroundColor Yellow }
    if ($warnings.Count -gt 10) {
        Write-Host "... y $($warnings.Count - 10) warnings más" -ForegroundColor Yellow
    }
}

Write-Host "=== VERIFICACIÓN COMPLETADA ===" -ForegroundColor Cyan
Write-Host "Archivos verificados:" -ForegroundColor White
Write-Host "- Controllers/Admin/RolesController.cs" -ForegroundColor White
Write-Host "- Controllers/Admin/AdminController.cs" -ForegroundColor White
Write-Host "- Services/Permissions/PermissionService.cs" -ForegroundColor White
Write-Host "- Models/Permissions/ApplicationRoles.cs" -ForegroundColor White