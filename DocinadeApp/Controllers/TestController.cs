using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DocinadeApp.Data;
using DocinadeApp.Models;
using System.Text.Json;

namespace DocinadeApp.Controllers
{
    // Modelo simple para estadísticas
    public class EstadisticaPeriodo
    {
        public string Periodo { get; set; } = string.Empty;
        public int Cantidad { get; set; }
    }

    public class BuscadorEstudianteViewModel
    {
        public int TotalEstudiantes { get; set; }
        public List<EstadisticaPeriodo> EstudiantesPorPeriodo { get; set; } = new();
    }

    public class TestController : Controller
    {
        private readonly RubricasDbContext _context;
        private readonly ILogger<TestController> _logger;

        public TestController(RubricasDbContext context, ILogger<TestController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Vista de prueba para el componente BuscadorEstudiante
        /// </summary>
        public async Task<IActionResult> BuscadorEstudiante()
        {
            // Cargar algunos estudiantes para mostrar estadísticas
            var totalEstudiantes = await _context.Estudiantes.CountAsync();
            var estudiantesPorPeriodo = await _context.Estudiantes
                .Include(e => e.PeriodoAcademico)
                .Where(e => e.PeriodoAcademico != null)
                .GroupBy(e => e.PeriodoAcademico.Nombre)
                .Select(g => new EstadisticaPeriodo { 
                    Periodo = g.Key ?? "Sin período", 
                    Cantidad = g.Count() 
                })
                .ToListAsync();

            var viewModel = new BuscadorEstudianteViewModel
            {
                TotalEstudiantes = totalEstudiantes,
                EstudiantesPorPeriodo = estudiantesPorPeriodo
            };

            return View(viewModel);
        }

        /// <summary>
        /// Endpoint para b�squeda de estudiantes (compatible con el componente)
        /// </summary>
        [HttpGet]
        public async Task<JsonResult> BuscarEstudiantes(
            string term = "",
            int page = 1,
            int pageSize = 10,
            int? periodoId = null)
        {
            try
            {
                _logger.LogInformation($"?? B�squeda de estudiantes - T�rmino: '{term}', P�gina: {page}");

                var query = _context.Estudiantes
                    .Include(e => e.PeriodoAcademico)
                    .AsQueryable();

                // Filtro por per�odo si se especifica
                if (periodoId.HasValue)
                {
                    query = query.Where(e => e.PeriodoAcademicoId == periodoId.Value);
                }

                // B�squeda por t�rmino
                if (!string.IsNullOrWhiteSpace(term) && term.Length >= 2)
                {
                    term = term.Trim().ToLowerInvariant();
                    query = query.Where(e =>
                        e.Nombre.ToLower().Contains(term) ||
                        e.Apellidos.ToLower().Contains(term) ||
                        e.NumeroId.Contains(term) ||
                        (e.Nombre + " " + e.Apellidos).ToLower().Contains(term) ||
                        (e.Apellidos + " " + e.Nombre).ToLower().Contains(term)
                    );
                }
                else if (string.IsNullOrWhiteSpace(term))
                {
                    // Si no hay t�rmino de b�squeda, devolver lista vac�a para evitar carga innecesaria
                    return Json(new { 
                        results = new List<object>(),
                        pagination = new { more = false },
                        total = 0
                    });
                }

                // Contar total antes de paginaci�n
                var total = await query.CountAsync();

                // Aplicar paginaci�n
                var estudiantes = await query
                    .OrderBy(e => e.Apellidos)
                    .ThenBy(e => e.Nombre)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(e => new {
                        id = e.IdEstudiante,
                        text = $"{e.Apellidos}, {e.Nombre} ({e.NumeroId})",
                        nombre = e.Nombre,
                        apellidos = e.Apellidos,
                        numeroId = e.NumeroId,
                        correo = e.DireccionCorreo,
                        institucion = e.Institucion,
                        periodo = e.PeriodoAcademico != null ? 
                            $"{e.PeriodoAcademico.Anio} - {e.PeriodoAcademico.Ciclo}" : 
                            "Sin período"
                    })
                    .ToListAsync();

                var hasMore = (page * pageSize) < total;

                _logger.LogInformation($"? Encontrados {estudiantes.Count} de {total} estudiantes");

                return Json(new {
                    results = estudiantes,
                    pagination = new { more = hasMore },
                    total = total
                }, new JsonSerializerOptions
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "? Error en b�squeda de estudiantes");
                return Json(new { 
                    error = true, 
                    message = "Error al buscar estudiantes",
                    details = ex.Message 
                });
            }
        }

        /// <summary>
        /// Acci�n para simular selecci�n de estudiante desde el modal
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> SeleccionarEstudiante(int estudianteId)
        {
            try
            {
                var estudiante = await _context.Estudiantes
                    .Include(e => e.PeriodoAcademico)
                    .FirstOrDefaultAsync(e => e.IdEstudiante == estudianteId);

                if (estudiante == null)
                {
                    return Json(new { success = false, message = "Estudiante no encontrado" });
                }

                _logger.LogInformation($"? Estudiante seleccionado: {estudiante.NombreCompleto}");

                return Json(new {
                    success = true,
                    estudiante = new {
                        id = estudiante.IdEstudiante,
                        nombre = estudiante.Nombre,
                        apellidos = estudiante.Apellidos,
                        numeroId = estudiante.NumeroId,
                        correo = estudiante.DireccionCorreo,
                        institucion = estudiante.Institucion,
                        periodo = estudiante.PeriodoAcademico?.Nombre ?? "Sin per�odo"
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"? Error al seleccionar estudiante {estudianteId}");
                return Json(new { success = false, message = "Error al seleccionar estudiante" });
            }
        }

        /// <summary>
        /// Endpoint temporal para diagnosticar datos de estudiantes
        /// </summary>
        [HttpGet]
        public async Task<JsonResult> DiagnosticarEstudiantes()
        {
            try
            {
                var totalEstudiantes = await _context.Estudiantes.CountAsync();
                
                if (totalEstudiantes == 0)
                {
                    // Crear algunos estudiantes de prueba
                    var periodo = await _context.PeriodosAcademicos.FirstOrDefaultAsync() 
                        ?? new PeriodoAcademico
                        {
                            Nombre = "2025-I",
                            FechaInicio = new DateTime(2025, 1, 1),
                            FechaFin = new DateTime(2025, 6, 30),
                            Activo = true,
                            Estado = "Activo"
                        };

                    if (periodo.Id == 0)
                    {
                        _context.PeriodosAcademicos.Add(periodo);
                        await _context.SaveChangesAsync();
                    }

                    var estudiantesPrueba = new List<Estudiante>
                    {
                        new Estudiante
                        {
                            Nombre = "María José",
                            Apellidos = "González Rodríguez", 
                            NumeroId = "123456789",
                            DireccionCorreo = "maria.gonzalez@estudiante.edu",
                            Institucion = "Universidad de Costa Rica",
                            Anio = 2025,
                            PeriodoAcademicoId = periodo.Id
                        },
                        new Estudiante
                        {
                            Nombre = "Carlos Alberto",
                            Apellidos = "Méndez Vargas",
                            NumeroId = "987654321", 
                            DireccionCorreo = "carlos.mendez@estudiante.edu",
                            Institucion = "Universidad de Costa Rica",
                            Anio = 2025,
                            PeriodoAcademicoId = periodo.Id
                        },
                        new Estudiante
                        {
                            Nombre = "Ana Lucía",
                            Apellidos = "Jiménez Castro",
                            NumeroId = "456123789",
                            DireccionCorreo = "ana.jimenez@estudiante.edu", 
                            Institucion = "Tecnológico de Costa Rica",
                            Anio = 2025,
                            PeriodoAcademicoId = periodo.Id
                        },
                        new Estudiante
                        {
                            Nombre = "Diego Fernando",
                            Apellidos = "Morales López",
                            NumeroId = "789456123",
                            DireccionCorreo = "diego.morales@estudiante.edu",
                            Institucion = "Universidad Nacional",
                            Anio = 2025,
                            PeriodoAcademicoId = periodo.Id
                        },
                        new Estudiante
                        {
                            Nombre = "Sofía Andrea",
                            Apellidos = "Herrera Sánchez",
                            NumeroId = "321654987",
                            DireccionCorreo = "sofia.herrera@estudiante.edu",
                            Institucion = "UNED",
                            Anio = 2025,
                            PeriodoAcademicoId = periodo.Id
                        }
                    };

                    this._context.Estudiantes.AddRange(estudiantesPrueba);
                    await _context.SaveChangesAsync();
                    totalEstudiantes = estudiantesPrueba.Count;

                    _logger.LogInformation($"✅ Se crearon {totalEstudiantes} estudiantes de prueba");
                }

                var primeros5 = await _context.Estudiantes
                    .Take(5)
                    .Select(e => new {
                        id = e.IdEstudiante,
                        nombre = e.Nombre,
                        apellidos = e.Apellidos,
                        numeroId = e.NumeroId
                    })
                    .ToListAsync();

                return Json(new {
                    totalEstudiantes = totalEstudiantes,
                    ejemplos = primeros5,
                    mensaje = totalEstudiantes > 0 ? "Datos disponibles" : "Se crearon datos de prueba"
                });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
    }
}