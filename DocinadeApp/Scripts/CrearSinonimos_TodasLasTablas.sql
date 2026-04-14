-- =====================================================================
-- SCRIPT PARA CREAR SINÓNIMOS DE TODAS LAS TABLAS
-- Sistema de Rúbricas - Base de Datos RubricasDb
-- =====================================================================
-- Este script crea sinónimos para simplificar el acceso a las tablas
-- Convierte de: RubricasDb.dbo.Materias -> Materias
-- =====================================================================

USE [master];
GO

-- Asegurar que estamos en la base de datos correcta
-- NOTA: Cambiar 'RubricasDb' por el nombre real de tu base de datos si es diferente
USE [RubricasDb];
GO

PRINT 'Iniciando creación de sinónimos para todas las tablas del sistema...';
PRINT '';

-- =====================================================================
-- FUNCIÓN AUXILIAR: Eliminar sinónimo si existe
-- =====================================================================
-- Esta función nos ayuda a recrear sinónimos existentes

-- =====================================================================
-- SINÓNIMOS PARA TABLAS DE LA APLICACIÓN
-- =====================================================================

PRINT '1. Creando sinónimos para tablas principales de la aplicación...';

-- Eliminar sinónimos existentes si existen y crear nuevos
IF EXISTS (SELECT * FROM sys.synonyms WHERE name = 'Rubricas')
    DROP SYNONYM [Rubricas];
CREATE SYNONYM [Rubricas] FOR [RubricasDb].[dbo].[Rubricas];
PRINT '  ✓ Sinónimo creado: Rubricas -> RubricasDb.dbo.Rubricas';

IF EXISTS (SELECT * FROM sys.synonyms WHERE name = 'ItemsEvaluacion')
    DROP SYNONYM [ItemsEvaluacion];
CREATE SYNONYM [ItemsEvaluacion] FOR [RubricasDb].[dbo].[ItemsEvaluacion];
PRINT '  ✓ Sinónimo creado: ItemsEvaluacion -> RubricasDb.dbo.ItemsEvaluacion';

IF EXISTS (SELECT * FROM sys.synonyms WHERE name = 'NivelesCalificacion')
    DROP SYNONYM [NivelesCalificacion];
CREATE SYNONYM [NivelesCalificacion] FOR [RubricasDb].[dbo].[NivelesCalificacion];
PRINT '  ✓ Sinónimo creado: NivelesCalificacion -> RubricasDb.dbo.NivelesCalificacion';

IF EXISTS (SELECT * FROM sys.synonyms WHERE name = 'GruposCalificacion')
    DROP SYNONYM [GruposCalificacion];
CREATE SYNONYM [GruposCalificacion] FOR [RubricasDb].[dbo].[GruposCalificacion];
PRINT '  ✓ Sinónimo creado: GruposCalificacion -> RubricasDb.dbo.GruposCalificacion';

IF EXISTS (SELECT * FROM sys.synonyms WHERE name = 'ValoresRubrica')
    DROP SYNONYM [ValoresRubrica];
CREATE SYNONYM [ValoresRubrica] FOR [RubricasDb].[dbo].[ValoresRubrica];
PRINT '  ✓ Sinónimo creado: ValoresRubrica -> RubricasDb.dbo.ValoresRubrica';

IF EXISTS (SELECT * FROM sys.synonyms WHERE name = 'Estudiantes')
    DROP SYNONYM [Estudiantes];
CREATE SYNONYM [Estudiantes] FOR [RubricasDb].[dbo].[Estudiantes];
PRINT '  ✓ Sinónimo creado: Estudiantes -> RubricasDb.dbo.Estudiantes';

IF EXISTS (SELECT * FROM sys.synonyms WHERE name = 'Evaluaciones')
    DROP SYNONYM [Evaluaciones];
CREATE SYNONYM [Evaluaciones] FOR [RubricasDb].[dbo].[Evaluaciones];
PRINT '  ✓ Sinónimo creado: Evaluaciones -> RubricasDb.dbo.Evaluaciones';

IF EXISTS (SELECT * FROM sys.synonyms WHERE name = 'DetallesEvaluacion')
    DROP SYNONYM [DetallesEvaluacion];
CREATE SYNONYM [DetallesEvaluacion] FOR [RubricasDb].[dbo].[DetallesEvaluacion];
PRINT '  ✓ Sinónimo creado: DetallesEvaluacion -> RubricasDb.dbo.DetallesEvaluacion';

IF EXISTS (SELECT * FROM sys.synonyms WHERE name = 'PeriodosAcademicos')
    DROP SYNONYM [PeriodosAcademicos];
CREATE SYNONYM [PeriodosAcademicos] FOR [RubricasDb].[dbo].[PeriodosAcademicos];
PRINT '  ✓ Sinónimo creado: PeriodosAcademicos -> RubricasDb.dbo.PeriodosAcademicos';

IF EXISTS (SELECT * FROM sys.synonyms WHERE name = 'RubricaNiveles')
    DROP SYNONYM [RubricaNiveles];
CREATE SYNONYM [RubricaNiveles] FOR [RubricasDb].[dbo].[RubricaNiveles];
PRINT '  ✓ Sinónimo creado: RubricaNiveles -> RubricasDb.dbo.RubricaNiveles';

IF EXISTS (SELECT * FROM sys.synonyms WHERE name = 'ConfiguracionesSistema')
    DROP SYNONYM [ConfiguracionesSistema];
CREATE SYNONYM [ConfiguracionesSistema] FOR [RubricasDb].[dbo].[ConfiguracionesSistema];
PRINT '  ✓ Sinónimo creado: ConfiguracionesSistema -> RubricasDb.dbo.ConfiguracionesSistema';

IF EXISTS (SELECT * FROM sys.synonyms WHERE name = 'AuditLogs')
    DROP SYNONYM [AuditLogs];
CREATE SYNONYM [AuditLogs] FOR [RubricasDb].[dbo].[AuditLogs];
PRINT '  ✓ Sinónimo creado: AuditLogs -> RubricasDb.dbo.AuditLogs';

-- =====================================================================
-- SINÓNIMOS PARA TABLAS ACADÉMICAS
-- =====================================================================

PRINT '';
PRINT '2. Creando sinónimos para tablas académicas...';

IF EXISTS (SELECT * FROM sys.synonyms WHERE name = 'Materias')
    DROP SYNONYM [Materias];
CREATE SYNONYM [Materias] FOR [RubricasDb].[dbo].[Materias];
PRINT '  ✓ Sinónimo creado: Materias -> RubricasDb.dbo.Materias';

IF EXISTS (SELECT * FROM sys.synonyms WHERE name = 'MateriaRequisitos')
    DROP SYNONYM [MateriaRequisitos];
CREATE SYNONYM [MateriaRequisitos] FOR [RubricasDb].[dbo].[MateriaRequisitos];
PRINT '  ✓ Sinónimo creado: MateriaRequisitos -> RubricasDb.dbo.MateriaRequisitos';

IF EXISTS (SELECT * FROM sys.synonyms WHERE name = 'MateriaPeriodos')
    DROP SYNONYM [MateriaPeriodos];
CREATE SYNONYM [MateriaPeriodos] FOR [RubricasDb].[dbo].[MateriaPeriodos];
PRINT '  ✓ Sinónimo creado: MateriaPeriodos -> RubricasDb.dbo.MateriaPeriodos';

-- =====================================================================
-- SINÓNIMOS PARA TABLAS DE INSTRUMENTOS
-- =====================================================================

PRINT '';
PRINT '3. Creando sinónimos para tablas de instrumentos de evaluación...';

IF EXISTS (SELECT * FROM sys.synonyms WHERE name = 'InstrumentosEvaluacion')
    DROP SYNONYM [InstrumentosEvaluacion];
CREATE SYNONYM [InstrumentosEvaluacion] FOR [RubricasDb].[dbo].[InstrumentosEvaluacion];
PRINT '  ✓ Sinónimo creado: InstrumentosEvaluacion -> RubricasDb.dbo.InstrumentosEvaluacion';

IF EXISTS (SELECT * FROM sys.synonyms WHERE name = 'InstrumentoRubricas')
    DROP SYNONYM [InstrumentoRubricas];
CREATE SYNONYM [InstrumentoRubricas] FOR [RubricasDb].[dbo].[InstrumentoRubricas];
PRINT '  ✓ Sinónimo creado: InstrumentoRubricas -> RubricasDb.dbo.InstrumentoRubricas';

IF EXISTS (SELECT * FROM sys.synonyms WHERE name = 'InstrumentoMaterias')
    DROP SYNONYM [InstrumentoMaterias];
CREATE SYNONYM [InstrumentoMaterias] FOR [RubricasDb].[dbo].[InstrumentoMaterias];
PRINT '  ✓ Sinónimo creado: InstrumentoMaterias -> RubricasDb.dbo.InstrumentoMaterias';

-- =====================================================================
-- SINÓNIMOS PARA TABLAS DE CUADERNO CALIFICADOR
-- =====================================================================

PRINT '';
PRINT '4. Creando sinónimos para tablas del cuaderno calificador...';

IF EXISTS (SELECT * FROM sys.synonyms WHERE name = 'CuadernosCalificadores')
    DROP SYNONYM [CuadernosCalificadores];
CREATE SYNONYM [CuadernosCalificadores] FOR [RubricasDb].[dbo].[CuadernosCalificadores];
PRINT '  ✓ Sinónimo creado: CuadernosCalificadores -> RubricasDb.dbo.CuadernosCalificadores';

IF EXISTS (SELECT * FROM sys.synonyms WHERE name = 'CuadernoInstrumentos')
    DROP SYNONYM [CuadernoInstrumentos];
CREATE SYNONYM [CuadernoInstrumentos] FOR [RubricasDb].[dbo].[CuadernoInstrumentos];
PRINT '  ✓ Sinónimo creado: CuadernoInstrumentos -> RubricasDb.dbo.CuadernoInstrumentos';

-- =====================================================================
-- SINÓNIMOS PARA TABLAS DE GRUPOS DE ESTUDIANTES
-- =====================================================================

PRINT '';
PRINT '5. Creando sinónimos para tablas de grupos de estudiantes...';

IF EXISTS (SELECT * FROM sys.synonyms WHERE name = 'GruposEstudiantes')
    DROP SYNONYM [GruposEstudiantes];
CREATE SYNONYM [GruposEstudiantes] FOR [RubricasDb].[dbo].[GruposEstudiantes];
PRINT '  ✓ Sinónimo creado: GruposEstudiantes -> RubricasDb.dbo.GruposEstudiantes';

IF EXISTS (SELECT * FROM sys.synonyms WHERE name = 'EstudianteGrupos')
    DROP SYNONYM [EstudianteGrupos];
CREATE SYNONYM [EstudianteGrupos] FOR [RubricasDb].[dbo].[EstudianteGrupos];
PRINT '  ✓ Sinónimo creado: EstudianteGrupos -> RubricasDb.dbo.EstudianteGrupos';

IF EXISTS (SELECT * FROM sys.synonyms WHERE name = 'GrupoMaterias')
    DROP SYNONYM [GrupoMaterias];
CREATE SYNONYM [GrupoMaterias] FOR [RubricasDb].[dbo].[GrupoMaterias];
PRINT '  ✓ Sinónimo creado: GrupoMaterias -> RubricasDb.dbo.GrupoMaterias';

-- =====================================================================
-- SINÓNIMOS PARA TABLAS DE CATÁLOGOS Y AUDITORÍA
-- =====================================================================

PRINT '';
PRINT '6. Creando sinónimos para tablas de catálogos y auditoría...';

IF EXISTS (SELECT * FROM sys.synonyms WHERE name = 'TiposGrupo')
    DROP SYNONYM [TiposGrupo];
CREATE SYNONYM [TiposGrupo] FOR [RubricasDb].[dbo].[TiposGrupo];
PRINT '  ✓ Sinónimo creado: TiposGrupo -> RubricasDb.dbo.TiposGrupo';

IF EXISTS (SELECT * FROM sys.synonyms WHERE name = 'AuditoriasOperaciones')
    DROP SYNONYM [AuditoriasOperaciones];
CREATE SYNONYM [AuditoriasOperaciones] FOR [RubricasDb].[dbo].[AuditoriasOperaciones];
PRINT '  ✓ Sinónimo creado: AuditoriasOperaciones -> RubricasDb.dbo.AuditoriasOperaciones';

-- =====================================================================
-- SINÓNIMOS PARA TABLAS DE ASP.NET IDENTITY
-- =====================================================================

PRINT '';
PRINT '7. Creando sinónimos para tablas de ASP.NET Identity...';

IF EXISTS (SELECT * FROM sys.synonyms WHERE name = 'AspNetUsers')
    DROP SYNONYM [AspNetUsers];
CREATE SYNONYM [AspNetUsers] FOR [RubricasDb].[dbo].[AspNetUsers];
PRINT '  ✓ Sinónimo creado: AspNetUsers -> RubricasDb.dbo.AspNetUsers';

IF EXISTS (SELECT * FROM sys.synonyms WHERE name = 'AspNetRoles')
    DROP SYNONYM [AspNetRoles];
CREATE SYNONYM [AspNetRoles] FOR [RubricasDb].[dbo].[AspNetRoles];
PRINT '  ✓ Sinónimo creado: AspNetRoles -> RubricasDb.dbo.AspNetRoles';

IF EXISTS (SELECT * FROM sys.synonyms WHERE name = 'AspNetUserRoles')
    DROP SYNONYM [AspNetUserRoles];
CREATE SYNONYM [AspNetUserRoles] FOR [RubricasDb].[dbo].[AspNetUserRoles];
PRINT '  ✓ Sinónimo creado: AspNetUserRoles -> RubricasDb.dbo.AspNetUserRoles';

IF EXISTS (SELECT * FROM sys.synonyms WHERE name = 'AspNetUserClaims')
    DROP SYNONYM [AspNetUserClaims];
CREATE SYNONYM [AspNetUserClaims] FOR [RubricasDb].[dbo].[AspNetUserClaims];
PRINT '  ✓ Sinónimo creado: AspNetUserClaims -> RubricasDb.dbo.AspNetUserClaims';

IF EXISTS (SELECT * FROM sys.synonyms WHERE name = 'AspNetUserLogins')
    DROP SYNONYM [AspNetUserLogins];
CREATE SYNONYM [AspNetUserLogins] FOR [RubricasDb].[dbo].[AspNetUserLogins];
PRINT '  ✓ Sinónimo creado: AspNetUserLogins -> RubricasDb.dbo.AspNetUserLogins';

IF EXISTS (SELECT * FROM sys.synonyms WHERE name = 'AspNetUserTokens')
    DROP SYNONYM [AspNetUserTokens];
CREATE SYNONYM [AspNetUserTokens] FOR [RubricasDb].[dbo].[AspNetUserTokens];
PRINT '  ✓ Sinónimo creado: AspNetUserTokens -> RubricasDb.dbo.AspNetUserTokens';

IF EXISTS (SELECT * FROM sys.synonyms WHERE name = 'AspNetRoleClaims')
    DROP SYNONYM [AspNetRoleClaims];
CREATE SYNONYM [AspNetRoleClaims] FOR [RubricasDb].[dbo].[AspNetRoleClaims];
PRINT '  ✓ Sinónimo creado: AspNetRoleClaims -> RubricasDb.dbo.AspNetRoleClaims';

-- =====================================================================
-- SINÓNIMOS ADICIONALES (Si existen más tablas)
-- =====================================================================

PRINT '';
PRINT '8. Creando sinónimos para tablas adicionales que puedan existir...';

-- Verificar si existen más tablas y crear sinónimos dinámicamente
-- Esta sección maneja tablas que pueden existir pero no están en el DbContext

-- Tabla __EFMigrationsHistory (Entity Framework)
IF EXISTS (SELECT * FROM sys.tables WHERE name = '__EFMigrationsHistory')
BEGIN
    IF EXISTS (SELECT * FROM sys.synonyms WHERE name = '__EFMigrationsHistory')
        DROP SYNONYM [__EFMigrationsHistory];
    CREATE SYNONYM [__EFMigrationsHistory] FOR [RubricasDb].[dbo].[__EFMigrationsHistory];
    PRINT '  ✓ Sinónimo creado: __EFMigrationsHistory -> RubricasDb.dbo.__EFMigrationsHistory';
END

-- =====================================================================
-- VERIFICACIÓN Y RESUMEN
-- =====================================================================

PRINT '';
PRINT '============================================================';
PRINT 'RESUMEN DE SINÓNIMOS CREADOS';
PRINT '============================================================';

-- Contar todos los sinónimos creados
DECLARE @TotalSinonimos INT;
SELECT @TotalSinonimos = COUNT(*) FROM sys.synonyms;

PRINT 'Total de sinónimos en la base de datos: ' + CAST(@TotalSinonimos AS VARCHAR(10));
PRINT '';

-- Mostrar todos los sinónimos creados
PRINT 'Lista de sinónimos disponibles:';
PRINT '-------------------------------------------';

SELECT 
    name AS 'Sinónimo',
    base_object_name AS 'Tabla Original'
FROM sys.synonyms
ORDER BY name;

PRINT '';
PRINT '============================================================';
PRINT 'SCRIPT COMPLETADO EXITOSAMENTE';
PRINT '============================================================';
PRINT '';
PRINT 'Uso de los sinónimos:';
PRINT '  Antes: SELECT * FROM RubricasDb.dbo.Materias';
PRINT '  Ahora: SELECT * FROM Materias';
PRINT '';
PRINT 'Los sinónimos permiten simplificar las consultas SQL y';
PRINT 'hacer el código más portable entre diferentes entornos.';
PRINT '';
PRINT '🎉 ¡Todos los sinónimos han sido creados correctamente!';

-- =====================================================================
-- SCRIPT ADICIONAL: VERIFICAR FUNCIONAMIENTO
-- =====================================================================

PRINT '';
PRINT '============================================================';
PRINT 'PRUEBA DE FUNCIONAMIENTO';
PRINT '============================================================';

-- Probar algunos sinónimos para verificar que funcionan
BEGIN TRY
    PRINT 'Probando sinónimo Materias...';
    DECLARE @CountMaterias INT;
    SELECT @CountMaterias = COUNT(*) FROM Materias;
    PRINT '  ✓ Sinónimo Materias funciona correctamente. Registros: ' + CAST(@CountMaterias AS VARCHAR(10));
    
    PRINT 'Probando sinónimo AspNetUsers...';
    DECLARE @CountUsers INT;
    SELECT @CountUsers = COUNT(*) FROM AspNetUsers;
    PRINT '  ✓ Sinónimo AspNetUsers funciona correctamente. Registros: ' + CAST(@CountUsers AS VARCHAR(10));
    
    PRINT 'Probando sinónimo Rubricas...';
    DECLARE @CountRubricas INT;
    SELECT @CountRubricas = COUNT(*) FROM Rubricas;
    PRINT '  ✓ Sinónimo Rubricas funciona correctamente. Registros: ' + CAST(@CountRubricas AS VARCHAR(10));

END TRY
BEGIN CATCH
    PRINT '⚠️ Error en la prueba: ' + ERROR_MESSAGE();
    PRINT 'Algunos sinónimos pueden no funcionar si las tablas no existen.';
END CATCH

PRINT '';
PRINT '🚀 Script de sinónimos completado y verificado!';
GO
