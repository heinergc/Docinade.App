// Organización de Models del área de Auditoría
// ===============================================

// Este directorio contiene los modelos organizados por funcionalidad:

// 1. AuditLogEntity.cs
//    - AuditLog: Entidad principal de auditoría con campos extendidos
//    - Incluye campos adicionales: DurationMs, SessionId, ClientInfo, Referrer, 
//      Metadata, HttpMethod, RequestUrl, ResponseStatusCode

// 2. AuditActionTypes.cs
//    - Tipos de acciones organizados por módulos (Auth, Usuarios, Roles, etc.)
//    - Métodos auxiliares: GetAllActions(), GetActionsByCategory(), IsValidAction()
//    - Estructura jerárquica por funcionalidad del sistema

// 3. AuditEntityTypes.cs
//    - Tipos de entidades del sistema para auditoría
//    - Métodos auxiliares: GetAllEntityTypes(), IsValidEntityType(), GetDisplayName()
//    - Mapeo de nombres técnicos a nombres amigables

// 4. AuditLogLevels.cs
//    - Niveles de log: TRACE, DEBUG, INFORMATION, WARNING, ERROR, CRITICAL
//    - Métodos auxiliares: GetDisplayName(), GetCssClass(), GetSeverityLevel(), GetIcon()
//    - Soporte para UI con colores e iconos

// 5. AuditQueryModels.cs
//    - AuditFilter: Filtros para consultas de auditoría
//    - AuditQueryResult: Resultado paginado de consultas
//    - AuditStatistics: Estadísticas completas del sistema
//    - TopUser, TopAction, RecentError: Modelos de estadísticas específicas
//    - AuditRetentionPolicy: Configuración de retención de logs
//    - AuditExportConfiguration: Configuración de exportación
//    - AuditAlertConfiguration: Configuración de alertas

// Archivos legacy:
// - AuditLog.cs: Marcador de compatibilidad con alias (AuditLog = AuditLogEntity)
// - *.legacy: Backups de archivos originales con duplicados
// - *.old: Versiones anteriores para referencia

// Características agregadas:
// ✅ Eliminación de duplicados entre archivos
// ✅ Organización modular por funcionalidad
// ✅ Métodos auxiliares para obtener listas y validaciones
// ✅ Soporte extendido para UI (colores, iconos, nombres amigables)
// ✅ Modelos de consulta y filtrado avanzados
// ✅ Configuración de retención y exportación
// ✅ Sistema de alertas configurable
// ✅ Estadísticas detalladas del sistema
// ✅ Compatibilidad hacia atrás con alias

// Mejoras implementadas:
// 🔄 Estructura jerárquica de acciones por módulo
// 📊 Estadísticas detalladas con múltiples métricas
// 🎨 Soporte para UI con CSS classes e iconos
// 📁 Configuración avanzada de exportación
// 🔔 Sistema de alertas por tipos de eventos
// 📈 Métricas de rendimiento y actividad
// 🗂️ Filtrado y paginación avanzados

namespace DocinadeApp.Models.Audit
{
    // Este namespace contiene todos los modelos de auditoría organizados
    // Usar los archivos específicos según la funcionalidad requerida
}