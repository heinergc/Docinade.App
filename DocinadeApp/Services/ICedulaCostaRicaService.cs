namespace RubricasApp.Web.Services
{
    public interface ICedulaCostaRicaService
    {
        Task<CedulaInfoResult> ConsultarCedulaAsync(string cedula);
    }
}
