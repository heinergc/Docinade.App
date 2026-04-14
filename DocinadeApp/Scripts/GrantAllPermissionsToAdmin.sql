-- =====================================================================================
-- Script SQL para otorgar todos los permisos al usuario admin@rubricas.edu
-- Sistema de Rúbricas Académicas
-- Fecha: 29 de julio de 2025
-- =====================================================================================

-- Verificar si el usuario existe
DECLARE @UserId NVARCHAR(450);
DECLARE @SuperAdminRoleId NVARCHAR(450);
DECLARE @AdminRoleId NVARCHAR(450);

-- Buscar el usuario admin@rubricas.edu
SELECT @UserId = Id FROM AspNetUsers WHERE UserName = 'admin@rubricas.edu' OR Email = 'admin@rubricas.edu';

IF @UserId IS NULL
BEGIN
    PRINT 'ERROR: Usuario admin@rubricas.edu no encontrado';
    PRINT 'Debe crear el usuario primero antes de ejecutar este script';
    RETURN;
END

PRINT 'Usuario encontrado: ' + @UserId;

-- =====================================================================================
-- PASO 1: Crear rol SuperAdmin si no existe
-- =====================================================================================

SELECT @SuperAdminRoleId = Id FROM AspNetRoles WHERE Name = 'SuperAdmin';

IF @SuperAdminRoleId IS NULL
BEGIN
    SET @SuperAdminRoleId = NEWID();
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (@SuperAdminRoleId, 'SuperAdmin', 'SUPERADMIN', NEWID());
    PRINT 'Rol SuperAdmin creado: ' + @SuperAdminRoleId;
END
ELSE
BEGIN
    PRINT 'Rol SuperAdmin ya existe: ' + @SuperAdminRoleId;
END

-- =====================================================================================
-- PASO 2: Buscar/Crear rol Admin
-- =====================================================================================

SELECT @AdminRoleId = Id FROM AspNetRoles WHERE Name = 'Admin';

IF @AdminRoleId IS NULL
BEGIN
    SET @AdminRoleId = NEWID();
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (@AdminRoleId, 'Admin', 'ADMIN', NEWID());
    PRINT 'Rol Admin creado: ' + @AdminRoleId;
END
ELSE
BEGIN
    PRINT 'Rol Admin ya existe: ' + @AdminRoleId;
END

-- =====================================================================================
-- PASO 3: Asignar roles al usuario (si no los tiene)
-- =====================================================================================

-- Asignar rol SuperAdmin al usuario
IF NOT EXISTS (SELECT 1 FROM AspNetUserRoles WHERE UserId = @UserId AND RoleId = @SuperAdminRoleId)
BEGIN
    INSERT INTO AspNetUserRoles (UserId, RoleId)
    VALUES (@UserId, @SuperAdminRoleId);
    PRINT 'Rol SuperAdmin asignado al usuario';
END
ELSE
BEGIN
    PRINT 'Usuario ya tiene rol SuperAdmin';
END

-- Asignar rol Admin al usuario  
IF NOT EXISTS (SELECT 1 FROM AspNetUserRoles WHERE UserId = @UserId AND RoleId = @AdminRoleId)
BEGIN
    INSERT INTO AspNetUserRoles (UserId, RoleId)
    VALUES (@UserId, @AdminRoleId);
    PRINT 'Rol Admin asignado al usuario';
END
ELSE
BEGIN
    PRINT 'Usuario ya tiene rol Admin';
END

-- =====================================================================================
-- PASO 4: Asignar todos los permisos individuales al usuario
-- =====================================================================================

PRINT 'Iniciando asignación de permisos individuales...';

-- Crear tabla temporal con todos los permisos del sistema
CREATE TABLE #TempPermisos (
    Permiso NVARCHAR(100),
    Categoria NVARCHAR(50),
    Descripcion NVARCHAR(500)
);

-- PERMISOS DE USUARIOS
INSERT INTO #TempPermisos VALUES ('usuarios.ver', 'Usuarios', 'Ver usuarios del sistema');
INSERT INTO #TempPermisos VALUES ('usuarios.crear', 'Usuarios', 'Crear nuevos usuarios');
INSERT INTO #TempPermisos VALUES ('usuarios.editar', 'Usuarios', 'Editar información de usuarios');
INSERT INTO #TempPermisos VALUES ('usuarios.eliminar', 'Usuarios', 'Eliminar usuarios del sistema');
INSERT INTO #TempPermisos VALUES ('usuarios.bloquear', 'Usuarios', 'Bloquear/desbloquear usuarios');
INSERT INTO #TempPermisos VALUES ('usuarios.cambiar_roles', 'Usuarios', 'Asignar y quitar roles a usuarios');
INSERT INTO #TempPermisos VALUES ('usuarios.ver_perfil_completo', 'Usuarios', 'Ver información detallada de usuarios');
INSERT INTO #TempPermisos VALUES ('usuarios.exportar', 'Usuarios', 'Exportar lista de usuarios');
INSERT INTO #TempPermisos VALUES ('usuarios.importar', 'Usuarios', 'Importar usuarios desde archivos');
INSERT INTO #TempPermisos VALUES ('usuarios.gestionar_roles', 'Usuarios', 'Gestionar roles de usuarios');
INSERT INTO #TempPermisos VALUES ('usuarios.gestionar_permisos', 'Usuarios', 'Gestionar permisos de usuarios');
INSERT INTO #TempPermisos VALUES ('usuarios.restablecer_contraseña', 'Usuarios', 'Restablecer contraseñas de usuarios');

-- PERMISOS DE RUBRICAS
INSERT INTO #TempPermisos VALUES ('rubricas.ver', 'Rúbricas', 'Ver rúbricas propias');
INSERT INTO #TempPermisos VALUES ('rubricas.ver_todas', 'Rúbricas', 'Ver todas las rúbricas del sistema');
INSERT INTO #TempPermisos VALUES ('rubricas.crear', 'Rúbricas', 'Crear nuevas rúbricas');
INSERT INTO #TempPermisos VALUES ('rubricas.editar', 'Rúbricas', 'Editar rúbricas propias');
INSERT INTO #TempPermisos VALUES ('rubricas.editar_todas', 'Rúbricas', 'Editar cualquier rúbrica');
INSERT INTO #TempPermisos VALUES ('rubricas.eliminar', 'Rúbricas', 'Eliminar rúbricas propias');
INSERT INTO #TempPermisos VALUES ('rubricas.eliminar_todas', 'Rúbricas', 'Eliminar cualquier rúbrica');
INSERT INTO #TempPermisos VALUES ('rubricas.duplicar', 'Rúbricas', 'Duplicar rúbricas');
INSERT INTO #TempPermisos VALUES ('rubricas.publicar', 'Rúbricas', 'Publicar rúbricas');
INSERT INTO #TempPermisos VALUES ('rubricas.archivar', 'Rúbricas', 'Archivar rúbricas');
INSERT INTO #TempPermisos VALUES ('rubricas.compartir', 'Rúbricas', 'Compartir rúbricas con otros usuarios');
INSERT INTO #TempPermisos VALUES ('rubricas.exportar', 'Rúbricas', 'Exportar rúbricas');
INSERT INTO #TempPermisos VALUES ('rubricas.importar', 'Rúbricas', 'Importar rúbricas');

-- PERMISOS DE EVALUACIONES
INSERT INTO #TempPermisos VALUES ('evaluaciones.ver', 'Evaluaciones', 'Ver evaluaciones propias');
INSERT INTO #TempPermisos VALUES ('evaluaciones.ver_todas', 'Evaluaciones', 'Ver todas las evaluaciones');
INSERT INTO #TempPermisos VALUES ('evaluaciones.crear', 'Evaluaciones', 'Crear nuevas evaluaciones');
INSERT INTO #TempPermisos VALUES ('evaluaciones.editar', 'Evaluaciones', 'Editar evaluaciones propias');
INSERT INTO #TempPermisos VALUES ('evaluaciones.editar_todas', 'Evaluaciones', 'Editar cualquier evaluación');
INSERT INTO #TempPermisos VALUES ('evaluaciones.eliminar', 'Evaluaciones', 'Eliminar evaluaciones propias');
INSERT INTO #TempPermisos VALUES ('evaluaciones.eliminar_todas', 'Evaluaciones', 'Eliminar cualquier evaluación');
INSERT INTO #TempPermisos VALUES ('evaluaciones.evaluar', 'Evaluaciones', 'Realizar evaluaciones');
INSERT INTO #TempPermisos VALUES ('evaluaciones.revisar', 'Evaluaciones', 'Revisar evaluaciones');
INSERT INTO #TempPermisos VALUES ('evaluaciones.aprobar', 'Evaluaciones', 'Aprobar evaluaciones');
INSERT INTO #TempPermisos VALUES ('evaluaciones.finalizar', 'Evaluaciones', 'Finalizar evaluaciones');
INSERT INTO #TempPermisos VALUES ('evaluaciones.reabrir', 'Evaluaciones', 'Reabrir evaluaciones finalizadas');
INSERT INTO #TempPermisos VALUES ('evaluaciones.exportar', 'Evaluaciones', 'Exportar evaluaciones');
INSERT INTO #TempPermisos VALUES ('evaluaciones.ver_resultados', 'Evaluaciones', 'Ver resultados de evaluaciones');
INSERT INTO #TempPermisos VALUES ('evaluaciones.ver_estadisticas', 'Evaluaciones', 'Ver estadísticas de evaluaciones');

-- PERMISOS DE ESTUDIANTES
INSERT INTO #TempPermisos VALUES ('estudiantes.ver', 'Estudiantes', 'Ver lista de estudiantes');
INSERT INTO #TempPermisos VALUES ('estudiantes.crear', 'Estudiantes', 'Crear nuevos estudiantes');
INSERT INTO #TempPermisos VALUES ('estudiantes.editar', 'Estudiantes', 'Editar información de estudiantes');
INSERT INTO #TempPermisos VALUES ('estudiantes.eliminar', 'Estudiantes', 'Eliminar estudiantes');
INSERT INTO #TempPermisos VALUES ('estudiantes.importar', 'Estudiantes', 'Importar estudiantes desde archivos');
INSERT INTO #TempPermisos VALUES ('estudiantes.exportar', 'Estudiantes', 'Exportar lista de estudiantes');
INSERT INTO #TempPermisos VALUES ('estudiantes.ver_historial', 'Estudiantes', 'Ver historial académico de estudiantes');
INSERT INTO #TempPermisos VALUES ('estudiantes.ver_notas', 'Estudiantes', 'Ver calificaciones de estudiantes');
INSERT INTO #TempPermisos VALUES ('estudiantes.editar_notas', 'Estudiantes', 'Modificar calificaciones de estudiantes');

-- PERMISOS DE REPORTES
INSERT INTO #TempPermisos VALUES ('reportes.ver_basicos', 'Reportes', 'Ver reportes básicos del sistema');
INSERT INTO #TempPermisos VALUES ('reportes.ver_avanzados', 'Reportes', 'Ver reportes avanzados');
INSERT INTO #TempPermisos VALUES ('reportes.ver_todos', 'Reportes', 'Acceso a todos los reportes');
INSERT INTO #TempPermisos VALUES ('reportes.crear_personalizados', 'Reportes', 'Crear reportes personalizados');
INSERT INTO #TempPermisos VALUES ('reportes.exportar', 'Reportes', 'Exportar reportes');
INSERT INTO #TempPermisos VALUES ('reportes.programar', 'Reportes', 'Programar generación automática de reportes');
INSERT INTO #TempPermisos VALUES ('reportes.ver_estadisticas_institucionales', 'Reportes', 'Ver estadísticas a nivel institucional');

-- PERMISOS DE CONFIGURACION
INSERT INTO #TempPermisos VALUES ('configuracion.ver', 'Configuración', 'Ver configuración del sistema');
INSERT INTO #TempPermisos VALUES ('configuracion.editar_sistema', 'Configuración', 'Modificar configuración general');
INSERT INTO #TempPermisos VALUES ('configuracion.editar_seguridad', 'Configuración', 'Modificar configuración de seguridad');
INSERT INTO #TempPermisos VALUES ('configuracion.gestionar_roles', 'Configuración', 'Crear y modificar roles');
INSERT INTO #TempPermisos VALUES ('configuracion.gestionar_permisos', 'Configuración', 'Asignar permisos a roles');
INSERT INTO #TempPermisos VALUES ('configuracion.backup', 'Configuración', 'Realizar copias de seguridad');
INSERT INTO #TempPermisos VALUES ('configuracion.restaurar', 'Configuración', 'Restaurar copias de seguridad');
INSERT INTO #TempPermisos VALUES ('configuracion.ver_logs', 'Configuración', 'Ver logs del sistema');
INSERT INTO #TempPermisos VALUES ('configuracion.limpiar_logs', 'Configuración', 'Limpiar logs del sistema');
INSERT INTO #TempPermisos VALUES ('configuracion.modo_mantenimiento', 'Configuración', 'Activar modo mantenimiento');
INSERT INTO #TempPermisos VALUES ('configuracion.gestionar', 'Configuración', 'Gestionar toda la configuración del sistema');
INSERT INTO #TempPermisos VALUES ('configuracion.inicializar', 'Configuración', 'Inicializar componentes del sistema');
INSERT INTO #TempPermisos VALUES ('configuracion.sincronizar_permisos', 'Configuración', 'Sincronizar permisos del sistema');
INSERT INTO #TempPermisos VALUES ('configuracion.verificar_estado', 'Configuración', 'Verificar el estado del sistema');

-- PERMISOS DE AUDITORIA
INSERT INTO #TempPermisos VALUES ('auditoria.ver', 'Auditoría', 'Ver registros de auditoría');
INSERT INTO #TempPermisos VALUES ('auditoria.ver_accesos', 'Auditoría', 'Ver registros de acceso al sistema');
INSERT INTO #TempPermisos VALUES ('auditoria.ver_cambios', 'Auditoría', 'Ver registros de cambios en datos');
INSERT INTO #TempPermisos VALUES ('auditoria.ver_errores', 'Auditoría', 'Ver registros de errores del sistema');
INSERT INTO #TempPermisos VALUES ('auditoria.exportar', 'Auditoría', 'Exportar registros de auditoría');
INSERT INTO #TempPermisos VALUES ('auditoria.limpiar', 'Auditoría', 'Limpiar registros de auditoría');
INSERT INTO #TempPermisos VALUES ('auditoria.ver_metricas', 'Auditoría', 'Ver métricas de uso del sistema');

-- PERMISOS DE PERIODOS
INSERT INTO #TempPermisos VALUES ('periodos.ver', 'Períodos Académicos', 'Ver períodos académicos');
INSERT INTO #TempPermisos VALUES ('periodos.crear', 'Períodos Académicos', 'Crear nuevos períodos académicos');
INSERT INTO #TempPermisos VALUES ('periodos.editar', 'Períodos Académicos', 'Editar períodos académicos');
INSERT INTO #TempPermisos VALUES ('periodos.eliminar', 'Períodos Académicos', 'Eliminar períodos académicos');
INSERT INTO #TempPermisos VALUES ('periodos.activar', 'Períodos Académicos', 'Activar períodos académicos');
INSERT INTO #TempPermisos VALUES ('periodos.cerrar', 'Períodos Académicos', 'Cerrar períodos académicos');
INSERT INTO #TempPermisos VALUES ('periodos.gestionar_calendario', 'Períodos Académicos', 'Gestionar calendario académico');

-- PERMISOS DE NIVELES
INSERT INTO #TempPermisos VALUES ('niveles.ver', 'Niveles de Calificación', 'Ver niveles de calificación');
INSERT INTO #TempPermisos VALUES ('niveles.crear', 'Niveles de Calificación', 'Crear nuevos niveles');
INSERT INTO #TempPermisos VALUES ('niveles.editar', 'Niveles de Calificación', 'Editar niveles de calificación');
INSERT INTO #TempPermisos VALUES ('niveles.eliminar', 'Niveles de Calificación', 'Eliminar niveles de calificación');
INSERT INTO #TempPermisos VALUES ('niveles.reordenar', 'Niveles de Calificación', 'Cambiar orden de niveles');
INSERT INTO #TempPermisos VALUES ('niveles.gestionar_grupos', 'Niveles de Calificación', 'Gestionar grupos de calificación');

-- =====================================================================================
-- PASO 5: Verificar si existe tabla de permisos de usuario (AspNetUserClaims)
-- =====================================================================================

PRINT 'Asignando permisos individuales al usuario...';

DECLARE @Permiso NVARCHAR(100);
DECLARE @Descripcion NVARCHAR(500);
DECLARE @Counter INT = 0;

DECLARE permiso_cursor CURSOR FOR
SELECT Permiso, Descripcion FROM #TempPermisos;

OPEN permiso_cursor;
FETCH NEXT FROM permiso_cursor INTO @Permiso, @Descripcion;

WHILE @@FETCH_STATUS = 0
BEGIN
    -- Verificar si el permiso ya existe para el usuario
    IF NOT EXISTS (
        SELECT 1 FROM AspNetUserClaims 
        WHERE UserId = @UserId 
        AND ClaimType = 'permission' 
        AND ClaimValue = @Permiso
    )
    BEGIN
        -- Insertar el permiso como claim del usuario
        INSERT INTO AspNetUserClaims (UserId, ClaimType, ClaimValue)
        VALUES (@UserId, 'permission', @Permiso);
        
        SET @Counter = @Counter + 1;
    END
    
    FETCH NEXT FROM permiso_cursor INTO @Permiso, @Descripcion;
END

CLOSE permiso_cursor;
DEALLOCATE permiso_cursor;

PRINT 'Permisos asignados: ' + CAST(@Counter AS NVARCHAR(10));

-- =====================================================================================
-- PASO 6: Asignar permisos a los roles también
-- =====================================================================================

PRINT 'Asignando permisos a roles SuperAdmin y Admin...';

DECLARE @RoleCounter INT = 0;

-- Cursor para asignar permisos a roles
DECLARE role_permiso_cursor CURSOR FOR
SELECT Permiso FROM #TempPermisos;

OPEN role_permiso_cursor;
FETCH NEXT FROM role_permiso_cursor INTO @Permiso;

WHILE @@FETCH_STATUS = 0
BEGIN
    -- Asignar permiso a SuperAdmin role
    IF NOT EXISTS (
        SELECT 1 FROM AspNetRoleClaims 
        WHERE RoleId = @SuperAdminRoleId 
        AND ClaimType = 'permission' 
        AND ClaimValue = @Permiso
    )
    BEGIN
        INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue)
        VALUES (@SuperAdminRoleId, 'permission', @Permiso);
        SET @RoleCounter = @RoleCounter + 1;
    END
    
    -- Asignar permiso a Admin role
    IF NOT EXISTS (
        SELECT 1 FROM AspNetRoleClaims 
        WHERE RoleId = @AdminRoleId 
        AND ClaimType = 'permission' 
        AND ClaimValue = @Permiso
    )
    BEGIN
        INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue)
        VALUES (@AdminRoleId, 'permission', @Permiso);
    END
    
    FETCH NEXT FROM role_permiso_cursor INTO @Permiso;
END

CLOSE role_permiso_cursor;
DEALLOCATE role_permiso_cursor;

PRINT 'Permisos asignados a roles: ' + CAST(@RoleCounter AS NVARCHAR(10));

-- =====================================================================================
-- PASO 7: Actualizar información del usuario
-- =====================================================================================

-- Asegurar que el usuario esté activo y confirmado
UPDATE AspNetUsers 
SET 
    IsActive = 1,
    Activo = 1,
    EmailConfirmed = 1,
    PhoneNumberConfirmed = 1,
    LockoutEnabled = 0,
    LockoutEnd = NULL,
    AccessFailedCount = 0
WHERE Id = @UserId;

PRINT 'Usuario actualizado - activo y confirmado';

-- =====================================================================================
-- PASO 8: Resumen final
-- =====================================================================================

SELECT 
    u.Id as UserId,
    u.UserName,
    u.Email,
    u.NombreCompleto,
    u.IsActive,
    u.Activo,
    u.EmailConfirmed,
    COUNT(DISTINCT ur.RoleId) as TotalRoles,
    COUNT(DISTINCT uc.ClaimValue) as TotalPermisosUsuario,
    (SELECT COUNT(DISTINCT rc.ClaimValue) 
     FROM AspNetRoleClaims rc 
     INNER JOIN AspNetUserRoles ur2 ON rc.RoleId = ur2.RoleId 
     WHERE ur2.UserId = u.Id AND rc.ClaimType = 'permission') as TotalPermisosRoles
FROM AspNetUsers u
LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
LEFT JOIN AspNetUserClaims uc ON u.Id = uc.UserId AND uc.ClaimType = 'permission'
WHERE u.Id = @UserId
GROUP BY u.Id, u.UserName, u.Email, u.NombreCompleto, u.IsActive, u.Activo, u.EmailConfirmed;

-- Mostrar roles asignados
SELECT 
    r.Name as RoleName,
    r.NormalizedName
FROM AspNetRoles r
INNER JOIN AspNetUserRoles ur ON r.Id = ur.RoleId
WHERE ur.UserId = @UserId;

-- Mostrar algunos permisos asignados
SELECT TOP 10
    uc.ClaimValue as Permiso
FROM AspNetUserClaims uc
WHERE uc.UserId = @UserId AND uc.ClaimType = 'permission'
ORDER BY uc.ClaimValue;

-- Limpiar tabla temporal
DROP TABLE #TempPermisos;

PRINT '======================================================================================';
PRINT 'SCRIPT COMPLETADO EXITOSAMENTE';
PRINT 'Usuario admin@rubricas.edu ahora tiene todos los permisos del sistema';
PRINT 'Roles asignados: SuperAdmin, Admin';
PRINT 'Total de permisos individuales: ' + CAST(@Counter AS NVARCHAR(10));
PRINT '======================================================================================';