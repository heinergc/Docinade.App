using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DocinadeApp.Data;
using DocinadeApp.Models;
using DocinadeApp.Services.Permissions;
using Microsoft.AspNetCore.Identity;
using DocinadeApp.Models.Identity;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using DocinadeApp.ViewModels;

// Agrega el using para el namespace donde está GrupoCalificacion si es necesario
// using DocinadeApp.Entidades; // <-- Descomenta y ajusta si tu modelo está en otro namespace

namespace DocinadeApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly RubricasDbContext _context;
        private readonly IPermissionService _permissionService;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, RubricasDbContext context, 
                            IPermissionService permissionService, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _permissionService = permissionService;
            _userManager = userManager;
        }

        // Método simple para crear datos básicos que funcione
        public async Task<IActionResult> CrearDatosBasicos()
        {
            try
            {
                // Primero verificar si las tablas están vacías
                string mensaje = "";
                
                // Contar registros actuales
                int countGrupos = 0;
                int countNiveles = 0;
                
                try
                {
                    countGrupos = await _context.Database.ExecuteSqlRawAsync("SELECT COUNT(*) FROM GruposCalificacion");
                }
                catch
                {
                    countGrupos = 0;
                }
                
                try
                {
                    countNiveles = await _context.Database.ExecuteSqlRawAsync("SELECT COUNT(*) FROM NivelesCalificacion");
                }
                catch
                {
                    countNiveles = 0;
                }
                
                mensaje += $"Grupos existentes: {countGrupos}, Niveles existentes: {countNiveles}. ";
                
                // Si no hay datos, crearlos con SQL directo
                if (countGrupos == 0 || countNiveles == 0)
                {
                    // Insertar niveles directamente
                    await _context.Database.ExecuteSqlRawAsync(@"
                        INSERT INTO NivelesCalificacion (NombreNivel, Descripcion, OrdenNivel, IdGrupo) VALUES 
                        ('Excelente', 'Desempeño sobresaliente (90-100%)', 1, NULL),
                        ('Bueno', 'Desempeño satisfactorio (75-89%)', 2, NULL),
                        ('Regular', 'Desempeño mínimo aceptable (60-74%)', 3, NULL),
                        ('Deficiente', 'Desempeño insuficiente (0-59%)', 4, NULL),
                        ('Sobresaliente', 'Desempeño excepcional', 0, NULL),
                        ('Aceptable', 'Desempeño aceptable', 5, NULL)
                    ");
                    
                    // Insertar grupo directamente
                    await _context.Database.ExecuteSqlRawAsync(@"
                        INSERT INTO GruposCalificacion (NombreGrupo, Descripcion, Estado, FechaCreacion) VALUES 
                        ('Grupo Estándar', 'Grupo de calificación estándar con niveles básicos', 'ACTIVO', datetime('now'))
                    ");
                    
                    // Asignar los primeros 4 niveles al grupo (asumiendo que el grupo tiene ID 1)
                    await _context.Database.ExecuteSqlRawAsync(@"
                        UPDATE NivelesCalificacion 
                        SET IdGrupo = (SELECT IdGrupo FROM GruposCalificacion WHERE NombreGrupo = 'Grupo Estándar' LIMIT 1)
                        WHERE NombreNivel IN ('Excelente', 'Bueno', 'Regular', 'Deficiente')
                    ");
                    
                    mensaje += "Datos básicos creados con SQL directo. ";
                }
                
                // Verificar resultados
                var grupos = await _context.GruposCalificacion
                    .Select(g => new { g.IdGrupo, g.NombreGrupo })
                    .ToListAsync();
                    
                var niveles = await _context.NivelesCalificacion
                    .Select(n => new { n.IdNivel, n.NombreNivel, n.IdGrupo })
                    .ToListAsync();
                
                return Json(new {
                    success = true,
                    mensaje = mensaje,
                    grupos = grupos,
                    niveles = niveles,
                    totalGrupos = grupos.Count,
                    totalNiveles = niveles.Count
                });
            }
            catch (Exception ex)
            {
                return Json(new {
                    success = false,
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        // Método directo para crear datos de ejemplo
        public async Task<IActionResult> CrearDatosEjemplo()
        {
            try
            {
                // Verificar si ya existen datos
                var gruposExistentes = await _context.GruposCalificacion.CountAsync();
                var nivelesExistentes = await _context.NivelesCalificacion.CountAsync();
                
                if (gruposExistentes > 0 && nivelesExistentes > 0)
                {
                    return Json(new { 
                        success = true, 
                        message = $"Ya existen datos: {gruposExistentes} grupos y {nivelesExistentes} niveles",
                        yaExisten = true
                    });
                }
                
                // Crear niveles básicos
                var niveles = new List<NivelCalificacion>
                {
                    new NivelCalificacion 
                    { 
                        NombreNivel = "Excelente", 
                        Descripcion = "Desempeño sobresaliente (90-100%)", 
                        OrdenNivel = 1 
                    },
                    new NivelCalificacion 
                    { 
                        NombreNivel = "Bueno", 
                        Descripcion = "Desempeño satisfactorio (75-89%)", 
                        OrdenNivel = 2 
                    },
                    new NivelCalificacion 
                    { 
                        NombreNivel = "Regular", 
                        Descripcion = "Desempeño mínimo aceptable (60-74%)", 
                        OrdenNivel = 3 
                    },
                    new NivelCalificacion 
                    { 
                        NombreNivel = "Deficiente", 
                        Descripcion = "Desempeño insuficiente (0-59%)", 
                        OrdenNivel = 4 
                    }
                };
                
                _context.NivelesCalificacion.AddRange(niveles);
                await _context.SaveChangesAsync();
                
                // Crear grupo estándar
                var grupoEstandar = new GrupoCalificacion
                {
                    NombreGrupo = "Grupo Estándar",
                    Descripcion = "Grupo de calificación estándar con 4 niveles básicos",
                    Estado = "ACTIVO",
                    FechaCreacion = DateTime.Now
                };
                
                _context.GruposCalificacion.Add(grupoEstandar);
                await _context.SaveChangesAsync();
                
                // Asignar niveles al grupo
                var nivelesCreados = await _context.NivelesCalificacion
                    .Where(n => n.IdGrupo == null)
                    .ToListAsync();
                    
                foreach (var nivel in nivelesCreados)
                {
                    nivel.IdGrupo = grupoEstandar.IdGrupo;
                }
                
                await _context.SaveChangesAsync();
                
                // Crear algunos niveles adicionales sin grupo (para grupos dinámicos)
                var nivelesLibres = new List<NivelCalificacion>
                {
                    new NivelCalificacion 
                    { 
                        NombreNivel = "Sobresaliente", 
                        Descripcion = "Desempeño excepcional", 
                        OrdenNivel = 0 
                    },
                    new NivelCalificacion 
                    { 
                        NombreNivel = "Aceptable", 
                        Descripcion = "Desempeño aceptable", 
                        OrdenNivel = 5 
                    },
                    new NivelCalificacion 
                    { 
                        NombreNivel = "Necesita Mejora", 
                        Descripcion = "Requiere trabajo adicional", 
                        OrdenNivel = 6 
                    }
                };
                
                _context.NivelesCalificacion.AddRange(nivelesLibres);
                await _context.SaveChangesAsync();
                
                // Verificar resultados
                var totalGrupos = await _context.GruposCalificacion.CountAsync();
                var totalNiveles = await _context.NivelesCalificacion.CountAsync();
                var nivelesEnGrupo = await _context.NivelesCalificacion.Where(n => n.IdGrupo != null).CountAsync();
                var nivelesSinGrupo = await _context.NivelesCalificacion.Where(n => n.IdGrupo == null).CountAsync();
                
                return Json(new { 
                    success = true, 
                    message = "Datos de ejemplo creados exitosamente",
                    resultados = new {
                        gruposCreados = totalGrupos,
                        nivelesCreados = totalNiveles,
                        nivelesAsignadosAGrupos = nivelesEnGrupo,
                        nivelesLibres = nivelesSinGrupo
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { 
                    success = false, 
                    message = $"Error al crear datos: {ex.Message}",
                    stackTrace = ex.StackTrace 
                });
            }
        }

        // Método para verificar el estado completo del sistema
        public async Task<IActionResult> VerificarEstadoSistema()
        {
            try
            {
                var estado = new
                {
                    fecha = DateTime.Now,
                    baseDatos = new
                    {
                        conexionExitosa = true,
                        tablas = new
                        {
                            rubricas = await _context.Rubricas.CountAsync(),
                            gruposCalificacion = await _context.GruposCalificacion.CountAsync(),
                            nivelesCalificacion = await _context.NivelesCalificacion.CountAsync(),
                            rubricaNiveles = await _context.RubricaNiveles.CountAsync(),
                            itemsEvaluacion = await _context.ItemsEvaluacion.CountAsync(),
                            valoresRubrica = await _context.ValoresRubrica.CountAsync()
                        }
                    },
                    gruposDisponibles = await _context.GruposCalificacion
                        .Where(g => g.Estado == "ACTIVO")
                        .Include(g => g.NivelesCalificacion)
                        .Select(g => new {
                            g.IdGrupo,
                            g.NombreGrupo,
                            CantidadNiveles = g.NivelesCalificacion.Count()
                        })
                        .ToListAsync(),
                    nivelesLibres = await _context.NivelesCalificacion
                        .Where(n => n.IdGrupo == null)
                        .Select(n => new {
                            n.IdNivel,
                            n.NombreNivel,
                            n.Descripcion
                        })
                        .ToListAsync()
                };
                
                return Json(estado);
            }
            catch (Exception ex)
            {
                return Json(new {
                    error = true,
                    mensaje = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            // Obtener configuración de empadronamiento público desde la base de datos
            var config = await _context.ConfiguracionesSistema
                .FirstOrDefaultAsync(c => c.Clave == "EmpadronamientoPublico.Habilitar");
            
            bool empadronamientoHabilitado = false;
            if (config != null && bool.TryParse(config.Valor, out var habilitado))
            {
                empadronamientoHabilitado = habilitado;
            }
            
            // Obtener estadísticas del sistema
            int rubricasCreadas = 0;
            int evaluacionesCompletadas = 0;
            int estudiantesEvaluados = 0;
            int reportesGenerados = 0;
            
            try
            {
                // Contar rúbricas creadas
                rubricasCreadas = await _context.Rubricas.CountAsync();
                
                // Contar evaluaciones completadas
                evaluacionesCompletadas = await _context.Evaluaciones.CountAsync();
                
                // Contar estudiantes únicos que han sido evaluados
                estudiantesEvaluados = await _context.Evaluaciones
                    .Select(e => e.IdEstudiante)
                    .Distinct()
                    .CountAsync();
                
                // Para reportes, podríamos usar una tabla de log o contar algo relacionado
                // Por ahora usaremos una estimación basada en evaluaciones/grupos
                reportesGenerados = await _context.GruposEstudiantes.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadísticas del sistema");
                // Si hay error, dejar los valores por defecto en 0
            }
            
            var model = new HomeIndexViewModel
            {
                EmpadronamientoPublicoHabilitado = empadronamientoHabilitado,
                RubricasCreadas = rubricasCreadas,
                EvaluacionesCompletadas = evaluacionesCompletadas,
                EstudiantesEvaluados = estudiantesEvaluados,
                ReportesGenerados = reportesGenerados
            };
            
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // Método temporal para ejecutar la migración
        //public async Task<IActionResult> EjecutarMigracionRubricaNivel()
        //{
        //    try
        //    {
        //        var migracion = new CrearTablaRubricaNivel(_context);
        //        await migracion.EjecutarAsync();
        //        return Json(new { success = true, message = "Migración ejecutada exitosamente" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = $"Error en migración: {ex.Message}" });
        //    }
        //}

        // Método temporal para diagnosticar rúbrica
        public async Task<IActionResult> DiagnosticarRubrica(int id = 8)
        {
            try
            {
                var rubrica = await _context.Rubricas.FindAsync(id);
                if (rubrica == null)
                {
                    return Json(new { error = "Rúbrica no encontrada" });
                }

                // Verificar si existe la tabla RubricaNiveles
                bool tablaExiste = false;
                try
                {
                    await _context.Database.ExecuteSqlRawAsync("SELECT COUNT(*) FROM RubricaNiveles LIMIT 1");
                    tablaExiste = true;
                }
                catch
                {
                    tablaExiste = false;
                }

                // Obtener niveles asociados si la tabla existe
                var nivelesAsociados = new List<object>();
                if (tablaExiste)
                {
                    try
                    {
                        var niveles = await _context.RubricaNiveles
                            .Where(rn => rn.IdRubrica == id)
                            .Include(rn => rn.NivelCalificacion)
                            .OrderBy(rn => rn.OrdenEnRubrica)
                            .Select(rn => new { 
                                rn.IdNivel, 
                                rn.NivelCalificacion.NombreNivel, 
                                rn.OrdenEnRubrica 
                            })
                            .ToListAsync();
                        nivelesAsociados = niveles.Cast<object>().ToList();
                    }
                    catch (Exception ex)
                    {
                        nivelesAsociados.Add(new { error = ex.Message });
                    }
                }

                // Obtener todos los niveles
                var todosLosNiveles = await _context.NivelesCalificacion
                    .Select(n => new { n.IdNivel, n.NombreNivel, n.OrdenNivel })
                    .ToListAsync();

                return Json(new { 
                    rubrica = new { rubrica.IdRubrica, rubrica.NombreRubrica },
                    tablaRubricaNivelesExiste = tablaExiste,
                    nivelesAsociados = nivelesAsociados,
                    todosLosNiveles = todosLosNiveles
                });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        // Método para listar todas las rúbricas y verificar sus relaciones
        public async Task<IActionResult> ListarRubricas()
        {
            try
            {
                var rubricas = await _context.Rubricas
                    .Select(r => new { 
                        r.IdRubrica, 
                        r.NombreRubrica, 
                        r.Estado,
                        r.FechaCreacion
                    })
                    .ToListAsync();

                var resultado = new List<object>();
                
                foreach (var rubrica in rubricas)
                {
                    var nivelesAsociados = await _context.RubricaNiveles
                        .Where(rn => rn.IdRubrica == rubrica.IdRubrica)
                        .CountAsync();

                    resultado.Add(new {
                        rubrica.IdRubrica,
                        rubrica.NombreRubrica,
                        rubrica.Estado,
                        rubrica.FechaCreacion,
                        TieneNivelesAsociados = nivelesAsociados > 0,
                        CantidadNiveles = nivelesAsociados
                    });
                }

                return Json(new { rubricas = resultado });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        // Método para asociar niveles comunes a rúbricas sin relaciones
        public async Task<IActionResult> AsociarNivelesComunes()
        {
            try
            {
                // Obtener rúbricas sin niveles asociados
                var rubricasSinNiveles = await _context.Rubricas
                    .Where(r => !_context.RubricaNiveles.Any(rn => rn.IdRubrica == r.IdRubrica))
                    .ToListAsync();

                if (!rubricasSinNiveles.Any())
                {
                    return Json(new { success = true, message = "Todas las rúbricas ya tienen niveles asociados" });
                }

                // Obtener niveles comunes (ordenados)
                var nivelesComunes = await _context.NivelesCalificacion
                    .OrderBy(n => n.OrdenNivel ?? int.MaxValue)
                    .ThenBy(n => n.NombreNivel)
                    .ToListAsync();

                int rubricasActualizadas = 0;
                
                foreach (var rubrica in rubricasSinNiveles)
                {
                    for (int i = 0; i < nivelesComunes.Count; i++)
                    {
                        var rubricaNivel = new RubricaNivel
                        {
                            IdRubrica = rubrica.IdRubrica,
                            IdNivel = nivelesComunes[i].IdNivel,
                            OrdenEnRubrica = i + 1
                        };
                        _context.RubricaNiveles.Add(rubricaNivel);
                    }
                    rubricasActualizadas++;
                }

                await _context.SaveChangesAsync();

                return Json(new { 
                    success = true, 
                    message = $"Se asociaron niveles comunes a {rubricasActualizadas} rúbricas",
                    rubricasActualizadas = rubricasActualizadas,
                    nivelesAsociados = nivelesComunes.Count
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // Método temporal para ejecutar la migración
        //public async Task<IActionResult> MigrarRubricaNivel()
        //{
        //    try
        //    {
        //        var script = new CrearTablaRubricaNivel(_context);
        //        await script.EjecutarAsync();
        //        return Content("Migración completada exitosamente. Puedes eliminar este método después de usar.");
        //    }
        //    catch (Exception ex)
        //    {
        //        return Content($"Error durante la migración: {ex.Message}");
        //    }
        //}

        // Método específico para diagnosticar rúbrica 9
        public async Task<IActionResult> DiagnosticarRubrica9()
        {
            try
            {
                var rubrica = await _context.Rubricas
                    .FirstOrDefaultAsync(r => r.IdRubrica == 9);
                
                if (rubrica == null)
                {
                    return Json(new { error = "Rúbrica 9 no encontrada" });
                }

                // Obtener niveles por RubricaNiveles (sistema anterior)
                var nivelesPorRubricaNiveles = await _context.RubricaNiveles
                    .Where(rn => rn.IdRubrica == 9)
                    .Include(rn => rn.NivelCalificacion)
                    .OrderBy(rn => rn.OrdenEnRubrica)
                    .Select(rn => new { 
                        rn.IdNivel, 
                        rn.NivelCalificacion.NombreNivel, 
                        rn.OrdenEnRubrica,
                        Origen = "RubricaNiveles"
                    })
                    .ToListAsync();

                // Obtener todos los niveles globales
                var todosLosNiveles = await _context.NivelesCalificacion
                    .OrderBy(n => n.OrdenNivel)
                    .Select(n => new { 
                        n.IdNivel, 
                        n.NombreNivel, 
                        n.OrdenNivel,
                        Origen = "Global"
                    })
                    .ToListAsync();

                // Verificar cuáles se están usando actualmente en ConfigurarRubrica
                var nivelesQueSeUsarian = nivelesPorRubricaNiveles.Any() 
                    ? nivelesPorRubricaNiveles.Cast<object>().ToList()
                    : todosLosNiveles.Cast<object>().ToList();

                return Json(new { 
                    rubrica = new { 
                        rubrica.IdRubrica, 
                        rubrica.NombreRubrica
                    },
                    nivelesPorRubricaNiveles = nivelesPorRubricaNiveles,
                    cantidadRubricaNiveles = nivelesPorRubricaNiveles.Count,
                    todosLosNiveles = todosLosNiveles,
                    cantidadNivelesGlobales = todosLosNiveles.Count,
                    nivelesQueSeUsarianEnConfiguracion = nivelesQueSeUsarian,
                    explicacion = "Si hay nivelesPorRubricaNiveles, usará esos. Si no, usará todosLosNiveles."
                });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        // Método para eliminar la rúbrica 9 completamente
        public async Task<IActionResult> EliminarRubrica9()
        {
            try
            {
                var rubrica = await _context.Rubricas.FindAsync(9);
                if (rubrica == null)
                {
                    return Json(new { success = false, message = "Rúbrica 9 no encontrada" });
                }

                // Obtener estadísticas antes de eliminar
                var itemsEvaluacion = await _context.ItemsEvaluacion
                    .Where(i => i.IdRubrica == 9)
                    .CountAsync();

                var valoresRubrica = await _context.ValoresRubrica
                    .Where(v => v.IdRubrica == 9)
                    .CountAsync();

                var rubricaNiveles = await _context.RubricaNiveles
                    .Where(rn => rn.IdRubrica == 9)
                    .CountAsync();

                var evaluaciones = await _context.Evaluaciones
                    .Where(e => e.IdRubrica == 9)
                    .CountAsync();

                // Eliminar en orden para respetar las dependencias
                
                // 1. Eliminar evaluaciones relacionadas
                var evaluacionesRelacionadas = await _context.Evaluaciones
                    .Where(e => e.IdRubrica == 9)
                    .ToListAsync();
                
                if (evaluacionesRelacionadas.Any())
                {
                    _context.Evaluaciones.RemoveRange(evaluacionesRelacionadas);
                }

                // 2. Eliminar valores de rúbrica
                var valoresRelacionados = await _context.ValoresRubrica
                    .Where(v => v.IdRubrica == 9)
                    .ToListAsync();
                
                if (valoresRelacionados.Any())
                {
                    _context.ValoresRubrica.RemoveRange(valoresRelacionados);
                }

                // 3. Eliminar relaciones RubricaNiveles
                var rubricaNivelesRelacionados = await _context.RubricaNiveles
                    .Where(rn => rn.IdRubrica == 9)
                    .ToListAsync();
                
                if (rubricaNivelesRelacionados.Any())
                {
                    _context.RubricaNiveles.RemoveRange(rubricaNivelesRelacionados);
                }

                // 4. Eliminar items de evaluación
                var itemsRelacionados = await _context.ItemsEvaluacion
                    .Where(i => i.IdRubrica == 9)
                    .ToListAsync();
                
                if (itemsRelacionados.Any())
                {
                    _context.ItemsEvaluacion.RemoveRange(itemsRelacionados);
                }

                // 5. Finalmente eliminar la rúbrica
                _context.Rubricas.Remove(rubrica);

                await _context.SaveChangesAsync();

                return Json(new { 
                    success = true, 
                    message = "Rúbrica 9 eliminada exitosamente junto con todos sus datos relacionados",
                    estadisticasEliminadas = new {
                        evaluaciones = evaluaciones,
                        valoresRubrica = valoresRubrica,
                        rubricaNiveles = rubricaNiveles,
                        itemsEvaluacion = itemsEvaluacion
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}", stackTrace = ex.StackTrace });
            }
        }

        // Método temporal para verificar y crear datos de grupos
        public async Task<IActionResult> VerificarYCrearDatosGrupos()
        {
            try
            {
                // Verificar si existen grupos
                var cantidadGrupos = await _context.GruposCalificacion.CountAsync();
                var cantidadNiveles = await _context.NivelesCalificacion.CountAsync();
                
                dynamic resultado = new {
                    gruposExistentes = cantidadGrupos,
                    nivelesExistentes = cantidadNiveles,
                    mensaje = $"Grupos: {cantidadGrupos}, Niveles: {cantidadNiveles}"
                };
                
                // Si no hay datos, crear algunos básicos
                if (cantidadGrupos == 0 || cantidadNiveles == 0)
                {
                    // Crear niveles básicos si no existen
                    if (cantidadNiveles == 0)
                    {
                        var nivelesBasicos = new List<NivelCalificacion>
                        {
                            new NivelCalificacion { NombreNivel = "Excelente", Descripcion = "Desempeño sobresaliente", OrdenNivel = 1 },
                            new NivelCalificacion { NombreNivel = "Bueno", Descripcion = "Desempeño satisfactorio", OrdenNivel = 2 },
                            new NivelCalificacion { NombreNivel = "Regular", Descripcion = "Desempeño mínimo aceptable", OrdenNivel = 3 },
                            new NivelCalificacion { NombreNivel = "Deficiente", Descripcion = "Desempeño insuficiente", OrdenNivel = 4 }
                        };
                        
                        _context.NivelesCalificacion.AddRange(nivelesBasicos);
                        await _context.SaveChangesAsync();
                    }
                    
                    // Crear grupo básico si no existe
                    if (cantidadGrupos == 0)
                    {
                        var grupoBasico = new GrupoCalificacion
                        {
                            NombreGrupo = "Grupo Estándar",
                            Descripcion = "Grupo de calificación estándar con niveles básicos",
                            Estado = "ACTIVO",
                            FechaCreacion = DateTime.Now
                        };
                        
                        _context.GruposCalificacion.Add(grupoBasico);
                        await _context.SaveChangesAsync();
                        
                        // Asignar los niveles al grupo
                        var niveles = await _context.NivelesCalificacion
                            .Where(n => n.IdGrupo == null)
                            .ToListAsync();
                            
                        foreach (var nivel in niveles)
                        {
                            nivel.IdGrupo = grupoBasico.IdGrupo;
                        }
                        
                        await _context.SaveChangesAsync();
                    }
                    
                    // Recalcular contadores
                    cantidadGrupos = await _context.GruposCalificacion.CountAsync();
                    cantidadNiveles = await _context.NivelesCalificacion.CountAsync();
                    
                    resultado = new {
                        gruposExistentes = cantidadGrupos,
                        nivelesExistentes = cantidadNiveles,
                        mensaje = $"Datos creados - Grupos: {cantidadGrupos}, Niveles: {cantidadNiveles}",
                        datosCreados = true
                    };
                }
                
                return Json(resultado);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        /// <summary>
        /// Método de debug para verificar permisos del usuario actual
        /// </summary>
        public async Task<IActionResult> DebugPermisos()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { error = "Usuario no autenticado" });
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return Json(new { error = "Usuario no encontrado" });
                }

                var userRoles = await _userManager.GetRolesAsync(user);
                var userPermissions = await _permissionService.GetUserPermissionsAsync(userId);
                var userClaims = await _userManager.GetClaimsAsync(user);

                // Verificar un permiso específico como ejemplo
                var testPermission = "Configuracion.VER";
                var hasTestPermission = await _permissionService.HasPermissionAsync(userId, testPermission);

                var result = new
                {
                    userId = userId,
                    userName = user.UserName,
                    email = user.Email,
                    isAuthenticated = User.Identity?.IsAuthenticated ?? false,
                    roles = userRoles.ToList(),
                    permissionsCount = userPermissions.Count,
                    permissions = userPermissions.Take(20).ToList(), // Primeros 20 para no saturar
                    claimsCount = userClaims.Count,
                    claims = userClaims.Select(c => new { type = c.Type, value = c.Value }).Take(10).ToList(),
                    testPermission = testPermission,
                    hasTestPermission = hasTestPermission,
                    isSuperAdmin = userRoles.Contains("SuperAdmin")
                };

                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        /// <summary>
        /// Debug de roles del sistema
        /// </summary>
        [Route("Home/DebugRoles")]
        public async Task<IActionResult> DebugRoles()
        {
            try
            {
                var roleManager = HttpContext.RequestServices.GetRequiredService<RoleManager<IdentityRole>>();
                
                // Obtener todos los roles del sistema
                var allRoles = await roleManager.Roles.ToListAsync();
                
                // Obtener usuario actual
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var currentUser = userId != null ? await _userManager.FindByIdAsync(userId) : null;
                var currentUserRoles = currentUser != null ? await _userManager.GetRolesAsync(currentUser) : new List<string>();
                
                var result = new
                {
                    currentUser = new
                    {
                        id = currentUser?.Id ?? "N/A",
                        email = currentUser?.Email ?? "N/A",
                        roles = currentUserRoles.ToList()
                    },
                    systemRoles = allRoles.Select(r => new {
                        id = r.Id,
                        name = r.Name,
                        normalizedName = r.NormalizedName
                    }).ToList(),
                    applicationRoleConstants = new
                    {
                        superAdministrador = Models.Permissions.ApplicationRoles.SuperAdministrador,
                        administrador = Models.Permissions.ApplicationRoles.Administrador,
                        coordinador = Models.Permissions.ApplicationRoles.Coordinador,
                        docente = Models.Permissions.ApplicationRoles.Docente,
                        evaluador = Models.Permissions.ApplicationRoles.Evaluador
                    },
                    recommendations = new List<string>()
                };
                
                // Agregar recomendaciones
                var recommendations = new List<string>();
                
                if (!allRoles.Any(r => r.Name == Models.Permissions.ApplicationRoles.SuperAdministrador))
                {
                    recommendations.Add($"Crear rol: {Models.Permissions.ApplicationRoles.SuperAdministrador}");
                }
                
                if (currentUser != null && !currentUserRoles.Contains(Models.Permissions.ApplicationRoles.SuperAdministrador))
                {
                    recommendations.Add($"Asignar rol {Models.Permissions.ApplicationRoles.SuperAdministrador} al usuario {currentUser.Email}");
                }
                
                return Json(new
                {
                    result.currentUser,
                    result.systemRoles,
                    result.applicationRoleConstants,
                    recommendations = recommendations
                });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }
    }
}

public class ErrorViewModel
{
    public string? RequestId { get; set; }
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}