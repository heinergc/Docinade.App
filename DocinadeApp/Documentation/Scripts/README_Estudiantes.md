# 🏫 SCRIPT DE INSERCIÓN DE ESTUDIANTES MEP COSTA RICA - GUÍA DE USO

## 📋 Descripción
Script SQL para insertar 50 estudiantes del MEP (Ministerio de Educación Pública) de Costa Rica en la base de datos SQL Server de RubricasApp.

## 📁 Archivo: `Insertar50Estudiantes.sql`

### 🎯 Características del Script:
- ✅ **50 estudiantes costarricenses** con datos realistas del MEP
- ✅ **5 colegios diferentes** del sistema educativo de Costa Rica  
- ✅ **Cédulas costarricenses válidas** en formato correcto
- ✅ **Correos institucionales** @estudiantes.mep.go.cr
- ✅ **Validaciones incorporadas** para evitar errores
- ✅ **Transacciones seguras** con rollback automático
- ✅ **Verificaciones automáticas** de la estructura
- ✅ **Reportes de inserción** con estadísticas del MEP

### 🏫 Distribución por Colegios del MEP:

| Colegio | Ubicación | Nivel | Estudiantes | Secciones |
|---------|-----------|--------|-------------|-----------|
| **Colegio Científico de Costa Rica** | San José | 11° | 15 | 11-A, 11-B, 11-C |
| **Liceo de Costa Rica** | San José | 10° | 10 | 10-A, 10-B |
| **Colegio de Cartago** | Cartago | 9° | 10 | 9-A, 9-B |
| **Liceo de Heredia** | Heredia | 8° | 10 | 8-A, 8-B |
| **Colegio de Puntarenas** | Puntarenas | 7° | 5 | 7-A |

### 📊 Campos del MEP Incluidos:
- **Nombre y Apellidos**: Nombres típicos costarricenses
- **NumeroId**: Cédulas costarricenses reales (formato 9 dígitos)
- **DireccionCorreo**: Emails del MEP @estudiantes.mep.go.cr
- **Institucion**: Colegios públicos reconocidos de Costa Rica
- **Grupos**: Niveles del 7° al 11° año (secundaria costarricense)
- **Año**: 2025 (curso lectivo)
- **PeriodoAcademicoId**: Período escolar MEP (feb-dic)

## 🚀 Instrucciones de Uso

### Método 1: SQL Server Management Studio (SSMS)
```sql
-- 1. Conectar a la instancia SQL Server
-- Server: SCPDTIC16584\SQLEXPRESS
-- Database: RubricasDb

-- 2. Abrir el archivo Insertar50Estudiantes.sql

-- 3. Ejecutar el script completo (F5)
```

### Método 2: Azure Data Studio
```sql
-- 1. Conectar a SQL Server
-- 2. Abrir archivo Insertar50Estudiantes.sql  
-- 3. Ejecutar script
```

### Método 3: Línea de Comandos (sqlcmd)
```bash
# Navegar al directorio del proyecto
cd C:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web

# Ejecutar script
sqlcmd -S SCPDTIC16584\SQLEXPRESS -E -i Scripts\Insertar50Estudiantes.sql
```

### Método 4: Desde la Aplicación (PowerShell)
```powershell
# Navegar al directorio del proyecto
cd C:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web

# Ejecutar usando configuración de la aplicación
sqlcmd -S SCPDTIC16584\SQLEXPRESS -E -d RubricasDb -i "Scripts\Insertar50Estudiantes.sql"
```

## ✅ Verificaciones del Script

El script incluye verificaciones automáticas:

1. **Existencia de tabla Estudiantes**
2. **Existencia de PeriodoAcademico activo** (crea uno si no existe)
3. **Transacciones seguras** con manejo de errores
4. **Reportes post-inserción** con estadísticas

## 📈 Salida Esperada

```
📚 Iniciando inserción de 50 estudiantes de prueba...
✅ Usando período académico con ID: 1
✅ Se insertaron exitosamente 50 estudiantes

📊 Verificación de datos insertados:
Grupos          CantidadEstudiantes
ADM-A1          5
ADM-B1          5
CON-A1          5
CON-A2          5
...

📈 Estadísticas generales:
Total de estudiantes: 50
Grupos diferentes: 9
Instituciones diferentes: 1

🎉 Script completado exitosamente
```

## 🔧 Resolución de Problemas

### Error: "La tabla Estudiantes no existe"
```sql
-- Verificar estructura de base de datos
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_NAME LIKE '%Estudiant%';

-- Ejecutar migraciones si es necesario
-- En PowerShell:
dotnet ef database update
```

### Error: "No se encontró período académico"
El script creará automáticamente un período académico del MEP por defecto:
- **Nombre**: 2025 Curso Lectivo
- **FechaInicio**: 2025-02-12 (inicio del curso lectivo en Costa Rica)
- **FechaFin**: 2025-12-06 (fin del curso lectivo)
- **Activo**: Sí

### Error: "Duplicación de NumeroId"
```sql
-- Limpiar datos existentes si es necesario
DELETE FROM Estudiantes WHERE NumeroId IN (
    '117850421', '207940312', '305820193', '401730264', '118640375'
    -- ... otros números de cédula costarricense
);
```

## 🗑️ Script de Limpieza (Opcional)

Si necesitas eliminar los datos insertados:

```sql
-- CUIDADO: Esto eliminará TODOS los estudiantes del MEP del período actual
DELETE FROM Estudiantes 
WHERE DireccionCorreo LIKE '%@estudiantes.mep.go.cr'
AND PeriodoAcademicoId = (
    SELECT TOP 1 Id FROM PeriodosAcademicos 
    WHERE Activo = 1 ORDER BY Id DESC
);

PRINT 'Estudiantes del MEP eliminados';
```

## 📝 Personalización para el MEP

Para modificar los datos del contexto costarricense:

1. **Cambiar colegios**: Reemplazar por otros colegios del MEP
2. **Cambiar dominio de email**: Mantener @estudiantes.mep.go.cr o usar @mep.go.cr
3. **Agregar más estudiantes**: Usar cédulas costarricenses válidas
4. **Cambiar niveles**: Usar formato del MEP (7°-11°)
5. **Actualizar direcciones**: Usar provincias de Costa Rica

### 🇨🇷 Formato de Cédulas Costarricenses:
- **Estructura**: P-AAAA-BBBB (9 dígitos)
- **P**: Provincia (1=San José, 2=Alajuela, 3=Cartago, 4=Heredia, 5=Guanacaste, 6=Puntarenas, 7=Limón)
- **AAAA**: Número consecutivo
- **BBBB**: Número consecutivo

## 🎯 Casos de Uso en el MEP

Este script es útil para:
- ✅ **Evaluaciones de colegios** del sistema público
- ✅ **Pruebas FARO** (Fortalecimiento de Aprendizajes)
- ✅ **Bachillerato por Madurez Suficiente**
- ✅ **Evaluaciones diagnósticas** del MEP
- ✅ **Capacitación docente** con datos realistas
- ✅ **Pruebas de sistemas** educativos

---

**Última actualización**: 22 de septiembre de 2025  
**Compatibilidad**: SQL Server Express 2019+  
**Base de datos**: RubricasDb  
**Contexto**: MEP Costa Rica 🇨🇷
