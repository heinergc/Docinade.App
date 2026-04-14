namespace RubricasApp.Web.Interfaces
{
    /// <summary>
    /// Interface para entidades que pertenecen a un usuario específico
    /// </summary>
    public interface IUserOwned
    {
        string? CreadoPorId { get; set; }
        
        /// <summary>
        /// Verifica si la entidad pertenece al usuario especificado
        /// </summary>
        bool BelongsToUser(string userId);
    }
}
