Write-Host "=== VERIFICACIÓN FINAL COMPLETA ===" -ForegroundColor Green
Set-Location "C:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web"

Write-Host "1. Limpiando proyecto..." -ForegroundColor Yellow
dotnet clean > final_clean.log 2>&1

Write-Host "2. Restaurando paquetes..." -ForegroundColor Yellow  
dotnet restore > final_restore.log 2>&1

Write-Host "3. Compilando proyecto..." -ForegroundColor Yellow
dotnet build > final_build.log 2>&1

Write-Host "`n=== ANÁLISIS DE RESULTADOS ===" -ForegroundColor Cyan

$buildOutput = Get-Content final_build.log
Write-Host "Salida completa de compilación:" -ForegroundColor White
$buildOutput

Write-Host "`n=== RESUMEN DE ERRORES ===" -ForegroundColor Magenta
$errors = $buildOutput | Where-Object { $_ -match "error CS\d+" }
$auditErrors = $errors | Where-Object { $_ -match "AuditActionTypes" }
$permissionErrors = $errors | Where-Object { $_ -match "ApplicationPermissions" }
$otherErrors = $errors | Where-Object { $_ -notmatch "AuditActionTypes" -and $_ -notmatch "ApplicationPermissions" }

Write-Host "Errores AuditActionTypes: $($auditErrors.Count)" -ForegroundColor $(if ($auditErrors.Count -eq 0) { "Green" } else { "Red" })
if ($auditErrors.Count -gt 0) {
    $auditErrors | ForEach-Object { Write-Host "  $_" -ForegroundColor Red }
}

Write-Host "Errores ApplicationPermissions: $($permissionErrors.Count)" -ForegroundColor $(if ($permissionErrors.Count -eq 0) { "Green" } else { "Red" })
if ($permissionErrors.Count -gt 0) {
    $permissionErrors | ForEach-Object { Write-Host "  $_" -ForegroundColor Red }
}

Write-Host "Otros errores: $($otherErrors.Count)" -ForegroundColor $(if ($otherErrors.Count -eq 0) { "Green" } else { "Red" })
if ($otherErrors.Count -gt 0) {
    $otherErrors | ForEach-Object { Write-Host "  $_" -ForegroundColor Red }
}

Write-Host "`nTotal de errores: $($errors.Count)" -ForegroundColor $(if ($errors.Count -eq 0) { "Green" } else { "Red" })

if ($errors.Count -eq 0) {
    Write-Host "`n🎉 ¡COMPILACIÓN EXITOSA! 🎉" -ForegroundColor Green
    Write-Host "Todas las referencias de AuditActionTypes han sido corregidas." -ForegroundColor Green
} else {
    Write-Host "`n⚠️ ERRORES PENDIENTES ⚠️" -ForegroundColor Red
}