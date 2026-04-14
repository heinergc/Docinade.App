Write-Host "=== BÚSQUEDA COMPLETA DE REFERENCIAS AUDITACTIONTYPES ===" -ForegroundColor Green
Set-Location "C:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web"

Write-Host "`nCompilando para identificar errores de AuditActionTypes..." -ForegroundColor Yellow
dotnet build > audit_compile_check.log 2>&1

$compileOutput = Get-Content audit_compile_check.log
$auditErrors = $compileOutput | Where-Object { $_ -match "AuditActionTypes" }

Write-Host "`n=== ERRORES RELACIONADOS CON AUDITACTIONTYPES ===" -ForegroundColor Cyan
if ($auditErrors.Count -gt 0) {
    $auditErrors | ForEach-Object { Write-Host $_ -ForegroundColor Red }
    Write-Host "`nTotal de errores AuditActionTypes: $($auditErrors.Count)" -ForegroundColor Red
} else {
    Write-Host "No se encontraron errores relacionados con AuditActionTypes ✅" -ForegroundColor Green
}

Write-Host "`n=== TODOS LOS ERRORES DE COMPILACIÓN ===" -ForegroundColor Cyan
$allErrors = $compileOutput | Where-Object { $_ -match "error CS\d+" }
if ($allErrors.Count -gt 0) {
    $allErrors | ForEach-Object { Write-Host $_ -ForegroundColor Red }
    Write-Host "`nTotal de errores de compilación: $($allErrors.Count)" -ForegroundColor Red
} else {
    Write-Host "No se encontraron errores de compilación ✅" -ForegroundColor Green
}

Write-Host "`n=== ADVERTENCIAS ===" -ForegroundColor Yellow
$warnings = $compileOutput | Where-Object { $_ -match "warning CS\d+" }
if ($warnings.Count -gt 0) {
    $warnings | ForEach-Object { Write-Host $_ -ForegroundColor Yellow }
    Write-Host "`nTotal de advertencias: $($warnings.Count)" -ForegroundColor Yellow
} else {
    Write-Host "No se encontraron advertencias ✅" -ForegroundColor Green
}