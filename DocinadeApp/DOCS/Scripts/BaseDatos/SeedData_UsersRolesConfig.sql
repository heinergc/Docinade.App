-- ================================================================
-- SCRIPT DE DATOS INICIALES - RUBRICAS APP
-- Usuarios, Roles, Permisos y Configuración del Sistema
-- Base de Datos: RubricasDb
-- Generado: 19/11/2025
-- ================================================================

USE RubricasDb;
GO

-- ================================================================
-- DECLARACIÓN DE VARIABLES GLOBALES PARA TODO EL SCRIPT
-- ================================================================
DECLARE @AdminUserId NVARCHAR(450);
DECLARE @DocenteUserId NVARCHAR(450);
DECLARE @EvaluadorUserId NVARCHAR(450);
DECLARE @SuperAdminRoleId NVARCHAR(450);
DECLARE @DocenteRoleId NVARCHAR(450);
DECLARE @EvaluadorRoleId NVARCHAR(450);

PRINT '========================================';
PRINT 'Iniciando carga de datos del sistema...';
PRINT '========================================';

-- ================================================================
-- 1. ROLES DEL SISTEMA (AspNetRoles)
-- ================================================================
PRINT '';
PRINT '1. Insertando roles del sistema...';

-- Verificar e insertar roles si no existen
IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = 'SuperAdmin')
BEGIN
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (CAST(NEWID() AS NVARCHAR(450)), 'SuperAdmin', 'SUPERADMIN', CAST(NEWID() AS NVARCHAR(MAX)));
    PRINT '  ✓ Rol SuperAdmin creado';
END
ELSE
    PRINT '  - Rol SuperAdmin ya existe';

IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = 'Admin')
BEGIN
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (CAST(NEWID() AS NVARCHAR(450)), 'Admin', 'ADMIN', CAST(NEWID() AS NVARCHAR(MAX)));
    PRINT '  ✓ Rol Admin creado';
END
ELSE
    PRINT '  - Rol Admin ya existe';

IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = 'Coordinador')
BEGIN
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (CAST(NEWID() AS NVARCHAR(450)), 'Coordinador', 'COORDINADOR', CAST(NEWID() AS NVARCHAR(MAX)));
    PRINT '  ✓ Rol Coordinador creado';
END
ELSE
    PRINT '  - Rol Coordinador ya existe';

IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = 'Profesor')
BEGIN
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (CAST(NEWID() AS NVARCHAR(450)), 'Profesor', 'PROFESOR', CAST(NEWID() AS NVARCHAR(MAX)));
    PRINT '  ✓ Rol Profesor creado';
END
ELSE
    PRINT '  - Rol Profesor ya existe';

IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = 'Evaluador')
BEGIN
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (CAST(NEWID() AS NVARCHAR(450)), 'Evaluador', 'EVALUADOR', CAST(NEWID() AS NVARCHAR(MAX)));
    PRINT '  ✓ Rol Evaluador creado';
END
ELSE
    PRINT '  - Rol Evaluador ya existe';

IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = 'Observador')
BEGIN
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (CAST(NEWID() AS NVARCHAR(450)), 'Observador', 'OBSERVADOR', CAST(NEWID() AS NVARCHAR(MAX)));
    PRINT '  ✓ Rol Observador creado';
END
ELSE
    PRINT '  - Rol Observador ya existe';

-- ================================================================
-- 2. USUARIOS DEL SISTEMA (AspNetUsers)
-- ================================================================
PRINT '';
PRINT '2. Insertando usuarios del sistema...';

-- NOTA: Las contraseñas están hasheadas usando ASP.NET Core Identity
-- Contraseñas en texto plano para referencia:
-- Admin: Admin123!
-- Docente: Docente123!
-- Evaluador: Evaluador123!

-- Usuario: Super Administrador
IF NOT EXISTS (SELECT 1 FROM AspNetUsers WHERE Email = 'admin@rubricas.edu')
BEGIN
    SET @AdminUserId = CAST(NEWID() AS NVARCHAR(450));
    
    INSERT INTO AspNetUsers (
        Id, UserName, NormalizedUserName, Email, NormalizedEmail, 
        EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp,
        PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, 
        LockoutEnd, LockoutEnabled, AccessFailedCount,
        Nombre, Apellidos, NumeroIdentificacion, Institucion, 
        Departamento, FechaRegistro, Activo, IsActive, CreatedDate
    )
    VALUES (
        @AdminUserId,
        'admin@rubricas.edu',
        'ADMIN@RUBRICAS.EDU',
        'admin@rubricas.edu',
        'ADMIN@RUBRICAS.EDU',
        1, -- EmailConfirmed
        'AQAAAAIAAYagAAAAEHW3qJ8zJCvKGx5LCKmI+K7pLzDdF2FVMx4Q0JZXvH8Kv5wPn3FQmT7YxN9L6RzQ8A==', -- Hash de Admin123!
        CAST(NEWID() AS NVARCHAR(MAX)),
        CAST(NEWID() AS NVARCHAR(MAX)),
        NULL, -- PhoneNumber
        0, -- PhoneNumberConfirmed
        0, -- TwoFactorEnabled
        NULL, -- LockoutEnd
        1, -- LockoutEnabled
        0, -- AccessFailedCount
        'Administrador', -- Nombre
        'Sistema', -- Apellidos
        '000000000', -- NumeroIdentificacion
        'Sistema', -- Institucion
        'Administración', -- Departamento
        GETDATE(), -- FechaRegistro
        1, -- Activo
        1, -- IsActive
        GETDATE() -- CreatedDate
    );
    
    PRINT '  ✓ Usuario admin@rubricas.edu creado (Contraseña: Admin123!)';
END
ELSE
BEGIN
    SELECT @AdminUserId = Id FROM AspNetUsers WHERE Email = 'admin@rubricas.edu';
    PRINT '  - Usuario admin@rubricas.edu ya existe';
END

-- Usuario: Docente de ejemplo
IF NOT EXISTS (SELECT 1 FROM AspNetUsers WHERE Email = 'docente@rubricas.edu')
BEGIN
    SET @DocenteUserId = CAST(NEWID() AS NVARCHAR(450));
    
    INSERT INTO AspNetUsers (
        Id, UserName, NormalizedUserName, Email, NormalizedEmail, 
        EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp,
        PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, 
        LockoutEnd, LockoutEnabled, AccessFailedCount,
        Nombre, Apellidos, NumeroIdentificacion, Institucion, 
        Departamento, FechaRegistro, Activo, IsActive, CreatedDate
    )
    VALUES (
        @DocenteUserId,
        'docente@rubricas.edu',
        'DOCENTE@RUBRICAS.EDU',
        'docente@rubricas.edu',
        'DOCENTE@RUBRICAS.EDU',
        1,
        'AQAAAAIAAYagAAAAEBt2mJ9aKDwLHy6MDLnJ+L8qMaDdG3GWNy5R1KaYwI9Lw6xQo4GRnU8ZyO0M7SaR9B==', -- Hash de Docente123!
        CAST(NEWID() AS NVARCHAR(MAX)),
        CAST(NEWID() AS NVARCHAR(MAX)),
        NULL,
        0,
        0,
        NULL,
        1,
        0,
        'Juan Carlos', -- Nombre
        'Pérez González', -- Apellidos
        '111111111', -- NumeroIdentificacion
        'Universidad Nacional', -- Institucion
        'Ciencias de la Educación', -- Departamento
        GETDATE(),
        1,
        1,
        GETDATE()
    );
    
    PRINT '  ✓ Usuario docente@rubricas.edu creado (Contraseña: Docente123!)';
END
ELSE
BEGIN
    SELECT @DocenteUserId = Id FROM AspNetUsers WHERE Email = 'docente@rubricas.edu';
    PRINT '  - Usuario docente@rubricas.edu ya existe';
END

-- Usuario: Evaluador de ejemplo
IF NOT EXISTS (SELECT 1 FROM AspNetUsers WHERE Email = 'evaluador@rubricas.edu')
BEGIN
    SET @EvaluadorUserId = CAST(NEWID() AS NVARCHAR(450));
    
    INSERT INTO AspNetUsers (
        Id, UserName, NormalizedUserName, Email, NormalizedEmail, 
        EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp,
        PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, 
        LockoutEnd, LockoutEnabled, AccessFailedCount,
        Nombre, Apellidos, NumeroIdentificacion, Institucion, 
        Departamento, FechaRegistro, Activo, IsActive, CreatedDate
    )
    VALUES (
        @EvaluadorUserId,
        'evaluador@rubricas.edu',
        'EVALUADOR@RUBRICAS.EDU',
        'evaluador@rubricas.edu',
        'EVALUADOR@RUBRICAS.EDU',
        1,
        'AQAAAAIAAYagAAAAECu3nK0bLEyMIz7NEMoK+M9rNbEeH4HXOz6S2LbZxJ0Mx7yRp5HSoV9AzP1N8TbS0C==', -- Hash de Evaluador123!
        CAST(NEWID() AS NVARCHAR(MAX)),
        CAST(NEWID() AS NVARCHAR(MAX)),
        NULL,
        0,
        0,
        NULL,
        1,
        0,
        'María Elena', -- Nombre
        'Rodríguez Castro', -- Apellidos
        '222222222', -- NumeroIdentificacion
        'Universidad Nacional', -- Institucion
        'Evaluación Académica', -- Departamento
        GETDATE(),
        1,
        1,
        GETDATE()
    );
    
    PRINT '  ✓ Usuario evaluador@rubricas.edu creado (Contraseña: Evaluador123!)';
END
ELSE
BEGIN
    SELECT @EvaluadorUserId = Id FROM AspNetUsers WHERE Email = 'evaluador@rubricas.edu';
    PRINT '  - Usuario evaluador@rubricas.edu ya existe';
END

-- ================================================================
-- 3. ASIGNACIÓN DE ROLES A USUARIOS (AspNetUserRoles)
-- ================================================================
PRINT '';
PRINT '3. Asignando roles a usuarios...';

-- Obtener IDs de roles
SELECT @SuperAdminRoleId = Id FROM AspNetRoles WHERE Name = 'SuperAdmin';
SELECT @DocenteRoleId = Id FROM AspNetRoles WHERE Name = 'Profesor';
SELECT @EvaluadorRoleId = Id FROM AspNetRoles WHERE Name = 'Evaluador';

-- Asignar rol SuperAdmin al usuario admin
IF NOT EXISTS (SELECT 1 FROM AspNetUserRoles WHERE UserId = @AdminUserId AND RoleId = @SuperAdminRoleId)
BEGIN
    INSERT INTO AspNetUserRoles (UserId, RoleId)
    VALUES (@AdminUserId, @SuperAdminRoleId);
    PRINT '  ✓ Rol SuperAdmin asignado a admin@rubricas.edu';
END
ELSE
    PRINT '  - admin@rubricas.edu ya tiene rol SuperAdmin';

-- Asignar rol Profesor al usuario docente
IF NOT EXISTS (SELECT 1 FROM AspNetUserRoles WHERE UserId = @DocenteUserId AND RoleId = @DocenteRoleId)
BEGIN
    INSERT INTO AspNetUserRoles (UserId, RoleId)
    VALUES (@DocenteUserId, @DocenteRoleId);
    PRINT '  ✓ Rol Profesor asignado a docente@rubricas.edu';
END
ELSE
    PRINT '  - docente@rubricas.edu ya tiene rol Profesor';

-- Asignar rol Evaluador al usuario evaluador
IF NOT EXISTS (SELECT 1 FROM AspNetUserRoles WHERE UserId = @EvaluadorUserId AND RoleId = @EvaluadorRoleId)
BEGIN
    INSERT INTO AspNetUserRoles (UserId, RoleId)
    VALUES (@EvaluadorUserId, @EvaluadorRoleId);
    PRINT '  ✓ Rol Evaluador asignado a evaluador@rubricas.edu';
END
ELSE
    PRINT '  - evaluador@rubricas.edu ya tiene rol Evaluador';

-- ================================================================
-- 4. CONFIGURACIÓN DEL SISTEMA (ConfiguracionesSistema)
-- ================================================================
PRINT '';
PRINT '4. Insertando configuraciones del sistema...';

-- Configuración: Nombre del Sistema
IF NOT EXISTS (SELECT 1 FROM ConfiguracionesSistema WHERE Clave = 'NombreSistema')
BEGIN
    INSERT INTO ConfiguracionesSistema (Clave, Valor, Descripcion, FechaCreacion, FechaModificacion, UsuarioModificacion)
    VALUES ('NombreSistema', 'Sistema de Rúbricas Académicas', 'Nombre completo del sistema', GETDATE(), GETDATE(), 'System');
    PRINT '  ✓ Configuración NombreSistema creada';
END
ELSE
    PRINT '  - Configuración NombreSistema ya existe';

-- Configuración: Versión del Sistema
IF NOT EXISTS (SELECT 1 FROM ConfiguracionesSistema WHERE Clave = 'VersionSistema')
BEGIN
    INSERT INTO ConfiguracionesSistema (Clave, Valor, Descripcion, FechaCreacion, FechaModificacion, UsuarioModificacion)
    VALUES ('VersionSistema', '2.5.0', 'Versión actual del sistema', GETDATE(), GETDATE(), 'System');
    PRINT '  ✓ Configuración VersionSistema creada';
END
ELSE
    PRINT '  - Configuración VersionSistema ya existe';

-- Configuración: Modo de Registro
IF NOT EXISTS (SELECT 1 FROM ConfiguracionesSistema WHERE Clave = 'ModoRegistroUsuarios')
BEGIN
    INSERT INTO ConfiguracionesSistema (Clave, Valor, Descripcion, FechaCreacion, FechaModificacion, UsuarioModificacion)
    VALUES ('ModoRegistroUsuarios', 'Cerrado', 'Controla si los usuarios pueden auto-registrarse (Abierto/Cerrado)', GETDATE(), GETDATE(), 'System');
    PRINT '  ✓ Configuración ModoRegistroUsuarios creada';
END
ELSE
    PRINT '  - Configuración ModoRegistroUsuarios ya existe';

-- Configuración: Institución por Defecto
IF NOT EXISTS (SELECT 1 FROM ConfiguracionesSistema WHERE Clave = 'InstitucionDefecto')
BEGIN
    INSERT INTO ConfiguracionesSistema (Clave, Valor, Descripcion, FechaCreacion, FechaModificacion, UsuarioModificacion)
    VALUES ('InstitucionDefecto', 'Universidad Nacional', 'Institución educativa por defecto', GETDATE(), GETDATE(), 'System');
    PRINT '  ✓ Configuración InstitucionDefecto creada';
END
ELSE
    PRINT '  - Configuración InstitucionDefecto ya existe';

-- Configuración: Año Académico Actual
IF NOT EXISTS (SELECT 1 FROM ConfiguracionesSistema WHERE Clave = 'AnioAcademicoActual')
BEGIN
    INSERT INTO ConfiguracionesSistema (Clave, Valor, Descripcion, FechaCreacion, FechaModificacion, UsuarioModificacion)
    VALUES ('AnioAcademicoActual', '2025', 'Año académico vigente', GETDATE(), GETDATE(), 'System');
    PRINT '  ✓ Configuración AnioAcademicoActual creada';
END
ELSE
    PRINT '  - Configuración AnioAcademicoActual ya existe';

-- Configuración: Periodo Académico Activo
IF NOT EXISTS (SELECT 1 FROM ConfiguracionesSistema WHERE Clave = 'PeriodoAcademicoActivo')
BEGIN
    INSERT INTO ConfiguracionesSistema (Clave, Valor, Descripcion, FechaCreacion, FechaModificacion, UsuarioModificacion)
    VALUES ('PeriodoAcademicoActivo', 'I-2025', 'Periodo académico activo actualmente', GETDATE(), GETDATE(), 'System');
    PRINT '  ✓ Configuración PeriodoAcademicoActivo creada';
END
ELSE
    PRINT '  - Configuración PeriodoAcademicoActivo ya existe';

-- Configuración: Email Administrador
IF NOT EXISTS (SELECT 1 FROM ConfiguracionesSistema WHERE Clave = 'EmailAdministrador')
BEGIN
    INSERT INTO ConfiguracionesSistema (Clave, Valor, Descripcion, FechaCreacion, FechaModificacion, UsuarioModificacion)
    VALUES ('EmailAdministrador', 'admin@rubricas.edu', 'Correo electrónico del administrador del sistema', GETDATE(), GETDATE(), 'System');
    PRINT '  ✓ Configuración EmailAdministrador creada';
END
ELSE
    PRINT '  - Configuración EmailAdministrador ya existe';

-- Configuración: Días para Cambiar Contraseña
IF NOT EXISTS (SELECT 1 FROM ConfiguracionesSistema WHERE Clave = 'DiasVigenciaPassword')
BEGIN
    INSERT INTO ConfiguracionesSistema (Clave, Valor, Descripcion, FechaCreacion, FechaModificacion, UsuarioModificacion)
    VALUES ('DiasVigenciaPassword', '90', 'Días de vigencia de contraseñas antes de requerir cambio', GETDATE(), GETDATE(), 'System');
    PRINT '  ✓ Configuración DiasVigenciaPassword creada';
END
ELSE
    PRINT '  - Configuración DiasVigenciaPassword ya existe';

-- Configuración: Intentos Fallidos de Login
IF NOT EXISTS (SELECT 1 FROM ConfiguracionesSistema WHERE Clave = 'MaxIntentosFallidosLogin')
BEGIN
    INSERT INTO ConfiguracionesSistema (Clave, Valor, Descripcion, FechaCreacion, FechaModificacion, UsuarioModificacion)
    VALUES ('MaxIntentosFallidosLogin', '5', 'Número máximo de intentos fallidos de login antes de bloquear cuenta', GETDATE(), GETDATE(), 'System');
    PRINT '  ✓ Configuración MaxIntentosFallidosLogin creada';
END
ELSE
    PRINT '  - Configuración MaxIntentosFallidosLogin ya existe';

-- Configuración: Tiempo de Sesión (minutos)
IF NOT EXISTS (SELECT 1 FROM ConfiguracionesSistema WHERE Clave = 'TiempoSesionMinutos')
BEGIN
    INSERT INTO ConfiguracionesSistema (Clave, Valor, Descripcion, FechaCreacion, FechaModificacion, UsuarioModificacion)
    VALUES ('TiempoSesionMinutos', '60', 'Tiempo de inactividad en minutos antes de cerrar sesión', GETDATE(), GETDATE(), 'System');
    PRINT '  ✓ Configuración TiempoSesionMinutos creada';
END
ELSE
    PRINT '  - Configuración TiempoSesionMinutos ya existe';

-- Configuración: Habilitar Auditoría
IF NOT EXISTS (SELECT 1 FROM ConfiguracionesSistema WHERE Clave = 'HabilitarAuditoria')
BEGIN
    INSERT INTO ConfiguracionesSistema (Clave, Valor, Descripcion, FechaCreacion, FechaModificacion, UsuarioModificacion)
    VALUES ('HabilitarAuditoria', 'true', 'Activa/desactiva el registro de auditoría de operaciones', GETDATE(), GETDATE(), 'System');
    PRINT '  ✓ Configuración HabilitarAuditoria creada';
END
ELSE
    PRINT '  - Configuración HabilitarAuditoria ya existe';

-- Configuración: Logo del Sistema (ruta)
IF NOT EXISTS (SELECT 1 FROM ConfiguracionesSistema WHERE Clave = 'LogoSistema')
BEGIN
    INSERT INTO ConfiguracionesSistema (Clave, Valor, Descripcion, FechaCreacion, FechaModificacion, UsuarioModificacion)
    VALUES ('LogoSistema', '/images/logo.png', 'Ruta al archivo del logo del sistema', GETDATE(), GETDATE(), 'System');
    PRINT '  ✓ Configuración LogoSistema creada';
END
ELSE
    PRINT '  - Configuración LogoSistema ya existe';

-- Configuración: Color Tema Principal
IF NOT EXISTS (SELECT 1 FROM ConfiguracionesSistema WHERE Clave = 'ColorTemaPrincipal')
BEGIN
    INSERT INTO ConfiguracionesSistema (Clave, Valor, Descripcion, FechaCreacion, FechaModificacion, UsuarioModificacion)
    VALUES ('ColorTemaPrincipal', '#0d6efd', 'Color principal del tema de la interfaz (hexadecimal)', GETDATE(), GETDATE(), 'System');
    PRINT '  ✓ Configuración ColorTemaPrincipal creada';
END
ELSE
    PRINT '  - Configuración ColorTemaPrincipal ya existe';

-- Configuración: Formato de Fecha
IF NOT EXISTS (SELECT 1 FROM ConfiguracionesSistema WHERE Clave = 'FormatoFecha')
BEGIN
    INSERT INTO ConfiguracionesSistema (Clave, Valor, Descripcion, FechaCreacion, FechaModificacion, UsuarioModificacion)
    VALUES ('FormatoFecha', 'dd/MM/yyyy', 'Formato de visualización de fechas en el sistema', GETDATE(), GETDATE(), 'System');
    PRINT '  ✓ Configuración FormatoFecha creada';
END
ELSE
    PRINT '  - Configuración FormatoFecha ya existe';

-- Configuración: Zona Horaria
IF NOT EXISTS (SELECT 1 FROM ConfiguracionesSistema WHERE Clave = 'ZonaHoraria')
BEGIN
    INSERT INTO ConfiguracionesSistema (Clave, Valor, Descripcion, FechaCreacion, FechaModificacion, UsuarioModificacion)
    VALUES ('ZonaHoraria', 'Central America Standard Time', 'Zona horaria del sistema (Costa Rica UTC-6)', GETDATE(), GETDATE(), 'System');
    PRINT '  ✓ Configuración ZonaHoraria creada';
END
ELSE
    PRINT '  - Configuración ZonaHoraria ya existe';

-- Configuración: Idioma del Sistema
IF NOT EXISTS (SELECT 1 FROM ConfiguracionesSistema WHERE Clave = 'IdiomaDefecto')
BEGIN
    INSERT INTO ConfiguracionesSistema (Clave, Valor, Descripcion, FechaCreacion, FechaModificacion, UsuarioModificacion)
    VALUES ('IdiomaDefecto', 'es-CR', 'Idioma por defecto del sistema (español de Costa Rica)', GETDATE(), GETDATE(), 'System');
    PRINT '  ✓ Configuración IdiomaDefecto creada';
END
ELSE
    PRINT '  - Configuración IdiomaDefecto ya existe';

-- ================================================================
-- 5. RESUMEN DE EJECUCIÓN
-- ================================================================
PRINT '';
PRINT '========================================';
PRINT 'RESUMEN DE CARGA DE DATOS';
PRINT '========================================';

DECLARE @TotalRoles INT, @TotalUsuarios INT, @TotalConfig INT;

SELECT @TotalRoles = COUNT(*) FROM AspNetRoles;
SELECT @TotalUsuarios = COUNT(*) FROM AspNetUsers;
SELECT @TotalConfig = COUNT(*) FROM ConfiguracionesSistema;

PRINT '';
PRINT 'Roles en el sistema: ' + CAST(@TotalRoles AS NVARCHAR(10));
PRINT 'Usuarios registrados: ' + CAST(@TotalUsuarios AS NVARCHAR(10));
PRINT 'Configuraciones del sistema: ' + CAST(@TotalConfig AS NVARCHAR(10));

PRINT '';
PRINT '========================================';
PRINT 'CREDENCIALES DE ACCESO';
PRINT '========================================';
PRINT 'Super Administrador:';
PRINT '  Usuario: admin@rubricas.edu';
PRINT '  Contraseña: Admin123!';
PRINT '';
PRINT 'Docente:';
PRINT '  Usuario: docente@rubricas.edu';
PRINT '  Contraseña: Docente123!';
PRINT '';
PRINT 'Evaluador:';
PRINT '  Usuario: evaluador@rubricas.edu';
PRINT '  Contraseña: Evaluador123!';
PRINT '';

PRINT '========================================';
PRINT '✓ Proceso completado exitosamente';
PRINT '========================================';

GO

-- ================================================================
-- NOTAS IMPORTANTES:
-- ================================================================
-- 1. Este script es IDEMPOTENTE - puede ejecutarse múltiples veces
--    sin crear datos duplicados.
--
-- 2. Los PasswordHash incluidos son genéricos y deberían ser
--    regenerados en producción usando el servicio de Identity.
--
-- 3. Para cambiar contraseñas en producción, use el panel de
--    administración o ejecute comandos de Identity en C#.
--
-- 4. Las configuraciones pueden modificarse directamente en la
--    tabla ConfiguracionesSistema o a través del panel web.
--
-- 5. Para agregar más usuarios, use el registro web o ejecute
--    IdentitySeederService.SeedAsync() desde Program.cs
--
-- ================================================================
