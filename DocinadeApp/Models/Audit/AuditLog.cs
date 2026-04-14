// Este archivo se mantiene por compatibilidad
// Las clases se han movido a archivos separados para mejor organización:

// AuditLogEntity.cs - Entidad principal AuditLog con campos adicionales
// AuditActionTypesNew.cs - Tipos de acciones organizados por módulos
// AuditEntityTypes.cs - Tipos de entidades (ya existente)
// AuditLogLevels.cs - Niveles de log con métodos auxiliares
// AuditQueryModels.cs - Modelos para filtros, consultas y configuración

// NOTA: Este archivo puede ser eliminado una vez que todas las referencias
// se hayan actualizado para usar los nuevos archivos separados.

// Para migrar:
// 1. Reemplazar referencias a AuditLog por AuditLogEntity
// 2. Usar AuditActionTypesNew en lugar de las constantes duplicadas
// 3. Actualizar servicios para usar AuditQueryModels




namespace RubricasApp.Web.Models.Audit
{
    // Alias para compatibilidad hacia atrás


    /// <summary>
    /// Marcador para mantener el namespace activo
    /// Este archivo contiene referencias legacy - usar los archivos específicos
    /// </summary>
    internal static class LegacyAuditModelsPlaceholder
    {
        // Constantes legacy para compatibilidad hacia atrás
        public const string LOGIN = "Auth.Login";
        public const string LOGOUT = "Auth.Logout";
        public const string CREATE = "Create";
        public const string UPDATE = "Update";
        public const string DELETE = "Delete";

        // Nota: Estas constantes están duplicadas en AuditActionTypesNew.cs
        // Migrar gradualmente a la nueva estructura
    }
}