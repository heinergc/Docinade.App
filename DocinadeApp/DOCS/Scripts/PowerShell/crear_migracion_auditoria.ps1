# Script para crear migración de auditoría
Write-Host "========================================" -ForegroundColor Green
Write-Host "Creando migración para tabla de auditoría" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green

try {
    Write-Host "1. Compilando proyecto..." -ForegroundColor Yellow
    dotnet build
    if ($LASTEXITCODE -ne 0) {
        throw "El proyecto no compila correctamente"
    }

    Write-Host "2. Generando migración para AuditLog..." -ForegroundColor Yellow
    dotnet ef migrations add AddAuditLogTable --project . --startup-project .
    if ($LASTEXITCODE -ne 0) {
        throw "No se pudo generar la migración"
    }

    Write-Host "3. Aplicando migración a la base de datos..." -ForegroundColor Yellow
    dotnet ef database update --project . --startup-project .
    if ($LASTEXITCODE -ne 0) {
        throw "No se pudo aplicar la migración"
    }

    Write-Host "========================================" -ForegroundColor Green
    Write-Host "¡Migración completada exitosamente!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
}
catch {
    Write-Host "ERROR: $_" -ForegroundColor Red
    exit 1
}

Read-Host "Presiona Enter para continuar"