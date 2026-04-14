# Script simple para compilar y verificar errores
Set-Location "C:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web"

Write-Host "Ejecutando dotnet build..." -ForegroundColor Green

# Ejecutar build y capturar resultado
$buildResult = & dotnet build 2>&1

# Mostrar resultado
if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ BUILD EXITOSO!" -ForegroundColor Green
    Write-Host "Los errores del UsersController han sido corregidos." -ForegroundColor Cyan
} else {
    Write-Host "❌ BUILD FALLÓ" -ForegroundColor Red
    Write-Host "Errores encontrados:" -ForegroundColor Yellow
}

# Mostrar salida del build
$buildResult | ForEach-Object {
    if ($_ -match "error CS") {
        Write-Host "🔸 $_" -ForegroundColor Red
    } elseif ($_ -match "warning CS") {
        Write-Host "⚠️  $_" -ForegroundColor Yellow
    } else {
        Write-Host "   $_" -ForegroundColor White
    }
}

Write-Host "`nBuild completado con código: $LASTEXITCODE" -ForegroundColor Cyan