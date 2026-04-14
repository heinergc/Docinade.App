namespace RubricasApp.Web.Services
{
    public interface IEstudianteImportService
    {
        Task<ImportResult> ImportarEstudiantesAsync(IFormFile archivo, int periodoAcademicoId);
    }
}
