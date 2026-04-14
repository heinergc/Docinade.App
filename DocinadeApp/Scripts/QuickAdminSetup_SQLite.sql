-- =====================================================================================
-- Script SQLite RÁPIDO para asignar todos los permisos a admin@rubricas.edu
-- Sistema de Rúbricas Académicas  
-- =====================================================================================

-- Crear roles si no existen
INSERT OR IGNORE INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp) 
VALUES (lower(hex(randomblob(16))), 'SuperAdmin', 'SUPERADMIN', lower(hex(randomblob(16))));

INSERT OR IGNORE INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp) 
VALUES (lower(hex(randomblob(16))), 'Admin', 'ADMIN', lower(hex(randomblob(16))));

-- Asignar roles al usuario admin
INSERT OR IGNORE INTO AspNetUserRoles (UserId, RoleId)
SELECT u.Id, r.Id FROM AspNetUsers u, AspNetRoles r
WHERE (u.UserName = 'admin@rubricas.edu' OR u.Email = 'admin@rubricas.edu') AND r.Name = 'SuperAdmin';

INSERT OR IGNORE INTO AspNetUserRoles (UserId, RoleId)
SELECT u.Id, r.Id FROM AspNetUsers u, AspNetRoles r
WHERE (u.UserName = 'admin@rubricas.edu' OR u.Email = 'admin@rubricas.edu') AND r.Name = 'Admin';

-- Lista completa de permisos
CREATE TEMP TABLE Permisos (permiso TEXT);
INSERT INTO Permisos VALUES 
('usuarios.ver'), ('usuarios.crear'), ('usuarios.editar'), ('usuarios.eliminar'), ('usuarios.bloquear'), ('usuarios.cambiar_roles'), ('usuarios.ver_perfil_completo'), ('usuarios.exportar'), ('usuarios.importar'), ('usuarios.gestionar_roles'), ('usuarios.gestionar_permisos'), ('usuarios.restablecer_contraseña'),
('rubricas.ver'), ('rubricas.ver_todas'), ('rubricas.crear'), ('rubricas.editar'), ('rubricas.editar_todas'), ('rubricas.eliminar'), ('rubricas.eliminar_todas'), ('rubricas.duplicar'), ('rubricas.publicar'), ('rubricas.archivar'), ('rubricas.compartir'), ('rubricas.exportar'), ('rubricas.importar'),
('evaluaciones.ver'), ('evaluaciones.ver_todas'), ('evaluaciones.crear'), ('evaluaciones.editar'), ('evaluaciones.editar_todas'), ('evaluaciones.eliminar'), ('evaluaciones.eliminar_todas'), ('evaluaciones.evaluar'), ('evaluaciones.revisar'), ('evaluaciones.aprobar'), ('evaluaciones.finalizar'), ('evaluaciones.reabrir'), ('evaluaciones.exportar'), ('evaluaciones.ver_resultados'), ('evaluaciones.ver_estadisticas'),
('estudiantes.ver'), ('estudiantes.crear'), ('estudiantes.editar'), ('estudiantes.eliminar'), ('estudiantes.importar'), ('estudiantes.exportar'), ('estudiantes.ver_historial'), ('estudiantes.ver_notas'), ('estudiantes.editar_notas'),
('reportes.ver_basicos'), ('reportes.ver_avanzados'), ('reportes.ver_todos'), ('reportes.crear_personalizados'), ('reportes.exportar'), ('reportes.programar'), ('reportes.ver_estadisticas_institucionales'),
('configuracion.ver'), ('configuracion.editar_sistema'), ('configuracion.editar_seguridad'), ('configuracion.gestionar_roles'), ('configuracion.gestionar_permisos'), ('configuracion.backup'), ('configuracion.restaurar'), ('configuracion.ver_logs'), ('configuracion.limpiar_logs'), ('configuracion.modo_mantenimiento'), ('configuracion.gestionar'), ('configuracion.inicializar'), ('configuracion.sincronizar_permisos'), ('configuracion.verificar_estado'),
('auditoria.ver'), ('auditoria.ver_accesos'), ('auditoria.ver_cambios'), ('auditoria.ver_errores'), ('auditoria.exportar'), ('auditoria.limpiar'), ('auditoria.ver_metricas'),
('periodos.ver'), ('periodos.crear'), ('periodos.editar'), ('periodos.eliminar'), ('periodos.activar'), ('periodos.cerrar'), ('periodos.gestionar_calendario'),
('niveles.ver'), ('niveles.crear'), ('niveles.editar'), ('niveles.eliminar'), ('niveles.reordenar'), ('niveles.gestionar_grupos'),
('materias.ver'), ('materias.crear'), ('materias.editar'), ('materias.eliminar'), ('materias.asignar_rubricas'), ('materias.crear_ofertas'), ('materias.gestionar_prerrequisitos'),
('admin.acceso_completo'), ('admin.gestionar_sistema'), ('admin.ver_estadisticas_globales');

-- Asignar permisos directos al usuario
INSERT OR IGNORE INTO AspNetUserClaims (UserId, ClaimType, ClaimValue)
SELECT u.Id, 'permission', p.permiso
FROM AspNetUsers u, Permisos p
WHERE (u.UserName = 'admin@rubricas.edu' OR u.Email = 'admin@rubricas.edu');

-- Asignar permisos a roles SuperAdmin y Admin
INSERT OR IGNORE INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue)
SELECT r.Id, 'permission', p.permiso
FROM AspNetRoles r, Permisos p
WHERE r.Name IN ('SuperAdmin', 'Admin');

-- Activar usuario
UPDATE AspNetUsers SET IsActive = 1, Activo = 1, EmailConfirmed = 1, LockoutEnabled = 0, LockoutEnd = NULL, AccessFailedCount = 0
WHERE UserName = 'admin@rubricas.edu' OR Email = 'admin@rubricas.edu';

-- Limpiar
DROP TABLE Permisos;

-- Verificación
SELECT 'admin@rubricas.edu configurado con ' || COUNT(*) || ' permisos' as Resultado
FROM AspNetUsers u JOIN AspNetUserClaims uc ON u.Id = uc.UserId
WHERE (u.UserName = 'admin@rubricas.edu' OR u.Email = 'admin@rubricas.edu') AND uc.ClaimType = 'permission';