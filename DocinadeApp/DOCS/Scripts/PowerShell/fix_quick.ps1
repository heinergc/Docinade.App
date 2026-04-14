Write-Host "INICIANDO CORRECCIÓN DE BASE DE DATOS..." -ForegroundColor Cyan
Write-Host "Problema: SQLite Error 1: 'no such column: a.CreatedDate'" -ForegroundColor Yellow
Write-Host ""

# Ir al directorio correcto
Set-Location "C:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web"

Write-Host "1. Removiendo migraciones existentes..." -ForegroundColor Green
# Remover todas las migraciones
Get-ChildItem -Path "Migrations" -Filter "*.cs" | Remove-Item -Force
Write-Host "   ✅ Migraciones eliminadas" -ForegroundColor Green

Write-Host "2. Eliminando base de datos actual..." -ForegroundColor Green
# Eliminar archivos de base de datos
@("RubricasDbNueva.db", "RubricasDbNueva.db-shm", "RubricasDbNueva.db-wal") | ForEach-Object {
    if (Test-Path $_) {
        Remove-Item $_ -Force
        Write-Host "   ✅ Eliminado: $_" -ForegroundColor Green
    }
}

Write-Host "3. Creando migración inicial completa..." -ForegroundColor Green
$result = dotnet ef migrations add InitialCreateWithIdentity 2>&1
if ($LASTEXITCODE -eq 0) {
    Write-Host "   ✅ Migración creada exitosamente" -ForegroundColor Green
} else {
    Write-Host "   ❌ Error en migración: $result" -ForegroundColor Red
    exit 1
}

Write-Host "4. Aplicando migración a la base de datos..." -ForegroundColor Green
$result = dotnet ef database update 2>&1
if ($LASTEXITCODE -eq 0) {
    Write-Host "   ✅ Base de datos actualizada exitosamente" -ForegroundColor Green
} else {
    Write-Host "   ❌ Error en actualización: $result" -ForegroundColor Red
    exit 1
}

Write-Host "5. Verificando resultado..." -ForegroundColor Green
if (Test-Path "RubricasDbNueva.db") {
    $size = (Get-Item "RubricasDbNueva.db").Length
    Write-Host "   ✅ Base de datos creada - Tamaño: $([math]::Round($size/1KB, 2)) KB" -ForegroundColor Green
} else {
    Write-Host "   ❌ Base de datos no encontrada" -ForegroundColor Red
}

Write-Host ""
Write-Host "CORRECCIÓN COMPLETADA!" -ForegroundColor Cyan
Write-Host "El error 'no such column: a.CreatedDate' debería estar resuelto." -ForegroundColor Green
Write-Host "Ahora puedes ejecutar: dotnet run" -ForegroundColor Yellow