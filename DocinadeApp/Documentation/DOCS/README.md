# Estructura de la Carpeta DOCS

Esta carpeta contiene toda la documentación, scripts, logs y archivos auxiliares del proyecto RubricasApp organizados por tipo de archivo.

## Estructura Organizativa

### 📁 Scripts/
Contiene todos los scripts de automatización y utilidades:

- **📁 PowerShell/**: Scripts de PowerShell (.ps1)
  - Scripts de compilación y verificación
  - Scripts de auditoría y verificación de errores
  - Scripts de migración de base de datos

- **📁 Batch/**: Scripts de lotes de Windows (.bat)
  - Scripts de compilación rápida
  - Scripts de limpieza y restauración
  - Scripts de diagnóstico

- **📁 Python/**: Scripts de Python (.py)
  - Scripts de verificación de compilación
  - Herramientas de análisis de código

### 📁 Documentation/
Contiene toda la documentación del proyecto:

- **ENVIO_CORREOS_README.md**: Documentación del sistema de envío de correos
- **ESTADO_CORRECCION_ERRORES.md**: Estado de corrección de errores
- **ETAPA2_RESUMEN_COMPLETO.md**: Resumen completo de la etapa 2
- **modelo-item-evaluacion.md**: Documentación del modelo de items de evaluación
- **RESUMEN_26_JULIO_2025.md**: Resumen del trabajo del 26 de julio
- **solucion-validacion-navigation-properties.md**: Solución para validación de propiedades de navegación
- **TRABAJO_17_JULIO_2025.md**: Documentación del trabajo del 17 de julio

### 📁 BuildLogs/
Contiene los logs y salidas de compilación:

- **build_clean.txt**: Log de limpieza de compilación
- **build_cmd.txt**: Comandos de compilación
- **build_output.txt**: Salida de compilación
- **compilation_output.txt**: Salida de compilación detallada
- **quick_build_test.txt**: Resultados de pruebas rápidas de compilación

### 📁 Backups/
Contiene archivos de respaldo:

- **CompilationTest.cs.bak**: Respaldo del archivo de prueba de compilación
- **MigrarEstado.cs.bak**: Respaldo del archivo de migración de estado

### 📁 Database/
Contiene archivos relacionados con la base de datos:

- **RubricasDbNueva.db-shm**: Archivo de memoria compartida de SQLite
- **RubricasDbNueva.db-wal**: Archivo de registro de escritura anticipada de SQLite

## Cómo Usar Esta Estructura

1. **Para ejecutar scripts**: Navega a la subcarpeta correspondiente en `Scripts/`
2. **Para consultar documentación**: Revisa los archivos en `Documentation/`
3. **Para revisar logs de compilación**: Consulta los archivos en `BuildLogs/`
4. **Para restaurar archivos**: Usa los respaldos en `Backups/`
5. **Para trabajar con base de datos**: Consulta los archivos en `Database/`

Esta organización facilita el mantenimiento y la localización rápida de archivos según su propósito específico.