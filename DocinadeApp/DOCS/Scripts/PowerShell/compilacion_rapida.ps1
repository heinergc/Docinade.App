Write-Host "Iniciando compilación rápida..." -ForegroundColor Yellow

try {
    Set-Location "C:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web"
    
    Write-Host "Limpiando proyecto..." -ForegroundColor Cyan
    dotnet clean -q
    
    Write-Host "Compilando proyecto..." -ForegroundColor Cyan
    $output = dotnet build --no-restore --verbosity normal 2>&1
    
    $errors = $output | Where-Object { $_ -match "error" }
    $warnings = $output | Where-Object { $_ -match "warning" }
    
    Write-Host "`n=== ERRORES DE COMPILACIÓN ===" -ForegroundColor Red
    if ($errors.Count -eq 0) {
        Write-Host "¡No hay errores de compilación!" -ForegroundColor Green
    } else {
        Write-Host "Total de errores encontrados: $($errors.Count)" -ForegroundColor Red
        $errors | ForEach-Object { Write-Host $_ -ForegroundColor Red }
    }
    
    Write-Host "`n=== WARNINGS ===" -ForegroundColor Yellow
    if ($warnings.Count -eq 0) {
        Write-Host "No hay warnings." -ForegroundColor Green
    } else {
        Write-Host "Total de warnings: $($warnings.Count)" -ForegroundColor Yellow
        $warnings | Select-Object -First 10 | ForEach-Object { Write-Host $_ -ForegroundColor Yellow }
        if ($warnings.Count -gt 10) {
            Write-Host "... y $($warnings.Count - 10) warnings más" -ForegroundColor Yellow
        }
    }
    
    Write-Host "`n=== RESUMEN ===" -ForegroundColor White
    Write-Host "Errores: $($errors.Count)" -ForegroundColor $(if ($errors.Count -eq 0) { "Green" } else { "Red" })
    Write-Host "Warnings: $($warnings.Count)" -ForegroundColor $(if ($warnings.Count -eq 0) { "Green" } else { "Yellow" })
    
} catch {
    Write-Host "Error durante la compilación: $_" -ForegroundColor Red
}

Read-Host "`nPresiona Enter para continuar"