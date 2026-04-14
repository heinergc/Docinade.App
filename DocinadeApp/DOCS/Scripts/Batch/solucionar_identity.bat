@echo off
echo ====================================
echo   SOLUCION RAPIDA PARA IDENTITY
echo ====================================
echo.

echo 1. Eliminando base de datos actual...
if exist RubricasDbNueva.db (
    del RubricasDbNueva.db
    echo    Base de datos eliminada.
) else (
    echo    No se encontro base de datos para eliminar.
)

echo.
echo 2. La aplicacion recreara automaticamente la base de datos
echo    con todas las tablas de Identity cuando se ejecute.
echo.
echo 3. Ejecute la aplicacion ahora con: dotnet run
echo.

echo ====================================
echo   PROCESO COMPLETADO
echo ====================================
echo.
echo La aplicacion creara automaticamente:
echo - Todas las tablas de ASP.NET Identity
echo - Todas las tablas de la aplicacion
echo - Datos iniciales y usuarios por defecto
echo.
pause