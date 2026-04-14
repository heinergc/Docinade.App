using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using DocinadeApp.Data;
using DocinadeApp.DTOs.Calificador;
using DocinadeApp.Services.Calificador;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;

namespace DocinadeApp.Controllers
{
    [Authorize]
    public class CalificadorPQ2025Controller : Controller
    {
        private readonly ICalificadorService _calificadorService;
        private readonly RubricasDbContext _context;
        private readonly ILogger<CalificadorPQ2025Controller> _logger;
        
        public CalificadorPQ2025Controller(
            ICalificadorService calificadorService,
            RubricasDbContext context,
            ILogger<CalificadorPQ2025Controller> logger)
        {
            _calificadorService = calificadorService;
            _context = context;
            _logger = logger;
        }
        
        // GET: CalificadorPQ2025
        public async Task<IActionResult> Index()
        {
            await CargarDatosViewBagAsync();
            return View();
        }
        
        // GET: CalificadorPQ2025/Generar
        public async Task<IActionResult> Generar(CalificadorQueryDto query)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await CargarDatosViewBagAsync();
                    return View("Index", query);
                }
                
                var cuaderno = await _calificadorService.GenerarCuadernoAsync(query);
                
                ViewBag.Query = query;
                return View("CuadernoGrid", cuaderno);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar cuaderno calificador");
                TempData["ErrorMessage"] = $"Error al generar el cuaderno: {ex.Message}";
                await CargarDatosViewBagAsync();
                return View("Index", query);
            }
        }
        
        // GET: CalificadorPQ2025/ExportarCsv
        public async Task<IActionResult> ExportarCsv(CalificadorQueryDto query)
        {
            try
            {
                var cuaderno = await _calificadorService.GenerarCuadernoAsync(query);
                var csv = GenerarCsv(cuaderno);
                
                var fileName = $"CuadernoCalificador_{cuaderno.MateriaNombre}_{cuaderno.PeriodoAcademicoNombre}_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                fileName = fileName.Replace(" ", "_").Replace("/", "_");
                
                // Usar UTF-8 con BOM para compatibilidad con Excel
                var preamble = Encoding.UTF8.GetPreamble();
                var data = Encoding.UTF8.GetBytes(csv);
                var result = new byte[preamble.Length + data.Length];
                Array.Copy(preamble, 0, result, 0, preamble.Length);
                Array.Copy(data, 0, result, preamble.Length, data.Length);
                
                return File(result, "text/csv; charset=utf-8", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al exportar cuaderno a CSV");
                TempData["ErrorMessage"] = $"Error al exportar: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
        
        // API: CalificadorPQ2025/Api/Columnas
        [HttpGet]
        public async Task<IActionResult> ObtenerColumnas(int materiaId, int periodoAcademicoId)
        {
            try
            {
                _logger.LogInformation("Obteniendo columnas para Materia: {MateriaId}, Periodo: {PeriodoId}", materiaId, periodoAcademicoId);
                
                var columnas = await _calificadorService.ObtenerColumnasAsync(materiaId, periodoAcademicoId);
                
                _logger.LogInformation("Columnas obtenidas: {Count}", columnas?.Count ?? 0);
                
                // Configurar opciones de JSON con encoding UTF-8
                var jsonOptions = new JsonSerializerOptions
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = false
                };
                
                var response = new
                {
                    success = true,
                    data = columnas ?? new List<CalificadorColumnDto>(),
                    count = columnas?.Count ?? 0
                };
                
                // Establecer Content-Type explícitamente
                Response.Headers.Add("Content-Type", "application/json; charset=utf-8");
                
                return Json(response, jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener columnas del cuaderno");
                
                var errorResponse = new
                {
                    success = false,
                    message = ex.Message,
                    data = new List<object>()
                };
                
                Response.Headers.Add("Content-Type", "application/json; charset=utf-8");
                return Json(errorResponse);
            }
        }
        
        // API: CalificadorPQ2025/Api/Validar
        [HttpGet]
        public async Task<IActionResult> ValidarParametros(int materiaId, int periodoAcademicoId)
        {
            try
            {
                var esValido = await _calificadorService.ValidarParametrosAsync(materiaId, periodoAcademicoId);
                
                var response = new
                {
                    success = true,
                    esValido = esValido
                };
                
                Response.Headers.Add("Content-Type", "application/json; charset=utf-8");
                return Json(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar parametros");
                
                var errorResponse = new
                {
                    success = false,
                    message = ex.Message
                };
                
                Response.Headers.Add("Content-Type", "application/json; charset=utf-8");
                return Json(errorResponse);
            }
        }
        
        // GET: CalificadorPQ2025/Estadisticas
        public async Task<IActionResult> Estadisticas(CalificadorQueryDto query)
        {
            try
            {
                var cuaderno = await _calificadorService.GenerarCuadernoAsync(query);
                
                var jsonOptions = new JsonSerializerOptions
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                
                var response = new
                {
                    success = true,
                    estadisticas = cuaderno.Estadisticas
                };
                
                Response.Headers.Add("Content-Type", "application/json; charset=utf-8");
                return Json(response, jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadisticas");
                
                var errorResponse = new
                {
                    success = false,
                    message = ex.Message
                };
                
                Response.Headers.Add("Content-Type", "application/json; charset=utf-8");
                return Json(errorResponse);
            }
        }
        
        private async Task CargarDatosViewBagAsync()
        {
            // Cargar materias activas
            ViewBag.MateriaId = new SelectList(
                await _context.Materias
                    .Where(m => m.Activa)
                    .OrderBy(m => m.Nombre)
                    .ToListAsync(),
                "MateriaId", "Nombre"
            );
            
            // Cargar períodos académicos activos
            ViewBag.PeriodoAcademicoId = new SelectList(
                await _context.PeriodosAcademicos
                    .Where(p => p.Activo)
                    .OrderByDescending(p => p.Anio)
                    .ThenBy(p => p.Ciclo)
                    .ToListAsync(),
                "Id", "NombreCompleto"
            );
            
            // Buscar específicamente "Primer Cuatrimestre 2025"
            var periodoObjetivo = await _context.PeriodosAcademicos
                .FirstOrDefaultAsync(p => p.Anio == 2025 && 
                    (p.Ciclo.ToUpper().Contains("PRIMER") || p.Ciclo.ToUpper().Contains("1") || p.Ciclo.ToUpper().Contains("I")) &&
                    p.Activo);
                    
            if (periodoObjetivo != null)
            {
                ViewBag.PeriodoObjetivoId = periodoObjetivo.Id;
                ViewBag.PeriodoObjetivoNombre = periodoObjetivo.NombreCompleto;
            }
            
            // Opciones de modo de calculo
            ViewBag.ModoCalculo = new SelectList(new[]
            {
                new { Value = "PROMEDIO", Text = "Promedio de rúbricas por instrumento" },
                new { Value = "SUMA", Text = "Suma de rúbricas por instrumento (máx 100)" },
                new { Value = "MEJOR_NOTA", Text = "Mejor nota de rúbricas por instrumento" }
            }, "Value", "Text", "PROMEDIO");
        }
        
        private string GenerarCsv(CuadernoCalificadorDto cuaderno)
        {
            var sb = new StringBuilder();
            
            // Encabezado del archivo
            sb.AppendLine($"# Cuaderno Calificador - {cuaderno.MateriaNombre}");
            sb.AppendLine($"# Período: {cuaderno.PeriodoAcademicoNombre}");
            sb.AppendLine($"# Generado: {cuaderno.FechaGeneracion:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"# Total Estudiantes: {cuaderno.Estadisticas.TotalEstudiantes}");
            sb.AppendLine($"# Promedio General: {cuaderno.Estadisticas.PromedioGeneral:F2}");
            sb.AppendLine();
            
            // Encabezados de columnas
            var encabezados = new List<string> { "Estudiante", "Numero ID" };
            encabezados.AddRange(cuaderno.Columnas.Select(c => $"{c.InstrumentoNombre} ? {c.RubricaNombre}"));
            encabezados.Add("Total Final");
            
            sb.AppendLine(string.Join(",", encabezados.Select(h => $"\"{h}\"")));
            
            // Filas de datos
            foreach (var fila in cuaderno.Filas.OrderBy(f => f.EstudianteNombre))
            {
                var valores = new List<string>
                {
                    $"\"{fila.EstudianteNombre}\"",
                    $"\"{fila.NumeroId}\""
                };
                
                foreach (var columna in cuaderno.Columnas)
                {
                    if (fila.CalificacionesPorInstrumentoRubrica.TryGetValue(columna.ClaveColumna, out decimal calificacion))
                    {
                        valores.Add(calificacion.ToString("F2"));
                    }
                    else
                    {
                        valores.Add("0.00");
                    }
                }
                
                valores.Add(fila.TotalFinal.ToString("F2"));
                
                sb.AppendLine(string.Join(",", valores));
            }
            
            // Estadísticas al final
            sb.AppendLine();
            sb.AppendLine("# ESTADÍSTICAS");
            sb.AppendLine($"# Nota Máxima: {cuaderno.Estadisticas.NotaMaxima:F2}");
            sb.AppendLine($"# Nota Mínima: {cuaderno.Estadisticas.NotaMinima:F2}");
            sb.AppendLine($"# Estudiantes con todas las notas: {cuaderno.Estadisticas.EstudiantesConTodasLasNotas}");
            sb.AppendLine($"# Estudiantes con notas pendientes: {cuaderno.Estadisticas.EstudiantesConNotasPendientes}");
            
            return sb.ToString();
        }
    }
}