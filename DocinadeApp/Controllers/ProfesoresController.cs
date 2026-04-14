using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RubricasApp.Web.Data;
using RubricasApp.Web.Models;
using RubricasApp.Web.ViewModels;
using RubricasApp.Web.Services;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;

namespace RubricasApp.Web.Controllers
{
    /// <summary>
    /// Controlador para la gestión completa de profesores con formulario multi-step
    /// </summary>
    [Authorize]
    public class ProfesoresController : BaseController
    {
        private readonly ILogger<ProfesoresController> _logger;
        private readonly ICedulaCostaRicaService _cedulaService;
        private readonly IPdfService _pdfService;
        private const string TEMP_DATA_KEY = "ProfesorData";

        public ProfesoresController(
            IPeriodoAcademicoService periodoService,
            RubricasDbContext context,
            ILogger<ProfesoresController> logger,
            ICedulaCostaRicaService cedulaService,
            IPdfService pdfService) : base(periodoService, context)
        {
            _logger = logger;
            _cedulaService = cedulaService;
            _pdfService = pdfService;
        }

        #region CRUD Principal - Index, Details, Create, Edit, Delete

        /// <summary>
        /// Lista de profesores con búsqueda y filtros
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index(string? buscar, int? provinciaId, bool? activo, int pagina = 1)
        {
            try
            {
                var query = _context.Set<Profesor>()
                    .Include(p => p.Provincia)
                    .Include(p => p.Canton)
                    .Include(p => p.Distrito)
                    .Include(p => p.Escuela)
                    .ThenInclude(e => e!.Facultad)
                    .ThenInclude(f => f.Institucion)
                    .AsQueryable();

                // Aplicar filtros
                if (!string.IsNullOrEmpty(buscar))
                {
                    query = query.Where(p =>
                        p.Nombres.Contains(buscar) ||
                        p.PrimerApellido.Contains(buscar) ||
                        p.SegundoApellido!.Contains(buscar) ||
                        p.Cedula.Contains(buscar) ||
                        p.EmailPersonal.Contains(buscar));
                }

                if (provinciaId.HasValue)
                {
                    query = query.Where(p => p.ProvinciaId == provinciaId.Value);
                }

                if (activo.HasValue)
                {
                    query = query.Where(p => p.Estado == activo.Value);
                }

                // Ordenar
                query = query.OrderBy(p => p.PrimerApellido)
                           .ThenBy(p => p.SegundoApellido)
                           .ThenBy(p => p.Nombres);

                // Paginación
                const int tamanioPagina = 20;
                var totalRegistros = await query.CountAsync();
                var profesores = await query
                    .Skip((pagina - 1) * tamanioPagina)
                    .Take(tamanioPagina)
                    .ToListAsync();

                // ViewBag para filtros
                ViewBag.Buscar = buscar;
                ViewBag.ProvinciaId = provinciaId;
                ViewBag.Activo = activo;
                ViewBag.PaginaActual = pagina;
                ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalRegistros / tamanioPagina);
                ViewBag.TotalRegistros = totalRegistros;

                // Lista de provincias para filtro
                ViewBag.Provincias = await _context.Set<Provincia>()
                    .Where(p => p.Estado)
                    .Select(p => new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.Nombre
                    })
                    .ToListAsync();

                return View(profesores);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar la lista de profesores");
                TempData["Error"] = "Error al cargar la lista de profesores.";
                return View(new List<Profesor>());
            }
        }

        /// <summary>
        /// Detalles completos de un profesor
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var profesor = await _context.Set<Profesor>()
                    .Include(p => p.Provincia)
                    .Include(p => p.Canton)
                    .Include(p => p.Distrito)
                    .Include(p => p.Escuela)
                        .ThenInclude(e => e!.Facultad)
                        .ThenInclude(f => f.Institucion)
                    .Include(p => p.FormacionAcademica)
                    .Include(p => p.ExperienciaLaboral)
                    .Include(p => p.Capacitaciones)
                    .Include(p => p.ProfesorGrupos)
                        .ThenInclude(pg => pg.Grupo)
                    .Include(p => p.ProfesorGrupos)
                        .ThenInclude(pg => pg.Materia)
                    .Include(p => p.ProfesorGrupos)
                        .ThenInclude(pg => pg.PeriodoAcademico)
                    .Include(p => p.ProfesoresGuia)
                        .ThenInclude(pg => pg.Grupo)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (profesor == null)
                {
                    TempData["Error"] = "Profesor no encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                return View(profesor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar detalles del profesor {ProfesorId}", id);
                TempData["Error"] = "Error al cargar los detalles del profesor.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Formulario de creación de profesor - Paso 1
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                var viewModel = new ProfesorViewModel();
                await CargarListasDropdown(viewModel);
                
                // Limpiar datos temporales previos
                TempData.Remove(TEMP_DATA_KEY);
                
                return View("CreateStep1", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al inicializar formulario de creación de profesor");
                TempData["Error"] = "Error al cargar el formulario.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Formulario Paso 1 - Información Personal (GET)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> CreateStep1()
        {
            try
            {
                var viewModel = new ProfesorViewModel();
                await CargarListasDropdown(viewModel);
                
                // Limpiar datos temporales previos
                TempData.Remove(TEMP_DATA_KEY);
                
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar formulario paso 1");
                TempData["Error"] = "Error al cargar el formulario.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Procesamiento Paso 1 - Información Personal
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStep1(ProfesorViewModel model)
        {
            try
            {
                _logger.LogInformation("Iniciando procesamiento CreateStep1");

                // Validar solo los campos del Paso 1
                if (!ValidarPaso1(model))
                {
                    _logger.LogWarning("Validación del Paso 1 falló");
                    await CargarListasDropdown(model);
                    return View(model);
                }

                _logger.LogInformation("Validación del Paso 1 exitosa");

                // Verificar que la cédula no exista
                var cedulaExiste = await _context.Set<Profesor>()
                    .AnyAsync(p => p.Cedula == model.Cedula);

                if (cedulaExiste)
                {
                    _logger.LogWarning("Cédula {Cedula} ya existe", model.Cedula);
                    ModelState.AddModelError("Cedula", "Ya existe un profesor registrado con esta cédula.");
                    await CargarListasDropdown(model);
                    return View(model);
                }

                _logger.LogInformation("Verificación de cédula exitosa");

                // Limpiar listas para serialización (evitar problemas con SelectListItem)
                var modelParaGuardar = new ProfesorViewModel
                {
                    Id = model.Id,
                    Nombres = model.Nombres,
                    PrimerApellido = model.PrimerApellido,
                    SegundoApellido = model.SegundoApellido,
                    Cedula = model.Cedula,
                    TipoCedula = model.TipoCedula,
                    Sexo = model.Sexo,
                    FechaNacimiento = model.FechaNacimiento,
                    EstadoCivil = model.EstadoCivil,
                    Nacionalidad = model.Nacionalidad
                };

                _logger.LogInformation("Modelo preparado para serialización");

                // Guardar datos temporalmente
                var jsonData = JsonSerializer.Serialize(modelParaGuardar);
                TempData[TEMP_DATA_KEY] = jsonData;
                
                _logger.LogInformation("Datos serializados y guardados en TempData");
                
                return RedirectToAction(nameof(CreateStep2));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en paso 1 de creación de profesor");
                TempData["Error"] = "Error al procesar la información.";
                await CargarListasDropdown(model);
                return View(model);
            }
        }

        /// <summary>
        /// Formulario Paso 2 - Información de Contacto
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> CreateStep2()
        {
            var model = RecuperarDatosTemporales();
            if (model == null)
            {
                TempData["Error"] = "Sesión expirada. Por favor, comience nuevamente.";
                return RedirectToAction(nameof(Create));
            }

            await CargarListasDropdown(model);
            return View(model);
        }

        /// <summary>
        /// Procesamiento Paso 2 - Información de Contacto
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStep2(ProfesorViewModel model)
        {
            try
            {
                var datosTemporales = RecuperarDatosTemporales();
                if (datosTemporales == null)
                {
                    TempData["Error"] = "Sesión expirada. Por favor, comience nuevamente.";
                    return RedirectToAction(nameof(Create));
                }

                // Actualizar datos del paso 2
                datosTemporales.EmailPersonal = model.EmailPersonal;
                datosTemporales.EmailInstitucional = model.EmailInstitucional;
                datosTemporales.TelefonoFijo = model.TelefonoFijo;
                datosTemporales.TelefonoCelular = model.TelefonoCelular;
                datosTemporales.TelefonoOficina = model.TelefonoOficina;
                datosTemporales.Extension = model.Extension;

                // Validar paso 2
                if (!ValidarPaso2(datosTemporales))
                {
                    await CargarListasDropdown(datosTemporales);
                    return View(datosTemporales);
                }

                // Verificar que el email no exista
                var emailExiste = await _context.Set<Profesor>()
                    .AnyAsync(p => p.EmailPersonal == model.EmailPersonal);

                if (emailExiste)
                {
                    ModelState.AddModelError("EmailPersonal", "Ya existe un profesor registrado con este email.");
                    await CargarListasDropdown(datosTemporales);
                    return View(datosTemporales);
                }

                // Guardar datos actualizados
                TempData[TEMP_DATA_KEY] = JsonSerializer.Serialize(datosTemporales);
                
                return RedirectToAction(nameof(CreateStep3));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en paso 2 de creación de profesor");
                TempData["Error"] = "Error al procesar la información.";
                await CargarListasDropdown(model);
                return View(model);
            }
        }

        /// <summary>
        /// Formulario Paso 3 - Dirección
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> CreateStep3()
        {
            var model = RecuperarDatosTemporales();
            if (model == null)
            {
                TempData["Error"] = "Sesión expirada. Por favor, comience nuevamente.";
                return RedirectToAction(nameof(Create));
            }

            await CargarListasDropdown(model);
            return View(model);
        }

        /// <summary>
        /// Procesamiento Paso 3 - Dirección
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStep3(ProfesorViewModel model)
        {
            try
            {
                var datosTemporales = RecuperarDatosTemporales();
                if (datosTemporales == null)
                {
                    TempData["Error"] = "Sesión expirada. Por favor, comience nuevamente.";
                    return RedirectToAction(nameof(Create));
                }

                // Actualizar datos del paso 3
                datosTemporales.DireccionExacta = model.DireccionExacta;
                datosTemporales.ProvinciaId = model.ProvinciaId;
                datosTemporales.CantonId = model.CantonId;
                datosTemporales.DistritoId = model.DistritoId;
                datosTemporales.CodigoPostal = model.CodigoPostal;

                // Guardar datos actualizados
                TempData[TEMP_DATA_KEY] = JsonSerializer.Serialize(datosTemporales);
                
                return RedirectToAction(nameof(CreateStep4));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en paso 3 de creación de profesor");
                TempData["Error"] = "Error al procesar la información.";
                await CargarListasDropdown(model);
                return View(model);
            }
        }

        /// <summary>
        /// Formulario Paso 4 - Información Académica
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> CreateStep4()
        {
            var model = RecuperarDatosTemporales();
            if (model == null)
            {
                TempData["Error"] = "Sesión expirada. Por favor, comience nuevamente.";
                return RedirectToAction(nameof(Create));
            }

            await CargarListasDropdown(model);
            return View(model);
        }

        /// <summary>
        /// Procesamiento Paso 4 - Información Académica
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStep4(ProfesorViewModel model, bool fromEdit = false, int profesorId = 0)
        {
            try
            {
                // Si viene desde Edit, actualizar directamente en la base de datos
                if (fromEdit && profesorId > 0)
                {
                    var profesor = await _context.Set<Profesor>()
                        .Include(p => p.FormacionAcademica)
                        .Include(p => p.Capacitaciones)
                        .FirstOrDefaultAsync(p => p.Id == profesorId);
                        
                    if (profesor == null)
                    {
                        TempData["Error"] = "Profesor no encontrado.";
                        return RedirectToAction(nameof(Index));
                    }
                    
                    // Eliminar formaciones y capacitaciones existentes
                    _context.Set<ProfesorFormacionAcademica>().RemoveRange(profesor.FormacionAcademica);
                    _context.Set<ProfesorCapacitacion>().RemoveRange(profesor.Capacitaciones);
                    
                    // Agregar nuevas formaciones
                    if (model.FormacionAcademica != null && model.FormacionAcademica.Any())
                    {
                        foreach (var formacion in model.FormacionAcademica)
                        {
                            _context.Set<ProfesorFormacionAcademica>().Add(new ProfesorFormacionAcademica
                            {
                                ProfesorId = profesorId,
                                TipoFormacion = formacion.TipoFormacion,
                                TituloObtenido = formacion.TituloObtenido,
                                InstitucionEducativa = formacion.InstitucionEducativa,
                                PaisInstitucion = formacion.PaisInstitucion,
                                AnioInicio = formacion.AnioInicio,
                                AnioFinalizacion = formacion.AnioFinalizacion,
                                EnCurso = formacion.EnCurso,
                                PromedioGeneral = formacion.PromedioGeneral,
                                EsTituloReconocidoCONARE = formacion.EsTituloReconocidoCONARE,
                                NumeroReconocimiento = formacion.NumeroReconocimiento
                            });
                        }
                        
                        // Actualizar datos principales del profesor con la última formación
                        var ultimaFormacion = model.FormacionAcademica.Last();
                        profesor.GradoAcademico = ultimaFormacion.TipoFormacion;
                        profesor.TituloAcademico = ultimaFormacion.TituloObtenido;
                        profesor.InstitucionGraduacion = ultimaFormacion.InstitucionEducativa;
                        profesor.PaisGraduacion = ultimaFormacion.PaisInstitucion;
                        profesor.AnioGraduacion = ultimaFormacion.AnioFinalizacion;
                    }
                    
                    // Agregar nuevas capacitaciones
                    if (model.Capacitaciones != null && model.Capacitaciones.Any())
                    {
                        foreach (var capacitacion in model.Capacitaciones)
                        {
                            _context.Set<ProfesorCapacitacion>().Add(new ProfesorCapacitacion
                            {
                                ProfesorId = profesorId,
                                NombreCapacitacion = capacitacion.NombreCapacitacion,
                                InstitucionOrganizadora = capacitacion.InstitucionOrganizadora,
                                TipoCapacitacion = capacitacion.TipoCapacitacion,
                                Modalidad = capacitacion.Modalidad,
                                FechaInicio = capacitacion.FechaInicio,
                                FechaFin = capacitacion.FechaFin,
                                HorasCapacitacion = capacitacion.HorasCapacitacion,
                                CertificadoObtenido = capacitacion.CertificadoObtenido,
                                CalificacionObtenida = capacitacion.CalificacionObtenida,
                                AreaConocimiento = capacitacion.AreaConocimiento
                            });
                        }
                    }
                    
                    await _context.SaveChangesAsync();
                    
                    TempData["Success"] = "Formación académica y capacitaciones actualizadas correctamente.";
                    return RedirectToAction(nameof(Edit), new { id = profesorId });
                }
                
                // Flujo normal del wizard
                var datosTemporales = RecuperarDatosTemporales();
                if (datosTemporales == null)
                {
                    TempData["Error"] = "Sesión expirada. Por favor, comience nuevamente.";
                    return RedirectToAction(nameof(Create));
                }

                // Actualizar la lista de formaciones académicas
                datosTemporales.FormacionAcademica = model.FormacionAcademica ?? new List<ProfesorFormacionAcademicaDto>();

                // Actualizar la lista de capacitaciones
                datosTemporales.Capacitaciones = model.Capacitaciones ?? new List<ProfesorCapacitacionDto>();

                // Si hay formaciones académicas, tomar la última como datos principales
                if (datosTemporales.FormacionAcademica.Any())
                {
                    var ultimaFormacion = datosTemporales.FormacionAcademica.Last();
                    datosTemporales.GradoAcademico = ultimaFormacion.TipoFormacion;
                    datosTemporales.TituloAcademico = ultimaFormacion.TituloObtenido;
                    datosTemporales.InstitucionGraduacion = ultimaFormacion.InstitucionEducativa;
                    datosTemporales.PaisGraduacion = ultimaFormacion.PaisInstitucion;
                    datosTemporales.AnioGraduacion = ultimaFormacion.AnioFinalizacion;
                    
                    _logger.LogInformation("Se guardaron {Count} formaciones académicas. Última formación establecida como principal.", 
                        datosTemporales.FormacionAcademica.Count);
                }

                _logger.LogInformation($"Guardando {datosTemporales.Capacitaciones.Count} capacitaciones en TempData");

                // Guardar datos actualizados
                TempData[TEMP_DATA_KEY] = JsonSerializer.Serialize(datosTemporales);
                
                return RedirectToAction(nameof(CreateStep5));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en paso 4 de creación de profesor");
                TempData["Error"] = "Error al procesar la información.";
                await CargarListasDropdown(model);
                return View(model);
            }
        }

        /// <summary>
        /// Formulario Paso 5 - Información Laboral
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> CreateStep5()
        {
            var model = RecuperarDatosTemporales();
            if (model == null)
            {
                TempData["Error"] = "Sesión expirada. Por favor, comience nuevamente.";
                return RedirectToAction(nameof(Create));
            }

            await CargarListasDropdown(model);
            return View(model);
        }

        /// <summary>
        /// Procesamiento Paso 5 - Información Laboral
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStep5(ProfesorViewModel model, bool fromEdit = false, int profesorId = 0)
        {
            try
            {
                // Si viene desde Edit, actualizar directamente en la base de datos
                if (fromEdit && profesorId > 0)
                {
                    var profesor = await _context.Set<Profesor>()
                        .Include(p => p.ExperienciaLaboral)
                        .FirstOrDefaultAsync(p => p.Id == profesorId);
                        
                    if (profesor == null)
                    {
                        TempData["Error"] = "Profesor no encontrado.";
                        return RedirectToAction(nameof(Index));
                    }
                    
                    // Actualizar datos laborales del profesor
                    profesor.EscuelaId = model.EscuelaId;
                    profesor.CodigoEmpleado = model.CodigoEmpleado;
                    profesor.FechaIngreso = model.FechaIngreso;
                    profesor.FechaRetiro = model.FechaRetiro;
                    profesor.TipoContrato = model.TipoContrato;
                    profesor.RegimenLaboral = model.RegimenLaboral;
                    profesor.CategoriaLaboral = model.CategoriaLaboral;
                    profesor.TipoJornada = model.TipoJornada;
                    profesor.HorasLaborales = model.HorasLaborales;
                    
                    // Eliminar experiencias laborales existentes
                    _context.Set<ProfesorExperienciaLaboral>().RemoveRange(profesor.ExperienciaLaboral);
                    
                    // Agregar nuevas experiencias laborales
                    if (model.ExperienciaLaboral != null && model.ExperienciaLaboral.Any())
                    {
                        foreach (var experiencia in model.ExperienciaLaboral)
                        {
                            _context.Set<ProfesorExperienciaLaboral>().Add(new ProfesorExperienciaLaboral
                            {
                                ProfesorId = profesorId,
                                NombreInstitucion = experiencia.NombreInstitucion,
                                CargoDesempenado = experiencia.CargoDesempenado,
                                TipoInstitucion = experiencia.TipoInstitucion,
                                FechaInicio = experiencia.FechaInicio,
                                FechaFin = experiencia.FechaFin,
                                TrabajandoActualmente = experiencia.TrabajandoActualmente,
                                DescripcionFunciones = experiencia.DescripcionFunciones,
                                TipoContrato = experiencia.TipoContrato,
                                JornadaLaboral = experiencia.JornadaLaboral,
                                FechaCreacion = DateTime.Now
                            });
                        }
                    }
                    
                    await _context.SaveChangesAsync();
                    
                    TempData["Success"] = "Información laboral y experiencias actualizadas correctamente.";
                    return RedirectToAction(nameof(Edit), new { id = profesorId });
                }
                
                // Flujo normal del wizard
                var datosTemporales = RecuperarDatosTemporales();
                if (datosTemporales == null)
                {
                    TempData["Error"] = "Sesión expirada. Por favor, comience nuevamente.";
                    return RedirectToAction(nameof(Create));
                }

                // Actualizar datos del paso 5
                datosTemporales.EscuelaId = model.EscuelaId;
                datosTemporales.CodigoEmpleado = model.CodigoEmpleado;
                datosTemporales.FechaIngreso = model.FechaIngreso;
                datosTemporales.FechaRetiro = model.FechaRetiro;
                datosTemporales.TipoContrato = model.TipoContrato;
                datosTemporales.RegimenLaboral = model.RegimenLaboral;
                datosTemporales.CategoriaLaboral = model.CategoriaLaboral;
                datosTemporales.TipoJornada = model.TipoJornada;
                datosTemporales.HorasLaborales = model.HorasLaborales;
                
                // Actualizar lista de experiencias laborales
                datosTemporales.ExperienciaLaboral = model.ExperienciaLaboral ?? new List<ProfesorExperienciaLaboralDto>();
                _logger.LogInformation($"Guardando {datosTemporales.ExperienciaLaboral.Count} experiencias laborales en TempData");

                // Validar información laboral básica
                if (!ValidarPaso5(datosTemporales))
                {
                    await CargarListasDropdown(datosTemporales);
                    return View(datosTemporales);
                }

                // Guardar datos actualizados
                TempData[TEMP_DATA_KEY] = JsonSerializer.Serialize(datosTemporales);
                
                return RedirectToAction(nameof(CreateStep6));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en paso 5 de creación de profesor");
                TempData["Error"] = "Error al procesar la información.";
                await CargarListasDropdown(model);
                return View(model);
            }
        }

        /// <summary>
        /// Formulario Paso 6 - Información Adicional y Contacto de Emergencia
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> CreateStep6(int? profesorId = null)
        {
            ProfesorViewModel model;
            
            // Si se proporciona un profesorId, cargar desde la base de datos (modo edición)
            if (profesorId.HasValue && profesorId.Value > 0)
            {
                var profesor = await _context.Set<Profesor>()
                    .Include(p => p.ProfesorGrupos)
                        .ThenInclude(pg => pg.Grupo)
                    .Include(p => p.ProfesorGrupos)
                        .ThenInclude(pg => pg.Materia)
                    .Include(p => p.ProfesorGrupos)
                        .ThenInclude(pg => pg.PeriodoAcademico)
                    .FirstOrDefaultAsync(p => p.Id == profesorId.Value);

                if (profesor == null)
                {
                    TempData["Error"] = "Profesor no encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                // Mapear a ViewModel
                model = new ProfesorViewModel
                {
                    Id = profesor.Id,
                    AreasEspecializacion = profesor.AreasEspecializacion,
                    IdiomasHabla = profesor.IdiomasHabla,
                    NivelIngles = profesor.NivelIngles,
                    ExperienciaDocente = profesor.ExperienciaDocente,
                    ContactoEmergenciaNombre = profesor.ContactoEmergenciaNombre,
                    ContactoEmergenciaParentesco = profesor.ContactoEmergenciaParentesco,
                    ContactoEmergenciaTelefono = profesor.ContactoEmergenciaTelefono,
                    NotificacionesEmail = profesor.NotificacionesEmail,
                    NotificacionesSMS = profesor.NotificacionesSMS,
                    ProfesorGrupos = profesor.ProfesorGrupos?.Select(pg => new ProfesorGrupoCreateDto
                    {
                        ProfesorGrupoId = pg.Id,
                        ProfessorId = pg.ProfesorId,
                        GrupoId = pg.GrupoId,
                        MateriaId = pg.MateriaId,
                        PeriodoId = pg.PeriodoAcademicoId,
                        EsProfesorGuia = pg.EsProfesorPrincipal,
                        Estado = pg.Estado
                    }).ToList() ?? new List<ProfesorGrupoCreateDto>()
                };

                // Guardar en TempData para consistencia con el flujo normal
                TempData[TEMP_DATA_KEY] = JsonSerializer.Serialize(model);
                
                ViewBag.FromEdit = true;
                ViewBag.ProfesorId = profesorId.Value;
            }
            else
            {
                // Flujo normal del wizard
                model = RecuperarDatosTemporales();
                if (model == null)
                {
                    TempData["Error"] = "Sesión expirada. Por favor, comience nuevamente.";
                    return RedirectToAction(nameof(Create));
                }
            }

            // Cargar listas de grupos, materias y períodos para los dropdowns
            await CargarListasDropdown(model);

            return View(model);
        }

        /// <summary>
        /// Procesamiento Final - Guardar Profesor
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStep6(ProfesorViewModel model, bool fromEdit = false, int profesorId = 0)
        {
            try
            {
                // Si viene desde Edit, actualizar directamente
                if (fromEdit && profesorId > 0)
                {
                    var profesorExistente = await _context.Set<Profesor>()
                        .Include(p => p.ProfesorGrupos)
                        .Include(p => p.ProfesoresGuia)
                        .FirstOrDefaultAsync(p => p.Id == profesorId);

                    if (profesorExistente == null)
                    {
                        TempData["Error"] = "Profesor no encontrado.";
                        return RedirectToAction(nameof(Index));
                    }

                    // Actualizar información adicional y contacto de emergencia
                    profesorExistente.AreasEspecializacion = model.AreasEspecializacion;
                    profesorExistente.IdiomasHabla = model.IdiomasHabla;
                    profesorExistente.NivelIngles = model.NivelIngles;
                    profesorExistente.ExperienciaDocente = model.ExperienciaDocente;
                    profesorExistente.ContactoEmergenciaNombre = model.ContactoEmergenciaNombre;
                    profesorExistente.ContactoEmergenciaParentesco = model.ContactoEmergenciaParentesco;
                    profesorExistente.ContactoEmergenciaTelefono = model.ContactoEmergenciaTelefono;
                    profesorExistente.NotificacionesEmail = model.NotificacionesEmail;
                    profesorExistente.NotificacionesSMS = model.NotificacionesSMS;

                    // Procesar asignación de grupos
                    if (model.ProfesorGrupos != null && model.ProfesorGrupos.Any())
                    {
                        // Eliminar asignaciones existentes que no estén en el modelo
                        var gruposActualesIds = model.ProfesorGrupos
                            .Where(pg => pg.ProfesorGrupoId > 0)
                            .Select(pg => pg.ProfesorGrupoId)
                            .ToList();

                        var gruposAEliminar = profesorExistente.ProfesorGrupos
                            .Where(pg => !gruposActualesIds.Contains(pg.Id))
                            .ToList();

                        foreach (var grupoEliminar in gruposAEliminar)
                        {
                            _context.Remove(grupoEliminar);
                        }

                        // Eliminar registros de ProfesorGuia relacionados
                        var guiasAEliminar = profesorExistente.ProfesoresGuia
                            .Where(pg => gruposAEliminar.Select(g => g.GrupoId).Contains(pg.GrupoId))
                            .ToList();

                        foreach (var guiaEliminar in guiasAEliminar)
                        {
                            _context.Remove(guiaEliminar);
                        }

                        // Actualizar o crear asignaciones de grupos
                        foreach (var grupoDto in model.ProfesorGrupos)
                        {
                            if (grupoDto.ProfesorGrupoId > 0)
                            {
                                // Actualizar existente
                                var grupoExistente = profesorExistente.ProfesorGrupos
                                    .FirstOrDefault(pg => pg.Id == grupoDto.ProfesorGrupoId);

                                if (grupoExistente != null)
                                {
                                    grupoExistente.GrupoId = grupoDto.GrupoId;
                                    grupoExistente.MateriaId = grupoDto.MateriaId;
                                    grupoExistente.PeriodoAcademicoId = grupoDto.PeriodoId;
                                    grupoExistente.EsProfesorPrincipal = true;
                                    grupoExistente.Estado = grupoDto.Estado;
                                }
                            }
                            else
                            {
                                // Crear nuevo
                                var nuevoGrupo = new ProfesorGrupo
                                {
                                    ProfesorId = profesorId,
                                    GrupoId = grupoDto.GrupoId,
                                    MateriaId = grupoDto.MateriaId,
                                    PeriodoAcademicoId = grupoDto.PeriodoId,
                                    EsProfesorPrincipal = true,
                                    Estado = true,
                                    FechaAsignacion = DateTime.Now
                                };
                                _context.Add(nuevoGrupo);
                            }

                            // Manejar ProfesorGuia
                            if (grupoDto.EsProfesorGuia)
                            {
                                // Verificar si ya existe como guía de este grupo
                                var guiaExistente = profesorExistente.ProfesoresGuia
                                    .FirstOrDefault(pg => pg.GrupoId == grupoDto.GrupoId);

                                if (guiaExistente == null)
                                {
                                    // Crear nuevo registro de guía
                                    var nuevaGuia = new ProfesorGuia
                                    {
                                        ProfesorId = profesorId,
                                        GrupoId = grupoDto.GrupoId,
                                        FechaAsignacion = DateTime.Now,
                                        Estado = true
                                    };
                                    _context.Add(nuevaGuia);
                                }
                                else if (!guiaExistente.Estado)
                                {
                                    // Reactivar si estaba inactivo
                                    guiaExistente.Estado = true;
                                    guiaExistente.FechaAsignacion = DateTime.Now;
                                }
                            }
                            else
                            {
                                // Si desmarcó la opción de guía, desactivar el registro
                                var guiaExistente = profesorExistente.ProfesoresGuia
                                    .FirstOrDefault(pg => pg.GrupoId == grupoDto.GrupoId && pg.Estado);

                                if (guiaExistente != null)
                                {
                                    guiaExistente.Estado = false;
                                    guiaExistente.FechaFin = DateTime.Now;
                                }
                            }
                        }
                    }

                    // Actualizar auditoría
                    profesorExistente.FechaModificacion = DateTime.Now;
                    profesorExistente.ModificadoPor = User.Identity?.Name;

                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Información adicional y asignación de grupos del profesor {ProfesorId} actualizada desde Edit por {Usuario}", 
                        profesorId, User.Identity?.Name);

                    TempData["Success"] = "Información adicional y asignación de grupos actualizada correctamente.";
                    return RedirectToAction(nameof(Edit), new { id = profesorId });
                }

                // Flujo normal del wizard: crear profesor
                var datosTemporales = RecuperarDatosTemporales();
                if (datosTemporales == null)
                {
                    TempData["Error"] = "Sesión expirada. Por favor, comience nuevamente.";
                    return RedirectToAction(nameof(Create));
                }

                // Actualizar datos finales
                datosTemporales.AreasEspecializacion = model.AreasEspecializacion;
                datosTemporales.IdiomasHabla = model.IdiomasHabla;
                datosTemporales.NivelIngles = model.NivelIngles;
                datosTemporales.ExperienciaDocente = model.ExperienciaDocente;
                datosTemporales.ContactoEmergenciaNombre = model.ContactoEmergenciaNombre;
                datosTemporales.ContactoEmergenciaParentesco = model.ContactoEmergenciaParentesco;
                datosTemporales.ContactoEmergenciaTelefono = model.ContactoEmergenciaTelefono;
                datosTemporales.NotificacionesEmail = model.NotificacionesEmail;
                datosTemporales.NotificacionesSMS = model.NotificacionesSMS;

                // Crear entidad Profesor
                var profesor = new Profesor
                {
                    // Información Personal
                    Nombres = datosTemporales.Nombres,
                    PrimerApellido = datosTemporales.PrimerApellido,
                    SegundoApellido = datosTemporales.SegundoApellido,
                    Cedula = datosTemporales.Cedula,
                    TipoCedula = datosTemporales.TipoCedula,
                    Sexo = datosTemporales.Sexo,
                    FechaNacimiento = datosTemporales.FechaNacimiento,
                    EstadoCivil = datosTemporales.EstadoCivil,
                    Nacionalidad = datosTemporales.Nacionalidad,

                    // Información de Contacto
                    EmailPersonal = datosTemporales.EmailPersonal,
                    EmailInstitucional = datosTemporales.EmailInstitucional,
                    TelefonoFijo = datosTemporales.TelefonoFijo,
                    TelefonoCelular = datosTemporales.TelefonoCelular,
                    TelefonoOficina = datosTemporales.TelefonoOficina,
                    Extension = datosTemporales.Extension,

                    // Dirección
                    DireccionExacta = datosTemporales.DireccionExacta,
                    ProvinciaId = datosTemporales.ProvinciaId,
                    CantonId = datosTemporales.CantonId,
                    DistritoId = datosTemporales.DistritoId,
                    CodigoPostal = datosTemporales.CodigoPostal,

                    // Información Académica
                    GradoAcademico = datosTemporales.GradoAcademico,
                    TituloAcademico = datosTemporales.TituloAcademico,
                    InstitucionGraduacion = datosTemporales.InstitucionGraduacion,
                    PaisGraduacion = datosTemporales.PaisGraduacion,
                    AnioGraduacion = datosTemporales.AnioGraduacion,
                    NumeroColegiadoProfesional = datosTemporales.NumeroColegiadoProfesional,

                    // Información Laboral
                    EscuelaId = datosTemporales.EscuelaId,
                    CodigoEmpleado = datosTemporales.CodigoEmpleado,
                    FechaIngreso = datosTemporales.FechaIngreso,
                    FechaRetiro = datosTemporales.FechaRetiro,
                    TipoContrato = datosTemporales.TipoContrato,
                    RegimenLaboral = datosTemporales.RegimenLaboral,
                    CategoriaLaboral = datosTemporales.CategoriaLaboral,
                    TipoJornada = datosTemporales.TipoJornada,
                    HorasLaborales = datosTemporales.HorasLaborales,

                    // Información Adicional
                    AreasEspecializacion = datosTemporales.AreasEspecializacion,
                    IdiomasHabla = datosTemporales.IdiomasHabla,
                    NivelIngles = datosTemporales.NivelIngles,
                    ExperienciaDocente = datosTemporales.ExperienciaDocente,

                    // Contacto de Emergencia
                    ContactoEmergenciaNombre = datosTemporales.ContactoEmergenciaNombre,
                    ContactoEmergenciaParentesco = datosTemporales.ContactoEmergenciaParentesco,
                    ContactoEmergenciaTelefono = datosTemporales.ContactoEmergenciaTelefono,

                    // Configuraciones
                    NotificacionesEmail = datosTemporales.NotificacionesEmail,
                    NotificacionesSMS = datosTemporales.NotificacionesSMS,
                    Estado = true,

                    // Auditoría
                    FechaCreacion = DateTime.Now,
                    CreadoPor = User.Identity?.Name,
                    Version = 1
                };

                _context.Add(profesor);
                await _context.SaveChangesAsync();

                // Guardar formaciones académicas
                if (datosTemporales.FormacionAcademica != null && datosTemporales.FormacionAcademica.Any())
                {
                    foreach (var formacionDto in datosTemporales.FormacionAcademica)
                    {
                        var formacion = new ProfesorFormacionAcademica
                        {
                            ProfesorId = profesor.Id,
                            TipoFormacion = formacionDto.TipoFormacion,
                            TituloObtenido = formacionDto.TituloObtenido,
                            InstitucionEducativa = formacionDto.InstitucionEducativa,
                            PaisInstitucion = formacionDto.PaisInstitucion,
                            AnioInicio = formacionDto.AnioInicio,
                            AnioFinalizacion = formacionDto.AnioFinalizacion,
                            EnCurso = formacionDto.EnCurso,
                            PromedioGeneral = formacionDto.PromedioGeneral,
                            EsTituloReconocidoCONARE = formacionDto.EsTituloReconocidoCONARE,
                            NumeroReconocimiento = formacionDto.NumeroReconocimiento,
                            FechaCreacion = DateTime.Now
                        };
                        
                        _context.Add(formacion);
                    }
                    
                    await _context.SaveChangesAsync();
                    
                    _logger.LogInformation("Se guardaron {Count} formaciones académicas para el profesor {ProfesorId}", 
                        datosTemporales.FormacionAcademica.Count, profesor.Id);
                }
                
                // Guardar experiencias laborales
                if (datosTemporales.ExperienciaLaboral != null && datosTemporales.ExperienciaLaboral.Any())
                {
                    foreach (var experienciaDto in datosTemporales.ExperienciaLaboral)
                    {
                        var experiencia = new ProfesorExperienciaLaboral
                        {
                            ProfesorId = profesor.Id,
                            NombreInstitucion = experienciaDto.NombreInstitucion,
                            CargoDesempenado = experienciaDto.CargoDesempenado,
                            TipoInstitucion = experienciaDto.TipoInstitucion,
                            FechaInicio = experienciaDto.FechaInicio,
                            FechaFin = experienciaDto.FechaFin,
                            TrabajandoActualmente = experienciaDto.TrabajandoActualmente,
                            DescripcionFunciones = experienciaDto.DescripcionFunciones,
                            TipoContrato = experienciaDto.TipoContrato,
                            JornadaLaboral = experienciaDto.JornadaLaboral,
                            FechaCreacion = DateTime.Now
                        };
                        
                        _context.Add(experiencia);
                    }
                    
                    await _context.SaveChangesAsync();
                    
                    _logger.LogInformation("Se guardaron {Count} experiencias laborales para el profesor {ProfesorId}", 
                        datosTemporales.ExperienciaLaboral.Count, profesor.Id);
                }

                // Guardar capacitaciones
                if (datosTemporales.Capacitaciones != null && datosTemporales.Capacitaciones.Any())
                {
                    foreach (var capacitacionDto in datosTemporales.Capacitaciones)
                    {
                        var capacitacion = new ProfesorCapacitacion
                        {
                            ProfesorId = profesor.Id,
                            NombreCapacitacion = capacitacionDto.NombreCapacitacion,
                            InstitucionOrganizadora = capacitacionDto.InstitucionOrganizadora,
                            TipoCapacitacion = capacitacionDto.TipoCapacitacion,
                            Modalidad = capacitacionDto.Modalidad,
                            FechaInicio = capacitacionDto.FechaInicio,
                            FechaFin = capacitacionDto.FechaFin,
                            HorasCapacitacion = capacitacionDto.HorasCapacitacion,
                            CertificadoObtenido = capacitacionDto.CertificadoObtenido,
                            CalificacionObtenida = capacitacionDto.CalificacionObtenida,
                            AreaConocimiento = capacitacionDto.AreaConocimiento,
                            FechaCreacion = DateTime.Now
                        };
                        
                        _context.Add(capacitacion);
                    }
                    
                    await _context.SaveChangesAsync();
                    
                    _logger.LogInformation("Se guardaron {Count} capacitaciones para el profesor {ProfesorId}", 
                        datosTemporales.Capacitaciones.Count, profesor.Id);
                }

                // Guardar asignaciones de grupos y materias
                if (model.ProfesorGrupos != null && model.ProfesorGrupos.Any())
                {
                    foreach (var grupoDto in model.ProfesorGrupos)
                    {
                        // Crear registro en ProfesorGrupo
                        var profesorGrupo = new ProfesorGrupo
                        {
                            ProfesorId = profesor.Id,
                            GrupoId = grupoDto.GrupoId,
                            MateriaId = grupoDto.MateriaId,
                            PeriodoAcademicoId = grupoDto.PeriodoId,
                            EsProfesorPrincipal = true,
                            Estado = true,
                            FechaAsignacion = DateTime.Now
                        };
                        
                        _context.Add(profesorGrupo);
                        
                        // Si es profesor guía, crear registro en ProfesorGuia
                        if (grupoDto.EsProfesorGuia)
                        {
                            var profesorGuia = new ProfesorGuia
                            {
                                ProfesorId = profesor.Id,
                                GrupoId = grupoDto.GrupoId,
                                FechaAsignacion = DateTime.Now,
                                Estado = true
                            };
                            
                            _context.Add(profesorGuia);
                        }
                    }
                    
                    await _context.SaveChangesAsync();
                    
                    _logger.LogInformation("Se guardaron {Count} asignaciones de grupos para el profesor {ProfesorId}", 
                        model.ProfesorGrupos.Count, profesor.Id);
                }

                // Limpiar datos temporales
                TempData.Remove(TEMP_DATA_KEY);

                _logger.LogInformation("Profesor {ProfesorId} creado exitosamente por {Usuario}", 
                    profesor.Id, User.Identity?.Name);

                TempData["Success"] = $"Profesor {profesor.NombreCompleto} creado exitosamente.";
                return RedirectToAction(nameof(Details), new { id = profesor.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear profesor");
                TempData["Error"] = "Error al guardar el profesor. Por favor, inténtelo nuevamente.";
                return View(model);
            }
        }

        #endregion

        #region Métodos Auxiliares

        /// <summary>
        /// Cargar listas para dropdowns
        /// </summary>
        private async Task CargarListasDropdown(ProfesorViewModel model)
        {
            // Provincias
            model.Provincias = await _context.Set<Provincia>()
                .Where(p => p.Estado)
                .OrderBy(p => p.Nombre)
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Nombre,
                    Selected = p.Id == model.ProvinciaId
                })
                .ToListAsync();

            // Cantones
            if (model.ProvinciaId.HasValue)
            {
                model.Cantones = await _context.Set<Canton>()
                    .Where(c => c.ProvinciaId == model.ProvinciaId.Value && c.Estado)
                    .OrderBy(c => c.Nombre)
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Nombre,
                        Selected = c.Id == model.CantonId
                    })
                    .ToListAsync();
            }

            // Distritos
            if (model.CantonId.HasValue)
            {
                model.Distritos = await _context.Set<Distrito>()
                    .Where(d => d.CantonId == model.CantonId.Value && d.Estado)
                    .OrderBy(d => d.Nombre)
                    .Select(d => new SelectListItem
                    {
                        Value = d.Id.ToString(),
                        Text = d.Nombre,
                        Selected = d.Id == model.DistritoId
                    })
                    .ToListAsync();
            }

            // Escuelas
            model.Escuelas = await _context.Set<Escuela>()
                .Include(e => e.Facultad)
                .ThenInclude(f => f.Institucion)
                .Where(e => e.Estado)
                .OrderBy(e => e.Facultad.Institucion.Nombre)
                .ThenBy(e => e.Facultad.Nombre)
                .ThenBy(e => e.Nombre)
                .Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = $"{e.Facultad.Institucion.Nombre} - {e.Facultad.Nombre} - {e.Nombre}",
                    Selected = e.Id == model.EscuelaId
                })
                .ToListAsync();

            // Grupos disponibles
            model.GruposDisponibles = await _context.Set<GrupoEstudiante>()
                .Where(g => g.Estado == EstadoGrupo.Activo)
                .OrderBy(g => g.Nombre)
                .Select(g => new SelectListItem
                {
                    Value = g.GrupoId.ToString(),
                    Text = g.Nombre ?? "Sin nombre"
                })
                .ToListAsync();

            // Materias disponibles
            model.MateriasDisponibles = await _context.Set<Materia>()
                .Where(m => m.Activa)
                .OrderBy(m => m.Nombre)
                .Select(m => new SelectListItem
                {
                    Value = m.MateriaId.ToString(),
                    Text = m.Nombre ?? "Sin nombre"
                })
                .ToListAsync();

            // Periodos disponibles
            model.PeriodosDisponibles = await _context.Set<PeriodoAcademico>()
                .Where(p => p.Estado == "Activo")
                .OrderBy(p => p.Nombre)
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Nombre ?? "Sin nombre"
                })
                .ToListAsync();
        }

        /// <summary>
        /// Recuperar datos temporales del wizard
        /// </summary>
        private ProfesorViewModel? RecuperarDatosTemporales()
        {
            if (TempData.ContainsKey(TEMP_DATA_KEY))
            {
                var jsonData = TempData[TEMP_DATA_KEY]?.ToString();
                if (!string.IsNullOrEmpty(jsonData))
                {
                    TempData.Keep(TEMP_DATA_KEY); // Mantener para el siguiente step
                    return JsonSerializer.Deserialize<ProfesorViewModel>(jsonData);
                }
            }
            return null;
        }

        /// <summary>
        /// Validar campos del Paso 1
        /// </summary>
        private bool ValidarPaso1(ProfesorViewModel model)
        {
            var esValido = true;

            if (string.IsNullOrWhiteSpace(model.Nombres))
            {
                ModelState.AddModelError("Nombres", "Los nombres son requeridos.");
                esValido = false;
            }

            if (string.IsNullOrWhiteSpace(model.PrimerApellido))
            {
                ModelState.AddModelError("PrimerApellido", "El primer apellido es requerido.");
                esValido = false;
            }

            if (string.IsNullOrWhiteSpace(model.Cedula))
            {
                ModelState.AddModelError("Cedula", "La cédula es requerida.");
                esValido = false;
            }

            return esValido;
        }

        /// <summary>
        /// Validar campos del Paso 2
        /// </summary>
        private bool ValidarPaso2(ProfesorViewModel model)
        {
            var esValido = true;

            if (string.IsNullOrWhiteSpace(model.EmailPersonal))
            {
                ModelState.AddModelError("EmailPersonal", "El email personal es requerido.");
                esValido = false;
            }

            if (string.IsNullOrWhiteSpace(model.TelefonoCelular))
            {
                ModelState.AddModelError("TelefonoCelular", "El teléfono celular es requerido.");
                esValido = false;
            }

            return esValido;
        }

        /// <summary>
        /// Validar campos del Paso 5
        /// </summary>
        private bool ValidarPaso5(ProfesorViewModel model)
        {
            var esValido = true;

            if (string.IsNullOrWhiteSpace(model.TipoContrato))
            {
                ModelState.AddModelError("TipoContrato", "El tipo de contrato es requerido.");
                esValido = false;
            }

            if (string.IsNullOrWhiteSpace(model.TipoJornada))
            {
                ModelState.AddModelError("TipoJornada", "El tipo de jornada es requerido.");
                esValido = false;
            }

            return esValido;
        }

        #endregion

        #region AJAX Methods para Dropdowns

        /// <summary>
        /// Obtener cantones por provincia
        /// </summary>
        [HttpGet]
        public async Task<JsonResult> GetCantonesByProvincia(int provinciaId)
        {
            try
            {
                var cantones = await _context.Set<Canton>()
                    .Where(c => c.ProvinciaId == provinciaId && c.Estado)
                    .OrderBy(c => c.Nombre)
                    .Select(c => new { value = c.Id, text = c.Nombre })
                    .ToListAsync();

                return Json(cantones);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar cantones de provincia {ProvinciaId}", provinciaId);
                return Json(new List<object>());
            }
        }

        /// <summary>
        /// Obtener distritos por cantón
        /// </summary>
        [HttpGet]
        public async Task<JsonResult> GetDistritosByCanton(int cantonId)
        {
            try
            {
                var distritos = await _context.Set<Distrito>()
                    .Where(d => d.CantonId == cantonId && d.Estado)
                    .OrderBy(d => d.Nombre)
                    .Select(d => new { value = d.Id, text = d.Nombre })
                    .ToListAsync();

                return Json(distritos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar distritos de cantón {CantonId}", cantonId);
                return Json(new List<object>());
            }
        }

        #endregion

        #region Edición y Eliminación

        /// <summary>
        /// Formulario de edición de profesor - GET
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var profesor = await _context.Set<Profesor>()
                    .Include(p => p.Provincia)
                    .Include(p => p.Canton)
                    .Include(p => p.Distrito)
                    .Include(p => p.Escuela)
                    .Include(p => p.FormacionAcademica)
                    .Include(p => p.ExperienciaLaboral)
                    .Include(p => p.Capacitaciones)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (profesor == null)
                {
                    TempData["Error"] = "Profesor no encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                // Mapear el profesor al ViewModel
                var viewModel = new ProfesorViewModel
                {
                    Id = profesor.Id,
                    Nombres = profesor.Nombres,
                    PrimerApellido = profesor.PrimerApellido,
                    SegundoApellido = profesor.SegundoApellido,
                    Cedula = profesor.Cedula,
                    TipoCedula = profesor.TipoCedula,
                    Sexo = profesor.Sexo,
                    FechaNacimiento = profesor.FechaNacimiento,
                    EstadoCivil = profesor.EstadoCivil,
                    Nacionalidad = profesor.Nacionalidad,
                    EmailPersonal = profesor.EmailPersonal,
                    EmailInstitucional = profesor.EmailInstitucional,
                    TelefonoFijo = profesor.TelefonoFijo,
                    TelefonoCelular = profesor.TelefonoCelular,
                    TelefonoOficina = profesor.TelefonoOficina,
                    Extension = profesor.Extension,
                    ProvinciaId = profesor.ProvinciaId,
                    CantonId = profesor.CantonId,
                    DistritoId = profesor.DistritoId,
                    DireccionExacta = profesor.DireccionExacta,
                    CodigoPostal = profesor.CodigoPostal,
                    ContactoEmergenciaNombre = profesor.ContactoEmergenciaNombre,
                    ContactoEmergenciaTelefono = profesor.ContactoEmergenciaTelefono,
                    ContactoEmergenciaParentesco = profesor.ContactoEmergenciaParentesco,
                    AreasEspecializacion = profesor.AreasEspecializacion,
                    IdiomasHabla = profesor.IdiomasHabla,
                    NivelIngles = profesor.NivelIngles,
                    ExperienciaDocente = profesor.ExperienciaDocente,
                    NotificacionesEmail = profesor.NotificacionesEmail,
                    NotificacionesSMS = profesor.NotificacionesSMS,
                    Estado = profesor.Estado,
                    FormacionAcademica = profesor.FormacionAcademica?.Select(f => new ProfesorFormacionAcademicaDto
                    {
                        Id = f.Id,
                        TipoFormacion = f.TipoFormacion,
                        TituloObtenido = f.TituloObtenido,
                        InstitucionEducativa = f.InstitucionEducativa,
                        PaisInstitucion = f.PaisInstitucion,
                        AnioInicio = f.AnioInicio,
                        AnioFinalizacion = f.AnioFinalizacion,
                        EnCurso = f.EnCurso,
                        PromedioGeneral = f.PromedioGeneral,
                        EsTituloReconocidoCONARE = f.EsTituloReconocidoCONARE,
                        NumeroReconocimiento = f.NumeroReconocimiento
                    }).ToList(),
                    ExperienciaLaboral = profesor.ExperienciaLaboral?.Select(e => new ProfesorExperienciaLaboralDto
                    {
                        Id = e.Id,
                        NombreInstitucion = e.NombreInstitucion,
                        CargoDesempenado = e.CargoDesempenado,
                        TipoInstitucion = e.TipoInstitucion,
                        FechaInicio = e.FechaInicio,
                        FechaFin = e.FechaFin,
                        TrabajandoActualmente = e.TrabajandoActualmente,
                        DescripcionFunciones = e.DescripcionFunciones,
                        TipoContrato = e.TipoContrato,
                        JornadaLaboral = e.JornadaLaboral,
                        MesesExperiencia = e.FechaFin.HasValue 
                            ? (int)((e.FechaFin.Value - e.FechaInicio).TotalDays / 30)
                            : (int)((DateTime.Now - e.FechaInicio).TotalDays / 30)
                    }).ToList(),
                    Capacitaciones = profesor.Capacitaciones?.Select(c => new ProfesorCapacitacionDto
                    {
                        Id = c.Id,
                        NombreCapacitacion = c.NombreCapacitacion,
                        InstitucionOrganizadora = c.InstitucionOrganizadora,
                        TipoCapacitacion = c.TipoCapacitacion,
                        FechaInicio = c.FechaInicio,
                        FechaFin = c.FechaFin,
                        HorasCapacitacion = c.HorasCapacitacion,
                        Modalidad = c.Modalidad,
                        CertificadoObtenido = c.CertificadoObtenido,
                        CalificacionObtenida = c.CalificacionObtenida,
                        AreaConocimiento = c.AreaConocimiento
                    }).ToList()
                };

                // Cargar listas dropdown
                await CargarListasDropdown(viewModel);

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar formulario de edición del profesor {ProfesorId}", id);
                TempData["Error"] = "Error al cargar el formulario de edición.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Procesar edición de profesor - POST
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProfesorViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await CargarListasDropdown(model);
                    return View(model);
                }

                var profesor = await _context.Set<Profesor>()
                    .FirstOrDefaultAsync(p => p.Id == model.Id);

                if (profesor == null)
                {
                    TempData["Error"] = "Profesor no encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                // Verificar si cambió la cédula y si la nueva ya existe
                if (profesor.Cedula != model.Cedula)
                {
                    var cedulaExiste = await _context.Set<Profesor>()
                        .AnyAsync(p => p.Cedula == model.Cedula && p.Id != model.Id);
                    
                    if (cedulaExiste)
                    {
                        ModelState.AddModelError("Cedula", "Ya existe otro profesor con esta cédula.");
                        await CargarListasDropdown(model);
                        return View(model);
                    }
                }

                // Verificar si cambió el email y si el nuevo ya existe
                if (profesor.EmailPersonal != model.EmailPersonal)
                {
                    var emailExiste = await _context.Set<Profesor>()
                        .AnyAsync(p => p.EmailPersonal == model.EmailPersonal && p.Id != model.Id);
                    
                    if (emailExiste)
                    {
                        ModelState.AddModelError("EmailPersonal", "Ya existe otro profesor con este email.");
                        await CargarListasDropdown(model);
                        return View(model);
                    }
                }

                // Actualizar los campos del profesor
                profesor.Nombres = model.Nombres;
                profesor.PrimerApellido = model.PrimerApellido;
                profesor.SegundoApellido = model.SegundoApellido;
                profesor.Cedula = model.Cedula;
                profesor.Sexo = model.Sexo;
                profesor.FechaNacimiento = model.FechaNacimiento;
                profesor.EstadoCivil = model.EstadoCivil;
                profesor.Nacionalidad = model.Nacionalidad;
                profesor.EmailPersonal = model.EmailPersonal;
                profesor.EmailInstitucional = model.EmailInstitucional;
                profesor.TelefonoFijo = model.TelefonoFijo;
                profesor.TelefonoCelular = model.TelefonoCelular;
                profesor.TelefonoOficina = model.TelefonoOficina;
                profesor.Extension = model.Extension;
                profesor.ProvinciaId = model.ProvinciaId;
                profesor.CantonId = model.CantonId;
                profesor.DistritoId = model.DistritoId;
                profesor.DireccionExacta = model.DireccionExacta;
                profesor.CodigoPostal = model.CodigoPostal;
                profesor.ContactoEmergenciaNombre = model.ContactoEmergenciaNombre;
                profesor.ContactoEmergenciaTelefono = model.ContactoEmergenciaTelefono;
                profesor.ContactoEmergenciaParentesco = model.ContactoEmergenciaParentesco;
                profesor.AreasEspecializacion = model.AreasEspecializacion;
                profesor.IdiomasHabla = model.IdiomasHabla;
                profesor.NivelIngles = model.NivelIngles;
                profesor.NotificacionesEmail = model.NotificacionesEmail;
                profesor.NotificacionesSMS = model.NotificacionesSMS;
                profesor.Estado = model.Estado;
                profesor.FechaModificacion = DateTime.Now;
                profesor.ModificadoPor = User.Identity?.Name ?? "Sistema";

                await _context.SaveChangesAsync();

                _logger.LogInformation("Profesor {ProfesorId} actualizado exitosamente por {Usuario}", 
                    profesor.Id, User.Identity?.Name);

                TempData["Success"] = $"Profesor {profesor.NombreCompleto} actualizado exitosamente.";
                return RedirectToAction(nameof(Details), new { id = profesor.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar profesor {ProfesorId}", model.Id);
                TempData["Error"] = "Error al actualizar el profesor. Por favor, intente nuevamente.";
                await CargarListasDropdown(model);
                return View(model);
            }
        }

        /// <summary>
        /// Actualizar solo información personal (Paso 1) - POST
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePersonal(ProfesorViewModel model)
        {
            try
            {
                var profesor = await _context.Set<Profesor>().FindAsync(model.Id);
                
                if (profesor == null)
                {
                    TempData["Error"] = "Profesor no encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                // Validar campos requeridos
                if (string.IsNullOrWhiteSpace(model.Nombres) || 
                    string.IsNullOrWhiteSpace(model.PrimerApellido) ||
                    string.IsNullOrWhiteSpace(model.Cedula) ||
                    string.IsNullOrWhiteSpace(model.Sexo))
                {
                    TempData["Error"] = "Los campos Nombres, Primer Apellido, Cédula y Sexo son obligatorios.";
                    return RedirectToAction(nameof(Edit), new { id = model.Id });
                }

                // Verificar si cambió la cédula y si la nueva ya existe
                if (profesor.Cedula != model.Cedula)
                {
                    var cedulaExiste = await _context.Set<Profesor>()
                        .AnyAsync(p => p.Cedula == model.Cedula && p.Id != model.Id);
                    
                    if (cedulaExiste)
                    {
                        TempData["Error"] = "Ya existe otro profesor con esta cédula.";
                        return RedirectToAction(nameof(Edit), new { id = model.Id });
                    }
                }

                // Actualizar solo campos de información personal
                profesor.Nombres = model.Nombres;
                profesor.PrimerApellido = model.PrimerApellido;
                profesor.SegundoApellido = model.SegundoApellido;
                profesor.Cedula = model.Cedula;
                profesor.TipoCedula = model.TipoCedula;
                profesor.FechaNacimiento = model.FechaNacimiento;
                profesor.Sexo = model.Sexo;
                profesor.EstadoCivil = model.EstadoCivil;
                profesor.Nacionalidad = model.Nacionalidad;
                profesor.Estado = model.Estado;
                profesor.FechaModificacion = DateTime.Now;
                profesor.ModificadoPor = User.Identity?.Name ?? "Sistema";

                await _context.SaveChangesAsync();

                _logger.LogInformation("Información personal actualizada para profesor {ProfesorId} por {Usuario}", 
                    profesor.Id, User.Identity?.Name);

                TempData["Success"] = "Información personal actualizada exitosamente.";
                return RedirectToAction(nameof(Edit), new { id = profesor.Id, section = "Personal" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar información personal del profesor {ProfesorId}", model.Id);
                TempData["Error"] = "Error al actualizar información personal. Por favor, intente nuevamente.";
                return RedirectToAction(nameof(Edit), new { id = model.Id });
            }
        }

        /// <summary>
        /// Actualizar solo información de contacto (Paso 2) - POST
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateContacto(ProfesorViewModel model)
        {
            try
            {
                var profesor = await _context.Set<Profesor>().FindAsync(model.Id);
                
                if (profesor == null)
                {
                    TempData["Error"] = "Profesor no encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                // Validar campos requeridos
                if (string.IsNullOrWhiteSpace(model.EmailPersonal) || 
                    string.IsNullOrWhiteSpace(model.TelefonoCelular))
                {
                    TempData["Error"] = "Los campos Email Personal y Teléfono Celular son obligatorios.";
                    return RedirectToAction(nameof(Edit), new { id = model.Id });
                }

                // Verificar si cambió el email y si el nuevo ya existe
                if (profesor.EmailPersonal != model.EmailPersonal)
                {
                    var emailExiste = await _context.Set<Profesor>()
                        .AnyAsync(p => p.EmailPersonal == model.EmailPersonal && p.Id != model.Id);
                    
                    if (emailExiste)
                    {
                        TempData["Error"] = "Ya existe otro profesor con este email personal.";
                        return RedirectToAction(nameof(Edit), new { id = model.Id });
                    }
                }

                // Actualizar solo campos de contacto
                profesor.EmailPersonal = model.EmailPersonal;
                profesor.EmailInstitucional = model.EmailInstitucional;
                profesor.TelefonoFijo = model.TelefonoFijo;
                profesor.TelefonoCelular = model.TelefonoCelular;
                profesor.TelefonoOficina = model.TelefonoOficina;
                profesor.Extension = model.Extension;
                profesor.FechaModificacion = DateTime.Now;
                profesor.ModificadoPor = User.Identity?.Name ?? "Sistema";

                await _context.SaveChangesAsync();

                _logger.LogInformation("Información de contacto actualizada para profesor {ProfesorId} por {Usuario}", 
                    profesor.Id, User.Identity?.Name);

                TempData["Success"] = "Información de contacto actualizada exitosamente.";
                return RedirectToAction(nameof(Edit), new { id = profesor.Id, section = "Contacto" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar información de contacto del profesor {ProfesorId}", model.Id);
                TempData["Error"] = "Error al actualizar información de contacto. Por favor, intente nuevamente.";
                return RedirectToAction(nameof(Edit), new { id = model.Id });
            }
        }

        /// <summary>
        /// Actualizar solo dirección (Paso 3) - POST
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateDireccion(ProfesorViewModel model)
        {
            try
            {
                var profesor = await _context.Set<Profesor>().FindAsync(model.Id);
                
                if (profesor == null)
                {
                    TempData["Error"] = "Profesor no encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                // Validar campos requeridos
                if (model.ProvinciaId == 0 || model.CantonId == 0 || model.DistritoId == 0)
                {
                    TempData["Error"] = "Debe seleccionar Provincia, Cantón y Distrito.";
                    return RedirectToAction(nameof(Edit), new { id = model.Id });
                }

                // Actualizar solo campos de dirección
                profesor.ProvinciaId = model.ProvinciaId;
                profesor.CantonId = model.CantonId;
                profesor.DistritoId = model.DistritoId;
                profesor.DireccionExacta = model.DireccionExacta;
                profesor.CodigoPostal = model.CodigoPostal;
                profesor.FechaModificacion = DateTime.Now;
                profesor.ModificadoPor = User.Identity?.Name ?? "Sistema";

                await _context.SaveChangesAsync();

                _logger.LogInformation("Dirección actualizada para profesor {ProfesorId} por {Usuario}", 
                    profesor.Id, User.Identity?.Name);

                TempData["Success"] = "Dirección actualizada exitosamente.";
                return RedirectToAction(nameof(Edit), new { id = profesor.Id, section = "Direccion" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar dirección del profesor {ProfesorId}", model.Id);
                TempData["Error"] = "Error al actualizar dirección. Por favor, intente nuevamente.";
                return RedirectToAction(nameof(Edit), new { id = model.Id });
            }
        }

        /// <summary>
        /// Actualizar información adicional y contacto de emergencia (Paso 6) - POST
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAdicional(ProfesorViewModel model)
        {
            try
            {
                var profesor = await _context.Set<Profesor>().FindAsync(model.Id);
                
                if (profesor == null)
                {
                    TempData["Error"] = "Profesor no encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                // Actualizar campos adicionales y contacto de emergencia
                profesor.AreasEspecializacion = model.AreasEspecializacion;
                profesor.IdiomasHabla = model.IdiomasHabla;
                profesor.NivelIngles = model.NivelIngles;
                profesor.ExperienciaDocente = model.ExperienciaDocente;
                profesor.ContactoEmergenciaNombre = model.ContactoEmergenciaNombre;
                profesor.ContactoEmergenciaTelefono = model.ContactoEmergenciaTelefono;
                profesor.ContactoEmergenciaParentesco = model.ContactoEmergenciaParentesco;
                profesor.NotificacionesEmail = model.NotificacionesEmail;
                profesor.NotificacionesSMS = model.NotificacionesSMS;
                profesor.FechaModificacion = DateTime.Now;
                profesor.ModificadoPor = User.Identity?.Name ?? "Sistema";

                await _context.SaveChangesAsync();

                _logger.LogInformation("Información adicional actualizada para profesor {ProfesorId} por {Usuario}", 
                    profesor.Id, User.Identity?.Name);

                TempData["Success"] = "Información adicional actualizada exitosamente.";
                return RedirectToAction(nameof(Edit), new { id = profesor.Id, section = "Adicional" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar información adicional del profesor {ProfesorId}", model.Id);
                TempData["Error"] = "Error al actualizar información adicional. Por favor, intente nuevamente.";
                return RedirectToAction(nameof(Edit), new { id = model.Id });
            }
        }

        /// <summary>
        /// Método Delete - Vista de confirmación
        /// </summary>
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var profesor = await _context.Set<Profesor>()
                    .Include(p => p.Provincia)
                    .Include(p => p.Canton)
                    .Include(p => p.Distrito)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (profesor == null)
                {
                    TempData["Error"] = "Profesor no encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                var viewModel = new ProfesorViewModel
                {
                    Id = profesor.Id,
                    Nombres = profesor.Nombres ?? string.Empty,
                    PrimerApellido = profesor.PrimerApellido ?? string.Empty,
                    SegundoApellido = profesor.SegundoApellido,
                    Cedula = profesor.Cedula ?? string.Empty,
                    EmailPersonal = profesor.EmailPersonal ?? string.Empty,
                    TelefonoCelular = profesor.TelefonoCelular ?? string.Empty,
                    Estado = profesor.Estado,
                    // Información de ubicación
                    ProvinciaId = profesor.ProvinciaId,
                    CantonId = profesor.CantonId,
                    DistritoId = profesor.DistritoId,
                    ProvinciaNombre = profesor.Provincia?.Nombre,
                    CantonNombre = profesor.Canton?.Nombre,
                    DistritoNombre = profesor.Distrito?.Nombre
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar profesor {ProfesorId} para eliminación", id);
                TempData["Error"] = "Error al cargar el profesor.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Confirmación de eliminación del profesor
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var profesor = await _context.Set<Profesor>()
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (profesor == null)
                {
                    TempData["Error"] = "Profesor no encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                var nombreCompleto = $"{profesor.Nombres} {profesor.PrimerApellido} {profesor.SegundoApellido}".Trim();

                // Eliminar explícitamente todos los registros relacionados en el orden correcto
                // Respetar las dependencias de foreign keys
                
                // 1. Eliminar ProfesorGuia (no depende de otras tablas hijas)
                var profesorGuias = await _context.Set<ProfesorGuia>()
                    .Where(pg => pg.ProfesorId == id)
                    .ToListAsync();
                if (profesorGuias.Any())
                {
                    _context.Set<ProfesorGuia>().RemoveRange(profesorGuias);
                    _logger.LogInformation("Eliminando {Count} registros de ProfesorGuia para profesor {ProfesorId}", 
                        profesorGuias.Count, id);
                }

                // 2. Eliminar ProfesorGrupo (no depende de otras tablas hijas)
                var profesorGrupos = await _context.Set<ProfesorGrupo>()
                    .Where(pg => pg.ProfesorId == id)
                    .ToListAsync();
                if (profesorGrupos.Any())
                {
                    _context.Set<ProfesorGrupo>().RemoveRange(profesorGrupos);
                    _logger.LogInformation("Eliminando {Count} registros de ProfesorGrupo para profesor {ProfesorId}", 
                        profesorGrupos.Count, id);
                }

                // 3. Eliminar ProfesorCapacitacion
                var capacitaciones = await _context.Set<ProfesorCapacitacion>()
                    .Where(c => c.ProfesorId == id)
                    .ToListAsync();
                if (capacitaciones.Any())
                {
                    _context.Set<ProfesorCapacitacion>().RemoveRange(capacitaciones);
                    _logger.LogInformation("Eliminando {Count} capacitaciones del profesor {ProfesorId}", 
                        capacitaciones.Count, id);
                }

                // 4. Eliminar ProfesorExperienciaLaboral
                var experiencias = await _context.Set<ProfesorExperienciaLaboral>()
                    .Where(e => e.ProfesorId == id)
                    .ToListAsync();
                if (experiencias.Any())
                {
                    _context.Set<ProfesorExperienciaLaboral>().RemoveRange(experiencias);
                    _logger.LogInformation("Eliminando {Count} experiencias laborales del profesor {ProfesorId}", 
                        experiencias.Count, id);
                }

                // 5. Eliminar ProfesorFormacionAcademica
                var formaciones = await _context.Set<ProfesorFormacionAcademica>()
                    .Where(f => f.ProfesorId == id)
                    .ToListAsync();
                if (formaciones.Any())
                {
                    _context.Set<ProfesorFormacionAcademica>().RemoveRange(formaciones);
                    _logger.LogInformation("Eliminando {Count} formaciones académicas del profesor {ProfesorId}", 
                        formaciones.Count, id);
                }

                // 6. Finalmente, eliminar el profesor
                _context.Set<Profesor>().Remove(profesor);
                
                // Guardar todos los cambios en una transacción
                await _context.SaveChangesAsync();

                _logger.LogWarning("Profesor {ProfesorId} ({NombreCompleto}) y todos sus datos relacionados eliminados permanentemente por {Usuario}", 
                    id, nombreCompleto, User.Identity?.Name);

                TempData["Success"] = $"Profesor {nombreCompleto} y todos sus datos relacionados eliminados permanentemente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar profesor {ProfesorId}: {Message}", id, ex.Message);
                TempData["Error"] = $"Error al eliminar el profesor: {ex.Message}";
                return RedirectToAction(nameof(Delete), new { id });
            }
        }

        /// <summary>
        /// Alternar el estado activo/inactivo del profesor
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                var profesor = await _context.Set<Profesor>().FindAsync(id);
                
                if (profesor == null)
                {
                    TempData["Error"] = "Profesor no encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                profesor.Estado = !profesor.Estado;
                profesor.FechaModificacion = DateTime.Now;
                profesor.ModificadoPor = User.Identity?.Name;

                _context.Update(profesor);
                await _context.SaveChangesAsync();

                var nombreCompleto = $"{profesor.Nombres} {profesor.PrimerApellido} {profesor.SegundoApellido}".Trim();
                var accion = profesor.Estado ? "activado" : "desactivado";
                
                _logger.LogInformation("Profesor {ProfesorId} {Accion} por {Usuario}", 
                    profesor.Id, accion, User.Identity?.Name);

                TempData["Success"] = $"Profesor {nombreCompleto} {accion} exitosamente.";
                
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar estado del profesor {ProfesorId}", id);
                TempData["Error"] = "Error al cambiar el estado del profesor.";
                return RedirectToAction(nameof(Index));
            }
        }

        #endregion

        #region Métodos auxiliares para consulta de cédula

        /// <summary>
        /// Endpoint para consultar información de una cédula de Costa Rica
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConsultarCedula([FromBody] ConsultarCedulaRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request?.Cedula))
                {
                    return Json(new { success = false, message = "Número de cédula requerido" });
                }

                // Verificar que no exista un profesor con esta cédula
                var existeProfesor = await _context.Set<Profesor>()
                    .AnyAsync(p => p.Cedula == request.Cedula);

                if (existeProfesor)
                {
                    return Json(new { 
                        success = false, 
                        message = "Ya existe un profesor registrado con esta cédula",
                        exists = true 
                    });
                }

                // Consultar información en la API externa
                var resultado = await _cedulaService.ConsultarCedulaAsync(request.Cedula);
                
                return Json(new { 
                    success = resultado.Success,
                    message = resultado.Message,
                    nombres = resultado.Nombres ?? "",
                    primerApellido = resultado.PrimerApellido ?? "",
                    segundoApellido = resultado.SegundoApellido ?? "",
                    exists = false
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al consultar cédula {Cedula}", request?.Cedula);
                return Json(new { 
                    success = false, 
                    message = "Error interno al consultar la cédula" 
                });
            }
        }

        #endregion

        #region Generación de Curriculum en PDF

        /// <summary>
        /// Genera el curriculum vitae de un profesor en PDF usando Rotativa
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GenerarCurriculumPdf(int id)
        {
            try
            {
                var curriculum = await ObtenerDatosProfesorParaCurriculum(id);
                
                if (curriculum == null)
                {
                    TempData["Error"] = "Profesor no encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                var fileName = $"CV_{curriculum.NombreCompleto.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd}.pdf";
                
                // Usar PdfService
                var pdfBytes = await _pdfService.GeneratePdfFromViewAsync("~/Views/Shared/_CurriculumTemplate.cshtml", curriculum, "Portrait");
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando curriculum PDF para profesor {ProfesorId}", id);
                TempData["Error"] = "Error al generar el curriculum en PDF.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        /// <summary>
        /// Genera currículums de múltiples profesores en un solo PDF usando Rotativa
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GenerarCurriculumsMasivoPdf([FromQuery] string ids)
        {
            try
            {
                if (string.IsNullOrEmpty(ids))
                {
                  TempData["Error"] = "Debe seleccionar al menos un profesor";
                    return RedirectToAction(nameof(Index));
                }

                // Parsear los IDs separados por coma
                var profesoresIds = ids.Split(',')
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => int.TryParse(x.Trim(), out int id) ? id : 0)
                    .Where(x => x > 0)
                    .ToArray();

                if (profesoresIds.Length == 0)
                {
                    TempData["Error"] = "No se encontraron IDs de profesores válidos";
                    return RedirectToAction(nameof(Index));
                }

                var curriculums = new List<ProfesorCurriculumViewModel>();
                
                foreach (var id in profesoresIds)
                {
                    var curriculum = await ObtenerDatosProfesorParaCurriculum(id);
                    if (curriculum != null)
                    {
                        curriculums.Add(curriculum);
                    }
                }

                if (curriculums.Count == 0)
                {
                    TempData["Error"] = "No se encontraron profesores válidos";
                    return RedirectToAction(nameof(Index));
                }

                var fileName = $"Curriculums_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                
                // Usar PdfService
                var model = new CurriculumsMasivosViewModel
                {
                    Curriculums = curriculums
                };
                
                var pdfBytes = await _pdfService.GeneratePdfFromViewAsync("~/Views/Shared/_CurriculumMasivoTemplate.cshtml", model, "Portrait");
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando curriculums masivo");
                TempData["Error"] = "Error al generar los currículums: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Método privado para obtener todos los datos del profesor para el curriculum
        /// </summary>
        private async Task<ProfesorCurriculumViewModel?> ObtenerDatosProfesorParaCurriculum(int id)
        {
            var profesor = await _context.Set<Profesor>()
                .Include(p => p.Provincia)
                .Include(p => p.Canton)
                .Include(p => p.Distrito)
                .Include(p => p.Escuela)
                    .ThenInclude(e => e!.Facultad)
                    .ThenInclude(f => f!.Institucion)
        .Include(p => p.FormacionAcademica)
        .Include(p => p.ExperienciaLaboral)
        .Include(p => p.Capacitaciones)
        .Include(p => p.ProfesorGrupos)
            .ThenInclude(pg => pg.Grupo)
        .Include(p => p.ProfesorGrupos)
            .ThenInclude(pg => pg.Materia)
        .Include(p => p.ProfesorGrupos)
            .ThenInclude(pg => pg.PeriodoAcademico)
        .FirstOrDefaultAsync(p => p.Id == id);            if (profesor == null)
                return null;

            var edad = profesor.FechaNacimiento.HasValue 
                ? DateTime.Now.Year - profesor.FechaNacimiento.Value.Year 
                : (int?)null;

            var viewModel = new ProfesorCurriculumViewModel
            {
                // Información Personal
                NombreCompleto = profesor.NombreCompleto,
                Cedula = profesor.Cedula,
                TipoCedula = profesor.TipoCedula,
                Sexo = profesor.Sexo,
                FechaNacimiento = profesor.FechaNacimiento,
                Edad = edad,
                EstadoCivil = profesor.EstadoCivil,
                Nacionalidad = profesor.Nacionalidad,
                
                // Contacto
                EmailPersonal = profesor.EmailPersonal,
                EmailInstitucional = profesor.EmailInstitucional,
                TelefonoCelular = profesor.TelefonoCelular,
                TelefonoFijo = profesor.TelefonoFijo,
                TelefonoOficina = profesor.TelefonoOficina,
                
                // Dirección
                DireccionExacta = profesor.DireccionExacta,
                ProvinciaNombre = profesor.Provincia?.Nombre,
                CantonNombre = profesor.Canton?.Nombre,
                DistritoNombre = profesor.Distrito?.Nombre,
                CodigoPostal = profesor.CodigoPostal,
                
                // Información Académica
                GradoAcademico = profesor.GradoAcademico,
                TituloAcademico = profesor.TituloAcademico,
                InstitucionGraduacion = profesor.InstitucionGraduacion,
                PaisGraduacion = profesor.PaisGraduacion,
                AnioGraduacion = profesor.AnioGraduacion,
                NumeroColegiadoProfesional = profesor.NumeroColegiadoProfesional,
                
                // Información Laboral
                EscuelaNombre = profesor.Escuela?.Nombre,
                FacultadNombre = profesor.Escuela?.Facultad?.Nombre,
                InstitucionNombre = profesor.Escuela?.Facultad?.Institucion?.Nombre,
                CodigoEmpleado = profesor.CodigoEmpleado,
                FechaIngreso = profesor.FechaIngreso,
                FechaRetiro = profesor.FechaRetiro,
                TipoContrato = profesor.TipoContrato,
                RegimenLaboral = profesor.RegimenLaboral,
                CategoriaLaboral = profesor.CategoriaLaboral,
                TipoJornada = profesor.TipoJornada,
                HorasLaborales = profesor.HorasLaborales,
                AniosServicio = profesor.AniosServicio,
                
                // Cargos
                EsDirector = profesor.EsDirector,
                EsCoordinador = profesor.EsCoordinador,
                EsDecano = profesor.EsDecano,
                CargoAdministrativo = profesor.CargoAdministrativo,
                FechaInicioCargoAdmin = profesor.FechaInicioCargoAdmin,
                
                // Información Adicional
                AreasEspecializacion = profesor.AreasEspecializacion,
                IdiomasHabla = profesor.IdiomasHabla,
                NivelIngles = profesor.NivelIngles,
                ExperienciaDocente = profesor.ExperienciaDocente,
                
                // Contacto de Emergencia
                ContactoEmergenciaNombre = profesor.ContactoEmergenciaNombre,
                ContactoEmergenciaParentesco = profesor.ContactoEmergenciaParentesco,
                ContactoEmergenciaTelefono = profesor.ContactoEmergenciaTelefono,
                
                // Formación Académica
                FormacionAcademica = profesor.FormacionAcademica.Select(f => new FormacionAcademicaItem
                {
                    TipoFormacion = f.TipoFormacion,
                    TituloObtenido = f.TituloObtenido,
                    InstitucionEducativa = f.InstitucionEducativa,
                    PaisInstitucion = f.PaisInstitucion,
                    AnioInicio = f.AnioInicio,
                    AnioFinalizacion = f.AnioFinalizacion,
                    EnCurso = f.EnCurso,
                    PromedioGeneral = f.PromedioGeneral,
                    EsTituloReconocidoCONARE = f.EsTituloReconocidoCONARE,
                    NumeroReconocimiento = f.NumeroReconocimiento
                }).OrderByDescending(f => f.AnioFinalizacion ?? 9999).ToList(),
                
                // Experiencia Laboral
                ExperienciaLaboral = profesor.ExperienciaLaboral.Select(e => new ExperienciaLaboralItem
                {
                    NombreInstitucion = e.NombreInstitucion,
                    CargoDesempenado = e.CargoDesempenado,
                    TipoInstitucion = e.TipoInstitucion,
                    FechaInicio = e.FechaInicio,
                    FechaFin = e.FechaFin,
                    TrabajandoActualmente = e.TrabajandoActualmente,
                    DescripcionFunciones = e.DescripcionFunciones,
                    TipoContrato = e.TipoContrato,
                    JornadaLaboral = e.JornadaLaboral,
                    MesesExperiencia = e.MesesExperiencia
                }).OrderByDescending(e => e.FechaInicio).ToList(),
                
                // Capacitaciones
                Capacitaciones = profesor.Capacitaciones.Select(c => new CapacitacionItem
                {
                    NombreCapacitacion = c.NombreCapacitacion,
                    InstitucionOrganizadora = c.InstitucionOrganizadora,
                    TipoCapacitacion = c.TipoCapacitacion,
                    Modalidad = c.Modalidad,
                    FechaInicio = c.FechaInicio,
                    FechaFin = c.FechaFin,
                    HorasCapacitacion = c.HorasCapacitacion,
                    CertificadoObtenido = c.CertificadoObtenido,
                    AreaConocimiento = c.AreaConocimiento
                }).OrderByDescending(c => c.FechaInicio).ToList(),
                
                // Grupos Asignados
                GruposAsignados = profesor.ProfesorGrupos
                    .Where(pg => pg.Estado)
                    .Select(pg => new GrupoAsignadoItem
                    {
                        NombreGrupo = pg.Grupo!.Nombre,
                        NombreMateria = pg.Materia!.Nombre,
                        PeriodoAcademico = pg.PeriodoAcademico!.NombreCompleto,
                        EsProfesorPrincipal = pg.EsProfesorPrincipal,
                        FechaAsignacion = pg.FechaAsignacion,
                        Estado = pg.Estado
                    }).OrderByDescending(g => g.FechaAsignacion).ToList()
            };

            return viewModel;
        }

        #endregion

        #region Gestión de Usuarios

        /// <summary>
        /// Muestra la vista para vincular un usuario a un profesor
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> VincularUsuario(int id)
        {
            var profesor = await _context.Set<Profesor>()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (profesor == null)
            {
                return NotFound();
            }

            // Buscar si ya tiene un usuario vinculado
            var usuarioVinculado = await _context.Users
                .FirstOrDefaultAsync(u => u.NumeroIdentificacion == profesor.Cedula);

            // Obtener usuarios disponibles (sin profesor asignado o el ya vinculado)
            var usuariosDisponibles = await _context.Users
                .Where(u => u.NumeroIdentificacion == null || u.NumeroIdentificacion == profesor.Cedula)
                .OrderBy(u => u.UserName)
                .ToListAsync();

            var usuariosSelectList = usuariosDisponibles.Select(u => new SelectListItem
            {
                Value = u.Id,
                Text = $"{u.Nombre} {u.Apellidos} ({u.Email})",
                Selected = u.Id == (usuarioVinculado != null ? usuarioVinculado.Id : string.Empty)
            }).ToList();

            var viewModel = new VincularUsuarioProfesorViewModel
            {
                ProfesorId = profesor.Id,
                NombreProfesor = $"{profesor.Nombres} {profesor.PrimerApellido} {profesor.SegundoApellido}".Trim(),
                CedulaProfesor = profesor.Cedula,
                UsuarioVinculadoId = usuarioVinculado?.Id,
                UsuarioVinculadoNombre = usuarioVinculado != null 
                    ? $"{usuarioVinculado.Nombre} {usuarioVinculado.Apellidos} ({usuarioVinculado.Email})"
                    : null,
                UsuariosDisponibles = usuariosSelectList
            };

            return View(viewModel);
        }

        /// <summary>
        /// Procesa la vinculación de un usuario a un profesor
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VincularUsuario(VincularUsuarioProfesorViewModel model)
        {
            try
            {
                var profesor = await _context.Set<Profesor>()
                    .FirstOrDefaultAsync(p => p.Id == model.ProfesorId);

                if (profesor == null)
                {
                    return NotFound();
                }

                // Si hay un usuario seleccionado, vincular
                if (!string.IsNullOrEmpty(model.UsuarioSeleccionadoId))
                {
                    var usuario = await _context.Users
                        .FirstOrDefaultAsync(u => u.Id == model.UsuarioSeleccionadoId);
                    
                    if (usuario == null)
                    {
                        TempData["Error"] = "Usuario no encontrado.";
                        return RedirectToAction(nameof(VincularUsuario), new { id = model.ProfesorId });
                    }

                    // Verificar si el usuario ya está vinculado a otro profesor
                    var otroProfesor = await _context.Set<Profesor>()
                        .FirstOrDefaultAsync(p => p.Cedula == usuario.NumeroIdentificacion && p.Id != profesor.Id);

                    if (otroProfesor != null)
                    {
                        TempData["Error"] = $"El usuario ya está vinculado al profesor {otroProfesor.Nombres} {otroProfesor.PrimerApellido}.";
                        return RedirectToAction(nameof(VincularUsuario), new { id = model.ProfesorId });
                    }

                    // Desvincular usuario anterior si existe
                    var usuarioAnterior = await _context.Users
                        .FirstOrDefaultAsync(u => u.NumeroIdentificacion == profesor.Cedula);
                    
                    if (usuarioAnterior != null && usuarioAnterior.Id != usuario.Id)
                    {
                        usuarioAnterior.NumeroIdentificacion = null;
                    }

                    // Vincular nuevo usuario
                    usuario.NumeroIdentificacion = profesor.Cedula;
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Usuario {usuario.Email} vinculado al profesor {profesor.Nombres} {profesor.PrimerApellido} (Cédula: {profesor.Cedula})");
                    TempData["Success"] = $"Usuario {usuario.Email} vinculado exitosamente al profesor.";
                }

                return RedirectToAction(nameof(Details), new { id = model.ProfesorId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al vincular usuario al profesor");
                TempData["Error"] = "Ocurrió un error al vincular el usuario.";
                return RedirectToAction(nameof(VincularUsuario), new { id = model.ProfesorId });
            }
        }

        /// <summary>
        /// Desvincula un usuario de un profesor
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DesvincularUsuario(int id)
        {
            try
            {
                var profesor = await _context.Set<Profesor>()
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (profesor == null)
                {
                    return NotFound();
                }

                var usuario = await _context.Users
                    .FirstOrDefaultAsync(u => u.NumeroIdentificacion == profesor.Cedula);

                if (usuario != null)
                {
                    usuario.NumeroIdentificacion = null;
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Usuario {usuario.Email} desvinculado del profesor {profesor.Nombres} {profesor.PrimerApellido}");
                    TempData["Success"] = "Usuario desvinculado exitosamente.";
                }
                else
                {
                    TempData["Warning"] = "No hay ningún usuario vinculado a este profesor.";
                }

                return RedirectToAction(nameof(Details), new { id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al desvincular usuario del profesor");
                TempData["Error"] = "Ocurrió un error al desvincular el usuario.";
                return RedirectToAction(nameof(Details), new { id = id });
            }
        }

        #endregion
    }

    /// <summary>
    /// Modelo para la petición de consulta de cédula
    /// </summary>
    public class ConsultarCedulaRequest
    {
        public string? Cedula { get; set; }
    }
}
