using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using DocinadeApp.Data;
using DocinadeApp.Models;

namespace DocinadeApp.Controllers
{
    [Authorize]
    public class GruposEstudiantesTestController : Controller
    {
        private readonly RubricasDbContext _context;
        private readonly ILogger<GruposEstudiantesTestController> _logger;

        public GruposEstudiantesTestController(
            RubricasDbContext context,
            ILogger<GruposEstudiantesTestController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: GruposEstudiantesTest
        public async Task<IActionResult> Index()
        {
            try
            {
                _logger.LogInformation("?? Iniciando prueba de acceso a grupos...");

                // Verificar conexi�n a la base de datos
                var canConnect = await _context.Database.CanConnectAsync();
                _logger.LogInformation($"? Conexi�n DB: {canConnect}");

                // Consulta b�sica sin includes para probar
                var gruposBasicos = await _context.GruposEstudiantes.ToListAsync();
                _logger.LogInformation($"? Grupos encontrados: {gruposBasicos.Count}");

                // Crear modelo simple para la vista
                var modelo = new
                {
                    TotalGrupos = gruposBasicos.Count,
                    Grupos = gruposBasicos.Select(g => new
                    {
                        g.GrupoId,
                        g.Codigo,
                        g.Nombre,
                        g.TipoGrupo,
                        g.Estado,
                        g.FechaCreacion
                    }).ToList()
                };

                ViewBag.Mensaje = $"? Sistema funcionando correctamente. {gruposBasicos.Count} grupos encontrados.";
                return View(modelo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "? Error en prueba de grupos");
                ViewBag.Error = $"Error: {ex.Message}";
                ViewBag.StackTrace = ex.StackTrace;
                return View(new { TotalGrupos = 0, Grupos = new List<object>() });
            }
        }

        // GET: Test de datos directos
        public async Task<IActionResult> TestDatos()
        {
            try
            {
                var resultados = new List<string>();

                // Test 1: Conexi�n b�sica
                var canConnect = await _context.Database.CanConnectAsync();
                resultados.Add($"?? Conexi�n DB: {(canConnect ? "? OK" : "? FALLO")}");

                // Test 2: Contar grupos
                var countGrupos = await _context.GruposEstudiantes.CountAsync();
                resultados.Add($"?? Total grupos: {countGrupos}");

                // Test 3: Obtener grupos simples
                var grupos = await _context.GruposEstudiantes
                    .Select(g => new { g.GrupoId, g.Codigo, g.Nombre })
                    .ToListAsync();
                resultados.Add($"?? Grupos obtenidos: {grupos.Count}");

                // Test 4: Verificar enums
                foreach (var grupo in grupos.Take(3))
                {
                    var grupoCompleto = await _context.GruposEstudiantes
                        .Where(g => g.GrupoId == grupo.GrupoId)
                        .FirstOrDefaultAsync();
                    
                    if (grupoCompleto != null)
                    {
                        resultados.Add($"?? Grupo {grupo.Codigo}: TipoGrupo={grupoCompleto.TipoGrupo}, Estado={grupoCompleto.Estado}");
                    }
                }

                // Test 5: Per�odos acad�micos
                var periodos = await _context.PeriodosAcademicos.CountAsync();
                resultados.Add($"?? Per�odos acad�micos: {periodos}");

                // Test 6: Estudiantes
                var estudiantes = await _context.Estudiantes.CountAsync();
                resultados.Add($"?? Estudiantes: {estudiantes}");

                ViewBag.Resultados = resultados;
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"? Error en test: {ex.Message}";
                ViewBag.StackTrace = ex.StackTrace;
                return View();
            }
        }
    }
}