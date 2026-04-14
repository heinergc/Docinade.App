Set-Location "C:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web"

Write-Host "🔧 VERIFICANDO CORRECCIONES DE CONFIGURACION SERVICE" -ForegroundColor Cyan
Write-Host ("=" * 60) -ForegroundColor Cyan

Write-Host "`n🔨 Ejecutando dotnet build..." -ForegroundColor Yellow
$buildResult = & dotnet build 2>&1

if ($LASTEXITCODE -eq 0) {
    Write-Host "`n✅ BUILD EXITOSO!" -ForegroundColor Green
    Write-Host "Todos los errores del ConfiguracionController han sido corregidos." -ForegroundColor Green
} else {
    Write-Host "`n❌ BUILD FALLÓ" -ForegroundColor Red
    Write-Host "Errores encontrados:" -ForegroundColor Yellow
}

$buildResult | ForEach-Object {
    if ($_ -match "error CS") {
        Write-Host "🔸 $_" -ForegroundColor Red
    } elseif ($_ -match "warning CS") {
        Write-Host "⚠️  $_" -ForegroundColor Yellow
    } else {
        Write-Host "   $_" -ForegroundColor White
    }
}

Write-Host "`n(" + "=" * 60) + ")" -ForegroundColor Cyan
Write-Host "Build completado con código: $LASTEXITCODE" -ForegroundColor Cyan