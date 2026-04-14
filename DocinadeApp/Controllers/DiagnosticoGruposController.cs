using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DocinadeApp.Data;
using DocinadeApp.Models;

namespace DocinadeApp.Controllers
{
    public class DiagnosticoGruposController : Controller
    {
        private readonly RubricasDbContext _context;
        private readonly ILogger<DiagnosticoGruposController> _logger;

        public DiagnosticoGruposController(RubricasDbContext context, ILogger<DiagnosticoGruposController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Vista principal de diagnóstico - CORREGIDO para carga síncrona
        public async Task<IActionResult> Index()
        {
            try
            {
                _logger.LogInformation("🔄 Iniciando carga de diagnóstico de grupos");

                // Cargar todas las estadísticas de forma síncrona y secuencial
                var totalGrupos = await _context.GruposEstudiantes.CountAsync();
                _logger.LogInformation($"📊 Total grupos: {totalGrupos}");

                var gruposActivos = await _context.GruposEstudiantes.CountAsync(g => g.Estado == EstadoGrupo.Activo);
                _logger.LogInformation($"📊 Grupos activos: {gruposActivos}");

                var totalEstudiantes = await _context.Estudiantes.CountAsync();
                _logger.LogInformation($"📊 Total estudiantes: {totalEstudiantes}");

                var totalRelaciones = await _context.EstudianteGrupos.CountAsync();
                _logger.LogInformation($"📊 Total relaciones: {totalRelaciones}");

                var relacionesActivas = await _context.EstudianteGrupos.CountAsync(eg => eg.Estado == EstadoAsignacion.Activo);
                _logger.LogInformation($"📊 Relaciones activas: {relacionesActivas}");

                // Crear objeto diagnóstico con datos confirmados
                var diagnostico = new
                {
                    TotalGrupos = totalGrupos,
                    GruposActivos = gruposActivos,
                    TotalEstudiantes = totalEstudiantes,
                    TotalRelacionesGrupoEstudiante = totalRelaciones,
                    RelacionesActivas = relacionesActivas,
                    FechaCarga = DateTime.Now
                };

                ViewBag.Diagnostico = diagnostico;
                
                // También cargar algunos datos adicionales para mostrar en la vista
                ViewBag.GruposDetalle = await _context.GruposEstudiantes
                    .Include(g => g.EstudianteGrupos.Where(eg => eg.Estado == EstadoAsignacion.Activo))
                    .Select(g => new
                    {
                        g.GrupoId,
                        g.Codigo,
                        g.Nombre,
                        Estado = g.Estado.ToString(),
                        CantidadEstudiantes = g.EstudianteGrupos.Count(eg => eg.Estado == EstadoAsignacion.Activo)
                    })
                    .Take(5)
                    .ToListAsync();

                ViewBag.EstudiantesDetalle = await _context.Estudiantes
                    .Select(e => new
                    {
                        e.IdEstudiante,
                        NombreCompleto = $"{e.Apellidos}, {e.Nombre}",
                        e.NumeroId
                    })
                    .Take(5)
                    .ToListAsync();

                _logger.LogInformation("✅ Diagnóstico cargado exitosamente");
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al cargar diagnóstico");
                ViewBag.Error = "Error al cargar el diagnóstico: " + ex.Message;
                ViewBag.Diagnostico = new
                {
                    TotalGrupos = 0,
                    GruposActivos = 0,
                    TotalEstudiantes = 0,
                    TotalRelacionesGrupoEstudiante = 0,
                    RelacionesActivas = 0,
                    FechaCarga = DateTime.Now
                };
                return View();
            }
        }

        // Crear datos de prueba para grupos y estudiantes - MEJORADO
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearDatosPrueba()
        {
            try
            {
                _logger.LogInformation("🔄 Iniciando creación de datos de prueba");

                // Verificar si ya existen grupos
                var gruposExistentes = await _context.GruposEstudiantes.CountAsync();
                _logger.LogInformation($"📊 Grupos existentes: {gruposExistentes}");

                if (gruposExistentes > 0)
                {
                    var relacionesExistentes = await _context.EstudianteGrupos.CountAsync(eg => eg.Estado == EstadoAsignacion.Activo);
                    if (relacionesExistentes > 0)
                    {
                        return Json(new { 
                            success = false, 
                            message = $"Ya existen {gruposExistentes} grupos y {relacionesExistentes} relaciones activas en la base de datos" 
                        });
                    }
                }

                // Obtener período académico activo
                var periodoActivo = await _context.PeriodosAcademicos
                    .Where(p => p.Activo)
                    .FirstOrDefaultAsync();
                    
                if (periodoActivo == null)
                {
                    periodoActivo = await _context.PeriodosAcademicos.FirstOrDefaultAsync();
                }

                if (periodoActivo == null)
                {
                    return Json(new { success = false, message = "No se encontró ningún período académico" });
                }

                _logger.LogInformation($"📅 Usando período académico: {periodoActivo.Nombre}");

                // Crear tipo de grupo si no existe
                var tipoGrupo = await _context.TiposGrupo.FirstOrDefaultAsync();
                if (tipoGrupo == null)
                {
                    tipoGrupo = new TipoGrupoCatalogo { Nombre = "Sección", Estado = "Activo" };
                    _context.TiposGrupo.Add(tipoGrupo);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("✅ Tipo de grupo creado");
                }

                var gruposCreados = 0;
                var estudiantesCreados = 0;
                var asignacionesCreadas = 0;

                // Crear grupos de prueba solo si no existen
                if (gruposExistentes == 0)
                {
                    var grupos = new List<GrupoEstudiante>
                    {
                        new GrupoEstudiante
                        {
                            Codigo = "G01",
                            Nombre = "Grupo 01 - Investigación de Operaciones",
                            Descripcion = "Grupo de prueba para IO",
                            IdTipoGrupo = tipoGrupo.IdTipoGrupo,
                            PeriodoAcademicoId = periodoActivo.Id,
                            Estado = EstadoGrupo.Activo,
                            CapacidadMaxima = 30
                        },
                        new GrupoEstudiante
                        {
                            Codigo = "G02", 
                            Nombre = "Grupo 02 - Investigación de Operaciones",
                            Descripcion = "Segundo grupo de prueba para IO",
                            IdTipoGrupo = tipoGrupo.IdTipoGrupo,
                            PeriodoAcademicoId = periodoActivo.Id,
                            Estado = EstadoGrupo.Activo,
                            CapacidadMaxima = 25
                        }
                    };

                    _context.GruposEstudiantes.AddRange(grupos);
                    await _context.SaveChangesAsync();
                    gruposCreados = grupos.Count;
                    _logger.LogInformation($"✅ {gruposCreados} grupos creados");
                }

                // Crear estudiantes de prueba si hay pocos
                var estudiantesExistentes = await _context.Estudiantes.CountAsync();
                if (estudiantesExistentes < 5)
                {
                    var estudiantesPrueba = new List<Estudiante>();
                    
                    // Generar estudiantes únicos
                    var nombresBase = new[] { "JUAN CARLOS", "MARÍA JOSÉ", "LUIS ALBERTO", "ANA CAROLINA", "DIEGO FERNANDO" };
                    var apellidosBase = new[] { "PÉREZ GONZÁLEZ", "RODRÍGUEZ LÓPEZ", "MORA JIMÉNEZ", "CASTRO VARGAS", "VILLALOBOS SOTO" };
                    
                    for (int i = 0; i < 5; i++)
                    {
                        var numeroIdUnico = $"12345678{i}";
                        var existeId = await _context.Estudiantes.AnyAsync(e => e.NumeroId == numeroIdUnico);
                        
                        if (!existeId)
                        {
                            estudiantesPrueba.Add(new Estudiante
                            {
                                Nombre = nombresBase[i],
                                Apellidos = apellidosBase[i],
                                NumeroId = numeroIdUnico,
                                DireccionCorreo = $"estudiante{i}@test.edu.cr",
                                Institucion = "SEDE CENTRAL",
                                Anio = 2025,
                                PeriodoAcademicoId = periodoActivo.Id
                            });
                        }
                    }

                    if (estudiantesPrueba.Any())
                    {
                        _context.Estudiantes.AddRange(estudiantesPrueba);
                        await _context.SaveChangesAsync();
                        estudiantesCreados = estudiantesPrueba.Count;
                        _logger.LogInformation($"✅ {estudiantesCreados} estudiantes creados");
                    }
                }

                // Crear asignaciones estudiante-grupo
                var todosLosGrupos = await _context.GruposEstudiantes.Take(5).ToListAsync();
                var todosLosEstudiantes = await _context.Estudiantes.Take(10).ToListAsync();

                foreach (var grupo in todosLosGrupos)
                {
                    var estudiantesParaGrupo = todosLosEstudiantes.Take(3).ToList();
                    
                    foreach (var estudiante in estudiantesParaGrupo)
                    {
                        // Verificar que no esté ya asignado
                        var yaAsignado = await _context.EstudianteGrupos
                            .AnyAsync(eg => eg.EstudianteId == estudiante.IdEstudiante && 
                                           eg.GrupoId == grupo.GrupoId);

                        if (!yaAsignado)
                        {
                            var asignacion = new EstudianteGrupo
                            {
                                EstudianteId = estudiante.IdEstudiante,
                                GrupoId = grupo.GrupoId,
                                Estado = EstadoAsignacion.Activo,
                                EsGrupoPrincipal = true,
                                FechaAsignacion = DateTime.Now
                            };

                            _context.EstudianteGrupos.Add(asignacion);
                            asignacionesCreadas++;
                        }
                    }
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation($"✅ {asignacionesCreadas} asignaciones creadas");

                return Json(new
                {
                    success = true,
                    message = "Datos de prueba creados exitosamente",
                    detalles = new
                    {
                        GruposCreados = gruposCreados,
                        EstudiantesCreados = estudiantesCreados,
                        AsignacionesCreadas = asignacionesCreadas
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al crear datos de prueba");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // API para verificar el estado actual - MEJORADO con mejor sincronización
        [HttpGet]
        public async Task<JsonResult> VerificarEstado()
        {
            try
            {
                _logger.LogInformation("🔄 Verificando estado del sistema");

                // Cargar datos de forma secuencial para evitar problemas de sincronización
                var grupos = await _context.GruposEstudiantes
                    .Include(g => g.EstudianteGrupos.Where(eg => eg.Estado == EstadoAsignacion.Activo))
                    .Select(g => new
                    {
                        g.GrupoId,
                        g.Codigo,
                        g.Nombre,
                        Estado = g.Estado.ToString(),
                        CantidadEstudiantes = g.EstudianteGrupos.Count(eg => eg.Estado == EstadoAsignacion.Activo)
                    })
                    .ToListAsync();

                var estudiantes = await _context.Estudiantes
                    .Select(e => new
                    {
                        e.IdEstudiante,
                        NombreCompleto = $"{e.Apellidos}, {e.Nombre}",
                        e.NumeroId
                    })
                    .Take(10)
                    .ToListAsync();

                var asignaciones = await _context.EstudianteGrupos
                    .Include(eg => eg.Estudiante)
                    .Include(eg => eg.Grupo)
                    .Select(eg => new
                    {
                        eg.Id,
                        EstudianteId = eg.EstudianteId,
                        EstudianteNombre = $"{eg.Estudiante.Apellidos}, {eg.Estudiante.Nombre}",
                        GrupoId = eg.GrupoId,
                        GrupoNombre = eg.Grupo.Nombre,
                        Estado = eg.Estado.ToString(),
                        FechaAsignacion = eg.FechaAsignacion.ToString("dd/MM/yyyy HH:mm")
                    })
                    .ToListAsync();

                var estado = new
                {
                    Timestamp = DateTime.Now,
                    Grupos = grupos,
                    Estudiantes = estudiantes,
                    AsignacionesGrupoEstudiante = asignaciones,
                    Resumen = new
                    {
                        TotalGrupos = grupos.Count,
                        TotalEstudiantes = estudiantes.Count,
                        TotalAsignaciones = asignaciones.Count,
                        AsignacionesActivas = asignaciones.Count(a => a.Estado == "Activo")
                    }
                };

                _logger.LogInformation($"✅ Estado verificado: {grupos.Count} grupos, {asignaciones.Count} asignaciones");

                return Json(estado, new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNamingPolicy = null,
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al verificar estado");
                return Json(new { 
                    error = true, 
                    message = ex.Message,
                    timestamp = DateTime.Now 
                });
            }
        }

        // Método adicional para diagnóstico rápido
        [HttpGet]
        public async Task<JsonResult> EstadoRapido()
        {
            try
            {
                var estadoRapido = new
                {
                    TotalGrupos = await _context.GruposEstudiantes.CountAsync(),
                    GruposActivos = await _context.GruposEstudiantes.CountAsync(g => g.Estado == EstadoGrupo.Activo),
                    TotalEstudiantes = await _context.Estudiantes.CountAsync(),
                    TotalRelaciones = await _context.EstudianteGrupos.CountAsync(),
                    RelacionesActivas = await _context.EstudianteGrupos.CountAsync(eg => eg.Estado == EstadoAsignacion.Activo),
                    Timestamp = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")
                };

                return Json(estadoRapido);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
    }
}