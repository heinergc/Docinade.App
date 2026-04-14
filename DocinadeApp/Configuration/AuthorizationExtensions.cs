using Microsoft.AspNetCore.Authorization;
using DocinadeApp.Authorization;
using DocinadeApp.Services.Permissions;
using DocinadeApp.Models.Permissions;

namespace DocinadeApp.Configuration
{
    /// <summary>
    /// Extensiones para configurar autorización y permisos en el sistema
    /// </summary>
    public static class AuthorizationExtensions
    {
        /// <summary>
        /// Configura la autorización basada en permisos para el sistema
        /// </summary>
        public static IServiceCollection AddCustomAuthorization(this IServiceCollection services)
        {
            // Agregar servicios de permisos
            services.AddScoped<IPermissionService, PermissionService>();

            // Configurar políticas personalizadas
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

            // Configurar autorización con políticas
            services.AddAuthorization(options =>
            {
                // Políticas basadas en roles (legacy - mantener para compatibilidad)
                options.AddPolicy("RequireAdministratorRole", policy =>
                    policy.RequireRole(ApplicationRoles.SuperAdministrador, ApplicationRoles.Administrador));
                
                options.AddPolicy("RequireCoordinatorRole", policy =>
                    policy.RequireRole(ApplicationRoles.SuperAdministrador, ApplicationRoles.Administrador, ApplicationRoles.Coordinador));
                
                options.AddPolicy("RequireDocenteRole", policy =>
                    policy.RequireRole(ApplicationRoles.SuperAdministrador, ApplicationRoles.Administrador, ApplicationRoles.Coordinador, ApplicationRoles.Docente));
                
                options.AddPolicy("RequireEvaluadorRole", policy =>
                    policy.RequireRole(ApplicationRoles.SuperAdministrador, ApplicationRoles.Administrador, ApplicationRoles.Coordinador, ApplicationRoles.Docente, ApplicationRoles.Evaluador));

                // Políticas explícitas para permisos críticos (para debugging)
                options.AddPolicy("configuracion.ver", policy =>
                    policy.Requirements.Add(new PermissionRequirement("configuracion.ver")));
                
                options.AddPolicy("configuracion.editar_sistema", policy =>
                    policy.Requirements.Add(new PermissionRequirement("configuracion.editar_sistema")));
                
                options.AddPolicy("configuracion.editar_seguridad", policy =>
                    policy.Requirements.Add(new PermissionRequirement("configuracion.editar_seguridad")));
                
                options.AddPolicy("configuracion.ver_logs", policy =>
                    policy.Requirements.Add(new PermissionRequirement("configuracion.ver_logs")));
                
                options.AddPolicy("configuracion.healthcheck", policy =>
                    policy.Requirements.Add(new PermissionRequirement("configuracion.healthcheck")));
                
                options.AddPolicy("configuracion.modo_mantenimiento", policy =>
                    policy.Requirements.Add(new PermissionRequirement("configuracion.modo_mantenimiento")));
                
                options.AddPolicy("configuracion.gestionar", policy =>
                    policy.Requirements.Add(new PermissionRequirement("configuracion.gestionar")));
                
                options.AddPolicy("configuracion.gestionar_roles", policy =>
                    policy.Requirements.Add(new PermissionRequirement("configuracion.gestionar_roles")));
                
                options.AddPolicy("configuracion.gestionar_permisos", policy =>
                    policy.Requirements.Add(new PermissionRequirement("configuracion.gestionar_permisos")));

                options.AddPolicy("configuracion.configurar_email", policy =>
                    policy.Requirements.Add(new PermissionRequirement("configuracion.configurar_email")));

                options.AddPolicy("usuarios.ver", policy =>
                    policy.Requirements.Add(new PermissionRequirement("usuarios.ver")));
                
                options.AddPolicy("roles.gestionar", policy =>
                    policy.Requirements.Add(new PermissionRequirement("roles.gestionar")));
                
                options.AddPolicy("auditoria.ver", policy =>
                    policy.Requirements.Add(new PermissionRequirement("auditoria.ver")));
                
                options.AddPolicy("auditoria.ver_metricas", policy =>
                    policy.Requirements.Add(new PermissionRequirement("auditoria.ver_metricas")));
                
                options.AddPolicy("auditoria.exportar", policy =>
                    policy.Requirements.Add(new PermissionRequirement("auditoria.exportar")));
                
                options.AddPolicy("auditoria.limpiar", policy =>
                    policy.Requirements.Add(new PermissionRequirement("auditoria.limpiar")));

                // Políticas para Items de Evaluación
                options.AddPolicy("items_evaluacion.ver", policy =>
                    policy.Requirements.Add(new PermissionRequirement("items_evaluacion.ver")));
                
                options.AddPolicy("items_evaluacion.crear", policy =>
                    policy.Requirements.Add(new PermissionRequirement("items_evaluacion.crear")));
                
                options.AddPolicy("items_evaluacion.editar", policy =>
                    policy.Requirements.Add(new PermissionRequirement("items_evaluacion.editar")));
                
                options.AddPolicy("items_evaluacion.eliminar", policy =>
                    policy.Requirements.Add(new PermissionRequirement("items_evaluacion.eliminar")));
                
                options.AddPolicy("items_evaluacion.duplicar", policy =>
                    policy.Requirements.Add(new PermissionRequirement("items_evaluacion.duplicar")));
                
                options.AddPolicy("items_evaluacion.importar", policy =>
                    policy.Requirements.Add(new PermissionRequirement("items_evaluacion.importar")));
                
                options.AddPolicy("items_evaluacion.exportar", policy =>
                    policy.Requirements.Add(new PermissionRequirement("items_evaluacion.exportar")));
                
                options.AddPolicy("items_evaluacion.reordenar", policy =>
                    policy.Requirements.Add(new PermissionRequirement("items_evaluacion.reordenar")));
                
                options.AddPolicy("items_evaluacion.gestionar_categorias", policy =>
                    policy.Requirements.Add(new PermissionRequirement("items_evaluacion.gestionar_categorias")));

                // Políticas para Instrumentos de Evaluación
                options.AddPolicy("instrumentos_evaluacion.ver", policy =>
                    policy.Requirements.Add(new PermissionRequirement("instrumentos_evaluacion.ver")));
                
                options.AddPolicy("instrumentos_evaluacion.crear", policy =>
                    policy.Requirements.Add(new PermissionRequirement("instrumentos_evaluacion.crear")));
                
                options.AddPolicy("instrumentos_evaluacion.editar", policy =>
                    policy.Requirements.Add(new PermissionRequirement("instrumentos_evaluacion.editar")));
                
                options.AddPolicy("instrumentos_evaluacion.eliminar", policy =>
                    policy.Requirements.Add(new PermissionRequirement("instrumentos_evaluacion.eliminar")));
                
                options.AddPolicy("instrumentos_evaluacion.duplicar", policy =>
                    policy.Requirements.Add(new PermissionRequirement("instrumentos_evaluacion.duplicar")));
                
                options.AddPolicy("instrumentos_evaluacion.asignar_materias", policy =>
                    policy.Requirements.Add(new PermissionRequirement("instrumentos_evaluacion.asignar_materias")));
                
                options.AddPolicy("instrumentos_evaluacion.activar_desactivar", policy =>
                    policy.Requirements.Add(new PermissionRequirement("instrumentos_evaluacion.activar_desactivar")));
                
                options.AddPolicy("instrumentos_evaluacion.gestionar_configuracion", policy =>
                    policy.Requirements.Add(new PermissionRequirement("instrumentos_evaluacion.gestionar_configuracion")));
                
                options.AddPolicy("instrumentos_evaluacion.exportar", policy =>
                    policy.Requirements.Add(new PermissionRequirement("instrumentos_evaluacion.exportar")));
                
                options.AddPolicy("instrumentos_evaluacion.importar", policy =>
                    policy.Requirements.Add(new PermissionRequirement("instrumentos_evaluacion.importar")));
                
                options.AddPolicy("instrumentos_evaluacion.ver_estadisticas", policy =>
                    policy.Requirements.Add(new PermissionRequirement("instrumentos_evaluacion.ver_estadisticas")));
                
                options.AddPolicy("instrumentos_evaluacion.publicar", policy =>
                    policy.Requirements.Add(new PermissionRequirement("instrumentos_evaluacion.publicar")));

                // Políticas para Niveles de Calificación
                options.AddPolicy("niveles.ver", policy =>
                    policy.Requirements.Add(new PermissionRequirement("niveles.ver")));
                
                options.AddPolicy("niveles.crear", policy =>
                    policy.Requirements.Add(new PermissionRequirement("niveles.crear")));
                
                options.AddPolicy("niveles.editar", policy =>
                    policy.Requirements.Add(new PermissionRequirement("niveles.editar")));
                
                options.AddPolicy("niveles.eliminar", policy =>
                    policy.Requirements.Add(new PermissionRequirement("niveles.eliminar")));
                
                options.AddPolicy("niveles.reordenar", policy =>
                    policy.Requirements.Add(new PermissionRequirement("niveles.reordenar")));
                
                options.AddPolicy("niveles.gestionar_grupos", policy =>
                    policy.Requirements.Add(new PermissionRequirement("niveles.gestionar_grupos")));

                // Políticas para Rúbricas
                options.AddPolicy("rubricas.ver", policy =>
                    policy.Requirements.Add(new PermissionRequirement("rubricas.ver")));
                
                options.AddPolicy("rubricas.ver_todas", policy =>
                    policy.Requirements.Add(new PermissionRequirement("rubricas.ver_todas")));
                
                options.AddPolicy("rubricas.crear", policy =>
                    policy.Requirements.Add(new PermissionRequirement("rubricas.crear")));
                
                options.AddPolicy("rubricas.editar", policy =>
                    policy.Requirements.Add(new PermissionRequirement("rubricas.editar")));
                
                options.AddPolicy("rubricas.editar_todas", policy =>
                    policy.Requirements.Add(new PermissionRequirement("rubricas.editar_todas")));
                
                options.AddPolicy("rubricas.eliminar", policy =>
                    policy.Requirements.Add(new PermissionRequirement("rubricas.eliminar")));
                
                options.AddPolicy("rubricas.eliminar_todas", policy =>
                    policy.Requirements.Add(new PermissionRequirement("rubricas.eliminar_todas")));
                
                options.AddPolicy("rubricas.duplicar", policy =>
                    policy.Requirements.Add(new PermissionRequirement("rubricas.duplicar")));
                
                options.AddPolicy("rubricas.publicar", policy =>
                    policy.Requirements.Add(new PermissionRequirement("rubricas.publicar")));
                
                options.AddPolicy("rubricas.archivar", policy =>
                    policy.Requirements.Add(new PermissionRequirement("rubricas.archivar")));
                
                options.AddPolicy("rubricas.compartir", policy =>
                    policy.Requirements.Add(new PermissionRequirement("rubricas.compartir")));
                
                options.AddPolicy("rubricas.exportar", policy =>
                    policy.Requirements.Add(new PermissionRequirement("rubricas.exportar")));
                
                options.AddPolicy("rubricas.importar", policy =>
                    policy.Requirements.Add(new PermissionRequirement("rubricas.importar")));

                // Políticas para Asistencia
                options.AddPolicy("asistencia.ver", policy =>
                    policy.Requirements.Add(new PermissionRequirement("asistencia.ver")));
                
                options.AddPolicy("asistencia.tomar_asistencia", policy =>
                    policy.Requirements.Add(new PermissionRequirement("asistencia.tomar_asistencia")));
                
                options.AddPolicy("asistencia.editar_asistencia", policy =>
                    policy.Requirements.Add(new PermissionRequirement("asistencia.editar_asistencia")));
                
                options.AddPolicy("asistencia.eliminar_registros", policy =>
                    policy.Requirements.Add(new PermissionRequirement("asistencia.eliminar_registros")));
                
                options.AddPolicy("asistencia.ver_resumen", policy =>
                    policy.Requirements.Add(new PermissionRequirement("asistencia.ver_resumen")));
                
                options.AddPolicy("asistencia.ver_diagnostico", policy =>
                    policy.Requirements.Add(new PermissionRequirement("asistencia.ver_diagnostico")));
                
                options.AddPolicy("asistencia.imprimir", policy =>
                    policy.Requirements.Add(new PermissionRequirement("asistencia.imprimir")));
                
                options.AddPolicy("asistencia.exportar", policy =>
                    policy.Requirements.Add(new PermissionRequirement("asistencia.exportar")));
                
                options.AddPolicy("asistencia.gestionar_grupos", policy =>
                    policy.Requirements.Add(new PermissionRequirement("asistencia.gestionar_grupos")));
                
                options.AddPolicy("asistencia.ver_estadisticas", policy =>
                    policy.Requirements.Add(new PermissionRequirement("asistencia.ver_estadisticas")));
                
                options.AddPolicy("asistencia.configurar_parametros", policy =>
                    policy.Requirements.Add(new PermissionRequirement("asistencia.configurar_parametros")));

                // Políticas para Tipos de Grupo
                options.AddPolicy("tipos_grupo.ver", policy =>
                    policy.Requirements.Add(new PermissionRequirement("tipos_grupo.ver")));
                
                options.AddPolicy("tipos_grupo.crear", policy =>
                    policy.Requirements.Add(new PermissionRequirement("tipos_grupo.crear")));
                
                options.AddPolicy("tipos_grupo.editar", policy =>
                    policy.Requirements.Add(new PermissionRequirement("tipos_grupo.editar")));
                
                options.AddPolicy("tipos_grupo.eliminar", policy =>
                    policy.Requirements.Add(new PermissionRequirement("tipos_grupo.eliminar")));
                
                options.AddPolicy("tipos_grupo.ver_detalles", policy =>
                    policy.Requirements.Add(new PermissionRequirement("tipos_grupo.ver_detalles")));
                
                options.AddPolicy("tipos_grupo.cambiar_estado", policy =>
                    policy.Requirements.Add(new PermissionRequirement("tipos_grupo.cambiar_estado")));
                
                options.AddPolicy("tipos_grupo.gestionar", policy =>
                    policy.Requirements.Add(new PermissionRequirement("tipos_grupo.gestionar")));

                // Políticas para Grupos de Estudiantes
                options.AddPolicy("grupos_estudiantes.ver", policy =>
                    policy.Requirements.Add(new PermissionRequirement("grupos_estudiantes.ver")));
                
                options.AddPolicy("grupos_estudiantes.crear", policy =>
                    policy.Requirements.Add(new PermissionRequirement("grupos_estudiantes.crear")));
                
                options.AddPolicy("grupos_estudiantes.editar", policy =>
                    policy.Requirements.Add(new PermissionRequirement("grupos_estudiantes.editar")));
                
                options.AddPolicy("grupos_estudiantes.eliminar", policy =>
                    policy.Requirements.Add(new PermissionRequirement("grupos_estudiantes.eliminar")));
                
                options.AddPolicy("grupos_estudiantes.ver_detalles", policy =>
                    policy.Requirements.Add(new PermissionRequirement("grupos_estudiantes.ver_detalles")));
                
                options.AddPolicy("grupos_estudiantes.asignar_estudiantes", policy =>
                    policy.Requirements.Add(new PermissionRequirement("grupos_estudiantes.asignar_estudiantes")));
                
                options.AddPolicy("grupos_estudiantes.desasignar_estudiantes", policy =>
                    policy.Requirements.Add(new PermissionRequirement("grupos_estudiantes.desasignar_estudiantes")));
                
                options.AddPolicy("grupos_estudiantes.asignar_materias", policy =>
                    policy.Requirements.Add(new PermissionRequirement("grupos_estudiantes.asignar_materias")));
                
                options.AddPolicy("grupos_estudiantes.desasignar_materias", policy =>
                    policy.Requirements.Add(new PermissionRequirement("grupos_estudiantes.desasignar_materias")));
                
                options.AddPolicy("grupos_estudiantes.ver_estadisticas", policy =>
                    policy.Requirements.Add(new PermissionRequirement("grupos_estudiantes.ver_estadisticas")));
                
                options.AddPolicy("grupos_estudiantes.exportar", policy =>
                    policy.Requirements.Add(new PermissionRequirement("grupos_estudiantes.exportar")));
                
                options.AddPolicy("grupos_estudiantes.exportar_estudiantes", policy =>
                    policy.Requirements.Add(new PermissionRequirement("grupos_estudiantes.exportar_estudiantes")));
                
                options.AddPolicy("grupos_estudiantes.gestionar_asignaciones", policy =>
                    policy.Requirements.Add(new PermissionRequirement("grupos_estudiantes.gestionar_asignaciones")));

                // Políticas para Materias
                options.AddPolicy("materias.ver", policy =>
                    policy.Requirements.Add(new PermissionRequirement("materias.ver")));
                
                options.AddPolicy("materias.crear", policy =>
                    policy.Requirements.Add(new PermissionRequirement("materias.crear")));
                
                options.AddPolicy("materias.editar", policy =>
                    policy.Requirements.Add(new PermissionRequirement("materias.editar")));
                
                options.AddPolicy("materias.eliminar", policy =>
                    policy.Requirements.Add(new PermissionRequirement("materias.eliminar")));
                
                options.AddPolicy("materias.ver_detalles", policy =>
                    policy.Requirements.Add(new PermissionRequirement("materias.ver_detalles")));
                
                options.AddPolicy("materias.asignar_instrumentos", policy =>
                    policy.Requirements.Add(new PermissionRequirement("materias.asignar_instrumentos")));
                
                options.AddPolicy("materias.quitar_instrumentos", policy =>
                    policy.Requirements.Add(new PermissionRequirement("materias.quitar_instrumentos")));
                
                options.AddPolicy("materias.ver_instrumentos", policy =>
                    policy.Requirements.Add(new PermissionRequirement("materias.ver_instrumentos")));
                
                options.AddPolicy("materias.gestionar_asignaciones", policy =>
                    policy.Requirements.Add(new PermissionRequirement("materias.gestionar_asignaciones")));

                // Políticas para Instrumento-Materias
                options.AddPolicy("instrumento_materias.ver", policy =>
                    policy.Requirements.Add(new PermissionRequirement("instrumento_materias.ver")));
                
                options.AddPolicy("instrumento_materias.crear", policy =>
                    policy.Requirements.Add(new PermissionRequirement("instrumento_materias.crear")));
                
                options.AddPolicy("instrumento_materias.editar", policy =>
                    policy.Requirements.Add(new PermissionRequirement("instrumento_materias.editar")));
                
                options.AddPolicy("instrumento_materias.eliminar", policy =>
                    policy.Requirements.Add(new PermissionRequirement("instrumento_materias.eliminar")));
                
                options.AddPolicy("instrumento_materias.ver_detalles", policy =>
                    policy.Requirements.Add(new PermissionRequirement("instrumento_materias.ver_detalles")));
                
                options.AddPolicy("instrumento_materias.gestionar_relaciones", policy =>
                    policy.Requirements.Add(new PermissionRequirement("instrumento_materias.gestionar_relaciones")));

                // Políticas para Instrumento-Rúbrica
                options.AddPolicy("instrumento_rubrica.ver", policy =>
                    policy.Requirements.Add(new PermissionRequirement("instrumento_rubrica.ver")));
                
                options.AddPolicy("instrumento_rubrica.crear", policy =>
                    policy.Requirements.Add(new PermissionRequirement("instrumento_rubrica.crear")));
                
                options.AddPolicy("instrumento_rubrica.editar", policy =>
                    policy.Requirements.Add(new PermissionRequirement("instrumento_rubrica.editar")));
                
                options.AddPolicy("instrumento_rubrica.eliminar", policy =>
                    policy.Requirements.Add(new PermissionRequirement("instrumento_rubrica.eliminar")));
                
                options.AddPolicy("instrumento_rubrica.ver_detalles", policy =>
                    policy.Requirements.Add(new PermissionRequirement("instrumento_rubrica.ver_detalles")));
                
                options.AddPolicy("instrumento_rubrica.ver_por_instrumento", policy =>
                    policy.Requirements.Add(new PermissionRequirement("instrumento_rubrica.ver_por_instrumento")));
                
                options.AddPolicy("instrumento_rubrica.ver_por_rubrica", policy =>
                    policy.Requirements.Add(new PermissionRequirement("instrumento_rubrica.ver_por_rubrica")));
                
                options.AddPolicy("instrumento_rubrica.asignar_materia", policy =>
                    policy.Requirements.Add(new PermissionRequirement("instrumento_rubrica.asignar_materia")));
                
                options.AddPolicy("instrumento_rubrica.gestionar_relaciones", policy =>
                    policy.Requirements.Add(new PermissionRequirement("instrumento_rubrica.gestionar_relaciones")));

                // Políticas para Cuaderno Calificador
                options.AddPolicy("cuaderno_calificador.ver", policy =>
                    policy.Requirements.Add(new PermissionRequirement("cuaderno_calificador.ver")));
                
                options.AddPolicy("cuaderno_calificador.ver_tradicional", policy =>
                    policy.Requirements.Add(new PermissionRequirement("cuaderno_calificador.ver_tradicional")));
                
                options.AddPolicy("cuaderno_calificador.exportar_excel", policy =>
                    policy.Requirements.Add(new PermissionRequirement("cuaderno_calificador.exportar_excel")));
                
                options.AddPolicy("cuaderno_calificador.exportar_dinamico", policy =>
                    policy.Requirements.Add(new PermissionRequirement("cuaderno_calificador.exportar_dinamico")));
                
                options.AddPolicy("cuaderno_calificador.gestionar", policy =>
                    policy.Requirements.Add(new PermissionRequirement("cuaderno_calificador.gestionar")));

                // Políticas para Administración
                options.AddPolicy("admin.ver_dashboard", policy =>
                    policy.Requirements.Add(new PermissionRequirement("admin.ver_dashboard")));
                
                options.AddPolicy("admin.inicializar_sistema", policy =>
                    policy.Requirements.Add(new PermissionRequirement("admin.inicializar_sistema")));
                
                options.AddPolicy("admin.sincronizar_permisos", policy =>
                    policy.Requirements.Add(new PermissionRequirement("admin.sincronizar_permisos")));
                
                options.AddPolicy("admin.ver_estadisticas", policy =>
                    policy.Requirements.Add(new PermissionRequirement("admin.ver_estadisticas")));
                
                options.AddPolicy("admin.verificar_salud", policy =>
                    policy.Requirements.Add(new PermissionRequirement("admin.verificar_salud")));
                
                options.AddPolicy("admin.gestionar_sistema", policy =>
                    policy.Requirements.Add(new PermissionRequirement("admin.gestionar_sistema")));
                
                options.AddPolicy("admin.acceso_completo", policy =>
                    policy.Requirements.Add(new PermissionRequirement("admin.acceso_completo")));

                // Política por defecto para usuarios autenticados (solo para áreas que requieren autenticación)
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();

                // No establecer FallbackPolicy para permitir acceso anónimo por defecto
                // Solo las acciones que requieran autenticación específicamente la tendrán
                // options.FallbackPolicy = null;
            });

            return services;
        }

        /// <summary>
        /// Inicializa los datos de autorización (roles y permisos)
        /// </summary>
        public static async Task InitializeAuthorizationDataAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;
            
            try
            {
                var permissionService = services.GetRequiredService<IPermissionService>();
                
                // Inicializar roles y permisos por defecto
                await permissionService.InitializeDefaultRolesAndPermissionsAsync();
                
                Console.WriteLine("✅ Autorización inicializada: roles y permisos configurados correctamente");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error inicializando autorización: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Verifica que la configuración de autorización sea correcta
        /// </summary>
        public static void ValidateAuthorizationConfiguration(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;
            
            try
            {
                // Verificar que los servicios estén registrados
                var policyProvider = services.GetService<IAuthorizationPolicyProvider>();
                var permissionService = services.GetService<IPermissionService>();
                var authorizationHandlers = services.GetServices<IAuthorizationHandler>();
                
                if (policyProvider == null)
                    throw new InvalidOperationException("IAuthorizationPolicyProvider no está registrado");
                
                if (permissionService == null)
                    throw new InvalidOperationException("IPermissionService no está registrado");
                
                if (!authorizationHandlers.Any(h => h is PermissionAuthorizationHandler))
                    throw new InvalidOperationException("PermissionAuthorizationHandler no está registrado");
                
                Console.WriteLine("✅ Configuración de autorización validada correctamente");
                Console.WriteLine($"   - PolicyProvider: {policyProvider.GetType().Name}");
                Console.WriteLine($"   - PermissionService: {permissionService.GetType().Name}");
                Console.WriteLine($"   - Authorization Handlers: {authorizationHandlers.Count()}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error validando configuración de autorización: {ex.Message}");
                throw;
            }
        }
    }
}