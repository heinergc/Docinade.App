@echo off
echo ========================================
echo Creando migración para tabla de auditoría
echo ========================================

echo 1. Compilando proyecto...
dotnet build
if %ERRORLEVEL% neq 0 (
    echo ERROR: El proyecto no compila correctamente
    pause
    exit /b 1
)

echo 2. Generando migración para AuditLog...
dotnet ef migrations add AddAuditLogTable --project . --startup-project .
if %ERRORLEVEL% neq 0 (
    echo ERROR: No se pudo generar la migración
    pause
    exit /b 1
)

echo 3. Aplicando migración a la base de datos...
dotnet ef database update --project . --startup-project .
if %ERRORLEVEL% neq 0 (
    echo ERROR: No se pudo aplicar la migración
    pause
    exit /b 1
)

echo ========================================
echo ¡Migración completada exitosamente!
echo ========================================
pause