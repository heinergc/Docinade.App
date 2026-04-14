using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using DocinadeApp.Services.Permissions;

namespace DocinadeApp.Authorization
{
    /// <summary>
    /// Proveedor de políticas de autorización personalizado para manejar permisos dinámicamente
    /// </summary>
    public class PermissionPolicyProvider : IAuthorizationPolicyProvider
    {
        private readonly DefaultAuthorizationPolicyProvider _defaultProvider;
        private readonly ILogger<PermissionPolicyProvider>? _logger;

        public PermissionPolicyProvider(IOptions<AuthorizationOptions> options, ILogger<PermissionPolicyProvider>? logger = null)
        {
            _defaultProvider = new DefaultAuthorizationPolicyProvider(options);
            _logger = logger;
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            return _defaultProvider.GetDefaultPolicyAsync();
        }

        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
        {
            return _defaultProvider.GetFallbackPolicyAsync();
        }

        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            _logger?.LogInformation($"🔍 Solicitando política: '{policyName}'");
            
            // Verificar si el policy name es directamente un permiso
            if (IsPermissionPolicy(policyName))
            {
                _logger?.LogInformation($"✅ Creando política dinámica para permiso: '{policyName}'");
                
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddRequirements(new PermissionRequirement(policyName))
                    .Build();

                return Task.FromResult<AuthorizationPolicy?>(policy);
            }

            // Si es una política de permiso con prefijo, crear dinámicamente
            if (policyName.StartsWith("RequirePermission:"))
            {
                var permission = policyName.Substring("RequirePermission:".Length);
                _logger?.LogInformation($"✅ Creando política con prefijo para permiso: '{permission}'");
                
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddRequirements(new PermissionRequirement(permission))
                    .Build();

                return Task.FromResult<AuthorizationPolicy?>(policy);
            }

            _logger?.LogWarning($"⚠️ Delegando política al proveedor por defecto: '{policyName}'");
            
            // Para otras políticas, usar el proveedor por defecto
            return _defaultProvider.GetPolicyAsync(policyName);
        }

        private bool IsPermissionPolicy(string policyName)
        {
            // Verificar si sigue el patrón de permisos
            // Acepta tanto formato "Configuracion.VER" como "configuracion.ver" o "configuracion.editar_sistema"
            if (string.IsNullOrEmpty(policyName) || !policyName.Contains('.')) 
            {
                _logger?.LogDebug($"Política '{policyName}' no sigue el patrón de permisos (no contiene punto)");
                return false;
            }
            
            var parts = policyName.Split('.');
            if (parts.Length != 2) 
            {
                _logger?.LogDebug($"Política '{policyName}' no tiene exactamente 2 partes separadas por punto");
                return false;
            }
            
            var module = parts[0].ToLower();
            var action = parts[1].ToLower(); // Cambiar a minúsculas para consistencia
            
            // Módulos válidos
            var validModules = new[] 
            {
                "usuarios", "roles", "rubricas", "evaluaciones", 
                "estudiantes", "profesores", "instituciones", "reportes", 
                "configuracion", "auditoria", "periodos", "niveles", 
                "items_evaluacion", "instrumentos_evaluacion", "asistencia",
                "tipos_grupo", "grupos_estudiantes", "materias", 
                "instrumento_materias", "instrumento_rubrica", "cuaderno_calificador",
                "sea", "admin"
            };
            
            // Acciones válidas (en minúsculas para facilitar comparación)
            var validActions = new[] 
            {
                "ver", "crear", "editar", "eliminar", "gestionar", 
                "manage", "healthcheck", "viewall", "manage_roles", "manage_permissions",
                "gestionar_roles", "gestionar_permisos", "ver_metricas", "exportar", "limpiar",
                "inicializar", "sincronizar_permisos", "verificar_estado",
                "editar_sistema", "editar_seguridad", "ver_logs", "limpiar_logs",
                "modo_mantenimiento", "backup", "restaurar", "ver_todas", "editar_todas",
                "eliminar_todas", "duplicar", "publicar", "archivar", "compartir",
                "importar", "evaluar", "revisar", "aprobar", "finalizar", "reabrir",
                "ver_resultados", "ver_estadisticas", "ver_historial", "ver_notas",
                "editar_notas", "ver_basicos", "ver_avanzados", "ver_todos",
                "crear_personalizados", "programar", "ver_estadisticas_institucionales",
                "ver_accesos", "ver_cambios", "ver_errores", "activar", "cerrar",
                "gestionar_calendario", "reordenar", "gestionar_grupos", "bloquear",
                "cambiar_roles", "ver_perfil_completo", "resetpassword",
                "view", "create", "edit", "delete", "cleanup", "viewstatistics",
                "export", "initialize", "syncpermissions", "gestionar_categorias",
                "asignar_materias", "activar_desactivar", "gestionar_configuracion",
                "imprimir", "ver_detalles", "cambiar_estado", "asignar_grupos",
                "gestionar_horarios", "asignar_estudiantes", "desasignar_estudiantes",
                "desasignar_materias", "exportar_estudiantes", "gestionar_asignaciones",
                "quitar_instrumentos", "ver_instrumentos", "gestionar_relaciones",
                "ver_por_instrumento", "ver_por_rubrica", "asignar_materia",
                "ver_tradicional", "exportar_excel", "exportar_dinamico",
                "ver_dashboard", "inicializar_sistema", "acceso_completo",
                "tomar_asistencia", "editar_asistencia", "eliminar_registros",
                "ver_resumen", "ver_diagnostico", "configurar_parametros",
                "items_ver", "items_crear", "items_editar", "items_eliminar",
                "ver_reporte", "exportar_csv", "exportar_excel", "exportar_pdf", "configurar"
            };
            
            var isValid = validModules.Contains(module) && validActions.Contains(action);
            
            _logger?.LogInformation($"🔎 Política '{policyName}' - Módulo: '{module}', Acción: '{action}', Válida: {isValid}");
            
            return isValid;
        }
    }
}