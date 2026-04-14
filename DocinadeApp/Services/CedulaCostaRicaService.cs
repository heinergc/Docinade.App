using System.Text.Json;

namespace RubricasApp.Web.Services
{
    public class CedulaCostaRicaService : ICedulaCostaRicaService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CedulaCostaRicaService> _logger;

        public CedulaCostaRicaService(HttpClient httpClient, IConfiguration configuration, ILogger<CedulaCostaRicaService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<CedulaInfoResult> ConsultarCedulaAsync(string cedula)
        {
            try
            {
                // Verificar si la API está habilitada
                var enabled = _configuration.GetValue<bool>("ExternalApis:CedulaCostaRica:Enabled");
                if (!enabled)
                {
                    _logger.LogWarning("API de cédulas de Costa Rica está deshabilitada");
                    return new CedulaInfoResult { Success = false, Message = "Servicio no disponible" };
                }

                // Obtener la URL base
                var baseUrl = _configuration.GetValue<string>("ExternalApis:CedulaCostaRica:BaseUrl");
                if (string.IsNullOrEmpty(baseUrl))
                {
                    _logger.LogError("URL base de la API de cédulas no configurada");
                    return new CedulaInfoResult { Success = false, Message = "Servicio no configurado" };
                }

                // Limpiar la cédula (quitar guiones y espacios)
                var cedulaLimpia = cedula.Replace("-", "").Replace(" ", "").Trim();
                
                if (string.IsNullOrEmpty(cedulaLimpia) || cedulaLimpia.Length < 9)
                {
                    return new CedulaInfoResult { Success = false, Message = "Cédula inválida" };
                }

                // Construir la URL de consulta
                var url = $"{baseUrl}/{cedulaLimpia}";
                
                _logger.LogInformation($"Consultando cédula {cedulaLimpia} en {url}");

                // Hacer la petición HTTP
                using var response = await _httpClient.GetAsync(url);
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Error en API de cédulas: {response.StatusCode}");
                    return new CedulaInfoResult { Success = false, Message = "Error en la consulta" };
                }

                var jsonContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"Respuesta de API: {jsonContent}");

                // Parsear la respuesta JSON
                var apiResponse = JsonSerializer.Deserialize<CedulaApiResponse>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (apiResponse?.Results?.Any() == true)
                {
                    var result = apiResponse.Results.First();
                    return new CedulaInfoResult
                    {
                        Success = true,
                        Nombres = result.Firstname ?? "",
                        PrimerApellido = result.Lastname1 ?? "",
                        SegundoApellido = result.Lastname2 ?? "",
                        Message = "Información encontrada correctamente"
                    };
                }
                else
                {
                    return new CedulaInfoResult 
                    { 
                        Success = false, 
                        Message = "No se encontró información para esta cédula" 
                    };
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error de conexión al consultar cédula");
                return new CedulaInfoResult { Success = false, Message = "Error de conexión" };
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error al procesar respuesta de API de cédulas");
                return new CedulaInfoResult { Success = false, Message = "Error en los datos recibidos" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al consultar cédula");
                return new CedulaInfoResult { Success = false, Message = "Error interno del servicio" };
            }
        }
    }

    public class CedulaInfoResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? Nombres { get; set; }
        public string? PrimerApellido { get; set; }
        public string? SegundoApellido { get; set; }
    }

    public class CedulaApiResponse
    {
        public List<CedulaResult>? Results { get; set; }
    }

    public class CedulaResult
    {
        public string? Firstname { get; set; }
        public string? Firstname1 { get; set; }
        public string? Firstname2 { get; set; }
        public string? Lastname1 { get; set; }
        public string? Lastname2 { get; set; }
        public string? Fullname { get; set; }
        public string? Lastname { get; set; }
        public string? Cedula { get; set; }
    }
}