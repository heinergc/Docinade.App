using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RubricasApp.Web.Data;
using RubricasApp.Web.Interfaces;
using RubricasApp.Web.Models;
using RubricasApp.Web.Services;
using RubricasApp.Web.ViewModels.Conducta;
using System.Security.Claims;

namespace RubricasApp.Web.Controllers
{
    [Authorize]
    public class ProgramaAccionesController : BaseController
    {
        private readonly IConductaService _conductaService;
        private readonly ILogger<ProgramaAccionesController> _logger;

        public ProgramaAccionesController(
            RubricasDbContext context,
            IPeriodoAcademicoService periodoService,
            IConductaService conductaService,
            ILogger<ProgramaAccionesController> logger)
            : base(periodoService, context)
        {
            _conductaService = conductaService;
            _logger = logger;
        }

        // GET: ProgramaAcciones
        public async Task<IActionResult> Index(int? idPeriodo, string? estado, int? idSupervisor)
        {
            try
            {
                // Obtener todos los programas
                var query = _context.ProgramasAccionesInstitucional
                    .Include(p => p.Estudiante)
                    .Include(p => p.Periodo)
                    .Include(p => p.ResponsableSupervision)
                    .Include(p => p.NotaConducta)
                    .AsQueryable();

                // Aplicar filtros
                if (idPeriodo.HasValue)
                {
                    query = query.Where(p => p.IdPeriodo == idPeriodo.Value);
                }

                if (!string.IsNullOrEmpty(estado))
                {
                    query = query.Where(p => p.Estado == estado);
                }

                if (idSupervisor.HasValue)
                {
                    query = query.Where(p => p.ResponsableSupervisionId == idSupervisor.ToString());
                }

                var programas = await query
                    .OrderByDescending(p => p.FechaCreacion)
                    .ToListAsync();

                // Mapear a ViewModels
                var modelo = programas.Select(p => new ProgramaAccionesListViewModel
                {
                    IdPrograma = p.IdPrograma,
                    TituloPrograma = p.TituloPrograma,
                    NombreEstudiante = p.Estudiante.NombreCompleto,
                    NumeroId = p.Estudiante.NumeroId,
                    NumeroIdentificacion = p.Estudiante.NumeroId, // Alias
                    Periodo = p.Periodo.NombreCompleto,
                    NombrePeriodo = p.Periodo.NombreCompleto, // Alias
                    FechaInicio = p.FechaInicio,
                    FechaFinPrevista = p.FechaFinPrevista,
                    FechaFinReal = p.FechaFinReal,
                    Estado = p.Estado,
                    ResultadoFinal = p.ResultadoFinal,
                    ResponsableSupervision = p.ResponsableSupervision.NombreCompleto,
                    NombreSupervisor = p.ResponsableSupervision.NombreCompleto, // Alias
                    AprobarConducta = p.AprobarConducta,
                    FechaVerificacion = p.FechaVerificacion
                }).ToList();

                // Cargar datos para filtros
                await CargarDatosFiltros(idPeriodo, idSupervisor);
                ViewBag.IdPeriodoSeleccionado = idPeriodo?.ToString();
                ViewBag.EstadoSeleccionado = estado;
                ViewBag.IdSupervisorSeleccionado = idSupervisor?.ToString();

                return View(modelo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar la lista de programas de acciones");
                TempData["Error"] = "Error al cargar los programas de acciones.";
                return View(new List<ProgramaAccionesListViewModel>());
            }
        }

        // GET: ProgramaAcciones/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var programa = await _context.ProgramasAccionesInstitucional
                    .Include(p => p.Estudiante)
                    .Include(p => p.Periodo)
                    .Include(p => p.ResponsableSupervision)
                    .Include(p => p.VerificadoPor)
                    .Include(p => p.NotaConducta)
                    .FirstOrDefaultAsync(p => p.IdPrograma == id);

                if (programa == null)
                {
                    TempData["Error"] = "Programa no encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                // Obtener grupo principal del estudiante
                var grupoEstudiante = await _context.EstudianteGrupos
                    .Include(eg => eg.Grupo)
                    .Where(eg => eg.EstudianteId == programa.IdEstudiante && eg.EsGrupoPrincipal)
                    .FirstOrDefaultAsync();

                // Mapear a ViewModel
                var viewModel = new ProgramaAccionesDetalleViewModel
                {
                    IdPrograma = programa.IdPrograma,
                    TituloPrograma = programa.TituloPrograma,
                    Descripcion = programa.Descripcion,
                    ObjetivosEspecificos = programa.ObjetivosEspecificos,
                    Objetivos = programa.ObjetivosEspecificos, // Alias
                    ActividadesARealizar = programa.ActividadesARealizar,
                    ActividadesPropuestas = programa.ActividadesARealizar, // Alias
                    MetasEsperadas = null, // No existe en modelo
                    RecursosNecesarios = null, // No existe en modelo
                    CompromisosEstudiante = programa.CompromisosEstudiante,
                    CompromisosFamilia = programa.CompromisosFamilia,
                    CriteriosEvaluacion = programa.CriteriosEvaluacion,
                    
                    IdEstudiante = programa.IdEstudiante,
                    IdPeriodo = programa.IdPeriodo,
                    NombreEstudiante = $"{programa.Estudiante.Nombre} {programa.Estudiante.Apellidos}".Trim(),
                    NumeroIdentificacion = programa.Estudiante.NumeroId ?? "N/A",
                    NombreGrupo = grupoEstudiante?.Grupo?.Nombre ?? "Sin grupo",
                    NombrePeriodo = programa.Periodo.Nombre,
                    NotaConducta = programa.NotaConducta?.NotaFinal ?? 0,
                    
                    FechaInicio = programa.FechaInicio,
                    FechaFinPrevista = programa.FechaFinPrevista,
                    FechaFinReal = programa.FechaFinReal,
                    
                    NombreResponsableSupervision = programa.ResponsableSupervision != null 
                        ? $"{programa.ResponsableSupervision.Nombre} {programa.ResponsableSupervision.Apellidos}".Trim() 
                        : "No asignado",
                    NombreSupervisor = programa.ResponsableSupervision != null 
                        ? $"{programa.ResponsableSupervision.Nombre} {programa.ResponsableSupervision.Apellidos}".Trim() 
                        : "No asignado", // Alias
                    ObservacionesSupervision = programa.ObservacionesSupervision,
                    
                    Estado = programa.Estado,
                    Resultado = programa.ResultadoFinal, // Alias
                    FechaCreacion = programa.FechaCreacion,
                    
                    Verificado = programa.VerificadoPorId != null, // Calculado
                    FechaVerificacion = programa.FechaVerificacion,
                    NombreVerificador = programa.VerificadoPor != null 
                        ? $"{programa.VerificadoPor.Nombre} {programa.VerificadoPor.Apellidos}".Trim() 
                        : null,
                    ResultadoFinal = programa.ResultadoFinal,
                    ConclusionesComite = programa.ConclusionesComite,
                    ObservacionesVerificacion = null, // No existe en modelo
                    AprobarConducta = programa.AprobarConducta
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar detalles del programa {IdPrograma}", id);
                TempData["Error"] = "Error al cargar los detalles del programa.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: ProgramaAcciones/Verificar/5
        public async Task<IActionResult> Verificar(int id)
        {
            try
            {
                var programa = await _context.ProgramasAccionesInstitucional
                    .Include(p => p.Estudiante)
                    .Include(p => p.Periodo)
                    .Include(p => p.ResponsableSupervision)
                    .Include(p => p.NotaConducta)
                    .FirstOrDefaultAsync(p => p.IdPrograma == id);

                if (programa == null)
                {
                    TempData["Error"] = "Programa no encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                if (programa.Estado != "En Proceso")
                {
                    TempData["Error"] = "Solo se pueden verificar programas en proceso.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                if (programa.FechaVerificacion.HasValue)
                {
                    TempData["Error"] = "Este programa ya ha sido verificado.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                // Obtener grupo principal del estudiante
                var grupoEstudiante = await _context.EstudianteGrupos
                    .Include(eg => eg.Grupo)
                    .Where(eg => eg.EstudianteId == programa.IdEstudiante && eg.EsGrupoPrincipal)
                    .FirstOrDefaultAsync();

                // Mapear a ViewModel
                var viewModel = new VerificarProgramaViewModel
                {
                    IdPrograma = programa.IdPrograma,
                    TituloPrograma = programa.TituloPrograma,
                    NombreEstudiante = $"{programa.Estudiante.Nombre} {programa.Estudiante.Apellidos}".Trim(),
                    NumeroIdentificacion = programa.Estudiante.NumeroId ?? "N/A",
                    NombreGrupo = grupoEstudiante?.Grupo?.Nombre ?? "Sin grupo",
                    NombrePeriodo = programa.Periodo.Nombre,
                    Descripcion = programa.Descripcion,
                    NombreSupervisor = programa.ResponsableSupervision != null 
                        ? $"{programa.ResponsableSupervision.Nombre} {programa.ResponsableSupervision.Apellidos}".Trim() 
                        : "No asignado",
                    Objetivos = programa.ObjetivosEspecificos,
                    CriteriosEvaluacion = null, // No existe en modelo
                    FechaInicio = programa.FechaInicio,
                    FechaFinPrevista = programa.FechaFinPrevista,
                    FechaFinReal = DateTime.Now,
                    FechaVerificacion = DateTime.Now,
                    ResultadoFinal = string.Empty,
                    ConclusionesComite = string.Empty,
                    ObservacionesVerificacion = null,
                    AprobarConducta = false
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar formulario de verificación del programa {IdPrograma}", id);
                TempData["Error"] = "Error al cargar el formulario de verificación.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        // POST: ProgramaAcciones/Verificar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Verificar(int idPrograma, string resultado, string? observaciones, DateTime fechaVerificacion)
        {
            try
            {
                var programa = await _context.ProgramasAccionesInstitucional
                    .Include(p => p.NotaConducta)
                    .FirstOrDefaultAsync(p => p.IdPrograma == idPrograma);

                if (programa == null)
                {
                    TempData["Error"] = "Programa no encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                // Obtener el ID del usuario actual
                var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Actualizar el programa
                programa.FechaVerificacion = fechaVerificacion;
                programa.VerificadoPorId = usuarioId;
                programa.ResultadoFinal = resultado;
                programa.ConclusionesComite = observaciones;
                programa.FechaFinReal = DateTime.Now;
                programa.Estado = resultado == "Satisfactorio" ? "Completado" : "No Completado";
                programa.AprobarConducta = resultado == "Satisfactorio";

                _context.Update(programa);
                await _context.SaveChangesAsync();

                // Si el programa fue exitoso, recalcular la nota del estudiante
                if (resultado == "Satisfactorio" && programa.NotaConducta != null)
                {
                    await _conductaService.CalcularNotaConductaAsync(
                        programa.IdEstudiante, 
                        programa.IdPeriodo
                    );
                }

                TempData["Success"] = "Programa verificado exitosamente.";
                _logger.LogInformation(
                    "Programa {IdPrograma} verificado con resultado: {Resultado} por usuario {UsuarioId}",
                    programa.IdPrograma, resultado, usuarioId
                );

                return RedirectToAction(nameof(Details), new { id = idPrograma });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar el programa {IdPrograma}", idPrograma);
                ModelState.AddModelError("", "Error al verificar el programa. Intente nuevamente.");
                return RedirectToAction(nameof(Verificar), new { id = idPrograma });
            }
        }

        // GET: ProgramaAcciones/Create
        public async Task<IActionResult> Create(int? idEstudiante, int? idPeriodo)
        {
            try
            {
                var viewModel = new CrearProgramaViewModel
                {
                    IdEstudiante = idEstudiante ?? 0,
                    IdPeriodo = idPeriodo ?? 0,
                    FechaInicio = DateTime.Now,
                    FechaFinPrevista = DateTime.Now.AddMonths(2),
                    Estado = "Pendiente"
                };

                await CargarDatosCreacion();
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar formulario de creación");
                TempData["Error"] = "Error al cargar el formulario.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: ProgramaAcciones/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CrearProgramaViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await CargarDatosCreacion();
                    return View(viewModel);
                }

                // Validar que el estudiante tenga una nota de conducta aplazada
                var notaConducta = await _context.NotasConducta
                    .FirstOrDefaultAsync(n => n.IdEstudiante == viewModel.IdEstudiante 
                                           && n.IdPeriodo == viewModel.IdPeriodo);

                if (notaConducta == null)
                {
                    ModelState.AddModelError("", "El estudiante no tiene una nota de conducta registrada para este período.");
                    await CargarDatosCreacion();
                    return View(viewModel);
                }

                // Validar que no tenga ya un programa o decisión
                var programaExistente = await _context.ProgramasAccionesInstitucional
                    .AnyAsync(p => p.IdEstudiante == viewModel.IdEstudiante 
                                && p.IdPeriodo == viewModel.IdPeriodo);

                if (programaExistente)
                {
                    ModelState.AddModelError("", "El estudiante ya tiene un programa asignado para este período.");
                    await CargarDatosCreacion();
                    return View(viewModel);
                }

                // Crear el programa
                var programa = new ProgramaAccionesInstitucional
                {
                    IdNotaConducta = notaConducta.IdNotaConducta,
                    IdEstudiante = viewModel.IdEstudiante,
                    IdPeriodo = viewModel.IdPeriodo,
                    TituloPrograma = viewModel.TituloPrograma,
                    Descripcion = viewModel.Descripcion,
                    ObjetivosEspecificos = viewModel.ObjetivosEspecificos,
                    ActividadesARealizar = viewModel.ActividadesARealizar,
                    CompromisosEstudiante = viewModel.CompromisosEstudiante,
                    CompromisosFamilia = viewModel.CompromisosFamilia,
                    CriteriosEvaluacion = viewModel.CriteriosEvaluacion,
                    FechaInicio = viewModel.FechaInicio,
                    FechaFinPrevista = viewModel.FechaFinPrevista,
                    ResponsableSupervisionId = viewModel.ResponsableSupervisionId,
                    Estado = viewModel.Estado ?? "Pendiente",
                    FechaCreacion = DateTime.Now
                };

                _context.ProgramasAccionesInstitucional.Add(programa);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Programa de acciones creado exitosamente.";
                _logger.LogInformation("Programa {IdPrograma} creado para estudiante {IdEstudiante}", 
                    programa.IdPrograma, programa.IdEstudiante);

                return RedirectToAction(nameof(Details), new { id = programa.IdPrograma });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear programa de acciones");
                ModelState.AddModelError("", "Error al crear el programa. Intente nuevamente.");
                await CargarDatosCreacion();
                return View(viewModel);
            }
        }

        // GET: ProgramaAcciones/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var programa = await _context.ProgramasAccionesInstitucional
                    .Include(p => p.Estudiante)
                    .Include(p => p.Periodo)
                    .FirstOrDefaultAsync(p => p.IdPrograma == id);

                if (programa == null)
                {
                    TempData["Error"] = "Programa no encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                // No permitir editar programas verificados
                if (programa.FechaVerificacion.HasValue)
                {
                    TempData["Error"] = "No se puede editar un programa que ya ha sido verificado.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                var viewModel = new EditarProgramaViewModel
                {
                    IdPrograma = programa.IdPrograma,
                    IdEstudiante = programa.IdEstudiante,
                    IdPeriodo = programa.IdPeriodo,
                    NombreEstudiante = programa.Estudiante.NombreCompleto,
                    NombrePeriodo = programa.Periodo.Nombre,
                    TituloPrograma = programa.TituloPrograma,
                    Descripcion = programa.Descripcion,
                    ObjetivosEspecificos = programa.ObjetivosEspecificos,
                    ActividadesARealizar = programa.ActividadesARealizar,
                    CompromisosEstudiante = programa.CompromisosEstudiante,
                    CompromisosFamilia = programa.CompromisosFamilia,
                    CriteriosEvaluacion = programa.CriteriosEvaluacion,
                    FechaInicio = programa.FechaInicio,
                    FechaFinPrevista = programa.FechaFinPrevista,
                    FechaFinReal = programa.FechaFinReal,
                    ResponsableSupervisionId = programa.ResponsableSupervisionId,
                    ObservacionesSupervision = programa.ObservacionesSupervision,
                    Estado = programa.Estado,
                    ResultadoFinal = programa.ResultadoFinal
                };

                await CargarDatosEdicion();
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar programa para edición {IdPrograma}", id);
                TempData["Error"] = "Error al cargar el programa.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: ProgramaAcciones/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditarProgramaViewModel viewModel)
        {
            if (id != viewModel.IdPrograma)
            {
                TempData["Error"] = "ID de programa no coincide.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    await CargarDatosEdicion();
                    return View(viewModel);
                }

                var programa = await _context.ProgramasAccionesInstitucional
                    .Include(p => p.Estudiante)
                    .Include(p => p.Periodo)
                    .FirstOrDefaultAsync(p => p.IdPrograma == id);

                if (programa == null)
                {
                    TempData["Error"] = "Programa no encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                // No permitir editar programas verificados
                if (programa.FechaVerificacion.HasValue)
                {
                    TempData["Error"] = "No se puede editar un programa que ya ha sido verificado.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                // Actualizar campos editables
                programa.TituloPrograma = viewModel.TituloPrograma;
                programa.Descripcion = viewModel.Descripcion;
                programa.ObjetivosEspecificos = viewModel.ObjetivosEspecificos;
                programa.ActividadesARealizar = viewModel.ActividadesARealizar;
                programa.CompromisosEstudiante = viewModel.CompromisosEstudiante;
                programa.CompromisosFamilia = viewModel.CompromisosFamilia;
                programa.CriteriosEvaluacion = viewModel.CriteriosEvaluacion;
                programa.FechaInicio = viewModel.FechaInicio;
                programa.FechaFinPrevista = viewModel.FechaFinPrevista;
                programa.FechaFinReal = viewModel.FechaFinReal;
                programa.ResponsableSupervisionId = viewModel.ResponsableSupervisionId;
                programa.ObservacionesSupervision = viewModel.ObservacionesSupervision;
                programa.Estado = viewModel.Estado;
                programa.ResultadoFinal = viewModel.ResultadoFinal;

                // Si el programa se marca como Completado con resultado Satisfactorio, aprobar conducta
                if (viewModel.Estado == "Completado" && 
                    !string.IsNullOrEmpty(viewModel.ResultadoFinal) && 
                    (viewModel.ResultadoFinal.Contains("Satisfactorio") || viewModel.ResultadoFinal.Contains("Exitoso")))
                {
                    programa.AprobarConducta = true;
                }

                // Preservar datos que vienen del ViewModel (para retorno a vista en caso de error)
                viewModel.NombreEstudiante = programa.Estudiante.NombreCompleto;
                viewModel.NombrePeriodo = programa.Periodo.Nombre;

                await _context.SaveChangesAsync();

                // Si se completó exitosamente, recalcular la nota de conducta
                if (programa.AprobarConducta && programa.Estado == "Completado")
                {
                    try
                    {
                        await _conductaService.CalcularNotaConductaAsync(
                            programa.IdEstudiante,
                            programa.IdPeriodo
                        );
                        
                        _logger.LogInformation(
                            "Nota de conducta recalculada para estudiante {IdEstudiante} en periodo {IdPeriodo} tras completar programa {IdPrograma}",
                            programa.IdEstudiante, programa.IdPeriodo, programa.IdPrograma
                        );
                    }
                    catch (Exception calcEx)
                    {
                        _logger.LogError(calcEx, 
                            "Error al recalcular nota de conducta para estudiante {IdEstudiante} tras completar programa {IdPrograma}",
                            programa.IdEstudiante, programa.IdPrograma
                        );
                        // No fallar la actualización del programa si falla el cálculo
                    }
                }

                TempData["Success"] = "Programa actualizado exitosamente.";
                _logger.LogInformation("Programa {IdPrograma} actualizado por usuario {UserId}", programa.IdPrograma, User.Identity?.Name);

                return RedirectToAction(nameof(Details), new { id = programa.IdPrograma });
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!await ProgramaExists(viewModel.IdPrograma))
                {
                    _logger.LogWarning("Intento de actualizar programa {IdPrograma} que fue eliminado", id);
                    TempData["Error"] = "El programa fue eliminado por otro usuario.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _logger.LogError(ex, "Error de concurrencia al actualizar programa {IdPrograma}", id);
                    TempData["Error"] = "El programa fue modificado por otro usuario. Por favor, recargue la página e intente nuevamente.";
                    return RedirectToAction(nameof(Edit), new { id });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar programa {IdPrograma}: {Message}", id, ex.Message);
                ModelState.AddModelError("", $"Error al actualizar el programa: {ex.Message}");
                await CargarDatosEdicion();
                return View(viewModel);
            }
        }

        // GET: ProgramaAcciones/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var programa = await _context.ProgramasAccionesInstitucional
                    .Include(p => p.Estudiante)
                    .Include(p => p.Periodo)
                    .Include(p => p.ResponsableSupervision)
                    .FirstOrDefaultAsync(p => p.IdPrograma == id);

                if (programa == null)
                {
                    TempData["Error"] = "Programa no encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                // No permitir eliminar programas verificados
                if (programa.FechaVerificacion.HasValue)
                {
                    TempData["Error"] = "No se puede eliminar un programa que ya ha sido verificado.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                var viewModel = new EliminarProgramaViewModel
                {
                    IdPrograma = programa.IdPrograma,
                    TituloPrograma = programa.TituloPrograma,
                    NombreEstudiante = programa.Estudiante.NombreCompleto,
                    NombrePeriodo = programa.Periodo.Nombre,
                    Estado = programa.Estado,
                    FechaCreacion = programa.FechaCreacion,
                    NombreSupervisor = programa.ResponsableSupervision?.NombreCompleto ?? "No asignado"
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar confirmación de eliminación {IdPrograma}", id);
                TempData["Error"] = "Error al cargar la confirmación.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: ProgramaAcciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var programa = await _context.ProgramasAccionesInstitucional
                    .FirstOrDefaultAsync(p => p.IdPrograma == id);

                if (programa == null)
                {
                    TempData["Error"] = "Programa no encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                // No permitir eliminar programas verificados
                if (programa.FechaVerificacion.HasValue)
                {
                    TempData["Error"] = "No se puede eliminar un programa que ya ha sido verificado.";
                    return RedirectToAction(nameof(Index));
                }

                _context.ProgramasAccionesInstitucional.Remove(programa);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Programa eliminado exitosamente.";
                _logger.LogInformation("Programa {IdPrograma} eliminado", id);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar programa {IdPrograma}", id);
                TempData["Error"] = "Error al eliminar el programa. Puede tener datos relacionados.";
                return RedirectToAction(nameof(Delete), new { id });
            }
        }

        // GET: ProgramaAcciones/ExportarProgramas
        public async Task<IActionResult> ExportarProgramas(int? idPeriodo, string? estado, int? idSupervisor)
        {
            try
            {
                // Obtener los datos
                var query = _context.ProgramasAccionesInstitucional
                    .Include(p => p.Estudiante)
                    .Include(p => p.Periodo)
                    .Include(p => p.ResponsableSupervision)
                    .Include(p => p.NotaConducta)
                        .ThenInclude(n => n.Estudiante)
                    .AsQueryable();

                // Aplicar filtros
                if (idPeriodo.HasValue)
                {
                    query = query.Where(p => p.IdPeriodo == idPeriodo.Value);
                }

                if (!string.IsNullOrEmpty(estado))
                {
                    query = query.Where(p => p.Estado == estado);
                }

                if (idSupervisor.HasValue)
                {
                    query = query.Where(p => p.ResponsableSupervisionId == idSupervisor.ToString());
                }

                var programas = await query
                    .OrderByDescending(p => p.FechaCreacion)
                    .ToListAsync();

                // Crear archivo CSV
                var csv = new System.Text.StringBuilder();
                csv.AppendLine("ID,Estudiante,Identificación,Período,Supervisor,Fecha Inicio,Fecha Fin Prevista,Estado,Verificado,Resultado");

                foreach (var programa in programas)
                {
                    csv.AppendLine($"{programa.IdPrograma}," +
                        $"\"{programa.Estudiante.NombreCompleto}\"," +
                        $"\"{programa.Estudiante.NumeroId}\"," +
                        $"\"{programa.Periodo.Nombre}\"," +
                        $"\"{programa.ResponsableSupervision?.NombreCompleto ?? "No asignado"}\"," +
                        $"{programa.FechaInicio:yyyy-MM-dd}," +
                        $"{programa.FechaFinPrevista:yyyy-MM-dd}," +
                        $"\"{programa.Estado}\"," +
                        $"{(programa.FechaVerificacion.HasValue ? "Sí" : "No")}," +
                        $"\"{programa.ResultadoFinal ?? ""}\"");
                }

                var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
                var fileName = $"ProgramasAcciones_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

                return File(bytes, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al exportar programas de acciones");
                TempData["Error"] = "Error al exportar los datos.";
                return RedirectToAction(nameof(Index));
            }
        }

        #region Métodos Auxiliares

        private async Task<bool> ProgramaExists(int id)
        {
            return await _context.ProgramasAccionesInstitucional.AnyAsync(p => p.IdPrograma == id);
        }

        private async Task CargarDatosCreacion()
        {
            // Estudiantes con nota de conducta aplazada
            var estudiantesAplazados = await _context.NotasConducta
                .Include(n => n.Estudiante)
                .Include(n => n.Periodo)
                .Where(n => n.Estado == "Aplazado")
                .Select(n => new { n.IdEstudiante, n.Estudiante.NombreCompleto, n.IdPeriodo, PeriodoNombre = n.Periodo.Nombre })
                .Distinct()
                .OrderBy(e => e.NombreCompleto)
                .ToListAsync();

            ViewBag.Estudiantes = estudiantesAplazados.Select(e => new SelectListItem
            {
                Value = e.IdEstudiante.ToString(),
                Text = $"{e.NombreCompleto} - Periodo: {e.PeriodoNombre}"
            }).ToList();

            // Períodos activos
            var periodos = await _context.PeriodosAcademicos
                .Where(p => p.Activo)
                .OrderByDescending(p => p.Anio)
                .ThenByDescending(p => p.NumeroPeriodo)
                .ToListAsync();

            ViewBag.Periodos = periodos.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Nombre
            }).ToList();

            // Supervisores (usuarios activos - de AspNetUsers)
            var supervisores = await _context.Users
                .Where(u => u.Activo)
                .OrderBy(u => u.Apellidos)
                .ThenBy(u => u.Nombre)
                .ToListAsync();

            ViewBag.Supervisores = supervisores.Select(s => new SelectListItem
            {
                Value = s.Id, // Id de AspNetUsers (string GUID)
                Text = $"{s.Apellidos} {s.Nombre}".Trim()
            }).ToList();

            // Estados
            ViewBag.Estados = new List<SelectListItem>
            {
                new SelectListItem { Value = "Pendiente", Text = "Pendiente" },
                new SelectListItem { Value = "En Proceso", Text = "En Proceso" },
                new SelectListItem { Value = "Completado", Text = "Completado" },
                new SelectListItem { Value = "No Completado", Text = "No Completado" }
            };
        }

        private async Task CargarDatosEdicion()
        {
            // Supervisores (usuarios activos - de AspNetUsers)
            var supervisores = await _context.Users
                .Where(u => u.Activo)
                .OrderBy(u => u.Apellidos)
                .ThenBy(u => u.Nombre)
                .ToListAsync();

            ViewBag.Supervisores = supervisores.Select(s => new SelectListItem
            {
                Value = s.Id, // Id de AspNetUsers (string GUID)
                Text = $"{s.Apellidos} {s.Nombre}".Trim()
            }).ToList();

            // Estados
            ViewBag.Estados = new List<SelectListItem>
            {
                new SelectListItem { Value = "Pendiente", Text = "Pendiente" },
                new SelectListItem { Value = "En Proceso", Text = "En Proceso" },
                new SelectListItem { Value = "Completado", Text = "Completado" },
                new SelectListItem { Value = "No Completado", Text = "No Completado" }
            };

            // Resultados Finales
            ViewBag.Resultados = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "-- Sin resultado aún --" },
                new SelectListItem { Value = "Satisfactorio", Text = "Satisfactorio" },
                new SelectListItem { Value = "No Satisfactorio", Text = "No Satisfactorio" },
                new SelectListItem { Value = "Exitoso", Text = "Exitoso" },
                new SelectListItem { Value = "No Exitoso", Text = "No Exitoso" }
            };
        }

        private async Task CargarDatosFiltros(int? idPeriodoSeleccionado, int? idSupervisorSeleccionado)
        {
            // Períodos
            var periodos = await _context.PeriodosAcademicos
                .Where(p => p.Activo)
                .OrderByDescending(p => p.Anio)
                .ThenByDescending(p => p.NumeroPeriodo)
                .ToListAsync();

            ViewBag.Periodos = periodos.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Nombre,
                Selected = p.Id == idPeriodoSeleccionado
            }).ToList();

            // Supervisores (profesores que son orientadores o del comité)
            var supervisores = await _context.Profesores
                .Where(p => p.Estado)
                .OrderBy(p => p.Nombres)
                .ThenBy(p => p.PrimerApellido)
                .ToListAsync();

            ViewBag.Supervisores = supervisores.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = $"{s.Nombres} {s.PrimerApellido} {s.SegundoApellido ?? ""}".Trim(),
                Selected = s.Id == idSupervisorSeleccionado
            }).ToList();
        }

        #endregion
    }
}
