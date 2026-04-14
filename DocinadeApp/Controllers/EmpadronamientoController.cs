using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RubricasApp.Web.Data;
using RubricasApp.Web.Models;
using RubricasApp.Web.DTOs;
using RubricasApp.Web.ViewModels;
using RubricasApp.Web.Authorization;
using RubricasApp.Web.Models.Permissions;
using System.Text.Json;

namespace RubricasApp.Web.Controllers
{
    [Authorize]
    public class EmpadronamientoController : Controller
    {
        private readonly RubricasDbContext _context;
        private readonly ILogger<EmpadronamientoController> _logger;

        public EmpadronamientoController(RubricasDbContext context, ILogger<EmpadronamientoController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Empadronamiento
        [RequirePermission(ApplicationPermissions.Estudiantes.VER)]
        public async Task<IActionResult> Index()
        {
            var estudiantes = await _context.Estudiantes
                .Include(e => e.PeriodoAcademico)
                .Select(e => new EstudianteEmpadronamientoListDto
                {
                    IdEstudiante = e.IdEstudiante,
                    NombreCompleto = e.Nombre + " " + e.Apellidos,
                    NumeroId = e.NumeroId,
                    DireccionCorreo = e.DireccionCorreo,
                    Institucion = e.Institucion,
                    PeriodoNombre = e.PeriodoAcademico!.Nombre,
                    TieneEmpadronamiento = _context.EstudiantesEmpadronamiento
                        .Any(ee => ee.IdEstudiante == e.IdEstudiante),
                    EtapaActual = _context.EstudiantesEmpadronamiento
                        .Where(ee => ee.IdEstudiante == e.IdEstudiante)
                        .Select(ee => ee.EtapaActual)
                        .FirstOrDefault()
                })
                .OrderBy(e => e.NombreCompleto)
                .ToListAsync();

            return View(estudiantes);
        }

        // GET: Empadronamiento/Details/5
        [RequirePermission(ApplicationPermissions.Estudiantes.VER)]
        public async Task<IActionResult> Details(int id)
        {
            var estudiante = await _context.Estudiantes
                .Include(e => e.PeriodoAcademico)
                .FirstOrDefaultAsync(e => e.IdEstudiante == id);

            if (estudiante == null)
            {
                return NotFound();
            }

            var empadronamiento = await _context.EstudiantesEmpadronamiento
                .FirstOrDefaultAsync(ee => ee.IdEstudiante == id);

            var viewModel = new EstudianteEmpadronamientoViewModel
            {
                IdEstudiante = estudiante.IdEstudiante,
                NombreCompletoEstudiante = estudiante.NombreCompleto,
                CorreoEstudiante = estudiante.DireccionCorreo,
                InstitucionEstudiante = estudiante.Institucion
            };

            if (empadronamiento != null)
            {
                viewModel.DatosEmpadronamiento = new EstudianteEmpadronamientoDto
                {
                    IdEstudiante = empadronamiento.IdEstudiante,
                    NumeroId = empadronamiento.NumeroId,
                    FechaNacimiento = empadronamiento.FechaNacimiento,
                    Genero = empadronamiento.Genero,
                    Nacionalidad = empadronamiento.Nacionalidad,
                    EstadoCivil = empadronamiento.EstadoCivil,
                    Provincia = empadronamiento.Provincia,
                    Canton = empadronamiento.Canton,
                    Distrito = empadronamiento.Distrito,
                    Barrio = empadronamiento.Barrio,
                    Senas = empadronamiento.Senas,
                    TelefonoAlterno = empadronamiento.TelefonoAlterno,
                    CorreoAlterno = empadronamiento.CorreoAlterno,
                    NombrePadre = empadronamiento.NombrePadre,
                    NombreMadre = empadronamiento.NombreMadre,
                    NombreTutor = empadronamiento.NombreTutor,
                    ContactoEmergencia = empadronamiento.ContactoEmergencia,
                    TelefonoEmergencia = empadronamiento.TelefonoEmergencia,
                    RelacionEmergencia = empadronamiento.RelacionEmergencia,
                    Alergias = empadronamiento.Alergias,
                    CondicionesMedicas = empadronamiento.CondicionesMedicas,
                    Medicamentos = empadronamiento.Medicamentos,
                    SeguroMedico = empadronamiento.SeguroMedico,
                    CentroMedicoHabitual = empadronamiento.CentroMedicoHabitual,
                    InstitucionProcedencia = empadronamiento.InstitucionProcedencia,
                    UltimoNivelCursado = empadronamiento.UltimoNivelCursado,
                    PromedioAnterior = empadronamiento.PromedioAnterior,
                    AdaptacionesPrevias = empadronamiento.AdaptacionesPrevias,
                    EtapaActual = empadronamiento.EtapaActual,
                    FechaEtapa = empadronamiento.FechaEtapa,
                    UsuarioEtapa = empadronamiento.UsuarioEtapa,
                    NotasInternas = empadronamiento.NotasInternas,
                    FechaCreacion = empadronamiento.FechaCreacion,
                    FechaModificacion = empadronamiento.FechaModificacion,
                    UsuarioCreacion = empadronamiento.UsuarioCreacion,
                    UsuarioModificacion = empadronamiento.UsuarioModificacion
                };
                
                // Actualizar etapas
                foreach (var etapa in viewModel.EtapasDisponibles)
                {
                    etapa.EsActual = etapa.Id == empadronamiento.EtapaActual;
                    if (etapa.EsActual)
                    {
                        etapa.FechaCompletado = empadronamiento.FechaEtapa;
                        etapa.UsuarioResponsable = empadronamiento.UsuarioEtapa;
                    }
                }
            }

            return View(viewModel);
        }

        // GET: Empadronamiento/Create/5
        [RequirePermission(ApplicationPermissions.Estudiantes.CREAR)]
        public async Task<IActionResult> Create(int id)
        {
            var estudiante = await _context.Estudiantes
                .Include(e => e.PeriodoAcademico)
                .FirstOrDefaultAsync(e => e.IdEstudiante == id);

            if (estudiante == null)
            {
                return NotFound();
            }

            // Verificar si ya existe empadronamiento
            var existeEmpadronamiento = await _context.EstudiantesEmpadronamiento
                .AnyAsync(ee => ee.IdEstudiante == id);

            if (existeEmpadronamiento)
            {
                return RedirectToAction(nameof(Edit), new { id });
            }

            var viewModel = new EstudianteEmpadronamientoViewModel
            {
                IdEstudiante = estudiante.IdEstudiante,
                NombreCompletoEstudiante = estudiante.NombreCompleto,
                CorreoEstudiante = estudiante.DireccionCorreo,
                InstitucionEstudiante = estudiante.Institucion,
                DatosEmpadronamiento = new EstudianteEmpadronamientoDto
                {
                    IdEstudiante = estudiante.IdEstudiante,
                    NumeroId = estudiante.NumeroId, // Pre-llenar con el número del estudiante
                    EtapaActual = "PreRegistro"
                }
            };

            return View(viewModel);
        }

        // POST: Empadronamiento/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(ApplicationPermissions.Estudiantes.CREAR)]
        public async Task<IActionResult> Create(EstudianteEmpadronamientoViewModel viewModel)
        {
            // Detectar si es una solicitud AJAX
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest" || 
                          Request.Headers["Accept"].ToString().Contains("application/json");

            if (!ModelState.IsValid)
            {
                if (isAjax)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    
                    return Json(new { 
                        success = false, 
                        message = "Por favor complete todos los campos requeridos correctamente.",
                        errors = errors 
                    });
                }
                return View(viewModel);
            }

            try
            {
                var estudianteId = viewModel.DatosEmpadronamiento?.IdEstudiante > 0
                    ? viewModel.DatosEmpadronamiento!.IdEstudiante
                    : viewModel.IdEstudiante;

                if (estudianteId <= 0)
                {
                    _logger.LogWarning("Intento de crear empadronamiento sin IdEstudiante válido. Payload: {@Payload}", viewModel);
                    if (isAjax)
                    {
                        return Json(new
                        {
                            success = false,
                            message = "No se pudo identificar al estudiante. Recargue la página e intente nuevamente."
                        });
                    }

                    ModelState.AddModelError(string.Empty, "No se pudo identificar al estudiante. Recargue la página e intente nuevamente.");
                    return View(viewModel);
                }

                // Evitar duplicados cuando ya existe un empadronamiento
                var existente = await _context.EstudiantesEmpadronamiento.FindAsync(estudianteId);
                if (existente != null)
                {
                    _logger.LogInformation("Empadronamiento ya existe para el estudiante {IdEstudiante}", estudianteId);
                    if (isAjax)
                    {
                        return Json(new
                        {
                            success = false,
                            message = "Este estudiante ya cuenta con un empadronamiento registrado.",
                            redirectUrl = Url.Action(nameof(Edit), new { id = estudianteId })
                        });
                    }

                    TempData["WarningMessage"] = "Este estudiante ya cuenta con un empadronamiento registrado.";
                    return RedirectToAction(nameof(Edit), new { id = estudianteId });
                }

                var datos = viewModel.DatosEmpadronamiento ?? new EstudianteEmpadronamientoDto
                {
                    IdEstudiante = estudianteId
                };

                var empadronamiento = new EstudianteEmpadronamiento
                {
                    IdEstudiante = estudianteId,
                    NumeroId = datos.NumeroId,
                    FechaNacimiento = datos.FechaNacimiento,
                    Genero = datos.Genero,
                    Nacionalidad = datos.Nacionalidad,
                    EstadoCivil = datos.EstadoCivil,
                    Provincia = datos.Provincia,
                    Canton = datos.Canton,
                    Distrito = datos.Distrito,
                    Barrio = datos.Barrio,
                    Senas = datos.Senas,
                    TelefonoAlterno = datos.TelefonoAlterno,
                    CorreoAlterno = datos.CorreoAlterno,
                    NombrePadre = datos.NombrePadre,
                    NombreMadre = datos.NombreMadre,
                    NombreTutor = datos.NombreTutor,
                    ContactoEmergencia = datos.ContactoEmergencia,
                    TelefonoEmergencia = datos.TelefonoEmergencia,
                    RelacionEmergencia = datos.RelacionEmergencia,
                    Alergias = datos.Alergias,
                    CondicionesMedicas = datos.CondicionesMedicas,
                    Medicamentos = datos.Medicamentos,
                    SeguroMedico = datos.SeguroMedico,
                    CentroMedicoHabitual = datos.CentroMedicoHabitual,
                    InstitucionProcedencia = datos.InstitucionProcedencia,
                    UltimoNivelCursado = datos.UltimoNivelCursado,
                    PromedioAnterior = datos.PromedioAnterior,
                    AdaptacionesPrevias = datos.AdaptacionesPrevias,
                    EtapaActual = "PreRegistro", // Siempre inicia en Pre-registro
                    FechaEtapa = DateTime.Now,
                    UsuarioEtapa = User.Identity?.Name,
                    NotasInternas = datos.NotasInternas,
                    UsuarioCreacion = User.Identity?.Name
                };

                _context.EstudiantesEmpadronamiento.Add(empadronamiento);
                await _context.SaveChangesAsync();

                if (isAjax)
                {
                    return Json(new { 
                        success = true, 
                        message = "Empadronamiento guardado exitosamente",
                        redirectUrl = Url.Action(nameof(Details), new { id = viewModel.IdEstudiante })
                    });
                }

                TempData["SuccessMessage"] = "Datos de empadronamiento creados exitosamente.";
                return RedirectToAction(nameof(Details), new { id = viewModel.IdEstudiante });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear empadronamiento para estudiante {IdEstudiante}", viewModel.IdEstudiante);
                
                if (isAjax)
                {
                    return Json(new { 
                        success = false, 
                        message = "Ocurrió un error al guardar los datos. Por favor, intente nuevamente.",
                        error = ex.Message 
                    });
                }
                
                ModelState.AddModelError("", "Ocurrió un error al guardar los datos. Por favor, intente nuevamente.");
                return View(viewModel);
            }
        }

        // GET: Empadronamiento/Edit/5
        [RequirePermission(ApplicationPermissions.Estudiantes.EDITAR)]
        public async Task<IActionResult> Edit(int id)
        {
            var estudiante = await _context.Estudiantes
                .Include(e => e.PeriodoAcademico)
                .FirstOrDefaultAsync(e => e.IdEstudiante == id);

            if (estudiante == null)
            {
                return NotFound();
            }

            var empadronamiento = await _context.EstudiantesEmpadronamiento
                .FirstOrDefaultAsync(ee => ee.IdEstudiante == id);

            if (empadronamiento == null)
            {
                return RedirectToAction(nameof(Create), new { id });
            }

            var viewModel = new EstudianteEmpadronamientoViewModel
            {
                IdEstudiante = estudiante.IdEstudiante,
                NombreCompletoEstudiante = estudiante.NombreCompleto,
                CorreoEstudiante = estudiante.DireccionCorreo,
                InstitucionEstudiante = estudiante.Institucion,
                DatosEmpadronamiento = new EstudianteEmpadronamientoDto
                {
                    IdEstudiante = empadronamiento.IdEstudiante,
                    NumeroId = empadronamiento.NumeroId,
                    FechaNacimiento = empadronamiento.FechaNacimiento,
                    Genero = empadronamiento.Genero,
                    Nacionalidad = empadronamiento.Nacionalidad,
                    EstadoCivil = empadronamiento.EstadoCivil,
                    Provincia = empadronamiento.Provincia,
                    Canton = empadronamiento.Canton,
                    Distrito = empadronamiento.Distrito,
                    Barrio = empadronamiento.Barrio,
                    Senas = empadronamiento.Senas,
                    TelefonoAlterno = empadronamiento.TelefonoAlterno,
                    CorreoAlterno = empadronamiento.CorreoAlterno,
                    NombrePadre = empadronamiento.NombrePadre,
                    NombreMadre = empadronamiento.NombreMadre,
                    NombreTutor = empadronamiento.NombreTutor,
                    ContactoEmergencia = empadronamiento.ContactoEmergencia,
                    TelefonoEmergencia = empadronamiento.TelefonoEmergencia,
                    RelacionEmergencia = empadronamiento.RelacionEmergencia,
                    Alergias = empadronamiento.Alergias,
                    CondicionesMedicas = empadronamiento.CondicionesMedicas,
                    Medicamentos = empadronamiento.Medicamentos,
                    SeguroMedico = empadronamiento.SeguroMedico,
                    CentroMedicoHabitual = empadronamiento.CentroMedicoHabitual,
                    InstitucionProcedencia = empadronamiento.InstitucionProcedencia,
                    UltimoNivelCursado = empadronamiento.UltimoNivelCursado,
                    PromedioAnterior = empadronamiento.PromedioAnterior,
                    AdaptacionesPrevias = empadronamiento.AdaptacionesPrevias,
                    EtapaActual = empadronamiento.EtapaActual,
                    FechaEtapa = empadronamiento.FechaEtapa,
                    UsuarioEtapa = empadronamiento.UsuarioEtapa,
                    NotasInternas = empadronamiento.NotasInternas,
                    FechaCreacion = empadronamiento.FechaCreacion,
                    FechaModificacion = empadronamiento.FechaModificacion,
                    UsuarioCreacion = empadronamiento.UsuarioCreacion,
                    UsuarioModificacion = empadronamiento.UsuarioModificacion
                }
            };

            return View(viewModel);
        }

        // POST: Empadronamiento/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(ApplicationPermissions.Estudiantes.EDITAR)]
        public async Task<IActionResult> Edit(int id, EstudianteEmpadronamientoViewModel viewModel)
        {
            if (id != viewModel.IdEstudiante)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            try
            {
                var empadronamiento = await _context.EstudiantesEmpadronamiento
                    .Include(ee => ee.Estudiante) // Incluir el estudiante relacionado
                    .FirstOrDefaultAsync(ee => ee.IdEstudiante == id);

                if (empadronamiento == null)
                {
                    return NotFound();
                }

                // Actualizar campos
                empadronamiento.NumeroId = viewModel.DatosEmpadronamiento!.NumeroId;
                empadronamiento.FechaNacimiento = viewModel.DatosEmpadronamiento.FechaNacimiento;
                empadronamiento.Genero = viewModel.DatosEmpadronamiento.Genero;
                empadronamiento.Nacionalidad = viewModel.DatosEmpadronamiento.Nacionalidad;
                empadronamiento.EstadoCivil = viewModel.DatosEmpadronamiento.EstadoCivil;
                empadronamiento.Provincia = viewModel.DatosEmpadronamiento.Provincia;
                empadronamiento.Canton = viewModel.DatosEmpadronamiento.Canton;
                empadronamiento.Distrito = viewModel.DatosEmpadronamiento.Distrito;
                empadronamiento.Barrio = viewModel.DatosEmpadronamiento.Barrio;
                empadronamiento.Senas = viewModel.DatosEmpadronamiento.Senas;
                empadronamiento.TelefonoAlterno = viewModel.DatosEmpadronamiento.TelefonoAlterno;
                empadronamiento.CorreoAlterno = viewModel.DatosEmpadronamiento.CorreoAlterno;
                empadronamiento.NombrePadre = viewModel.DatosEmpadronamiento.NombrePadre;
                empadronamiento.NombreMadre = viewModel.DatosEmpadronamiento.NombreMadre;
                empadronamiento.NombreTutor = viewModel.DatosEmpadronamiento.NombreTutor;
                empadronamiento.ContactoEmergencia = viewModel.DatosEmpadronamiento.ContactoEmergencia;
                empadronamiento.TelefonoEmergencia = viewModel.DatosEmpadronamiento.TelefonoEmergencia;
                empadronamiento.RelacionEmergencia = viewModel.DatosEmpadronamiento.RelacionEmergencia;
                empadronamiento.Alergias = viewModel.DatosEmpadronamiento.Alergias;
                empadronamiento.CondicionesMedicas = viewModel.DatosEmpadronamiento.CondicionesMedicas;
                empadronamiento.Medicamentos = viewModel.DatosEmpadronamiento.Medicamentos;
                empadronamiento.SeguroMedico = viewModel.DatosEmpadronamiento.SeguroMedico;
                empadronamiento.CentroMedicoHabitual = viewModel.DatosEmpadronamiento.CentroMedicoHabitual;
                empadronamiento.InstitucionProcedencia = viewModel.DatosEmpadronamiento.InstitucionProcedencia;
                empadronamiento.UltimoNivelCursado = viewModel.DatosEmpadronamiento.UltimoNivelCursado;
                empadronamiento.PromedioAnterior = viewModel.DatosEmpadronamiento.PromedioAnterior;
                empadronamiento.AdaptacionesPrevias = viewModel.DatosEmpadronamiento.AdaptacionesPrevias;
                
                // Mantener en PreRegistro si no se especifica otra etapa
                var nuevaEtapa = viewModel.DatosEmpadronamiento.EtapaActual;
                if (string.IsNullOrEmpty(nuevaEtapa))
                {
                    nuevaEtapa = "PreRegistro";
                }
                
                // Solo actualizar etapa si cambió
                if (empadronamiento.EtapaActual != nuevaEtapa)
                {
                    empadronamiento.EtapaActual = nuevaEtapa;
                    empadronamiento.FechaEtapa = DateTime.Now;
                    empadronamiento.UsuarioEtapa = User.Identity?.Name;
                    
                    // Activar estudiante automáticamente si se aprueba o matricula
                    if ((nuevaEtapa == "Aprobado" || nuevaEtapa == "Matriculado") && empadronamiento.Estudiante != null)
                    {
                        empadronamiento.Estudiante.Estado = 1; // Activo
                    }
                }
                
                empadronamiento.NotasInternas = viewModel.DatosEmpadronamiento.NotasInternas;
                empadronamiento.FechaModificacion = DateTime.Now;
                empadronamiento.UsuarioModificacion = User.Identity?.Name;

                _context.Update(empadronamiento);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Datos de empadronamiento actualizados exitosamente.";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await EstudianteEmpadronamientoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar empadronamiento para estudiante {IdEstudiante}", id);
                ModelState.AddModelError("", "Ocurrió un error al actualizar los datos. Por favor, intente nuevamente.");
                return View(viewModel);
            }
        }

        // POST: Empadronamiento/CambiarEtapa/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(ApplicationPermissions.Estudiantes.EDITAR)]
        public async Task<IActionResult> CambiarEtapa(int id, string nuevaEtapa)
        {
            try
            {
                var empadronamiento = await _context.EstudiantesEmpadronamiento
                    .FirstOrDefaultAsync(ee => ee.IdEstudiante == id);

                if (empadronamiento == null)
                {
                    return Json(new { success = false, message = "No se encontraron datos de empadronamiento." });
                }

                empadronamiento.EtapaActual = nuevaEtapa;
                empadronamiento.FechaEtapa = DateTime.Now;
                empadronamiento.UsuarioEtapa = User.Identity?.Name;
                empadronamiento.FechaModificacion = DateTime.Now;
                empadronamiento.UsuarioModificacion = User.Identity?.Name;

                _context.Update(empadronamiento);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Etapa actualizada exitosamente." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar etapa para estudiante {IdEstudiante}", id);
                return Json(new { success = false, message = "Error al actualizar la etapa." });
            }
        }

        private async Task<bool> EstudianteEmpadronamientoExists(int id)
        {
            return await _context.EstudiantesEmpadronamiento.AnyAsync(e => e.IdEstudiante == id);
        }
    }
}