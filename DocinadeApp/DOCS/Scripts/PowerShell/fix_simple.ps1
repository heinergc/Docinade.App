# Script simple para corregir el problema de migración
Write-Host "Corrigiendo problema de base de datos..." -ForegroundColor Yellow

# Ir al directorio del proyecto
Set-Location "C:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web"

# Paso 1: Remover migraciones existentes
Write-Host "1. Removiendo migraciones existentes..." -ForegroundColor Cyan
if (Test-Path "Migrations") {
    dotnet ef migrations remove --force
}

# Paso 2: Eliminar base de datos
Write-Host "2. Eliminando base de datos actual..." -ForegroundColor Cyan
if (Test-Path "RubricasDbNueva.db") { Remove-Item "RubricasDbNueva.db" -Force }
if (Test-Path "RubricasDbNueva.db-shm") { Remove-Item "RubricasDbNueva.db-shm" -Force }
if (Test-Path "RubricasDbNueva.db-wal") { Remove-Item "RubricasDbNueva.db-wal" -Force }

# Paso 3: Crear nueva migración
Write-Host "3. Creando nueva migración..." -ForegroundColor Cyan
dotnet ef migrations add InitialCreateComplete

# Paso 4: Actualizar base de datos
Write-Host "4. Actualizando base de datos..." -ForegroundColor Cyan
dotnet ef database update

Write-Host "Proceso completado!" -ForegroundColor Green