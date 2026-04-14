-- =====================================================================================
-- Script SQLite para crear y configurar el usuario admin@rubricas.edu con todos los permisos
-- Sistema de Rúbricas Académicas
-- Fecha: 13 de agosto de 2025
-- 
-- PROPÓSITO: Este script garantiza que cada vez que se elimine y recree la base de datos,
-- el usuario admin@rubricas.edu tenga acceso completo al sistema
-- =====================================================================================

-- =====================================================================================
-- PASO 1: Verificar existencia del usuario
-- =====================================================================================

SELECT 'Iniciando configuración del usuario admin@rubricas.edu...';

-- Verificar si el usuario existe
SELECT CASE 
    WHEN EXISTS(SELECT 1 FROM AspNetUsers WHERE UserName = 'admin@rubricas.edu' OR Email = 'admin@rubricas.edu')
    THEN 'Usuario admin@rubricas.edu encontrado - Configurando permisos...'
    ELSE 'ADVERTENCIA: Usuario admin@rubricas.edu no existe - El script configurará permisos pero el usuario debe ser creado'
END as StatusUsuario;

-- =====================================================================================
-- PASO 2: Crear roles del sistema si no existen
-- =====================================================================================

-- Crear rol SuperAdmin
INSERT OR IGNORE INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
VALUES (
    lower(hex(randomblob(16))), 
    'SuperAdmin', 
    'SUPERADMIN', 
    lower(hex(randomblob(16)))
);

-- Crear rol Admin 
INSERT OR IGNORE INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
VALUES (
    lower(hex(randomblob(16))), 
    'Admin', 
    'ADMIN', 
    lower(hex(randomblob(16)))
);

-- Crear rol Profesor
INSERT OR IGNORE INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
VALUES (
    lower(hex(randomblob(16))), 
    'Profesor', 
    'PROFESOR', 
    lower(hex(randomblob(16)))
);

-- Crear rol Estudiante
INSERT OR IGNORE INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
VALUES (
    lower(hex(randomblob(16))), 
    'Estudiante', 
    'ESTUDIANTE', 
    lower(hex(randomblob(16)))
);

SELECT 'Roles del sistema creados o verificados';

-- =====================================================================================
-- PASO 3: Asignar roles al usuario admin (solo si existe)
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

SELECT 'Roles asignados al usuario admin (si existe)';

-- =====================================================================================
-- PASO 4: Crear tabla temporal con TODOS los permisos del sistema actualizada
-- =====================================================================================

-- Crear tabla temporal para permisos
CREATE TEMP TABLE IF NOT EXISTS TempPermisos (
    Permiso TEXT PRIMARY KEY,
    Categoria TEXT,
    Descripcion TEXT
);

-- Limpiar tabla temporal por si existe
DELETE FROM TempPermisos;

-- =====================================================================================
-- PERMISOS DE USUARIOS (12 permisos)
-- =====================================================================================
INSERT OR IGNORE INTO TempPermisos VALUES ('usuarios.ver', 'Usuarios', 'Ver usuarios del sistema');
INSERT OR IGNORE INTO TempPermisos VALUES ('usuarios.crear', 'Usuarios', 'Crear nuevos usuarios');
INSERT OR IGNORE INTO TempPermisos VALUES ('usuarios.editar', 'Usuarios', 'Editar información de usuarios');
INSERT OR IGNORE INTO TempPermisos VALUES ('usuarios.eliminar', 'Usuarios', 'Eliminar usuarios del sistema');
INSERT OR IGNORE INTO TempPermisos VALUES ('usuarios.bloquear', 'Usuarios', 'Bloquear/desbloquear usuarios');
INSERT OR IGNORE INTO TempPermisos VALUES ('usuarios.cambiar_roles', 'Usuarios', 'Asignar y quitar roles a usuarios');
INSERT OR IGNORE INTO TempPermisos VALUES ('usuarios.ver_perfil_completo', 'Usuarios', 'Ver información detallada de usuarios');
INSERT OR IGNORE INTO TempPermisos VALUES ('usuarios.exportar', 'Usuarios', 'Exportar lista de usuarios');
INSERT OR IGNORE INTO TempPermisos VALUES ('usuarios.importar', 'Usuarios', 'Importar usuarios desde archivos');
INSERT OR IGNORE INTO TempPermisos VALUES ('usuarios.gestionar_roles', 'Usuarios', 'Gestionar roles de usuarios');
INSERT OR IGNORE INTO TempPermisos VALUES ('usuarios.gestionar_permisos', 'Usuarios', 'Gestionar permisos de usuarios');
INSERT OR IGNORE INTO TempPermisos VALUES ('usuarios.restablecer_contraseña', 'Usuarios', 'Restablecer contraseñas de usuarios');

-- =====================================================================================
-- PERMISOS DE RUBRICAS (13 permisos)
-- =====================================================================================
INSERT OR IGNORE INTO TempPermisos VALUES ('rubricas.ver', 'Rúbricas', 'Ver rúbricas propias');
INSERT OR IGNORE INTO TempPermisos VALUES ('rubricas.ver_todas', 'Rúbricas', 'Ver todas las rúbricas del sistema');
INSERT OR IGNORE INTO TempPermisos VALUES ('rubricas.crear', 'Rúbricas', 'Crear nuevas rúbricas');
INSERT OR IGNORE INTO TempPermisos VALUES ('rubricas.editar', 'Rúbricas', 'Editar rúbricas propias');
INSERT OR IGNORE INTO TempPermisos VALUES ('rubricas.editar_todas', 'Rúbricas', 'Editar cualquier rúbrica');
INSERT OR IGNORE INTO TempPermisos VALUES ('rubricas.eliminar', 'Rúbricas', 'Eliminar rúbricas propias');
INSERT OR IGNORE INTO TempPermisos VALUES ('rubricas.eliminar_todas', 'Rúbricas', 'Eliminar cualquier rúbrica');
INSERT OR IGNORE INTO TempPermisos VALUES ('rubricas.duplicar', 'Rúbricas', 'Duplicar rúbricas');
INSERT OR IGNORE INTO TempPermisos VALUES ('rubricas.publicar', 'Rúbricas', 'Publicar rúbricas');
INSERT OR IGNORE INTO TempPermisos VALUES ('rubricas.archivar', 'Rúbricas', 'Archivar rúbricas');
INSERT OR IGNORE INTO TempPermisos VALUES ('rubricas.compartir', 'Rúbricas', 'Compartir rúbricas con otros usuarios');
INSERT OR IGNORE INTO TempPermisos VALUES ('rubricas.exportar', 'Rúbricas', 'Exportar rúbricas');
INSERT OR IGNORE INTO TempPermisos VALUES ('rubricas.importar', 'Rúbricas', 'Importar rúbricas');

-- =====================================================================================
-- PERMISOS DE EVALUACIONES (15 permisos)
-- =====================================================================================
INSERT OR IGNORE INTO TempPermisos VALUES ('evaluaciones.ver', 'Evaluaciones', 'Ver evaluaciones propias');
INSERT OR IGNORE INTO TempPermisos VALUES ('evaluaciones.ver_todas', 'Evaluaciones', 'Ver todas las evaluaciones');
INSERT OR IGNORE INTO TempPermisos VALUES ('evaluaciones.crear', 'Evaluaciones', 'Crear nuevas evaluaciones');
INSERT OR IGNORE INTO TempPermisos VALUES ('evaluaciones.editar', 'Evaluaciones', 'Editar evaluaciones propias');
INSERT OR IGNORE INTO TempPermisos VALUES ('evaluaciones.editar_todas', 'Evaluaciones', 'Editar cualquier evaluación');
INSERT OR IGNORE INTO TempPermisos VALUES ('evaluaciones.eliminar', 'Evaluaciones', 'Eliminar evaluaciones propias');
INSERT OR IGNORE INTO TempPermisos VALUES ('evaluaciones.eliminar_todas', 'Evaluaciones', 'Eliminar cualquier evaluación');
INSERT OR IGNORE INTO TempPermisos VALUES ('evaluaciones.evaluar', 'Evaluaciones', 'Realizar evaluaciones');
INSERT OR IGNORE INTO TempPermisos VALUES ('evaluaciones.revisar', 'Evaluaciones', 'Revisar evaluaciones');
INSERT OR IGNORE INTO TempPermisos VALUES ('evaluaciones.aprobar', 'Evaluaciones', 'Aprobar evaluaciones');
INSERT OR IGNORE INTO TempPermisos VALUES ('evaluaciones.finalizar', 'Evaluaciones', 'Finalizar evaluaciones');
INSERT OR IGNORE INTO TempPermisos VALUES ('evaluaciones.reabrir', 'Evaluaciones', 'Reabrir evaluaciones finalizadas');
INSERT OR IGNORE INTO TempPermisos VALUES ('evaluaciones.exportar', 'Evaluaciones', 'Exportar evaluaciones');
INSERT OR IGNORE INTO TempPermisos VALUES ('evaluaciones.ver_resultados', 'Evaluaciones', 'Ver resultados de evaluaciones');
INSERT OR IGNORE INTO TempPermisos VALUES ('evaluaciones.ver_estadisticas', 'Evaluaciones', 'Ver estadísticas de evaluaciones');

-- =====================================================================================
-- PERMISOS DE ESTUDIANTES (9 permisos)
-- =====================================================================================
INSERT OR IGNORE INTO TempPermisos VALUES ('estudiantes.ver', 'Estudiantes', 'Ver lista de estudiantes');
INSERT OR IGNORE INTO TempPermisos VALUES ('estudiantes.crear', 'Estudiantes', 'Crear nuevos estudiantes');
INSERT OR IGNORE INTO TempPermisos VALUES ('estudiantes.editar', 'Estudiantes', 'Editar información de estudiantes');
INSERT OR IGNORE INTO TempPermisos VALUES ('estudiantes.eliminar', 'Estudiantes', 'Eliminar estudiantes');
INSERT OR IGNORE INTO TempPermisos VALUES ('estudiantes.importar', 'Estudiantes', 'Importar estudiantes desde archivos');
INSERT OR IGNORE INTO TempPermisos VALUES ('estudiantes.exportar', 'Estudiantes', 'Exportar lista de estudiantes');
INSERT OR IGNORE INTO TempPermisos VALUES ('estudiantes.ver_historial', 'Estudiantes', 'Ver historial académico de estudiantes');
INSERT OR IGNORE INTO TempPermisos VALUES ('estudiantes.ver_notas', 'Estudiantes', 'Ver calificaciones de estudiantes');
INSERT OR IGNORE INTO TempPermisos VALUES ('estudiantes.editar_notas', 'Estudiantes', 'Modificar calificaciones de estudiantes');

-- =====================================================================================
-- PERMISOS DE REPORTES (7 permisos)
-- =====================================================================================
INSERT OR IGNORE INTO TempPermisos VALUES ('reportes.ver_basicos', 'Reportes', 'Ver reportes básicos del sistema');
INSERT OR IGNORE INTO TempPermisos VALUES ('reportes.ver_avanzados', 'Reportes', 'Ver reportes avanzados');
INSERT OR IGNORE INTO TempPermisos VALUES ('reportes.ver_todos', 'Reportes', 'Acceso a todos los reportes');
INSERT OR IGNORE INTO TempPermisos VALUES ('reportes.crear_personalizados', 'Reportes', 'Crear reportes personalizados');
INSERT OR IGNORE INTO TempPermisos VALUES ('reportes.exportar', 'Reportes', 'Exportar reportes');
INSERT OR IGNORE INTO TempPermisos VALUES ('reportes.programar', 'Reportes', 'Programar generación automática de reportes');
INSERT OR IGNORE INTO TempPermisos VALUES ('reportes.ver_estadisticas_institucionales', 'Reportes', 'Ver estadísticas a nivel institucional');

-- =====================================================================================
-- PERMISOS DE CONFIGURACION (14 permisos)
-- =====================================================================================
INSERT OR IGNORE INTO TempPermisos VALUES ('configuracion.ver', 'Configuración', 'Ver configuración del sistema');
INSERT OR IGNORE INTO TempPermisos VALUES ('configuracion.editar_sistema', 'Configuración', 'Modificar configuración general');
INSERT OR IGNORE INTO TempPermisos VALUES ('configuracion.editar_seguridad', 'Configuración', 'Modificar configuración de seguridad');
INSERT OR IGNORE INTO TempPermisos VALUES ('configuracion.gestionar_roles', 'Configuración', 'Crear y modificar roles');
INSERT OR IGNORE INTO TempPermisos VALUES ('configuracion.gestionar_permisos', 'Configuración', 'Asignar permisos a roles');
INSERT OR IGNORE INTO TempPermisos VALUES ('configuracion.backup', 'Configuración', 'Realizar copias de seguridad');
INSERT OR IGNORE INTO TempPermisos VALUES ('configuracion.restaurar', 'Configuración', 'Restaurar copias de seguridad');
INSERT OR IGNORE INTO TempPermisos VALUES ('configuracion.ver_logs', 'Configuración', 'Ver logs del sistema');
INSERT OR IGNORE INTO TempPermisos VALUES ('configuracion.limpiar_logs', 'Configuración', 'Limpiar logs del sistema');
INSERT OR IGNORE INTO TempPermisos VALUES ('configuracion.modo_mantenimiento', 'Configuración', 'Activar modo mantenimiento');
INSERT OR IGNORE INTO TempPermisos VALUES ('configuracion.gestionar', 'Configuración', 'Gestionar toda la configuración del sistema');
INSERT OR IGNORE INTO TempPermisos VALUES ('configuracion.inicializar', 'Configuración', 'Inicializar componentes del sistema');
INSERT OR IGNORE INTO TempPermisos VALUES ('configuracion.sincronizar_permisos', 'Configuración', 'Sincronizar permisos del sistema');
INSERT OR IGNORE INTO TempPermisos VALUES ('configuracion.verificar_estado', 'Configuración', 'Verificar el estado del sistema');

-- =====================================================================================
-- PERMISOS DE AUDITORIA (7 permisos)
-- =====================================================================================
INSERT OR IGNORE INTO TempPermisos VALUES ('auditoria.ver', 'Auditoría', 'Ver registros de auditoría');
INSERT OR IGNORE INTO TempPermisos VALUES ('auditoria.ver_accesos', 'Auditoría', 'Ver registros de acceso al sistema');
INSERT OR IGNORE INTO TempPermisos VALUES ('auditoria.ver_cambios', 'Auditoría', 'Ver registros de cambios en datos');
INSERT OR IGNORE INTO TempPermisos VALUES ('auditoria.ver_errores', 'Auditoría', 'Ver registros de errores del sistema');
INSERT OR IGNORE INTO TempPermisos VALUES ('auditoria.exportar', 'Auditoría', 'Exportar registros de auditoría');
INSERT OR IGNORE INTO TempPermisos VALUES ('auditoria.limpiar', 'Auditoría', 'Limpiar registros de auditoría');
INSERT OR IGNORE INTO TempPermisos VALUES ('auditoria.ver_metricas', 'Auditoría', 'Ver métricas de uso del sistema');

-- =====================================================================================
-- PERMISOS DE PERIODOS ACADEMICOS (7 permisos)
-- =====================================================================================
INSERT OR IGNORE INTO TempPermisos VALUES ('periodos.ver', 'Períodos Académicos', 'Ver períodos académicos');
INSERT OR IGNORE INTO TempPermisos VALUES ('periodos.crear', 'Períodos Académicos', 'Crear nuevos períodos académicos');
INSERT OR IGNORE INTO TempPermisos VALUES ('periodos.editar', 'Períodos Académicos', 'Editar períodos académicos');
INSERT OR IGNORE INTO TempPermisos VALUES ('periodos.eliminar', 'Períodos Académicos', 'Eliminar períodos académicos');
INSERT OR IGNORE INTO TempPermisos VALUES ('periodos.activar', 'Períodos Académicos', 'Activar períodos académicos');
INSERT OR IGNORE INTO TempPermisos VALUES ('periodos.cerrar', 'Períodos Académicos', 'Cerrar períodos académicos');
INSERT OR IGNORE INTO TempPermisos VALUES ('periodos.gestionar_calendario', 'Períodos Académicos', 'Gestionar calendario académico');

-- =====================================================================================
-- PERMISOS DE NIVELES DE CALIFICACION (6 permisos)
-- =====================================================================================
INSERT OR IGNORE INTO TempPermisos VALUES ('niveles.ver', 'Niveles de Calificación', 'Ver niveles de calificación');
INSERT OR IGNORE INTO TempPermisos VALUES ('niveles.crear', 'Niveles de Calificación', 'Crear nuevos niveles');
INSERT OR IGNORE INTO TempPermisos VALUES ('niveles.editar', 'Niveles de Calificación', 'Editar niveles de calificación');
INSERT OR IGNORE INTO TempPermisos VALUES ('niveles.eliminar', 'Niveles de Calificación', 'Eliminar niveles de calificación');
INSERT OR IGNORE INTO TempPermisos VALUES ('niveles.reordenar', 'Niveles de Calificación', 'Cambiar orden de niveles');
INSERT OR IGNORE INTO TempPermisos VALUES ('niveles.gestionar_grupos', 'Niveles de Calificación', 'Gestionar grupos de calificación');

-- =====================================================================================
-- PERMISOS ADICIONALES PARA MATERIAS Y MÓDULOS ACADÉMICOS
-- =====================================================================================
INSERT OR IGNORE INTO TempPermisos VALUES ('materias.ver', 'Materias', 'Ver materias del sistema');
INSERT OR IGNORE INTO TempPermisos VALUES ('materias.crear', 'Materias', 'Crear nuevas materias');
INSERT OR IGNORE INTO TempPermisos VALUES ('materias.editar', 'Materias', 'Editar materias');
INSERT OR IGNORE INTO TempPermisos VALUES ('materias.eliminar', 'Materias', 'Eliminar materias');
INSERT OR IGNORE INTO TempPermisos VALUES ('materias.asignar_rubricas', 'Materias', 'Asignar rúbricas a materias');
INSERT OR IGNORE INTO TempPermisos VALUES ('materias.crear_ofertas', 'Materias', 'Crear ofertas de materias');
INSERT OR IGNORE INTO TempPermisos VALUES ('materias.gestionar_prerrequisitos', 'Materias', 'Gestionar prerrequisitos de materias');

-- =====================================================================================
-- PERMISOS PARA SISTEMA DE ADMINISTRACION
-- =====================================================================================
INSERT OR IGNORE INTO TempPermisos VALUES ('admin.acceso_completo', 'Administración', 'Acceso completo al panel de administración');
INSERT OR IGNORE INTO TempPermisos VALUES ('admin.gestionar_sistema', 'Administración', 'Gestionar configuración del sistema');
INSERT OR IGNORE INTO TempPermisos VALUES ('admin.ver_estadisticas_globales', 'Administración', 'Ver estadísticas globales del sistema');

-- Mostrar resumen de permisos cargados
SELECT 'Total de permisos cargados: ' || COUNT(*) FROM TempPermisos;
SELECT 'Permisos por categoría:';
SELECT Categoria, COUNT(*) as Cantidad FROM TempPermisos GROUP BY Categoria ORDER BY Cantidad DESC;

-- =====================================================================================
-- PASO 5: Asignar permisos directos al usuario admin (solo si existe)
-- =====================================================================================

-- Asignar todos los permisos directamente al usuario admin@rubricas.edu
INSERT OR IGNORE INTO AspNetUserClaims (UserId, ClaimType, ClaimValue)
SELECT 
    u.Id,
    'permission',
    tp.Permiso
FROM AspNetUsers u, TempPermisos tp
WHERE (u.UserName = 'admin@rubricas.edu' OR u.Email = 'admin@rubricas.edu');

SELECT 'Permisos directos asignados al usuario admin (si existe)';

-- =====================================================================================
-- PASO 6: Asignar permisos completos a roles SuperAdmin y Admin
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

SELECT 'Permisos completos asignados a roles SuperAdmin y Admin';

-- =====================================================================================
-- PASO 7: Configurar estado del usuario admin (solo si existe)
-- =====================================================================================

-- Asegurar que el usuario esté activo y configurado correctamente
UPDATE AspNetUsers 
SET 
    IsActive = 1,
    Activo = 1,
    EmailConfirmed = 1,
    PhoneNumberConfirmed = 1,
    LockoutEnabled = 0,
    LockoutEnd = NULL,
    AccessFailedCount = 0,
    LastLoginDate = datetime('now')
WHERE UserName = 'admin@rubricas.edu' OR Email = 'admin@rubricas.edu';

SELECT 'Usuario admin configurado como activo y confirmado (si existe)';

-- =====================================================================================
-- PASO 8: Resumen final y verificación
-- =====================================================================================

SELECT '======================================================================================';
SELECT 'CONFIGURACIÓN COMPLETADA - RESUMEN FINAL';
SELECT '======================================================================================';

-- Verificar si el usuario existe
SELECT 
    CASE 
        WHEN EXISTS(SELECT 1 FROM AspNetUsers WHERE UserName = 'admin@rubricas.edu' OR Email = 'admin@rubricas.edu')
        THEN 'ÉXITO: Usuario admin@rubricas.edu encontrado y configurado'
        ELSE 'ADVERTENCIA: Usuario admin@rubricas.edu NO EXISTE - Debe crearlo primero'
    END as EstadoUsuario;

-- Información del usuario (solo si existe)
SELECT 
    'INFORMACIÓN DEL USUARIO:' as Seccion;

SELECT 
    u.UserName as Usuario,
    u.Email as Email,
    CASE WHEN u.IsActive = 1 THEN 'Activo' ELSE 'Inactivo' END as Estado,
    CASE WHEN u.EmailConfirmed = 1 THEN 'Confirmado' ELSE 'No confirmado' END as EmailStatus,
    u.CreatedDate as FechaCreacion
FROM AspNetUsers u
WHERE u.UserName = 'admin@rubricas.edu' OR u.Email = 'admin@rubricas.edu';

-- Roles asignados (solo si el usuario existe)
SELECT 'ROLES ASIGNADOS:' as Seccion;
SELECT 
    r.Name as Rol,
    r.NormalizedName as RolNormalizado
FROM AspNetUsers u
JOIN AspNetUserRoles ur ON u.Id = ur.UserId
JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE u.UserName = 'admin@rubricas.edu' OR u.Email = 'admin@rubricas.edu'
ORDER BY r.Name;

-- Conteo de permisos directos
SELECT 'PERMISOS DIRECTOS DEL USUARIO:' as Seccion;
SELECT 
    COUNT(*) as TotalPermisosDirectos
FROM AspNetUsers u
JOIN AspNetUserClaims uc ON u.Id = uc.UserId
WHERE (u.UserName = 'admin@rubricas.edu' OR u.Email = 'admin@rubricas.edu')
AND uc.ClaimType = 'permission';

-- Permisos por categoría
SELECT 'PERMISOS POR CATEGORÍA:' as Seccion;
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
        WHEN uc.ClaimValue LIKE 'materias.%' THEN 'Materias'
        WHEN uc.ClaimValue LIKE 'admin.%' THEN 'Administración'
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
        WHEN uc.ClaimValue LIKE 'materias.%' THEN 'Materias'
        WHEN uc.ClaimValue LIKE 'admin.%' THEN 'Administración'
        ELSE 'Otros'
    END
ORDER BY CantidadPermisos DESC;

-- Verificación de roles en el sistema
SELECT 'ROLES DEL SISTEMA:' as Seccion;
SELECT 
    r.Name as Rol,
    COUNT(rc.ClaimValue) as TotalPermisos
FROM AspNetRoles r
LEFT JOIN AspNetRoleClaims rc ON r.Id = rc.RoleId AND rc.ClaimType = 'permission'
GROUP BY r.Id, r.Name
ORDER BY r.Name;

-- Limpiar tabla temporal
DROP TABLE IF EXISTS TempPermisos;

SELECT '======================================================================================';
SELECT 'SCRIPT FINALIZADO';
SELECT 'Total permisos asignados en el sistema: ' || (SELECT COUNT(*) FROM TempPermisos);
SELECT 'admin@rubricas.edu tiene acceso completo al sistema (si el usuario existe)';
SELECT '======================================================================================';

-- =====================================================================================
-- INSTRUCCIONES DE USO:
-- 
-- 1. Ejecute este script DESPUÉS de que la base de datos haya sido creada
-- 2. Si el usuario admin@rubricas.edu no existe, créelo primero usando la interfaz web
-- 3. Luego ejecute este script para asignar todos los permisos
-- 4. Para verificar los permisos, use el script VerifyAdminPermissions.sql
-- 
-- NOTA: Este script es seguro de ejecutar múltiples veces (usa INSERT OR IGNORE)
-- =====================================================================================