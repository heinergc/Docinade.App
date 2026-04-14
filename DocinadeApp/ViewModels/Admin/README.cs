// Organización de ViewModels del área de Administración
// =======================================================

// Este directorio contiene los ViewModels organizados por funcionalidad:

// 1. AdminBaseViewModels.cs
//    - AdminBaseViewModel: Clase base para todos los ViewModels de admin
//    - RoleSelectionItem: Item para selección de roles

// 2. DashboardViewModels.cs
//    - AdminDashboardViewModel: Dashboard principal de administración
//    - UserActivitySummary: Resumen de actividad de usuarios
//    - SystemAlert: Alertas del sistema

// 3. UserManagementViewModels.cs
//    - UserViewModel: Vista básica de usuario
//    - UserListViewModel: Lista paginada de usuarios
//    - UserDetailsViewModel: Detalles completos de usuario
//    - CreateUserViewModel: Creación de nuevo usuario
//    - EditUserViewModel: Edición de usuario existente

// 4. RoleManagementViewModels.cs
//    - RoleViewModel: Vista básica de rol
//    - RoleListViewModel: Lista paginada de roles
//    - CreateRoleViewModel: Creación de nuevo rol
//    - EditRoleViewModel: Edición de rol existente
//    - RoleDetailsViewModel: Detalles completos de rol
//    - PermissionSelectionItem: Item para selección de permisos

// 5. PermissionViewModels.cs
//    - PermissionViewModel: Vista básica de permiso
//    - PermissionListViewModel: Lista organizada de permisos
//    - AssignPermissionsToRoleViewModel: Asignación de permisos a roles
//    - AssignPermissionsToUserViewModel: Asignación de permisos a usuarios
//    - PermissionMatrixViewModel: Matriz de permisos por rol

// 6. AuditViewModels.cs
//    - AuditLogViewModel: Vista de log de auditoría
//    - AuditListViewModel: Lista paginada de logs
//    - AuditDetailsViewModel: Detalles de log específico
//    - AuditStatsViewModel: Estadísticas de auditoría
//    - ExportAuditViewModel: Exportación de logs
//    - ActionStatistic, UserStatistic, EntityStatistic: Estadísticas específicas

// 7. ConfigurationViewModels.cs
//    - SystemConfigurationViewModel: Configuración general del sistema
//    - SecurityConfigurationViewModel: Configuración de seguridad
//    - EmailConfigurationViewModel: Configuración de email
//    - DatabaseConfigurationViewModel: Configuración de base de datos
//    - SystemHealthViewModel: Estado de salud del sistema
//    - Clases de soporte para health checks

// Archivos legacy:
// - UserViewModels.cs: Marcador de compatibilidad (puede eliminarse)
// - AdminViewModels.cs.backup: Backup del archivo original con duplicados

namespace RubricasApp.Web.ViewModels.Admin
{
    // Este namespace contiene todos los ViewModels organizados
    // Usar los archivos específicos según la funcionalidad requerida
}