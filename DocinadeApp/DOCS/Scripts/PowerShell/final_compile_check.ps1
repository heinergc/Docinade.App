Write-Host "=== VERIFICACIÓN FINAL DE COMPILACIÓN ===" -ForegroundColor Green
Set-Location "C:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web"

Write-Host "Limpiando proyecto..." -ForegroundColor Yellow
dotnet clean > compile_clean.log 2>&1

Write-Host "Restaurando paquetes..." -ForegroundColor Yellow
dotnet restore > compile_restore.log 2>&1

Write-Host "Compilando proyecto..." -ForegroundColor Yellow
dotnet build > compile_build.log 2>&1

$buildOutput = Get-Content compile_build.log
Write-Host "=== RESULTADOS DE COMPILACIÓN ===" -ForegroundColor Cyan
$buildOutput

# Verificar errores
$errors = $buildOutput | Where-Object { $_ -match "error CS\d+" }
$warnings = $buildOutput | Where-Object { $_ -match "warning CS\d+" }

Write-Host "`n=== RESUMEN ===" -ForegroundColor Magenta
Write-Host "Errores encontrados: $($errors.Count)" -ForegroundColor $(if ($errors.Count -eq 0) { "Green" } else { "Red" })
Write-Host "Advertencias encontradas: $($warnings.Count)" -ForegroundColor $(if ($warnings.Count -eq 0) { "Green" } else { "Yellow" })

if ($errors.Count -eq 0) {
    Write-Host "`n¡COMPILACIÓN EXITOSA! ✅" -ForegroundColor Green
    Write-Host "Todos los errores han sido corregidos." -ForegroundColor Green
} else {
    Write-Host "`nERRORES PENDIENTES:" -ForegroundColor Red
    $errors | ForEach-Object { Write-Host $_ -ForegroundColor Red }
}

if ($warnings.Count -gt 0) {
    Write-Host "`nADVERTENCIAS:" -ForegroundColor Yellow
    $warnings | ForEach-Object { Write-Host $_ -ForegroundColor Yellow }
}