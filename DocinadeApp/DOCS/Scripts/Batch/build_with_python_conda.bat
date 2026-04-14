@echo off
cls
echo ============================================================
echo             COMPILACION DE PROYECTO RUBRICAS APP
echo ============================================================
echo Fecha: %date% %time%
echo Directorio: %cd%
echo Python configurado con Conda
echo ============================================================
echo.

echo 🧹 Limpiando proyecto...
dotnet clean > DOCS\BuildLogs\clean_output.txt 2>&1
if %errorlevel%==0 (
    echo ✅ Limpieza completada exitosamente
) else (
    echo ❌ Error en la limpieza
    type DOCS\BuildLogs\clean_output.txt
)

echo.
echo 📦 Restaurando dependencias...
dotnet restore > DOCS\BuildLogs\restore_output.txt 2>&1
if %errorlevel%==0 (
    echo ✅ Restauración completada exitosamente
) else (
    echo ❌ Error en la restauración
    type DOCS\BuildLogs\restore_output.txt
    goto :error
)

echo.
echo 🔨 Compilando proyecto...
dotnet build --configuration Debug --verbosity normal > DOCS\BuildLogs\build_output_28_julio.txt 2>&1
if %errorlevel%==0 (
    echo.
    echo ============================================================
    echo ✅ ¡COMPILACIÓN EXITOSA!
    echo ============================================================
    echo ✅ El proyecto se compiló sin errores
    echo 📄 Log guardado en: DOCS\BuildLogs\build_output_28_julio.txt
    echo.
    type DOCS\BuildLogs\build_output_28_julio.txt
    echo.
    echo 🎉 ¡PROYECTO LISTO PARA EJECUTAR!
    goto :success
) else (
    echo.
    echo ============================================================
    echo ❌ ERROR EN LA COMPILACIÓN
    echo ============================================================
    echo 💥 La compilación falló. Revisa los errores:
    echo 📄 Log de errores guardado en: DOCS\BuildLogs\build_output_28_julio.txt
    echo.
    type DOCS\BuildLogs\build_output_28_julio.txt
    goto :error
)

:success
echo.
echo ============================================================
echo                    RESUMEN FINAL
echo ============================================================
echo ✅ Python configurado con Conda: %CONDA_DEFAULT_ENV%
echo ✅ Limpieza: Exitosa
echo ✅ Restauración: Exitosa  
echo ✅ Compilación: Exitosa
echo 🚀 Estado: LISTO PARA EJECUTAR
echo ============================================================
pause
exit /b 0

:error
echo.
echo ============================================================
echo                    RESUMEN FINAL
echo ============================================================
echo ✅ Python configurado con Conda: %CONDA_DEFAULT_ENV%
echo ❌ Estado: ERROR EN COMPILACIÓN
echo 📋 Acción requerida: Revisar errores y corregir código
echo ============================================================
pause
exit /b 1