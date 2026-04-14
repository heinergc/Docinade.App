-- =====================================================================================
-- Script SQLite para otorgar todos los permisos al usuario admin@rubricas.edu
-- Sistema de Rúbricas Académicas
-- Fecha: 29 de julio de 2025
-- =====================================================================================

-- Variables (en SQLite usamos variables con declaraciones simples)
-- Verificar si el usuario existe
SELECT 'Buscando usuario admin@rubricas.edu...';

-- Verificar si el usuario existe
SELECT CASE 
    WHEN EXISTS(SELECT 1 FROM AspNetUsers WHERE UserName = 'admin@rubricas.edu' OR Email = 'admin@rubricas.edu')
    THEN 'Usuario encontrado'
    ELSE 'ERROR: Usuario admin@rubricas.edu no encontrado - debe crear el usuario primero'
END as Status;

-- =====================================================================================
-- PASO 1: Crear rol SuperAdmin si no existe
-- =====================================================================================

INSERT OR IGNORE INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
VALUES (
    lower(hex(randomblob(16))), 
    'SuperAdmin', 
    'SUPERADMIN', 
    lower(hex(randomblob(16)))
);

INSERT OR IGNORE INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
VALUES (
    lower(hex(randomblob(16))), 
    'Admin', 
    'ADMIN', 
    lower(hex(randomblob(16)))
);

SELECT 'Roles SuperAdmin y Admin creados o ya existen';

-- =====================================================================================
-- PASO 2: Asignar roles al usuario
-- =====================================================================================

-- Asignar rol SuperAdmin al usuario
INSERT OR IGNORE INTO AspNetUserRoles (UserId, RoleId)
SELECT 
    u.Id,
    r.Id
FROM AspNetUsers u, AspNetRoles r
WHERE (u.UserName = 'admin@rubricas.edu' OR u.Email = 'admin@rubricas.edu')
AND r.Name = 'SuperAdmin';

-- Asignar rol Admin al usuario
INSERT OR IGNORE INTO AspNetUserRoles (UserId, RoleId)
SELECT 
    u.Id,
    r.Id
FROM AspNetUsers u, AspNetRoles r
WHERE (u.UserName = 'admin@rubricas.edu' OR u.Email = 'admin@rubricas.edu')
AND r.Name = 'Admin';

SELECT 'Roles asignados al usuario';

-- =====================================================================================
-- PASO 3: Crear tabla temporal con todos los permisos del sistema
-- =====================================================================================

-- Crear tabla temporal para permisos
CREATE TEMP TABLE TempPermisos (
    Permiso TEXT,
    Categoria TEXT,
    Descripcion TEXT
);

-- PERMISOS DE USUARIOS
INSERT INTO TempPermisos VALUES ('usuarios.ver', 'Usuarios', 'Ver usuarios del sistema');
INSERT INTO TempPermisos VALUES ('usuarios.crear', 'Usuarios', 'Crear nuevos usuarios');
INSERT INTO TempPermisos VALUES ('usuarios.editar', 'Usuarios', 'Editar información de usuarios');
INSERT INTO TempPermisos VALUES ('usuarios.eliminar', 'Usuarios', 'Eliminar usuarios del sistema');
INSERT INTO TempPermisos VALUES ('usuarios.bloquear', 'Usuarios', 'Bloquear/desbloquear usuarios');
INSERT INTO TempPermisos VALUES ('usuarios.cambiar_roles', 'Usuarios', 'Asignar y quitar roles a usuarios');
INSERT INTO TempPermisos VALUES ('usuarios.ver_perfil_completo', 'Usuarios', 'Ver información detallada de usuarios');
INSERT INTO TempPermisos VALUES ('usuarios.exportar', 'Usuarios', 'Exportar lista de usuarios');
INSERT INTO TempPermisos VALUES ('usuarios.importar', 'Usuarios', 'Importar usuarios desde archivos');
INSERT INTO TempPermisos VALUES ('usuarios.gestionar_roles', 'Usuarios', 'Gestionar roles de usuarios');
INSERT INTO TempPermisos VALUES ('usuarios.gestionar_permisos', 'Usuarios', 'Gestionar permisos de usuarios');
INSERT INTO TempPermisos VALUES ('usuarios.restablecer_contraseña', 'Usuarios', 'Restablecer contraseñas de usuarios');

-- PERMISOS DE RUBRICAS
INSERT INTO TempPermisos VALUES ('rubricas.ver', 'Rúbricas', 'Ver rúbricas propias');
INSERT INTO TempPermisos VALUES ('rubricas.ver_todas', 'Rúbricas', 'Ver todas las rúbricas del sistema');
INSERT INTO TempPermisos VALUES ('rubricas.crear', 'Rúbricas', 'Crear nuevas rúbricas');
INSERT INTO TempPermisos VALUES ('rubricas.editar', 'Rúbricas', 'Editar rúbricas propias');
INSERT INTO TempPermisos VALUES ('rubricas.editar_todas', 'Rúbricas', 'Editar cualquier rúbrica');
INSERT INTO TempPermisos VALUES ('rubricas.eliminar', 'Rúbricas', 'Eliminar rúbricas propias');
INSERT INTO TempPermisos VALUES ('rubricas.eliminar_todas', 'Rúbricas', 'Eliminar cualquier rúbrica');
INSERT INTO TempPermisos VALUES ('rubricas.duplicar', 'Rúbricas', 'Duplicar rúbricas');
INSERT INTO TempPermisos VALUES ('rubricas.publicar', 'Rúbricas', 'Publicar rúbricas');
INSERT INTO TempPermisos VALUES ('rubricas.archivar', 'Rúbricas', 'Archivar rúbricas');
INSERT INTO TempPermisos VALUES ('rubricas.compartir', 'Rúbricas', 'Compartir rúbricas con otros usuarios');
INSERT INTO TempPermisos VALUES ('rubricas.exportar', 'Rúbricas', 'Exportar rúbricas');
INSERT INTO TempPermisos VALUES ('rubricas.importar', 'Rúbricas', 'Importar rúbricas');

-- PERMISOS DE EVALUACIONES
INSERT INTO TempPermisos VALUES ('evaluaciones.ver', 'Evaluaciones', 'Ver evaluaciones propias');
INSERT INTO TempPermisos VALUES ('evaluaciones.ver_todas', 'Evaluaciones', 'Ver todas las evaluaciones');
INSERT INTO TempPermisos VALUES ('evaluaciones.crear', 'Evaluaciones', 'Crear nuevas evaluaciones');
INSERT INTO TempPermisos VALUES ('evaluaciones.editar', 'Evaluaciones', 'Editar evaluaciones propias');
INSERT INTO TempPermisos VALUES ('evaluaciones.editar_todas', 'Evaluaciones', 'Editar cualquier evaluación');
INSERT INTO TempPermisos VALUES ('evaluaciones.eliminar', 'Evaluaciones', 'Eliminar evaluaciones propias');
INSERT INTO TempPermisos VALUES ('evaluaciones.eliminar_todas', 'Evaluaciones', 'Eliminar cualquier evaluación');
INSERT INTO TempPermisos VALUES ('evaluaciones.evaluar', 'Evaluaciones', 'Realizar evaluaciones');
INSERT INTO TempPermisos VALUES ('evaluaciones.revisar', 'Evaluaciones', 'Revisar evaluaciones');
INSERT INTO TempPermisos VALUES ('evaluaciones.aprobar', 'Evaluaciones', 'Aprobar evaluaciones');
INSERT INTO TempPermisos VALUES ('evaluaciones.finalizar', 'Evaluaciones', 'Finalizar evaluaciones');
INSERT INTO TempPermisos VALUES ('evaluaciones.reabrir', 'Evaluaciones', 'Reabrir evaluaciones finalizadas');
INSERT INTO TempPermisos VALUES ('evaluaciones.exportar', 'Evaluaciones', 'Exportar evaluaciones');
INSERT INTO TempPermisos VALUES ('evaluaciones.ver_resultados', 'Evaluaciones', 'Ver resultados de evaluaciones');
INSERT INTO TempPermisos VALUES ('evaluaciones.ver_estadisticas', 'Evaluaciones', 'Ver estadísticas de evaluaciones');

-- PERMISOS DE ESTUDIANTES
INSERT INTO TempPermisos VALUES ('estudiantes.ver', 'Estudiantes', 'Ver lista de estudiantes');
INSERT INTO TempPermisos VALUES ('estudiantes.crear', 'Estudiantes', 'Crear nuevos estudiantes');
INSERT INTO TempPermisos VALUES ('estudiantes.editar', 'Estudiantes', 'Editar información de estudiantes');
INSERT INTO TempPermisos VALUES ('estudiantes.eliminar', 'Estudiantes', 'Eliminar estudiantes');
INSERT INTO TempPermisos VALUES ('estudiantes.importar', 'Estudiantes', 'Importar estudiantes desde archivos');
INSERT INTO TempPermisos VALUES ('estudiantes.exportar', 'Estudiantes', 'Exportar lista de estudiantes');
INSERT INTO TempPermisos VALUES ('estudiantes.ver_historial', 'Estudiantes', 'Ver historial académico de estudiantes');
INSERT INTO TempPermisos VALUES ('estudiantes.ver_notas', 'Estudiantes', 'Ver calificaciones de estudiantes');
INSERT INTO TempPermisos VALUES ('estudiantes.editar_notas', 'Estudiantes', 'Modificar calificaciones de estudiantes');

-- PERMISOS DE REPORTES
INSERT INTO TempPermisos VALUES ('reportes.ver_basicos', 'Reportes', 'Ver reportes básicos del sistema');
INSERT INTO TempPermisos VALUES ('reportes.ver_avanzados', 'Reportes', 'Ver reportes avanzados');
INSERT INTO TempPermisos VALUES ('reportes.ver_todos', 'Reportes', 'Acceso a todos los reportes');
INSERT INTO TempPermisos VALUES ('reportes.crear_personalizados', 'Reportes', 'Crear reportes personalizados');
INSERT INTO TempPermisos VALUES ('reportes.exportar', 'Reportes', 'Exportar reportes');
INSERT INTO TempPermisos VALUES ('reportes.programar', 'Reportes', 'Programar generación automática de reportes');
INSERT INTO TempPermisos VALUES ('reportes.ver_estadisticas_institucionales', 'Reportes', 'Ver estadísticas a nivel institucional');

-- PERMISOS DE CONFIGURACION
INSERT INTO TempPermisos VALUES ('configuracion.ver', 'Configuración', 'Ver configuración del sistema');
INSERT INTO TempPermisos VALUES ('configuracion.editar_sistema', 'Configuración', 'Modificar configuración general');
INSERT INTO TempPermisos VALUES ('configuracion.editar_seguridad', 'Configuración', 'Modificar configuración de seguridad');
INSERT INTO TempPermisos VALUES ('configuracion.gestionar_roles', 'Configuración', 'Crear y modificar roles');
INSERT INTO TempPermisos VALUES ('configuracion.gestionar_permisos', 'Configuración', 'Asignar permisos a roles');
INSERT INTO TempPermisos VALUES ('configuracion.backup', 'Configuración', 'Realizar copias de seguridad');
INSERT INTO TempPermisos VALUES ('configuracion.restaurar', 'Configuración', 'Restaurar copias de seguridad');
INSERT INTO TempPermisos VALUES ('configuracion.ver_logs', 'Configuración', 'Ver logs del sistema');
INSERT INTO TempPermisos VALUES ('configuracion.limpiar_logs', 'Configuración', 'Limpiar logs del sistema');
INSERT INTO TempPermisos VALUES ('configuracion.modo_mantenimiento', 'Configuración', 'Activar modo mantenimiento');
INSERT INTO TempPermisos VALUES ('configuracion.gestionar', 'Configuración', 'Gestionar toda la configuración del sistema');
INSERT INTO TempPermisos VALUES ('configuracion.inicializar', 'Configuración', 'Inicializar componentes del sistema');
INSERT INTO TempPermisos VALUES ('configuracion.sincronizar_permisos', 'Configuración', 'Sincronizar permisos del sistema');
INSERT INTO TempPermisos VALUES ('configuracion.verificar_estado', 'Configuración', 'Verificar el estado del sistema');

-- PERMISOS DE AUDITORIA
INSERT INTO TempPermisos VALUES ('auditoria.ver', 'Auditoría', 'Ver registros de auditoría');
INSERT INTO TempPermisos VALUES ('auditoria.ver_accesos', 'Auditoría', 'Ver registros de acceso al sistema');
INSERT INTO TempPermisos VALUES ('auditoria.ver_cambios', 'Auditoría', 'Ver registros de cambios en datos');
INSERT INTO TempPermisos VALUES ('auditoria.ver_errores', 'Auditoría', 'Ver registros de errores del sistema');
INSERT INTO TempPermisos VALUES ('auditoria.exportar', 'Auditoría', 'Exportar registros de auditoría');
INSERT INTO TempPermisos VALUES ('auditoria.limpiar', 'Auditoría', 'Limpiar registros de auditoría');
INSERT INTO TempPermisos VALUES ('auditoria.ver_metricas', 'Auditoría', 'Ver métricas de uso del sistema');

-- PERMISOS DE PERIODOS
INSERT INTO TempPermisos VALUES ('periodos.ver', 'Períodos Académicos', 'Ver períodos académicos');
INSERT INTO TempPermisos VALUES ('periodos.crear', 'Períodos Académicos', 'Crear nuevos períodos académicos');
INSERT INTO TempPermisos VALUES ('periodos.editar', 'Períodos Académicos', 'Editar períodos académicos');
INSERT INTO TempPermisos VALUES ('periodos.eliminar', 'Períodos Académicos', 'Eliminar períodos académicos');
INSERT INTO TempPermisos VALUES ('periodos.activar', 'Períodos Académicos', 'Activar períodos académicos');
INSERT INTO TempPermisos VALUES ('periodos.cerrar', 'Períodos Académicos', 'Cerrar períodos académicos');
INSERT INTO TempPermisos VALUES ('periodos.gestionar_calendario', 'Períodos Académicos', 'Gestionar calendario académico');

-- PERMISOS DE NIVELES
INSERT INTO TempPermisos VALUES ('niveles.ver', 'Niveles de Calificación', 'Ver niveles de calificación');
INSERT INTO TempPermisos VALUES ('niveles.crear', 'Niveles de Calificación', 'Crear nuevos niveles');
INSERT INTO TempPermisos VALUES ('niveles.editar', 'Niveles de Calificación', 'Editar niveles de calificación');
INSERT INTO TempPermisos VALUES ('niveles.eliminar', 'Niveles de Calificación', 'Eliminar niveles de calificación');
INSERT INTO TempPermisos VALUES ('niveles.reordenar', 'Niveles de Calificación', 'Cambiar orden de niveles');
INSERT INTO TempPermisos VALUES ('niveles.gestionar_grupos', 'Niveles de Calificación', 'Gestionar grupos de calificación');

SELECT 'Permisos cargados en tabla temporal: ' || COUNT(*) || ' permisos' FROM TempPermisos;

-- =====================================================================================
-- PASO 4: Asignar permisos individuales al usuario
-- =====================================================================================

-- Asignar todos los permisos al usuario admin@rubricas.edu
INSERT OR IGNORE INTO AspNetUserClaims (UserId, ClaimType, ClaimValue)
SELECT 
    u.Id,
    'permission',
    tp.Permiso
FROM AspNetUsers u, TempPermisos tp
WHERE (u.UserName = 'admin@rubricas.edu' OR u.Email = 'admin@rubricas.edu');

SELECT 'Permisos individuales asignados al usuario';

-- =====================================================================================
-- PASO 5: Asignar permisos a los roles
-- =====================================================================================

-- Asignar todos los permisos al rol SuperAdmin
INSERT OR IGNORE INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue)
SELECT 
    r.Id,
    'permission',
    tp.Permiso
FROM AspNetRoles r, TempPermisos tp
WHERE r.Name = 'SuperAdmin';

-- Asignar todos los permisos al rol Admin
INSERT OR IGNORE INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue)
SELECT 
    r.Id,
    'permission',
    tp.Permiso
FROM AspNetRoles r, TempPermisos tp
WHERE r.Name = 'Admin';

SELECT 'Permisos asignados a roles SuperAdmin y Admin';

-- =====================================================================================
-- PASO 6: Actualizar información del usuario
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
WHERE UserName = 'admin@rubricas.edu' OR Email = 'admin@rubricas.edu';

SELECT 'Usuario actualizado - activo y confirmado';

-- =====================================================================================
-- PASO 7: Resumen final
-- =====================================================================================

SELECT 'RESUMEN FINAL:';

-- Información del usuario
SELECT 
    'Usuario: ' || u.UserName || ' (' || u.Email || ')' as Info,
    'Activo: ' || CASE WHEN u.IsActive = 1 THEN 'Sí' ELSE 'No' END as Estado,
    'Email confirmado: ' || CASE WHEN u.EmailConfirmed = 1 THEN 'Sí' ELSE 'No' END as EmailStatus
FROM AspNetUsers u
WHERE u.UserName = 'admin@rubricas.edu' OR u.Email = 'admin@rubricas.edu';

-- Roles asignados
SELECT 
    'Roles asignados:' as Tipo,
    GROUP_CONCAT(r.Name, ', ') as Roles
FROM AspNetUsers u
JOIN AspNetUserRoles ur ON u.Id = ur.UserId
JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE u.UserName = 'admin@rubricas.edu' OR u.Email = 'admin@rubricas.edu';

-- Conteo de permisos
SELECT 
    'Total permisos directos:' as Tipo,
    COUNT(*) as Cantidad
FROM AspNetUsers u
JOIN AspNetUserClaims uc ON u.Id = uc.UserId
WHERE (u.UserName = 'admin@rubricas.edu' OR u.Email = 'admin@rubricas.edu')
AND uc.ClaimType = 'permission';

-- Permisos por categoría
SELECT 
    CASE 
        WHEN uc.ClaimValue LIKE 'usuarios.%' THEN 'Usuarios'
        WHEN uc.ClaimValue LIKE 'rubricas.%' THEN 'Rúbricas'
        WHEN uc.ClaimValue LIKE 'evaluaciones.%' THEN 'Evaluaciones'
        WHEN uc.ClaimValue LIKE 'estudiantes.%' THEN 'Estudiantes'
        WHEN uc.ClaimValue LIKE 'reportes.%' THEN 'Reportes'
        WHEN uc.ClaimValue LIKE 'configuracion.%' THEN 'Configuración'
        WHEN uc.ClaimValue LIKE 'auditoria.%' THEN 'Auditoría'
        WHEN uc.ClaimValue LIKE 'periodos.%' THEN 'Períodos'
        WHEN uc.ClaimValue LIKE 'niveles.%' THEN 'Niveles'
        ELSE 'Otros'
    END as Categoria,
    COUNT(*) as CantidadPermisos
FROM AspNetUsers u
JOIN AspNetUserClaims uc ON u.Id = uc.UserId
WHERE (u.UserName = 'admin@rubricas.edu' OR u.Email = 'admin@rubricas.edu')
AND uc.ClaimType = 'permission'
GROUP BY 
    CASE 
        WHEN uc.ClaimValue LIKE 'usuarios.%' THEN 'Usuarios'
        WHEN uc.ClaimValue LIKE 'rubricas.%' THEN 'Rúbricas'
        WHEN uc.ClaimValue LIKE 'evaluaciones.%' THEN 'Evaluaciones'
        WHEN uc.ClaimValue LIKE 'estudiantes.%' THEN 'Estudiantes'
        WHEN uc.ClaimValue LIKE 'reportes.%' THEN 'Reportes'
        WHEN uc.ClaimValue LIKE 'configuracion.%' THEN 'Configuración'
        WHEN uc.ClaimValue LIKE 'auditoria.%' THEN 'Auditoría'
        WHEN uc.ClaimValue LIKE 'periodos.%' THEN 'Períodos'
        WHEN uc.ClaimValue LIKE 'niveles.%' THEN 'Niveles'
        ELSE 'Otros'
    END
ORDER BY CantidadPermisos DESC;

-- Algunos permisos asignados (muestra)
SELECT 'Algunos permisos asignados (primeros 10):' as Info;
SELECT 
    uc.ClaimValue as Permiso
FROM AspNetUsers u
JOIN AspNetUserClaims uc ON u.Id = uc.UserId
WHERE (u.UserName = 'admin@rubricas.edu' OR u.Email = 'admin@rubricas.edu')
AND uc.ClaimType = 'permission'
ORDER BY uc.ClaimValue
LIMIT 10;

-- Limpiar tabla temporal (se hace automáticamente al final de la sesión)
DROP TABLE TempPermisos;

SELECT '======================================================================================';
SELECT 'SCRIPT COMPLETADO EXITOSAMENTE';
SELECT 'Usuario admin@rubricas.edu ahora tiene todos los permisos del sistema';
SELECT 'Roles asignados: SuperAdmin, Admin';
SELECT '======================================================================================';