namespace DocinadeApp.Services
{
    public interface ICedulaCostaRicaService
    {
        Task<CedulaInfoResult> ConsultarCedulaAsync(string cedula);
    }
}
