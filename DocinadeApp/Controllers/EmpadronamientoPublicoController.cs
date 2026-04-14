using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RubricasApp.Web.Data;
using RubricasApp.Web.Models;
using RubricasApp.Web.ViewModels;
using System.Text.Json;

namespace RubricasApp.Web.Controllers
{
    /// <summary>
    /// Controlador para el auto-empadronamiento público de estudiantes
    /// NO requiere autenticación
    /// </summary>
    public class EmpadronamientoPublicoController : Controller
    {
        private readonly RubricasDbContext _context;
        private readonly ILogger<EmpadronamientoPublicoController> _logger;
        private readonly IConfiguration _configuration;
        private const string TEMP_DATA_KEY = "EmpadronamientoPublicoData";
        private const string IP_ATTEMPTS_KEY = "EmpadronamientoAttempts_";

        public EmpadronamientoPublicoController(
            RubricasDbContext context,
            ILogger<EmpadronamientoPublicoController> logger,
            IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// Página de inicio del empadronamiento público
        /// </summary>
        [HttpGet]
        public IActionResult Index()
        {
            // Verificar si el empadronamiento público está habilitado
            var habilitado = _configuration.GetValue<bool>("EmpadronamientoPublico:Habilitado");
            
            if (!habilitado)
            {
                var mensaje = _configuration.GetValue<string>("EmpadronamientoPublico:MensajeDeshabilitado");
                ViewBag.MensajeDeshabilitado = mensaje;
                return View("Deshabilitado");
            }

            return View();
        }

        /// <summary>
        /// GET: Iniciar el proceso de empadronamiento
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Iniciar()
        {
            // Verificar habilitación
            if (!VerificarSistemaHabilitado())
            {
                return RedirectToAction(nameof(Index));
            }

            // Verificar límite de intentos por IP
            if (!VerificarLimiteIntentos())
            {
                TempData["ErrorMessage"] = "Ha excedido el límite de intentos. Por favor, intente más tarde.";
                return RedirectToAction(nameof(Index));
            }

            // Crear nuevo ViewModel
            var viewModel = new EmpadronamientoPublicoViewModel
            {
                PasoActual = 1,
                TotalPasos = 5,
                SessionId = Guid.NewGuid().ToString(),
                FechaInicio = DateTime.Now
            };

            // Cargar períodos académicos activos
            await CargarDatosFormulario();

            // Guardar en TempData
            GuardarEnTempData(viewModel);

            return View("Paso1_DatosBasicos", viewModel);
        }

        /// <summary>
        /// POST: Paso 1 - Datos Básicos
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Paso1([Bind("NumeroId,Nombre,Apellidos,FechaNacimiento,Genero,Nacionalidad,EstadoCivil")] EmpadronamientoPublicoViewModel viewModel)
        {
            if (!VerificarSistemaHabilitado()) return RedirectToAction(nameof(Index));

            // Validar campos del Paso 1
            if (string.IsNullOrWhiteSpace(viewModel.NumeroId) ||
                string.IsNullOrWhiteSpace(viewModel.Nombre) ||
                string.IsNullOrWhiteSpace(viewModel.Apellidos) ||
                !viewModel.FechaNacimiento.HasValue ||
                string.IsNullOrWhiteSpace(viewModel.Genero))
            {
                ModelState.AddModelError("", "Por favor complete todos los campos obligatorios.");
                await CargarDatosFormulario();
                return View("Paso1_DatosBasicos", viewModel);
            }

            // Validar que no exista cédula duplicada
            var cedulaExiste = await _context.Estudiantes
                .AnyAsync(e => e.NumeroId == viewModel.NumeroId);

            if (cedulaExiste)
            {
                ModelState.AddModelError("NumeroId", "Esta cédula ya está registrada en el sistema. Si cree que es un error, contacte con soporte.");
                await CargarDatosFormulario();
                return View("Paso1_DatosBasicos", viewModel);
            }

            // Recuperar datos guardados y actualizar
            var datosGuardados = RecuperarDeTempData() ?? new EmpadronamientoPublicoViewModel();
            
            datosGuardados.NumeroId = viewModel.NumeroId;
            datosGuardados.Nombre = viewModel.Nombre;
            datosGuardados.Apellidos = viewModel.Apellidos;
            datosGuardados.FechaNacimiento = viewModel.FechaNacimiento;
            datosGuardados.Genero = viewModel.Genero;
            datosGuardados.Nacionalidad = viewModel.Nacionalidad;
            datosGuardados.EstadoCivil = viewModel.EstadoCivil;
            datosGuardados.PasoActual = 2;

            GuardarEnTempData(datosGuardados);

            return RedirectToAction(nameof(Paso2));
        }

        /// <summary>
        /// GET: Paso 2 - Contacto y Residencia
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Paso2()
        {
            if (!VerificarSistemaHabilitado()) return RedirectToAction(nameof(Index));

            var viewModel = RecuperarDeTempData();
            if (viewModel == null || viewModel.PasoActual < 2)
            {
                return RedirectToAction(nameof(Iniciar));
            }

            viewModel.PasoActual = 2;
            await CargarDatosFormulario();
            
            return View("Paso2_ContactoResidencia", viewModel);
        }

        /// <summary>
        /// POST: Paso 2 - Contacto y Residencia
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Paso2([Bind("DireccionCorreo,CorreoAlterno,TelefonoAlterno,Provincia,Canton,Distrito,Barrio,Senas")] EmpadronamientoPublicoViewModel viewModel, string action)
        {
            if (!VerificarSistemaHabilitado()) return RedirectToAction(nameof(Index));

            // Si presionó "Anterior"
            if (action == "anterior")
            {
                return RedirectToAction(nameof(Iniciar));
            }

            // Validar campos del Paso 2
            if (string.IsNullOrWhiteSpace(viewModel.DireccionCorreo) ||
                string.IsNullOrWhiteSpace(viewModel.TelefonoAlterno) ||
                string.IsNullOrWhiteSpace(viewModel.Provincia) ||
                string.IsNullOrWhiteSpace(viewModel.Canton) ||
                string.IsNullOrWhiteSpace(viewModel.Distrito) ||
                string.IsNullOrWhiteSpace(viewModel.Senas))
            {
                ModelState.AddModelError("", "Por favor complete todos los campos obligatorios.");
                await CargarDatosFormulario();
                viewModel.PasoActual = 2;
                return View("Paso2_ContactoResidencia", viewModel);
            }

            // Validar que no exista correo duplicado
            var correoExiste = await _context.Estudiantes
                .AnyAsync(e => e.DireccionCorreo == viewModel.DireccionCorreo);

            if (correoExiste)
            {
                ModelState.AddModelError("DireccionCorreo", "Este correo electrónico ya está registrado. Por favor use otro correo.");
                await CargarDatosFormulario();
                viewModel.PasoActual = 2;
                return View("Paso2_ContactoResidencia", viewModel);
            }

            // Recuperar y actualizar
            var datosGuardados = RecuperarDeTempData() ?? new EmpadronamientoPublicoViewModel();
            
            datosGuardados.DireccionCorreo = viewModel.DireccionCorreo;
            datosGuardados.CorreoAlterno = viewModel.CorreoAlterno;
            datosGuardados.TelefonoAlterno = viewModel.TelefonoAlterno;
            datosGuardados.Provincia = viewModel.Provincia;
            datosGuardados.Canton = viewModel.Canton;
            datosGuardados.Distrito = viewModel.Distrito;
            datosGuardados.Barrio = viewModel.Barrio;
            datosGuardados.Senas = viewModel.Senas;
            datosGuardados.PasoActual = 3;

            GuardarEnTempData(datosGuardados);

            return RedirectToAction(nameof(Paso3));
        }

        /// <summary>
        /// GET: Paso 3 - Responsables y Emergencia
        /// </summary>
        [HttpGet]
        public IActionResult Paso3()
        {
            if (!VerificarSistemaHabilitado()) return RedirectToAction(nameof(Index));

            var viewModel = RecuperarDeTempData();
            if (viewModel == null || viewModel.PasoActual < 3)
            {
                return RedirectToAction(nameof(Iniciar));
            }

            viewModel.PasoActual = 3;
            return View("Paso3_Responsables", viewModel);
        }

        /// <summary>
        /// POST: Paso 3 - Responsables y Emergencia
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Paso3([Bind("NombrePadre,NombreMadre,NombreTutor,ContactoEmergencia,TelefonoEmergencia,RelacionEmergencia")] EmpadronamientoPublicoViewModel viewModel, string action)
        {
            if (!VerificarSistemaHabilitado()) return RedirectToAction(nameof(Index));

            if (action == "anterior")
            {
                return RedirectToAction(nameof(Paso2));
            }

            // Validar campos obligatorios del Paso 3
            if (string.IsNullOrWhiteSpace(viewModel.ContactoEmergencia) ||
                string.IsNullOrWhiteSpace(viewModel.TelefonoEmergencia) ||
                string.IsNullOrWhiteSpace(viewModel.RelacionEmergencia))
            {
                ModelState.AddModelError("", "Por favor complete todos los campos obligatorios de contacto de emergencia.");
                viewModel.PasoActual = 3;
                return View("Paso3_Responsables", viewModel);
            }

            var datosGuardados = RecuperarDeTempData() ?? new EmpadronamientoPublicoViewModel();
            
            datosGuardados.NombrePadre = viewModel.NombrePadre;
            datosGuardados.NombreMadre = viewModel.NombreMadre;
            datosGuardados.NombreTutor = viewModel.NombreTutor;
            datosGuardados.ContactoEmergencia = viewModel.ContactoEmergencia;
            datosGuardados.TelefonoEmergencia = viewModel.TelefonoEmergencia;
            datosGuardados.RelacionEmergencia = viewModel.RelacionEmergencia;
            datosGuardados.PasoActual = 4;

            GuardarEnTempData(datosGuardados);

            return RedirectToAction(nameof(Paso4));
        }

        /// <summary>
        /// GET: Paso 4 - Información Académica
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Paso4()
        {
            if (!VerificarSistemaHabilitado()) return RedirectToAction(nameof(Index));

            var viewModel = RecuperarDeTempData();
            if (viewModel == null || viewModel.PasoActual < 4)
            {
                return RedirectToAction(nameof(Iniciar));
            }

            viewModel.PasoActual = 4;
            await CargarDatosFormulario();
            
            return View("Paso4_Academico", viewModel);
        }

        /// <summary>
        /// POST: Paso 4 - Información Académica
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Paso4([Bind("InstitucionProcedencia,UltimoNivelCursado,PromedioAnterior,AdaptacionesPrevias,Institucion,PeriodoAcademicoId")] EmpadronamientoPublicoViewModel viewModel, string action)
        {
            if (!VerificarSistemaHabilitado()) return RedirectToAction(nameof(Index));

            if (action == "anterior")
            {
                return RedirectToAction(nameof(Paso3));
            }

            var datosGuardados = RecuperarDeTempData() ?? new EmpadronamientoPublicoViewModel();
            
            datosGuardados.InstitucionProcedencia = viewModel.InstitucionProcedencia;
            datosGuardados.UltimoNivelCursado = viewModel.UltimoNivelCursado;
            datosGuardados.PromedioAnterior = viewModel.PromedioAnterior;
            datosGuardados.AdaptacionesPrevias = viewModel.AdaptacionesPrevias;
            datosGuardados.Institucion = viewModel.Institucion;
            datosGuardados.PeriodoAcademicoId = viewModel.PeriodoAcademicoId;
            datosGuardados.PasoActual = 5;

            GuardarEnTempData(datosGuardados);

            return RedirectToAction(nameof(Paso5));
        }

        /// <summary>
        /// GET: Paso 5 - Información de Salud y Confirmación
        /// </summary>
        [HttpGet]
        public IActionResult Paso5()
        {
            if (!VerificarSistemaHabilitado()) return RedirectToAction(nameof(Index));

            var viewModel = RecuperarDeTempData();
            if (viewModel == null || viewModel.PasoActual < 5)
            {
                return RedirectToAction(nameof(Iniciar));
            }

            viewModel.PasoActual = 5;
            return View("Paso5_Salud", viewModel);
        }

        /// <summary>
        /// POST: Paso 5 - Finalizar y Guardar
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Finalizar([Bind("Alergias,CondicionesMedicas,Medicamentos,SeguroMedico,CentroMedicoHabitual,AceptaTerminos")] EmpadronamientoPublicoViewModel viewModel, string action)
        {
            if (!VerificarSistemaHabilitado()) return RedirectToAction(nameof(Index));

            if (action == "anterior")
            {
                return RedirectToAction(nameof(Paso4));
            }

            var datosCompletos = RecuperarDeTempData();
            if (datosCompletos == null)
            {
                TempData["ErrorMessage"] = "Sesión expirada. Por favor, inicie el proceso nuevamente.";
                return RedirectToAction(nameof(Iniciar));
            }

            // Actualizar datos finales
            datosCompletos.Alergias = viewModel.Alergias;
            datosCompletos.CondicionesMedicas = viewModel.CondicionesMedicas;
            datosCompletos.Medicamentos = viewModel.Medicamentos;
            datosCompletos.SeguroMedico = viewModel.SeguroMedico;
            datosCompletos.CentroMedicoHabitual = viewModel.CentroMedicoHabitual;
            datosCompletos.AceptaTerminos = viewModel.AceptaTerminos;

            // Validar términos
            if (!datosCompletos.AceptaTerminos)
            {
                ModelState.AddModelError("AceptaTerminos", "Debe aceptar los términos y condiciones para completar el empadronamiento.");
                datosCompletos.PasoActual = 5;
                return View("Paso5_Salud", datosCompletos);
            }

            try
            {
                // Crear estudiante (Estado = 0, Inactivo hasta aprobación)
                var estudiante = new Estudiante
                {
                    NumeroId = datosCompletos.NumeroId,
                    Nombre = datosCompletos.Nombre,
                    Apellidos = datosCompletos.Apellidos,
                    DireccionCorreo = datosCompletos.DireccionCorreo,
                    Institucion = datosCompletos.Institucion ?? "Pendiente",
                    Grupos = "Pendiente de asignación",
                    Anio = datosCompletos.Anio,
                    PeriodoAcademicoId = datosCompletos.PeriodoAcademicoId ?? await ObtenerPeriodoActualId(),
                    Estado = 0 // Inactivo hasta que se apruebe el empadronamiento
                };

                _context.Estudiantes.Add(estudiante);
                await _context.SaveChangesAsync();

                // Crear empadronamiento
                var empadronamiento = new EstudianteEmpadronamiento
                {
                    IdEstudiante = estudiante.IdEstudiante,
                    NumeroId = datosCompletos.NumeroId,
                    FechaNacimiento = datosCompletos.FechaNacimiento,
                    Genero = datosCompletos.Genero,
                    Nacionalidad = datosCompletos.Nacionalidad,
                    EstadoCivil = datosCompletos.EstadoCivil,
                    Provincia = datosCompletos.Provincia,
                    Canton = datosCompletos.Canton,
                    Distrito = datosCompletos.Distrito,
                    Barrio = datosCompletos.Barrio,
                    Senas = datosCompletos.Senas,
                    TelefonoAlterno = datosCompletos.TelefonoAlterno,
                    CorreoAlterno = datosCompletos.CorreoAlterno,
                    NombrePadre = datosCompletos.NombrePadre,
                    NombreMadre = datosCompletos.NombreMadre,
                    NombreTutor = datosCompletos.NombreTutor,
                    ContactoEmergencia = datosCompletos.ContactoEmergencia,
                    TelefonoEmergencia = datosCompletos.TelefonoEmergencia,
                    RelacionEmergencia = datosCompletos.RelacionEmergencia,
                    Alergias = datosCompletos.Alergias,
                    CondicionesMedicas = datosCompletos.CondicionesMedicas,
                    Medicamentos = datosCompletos.Medicamentos,
                    SeguroMedico = datosCompletos.SeguroMedico,
                    CentroMedicoHabitual = datosCompletos.CentroMedicoHabitual,
                    InstitucionProcedencia = datosCompletos.InstitucionProcedencia,
                    UltimoNivelCursado = datosCompletos.UltimoNivelCursado,
                    PromedioAnterior = datosCompletos.PromedioAnterior,
                    AdaptacionesPrevias = datosCompletos.AdaptacionesPrevias,
                    EtapaActual = "PreRegistro",
                    FechaEtapa = DateTime.Now,
                    UsuarioEtapa = "Empadronamiento Público",
                    NotasInternas = $"Registro completado vía formulario público el {DateTime.Now:dd/MM/yyyy HH:mm}. Requiere aprobación.",
                    FechaCreacion = DateTime.Now,
                    UsuarioCreacion = "Sistema Público"
                };

                _context.EstudiantesEmpadronamiento.Add(empadronamiento);
                await _context.SaveChangesAsync();

                // Registrar intento exitoso
                RegistrarIntentoExitoso();

                // Limpiar TempData
                TempData.Remove(TEMP_DATA_KEY);

                // Generar código de confirmación
                var codigoConfirmacion = $"EMP-{estudiante.IdEstudiante:D6}-{DateTime.Now:yyyyMMdd}";

                // Enviar email de confirmación (opcional según configuración)
                var enviarEmail = _configuration.GetValue<bool>("EmpadronamientoPublico:EnviarEmailConfirmacion");
                if (enviarEmail)
                {
                    // TODO: Implementar envío de email
                    _logger.LogInformation($"Enviar email de confirmación a {datosCompletos.DireccionCorreo} con código {codigoConfirmacion}");
                }

                TempData["SuccessMessage"] = $"¡Empadronamiento completado exitosamente!";
                TempData["CodigoConfirmacion"] = codigoConfirmacion;
                
                return RedirectToAction(nameof(Confirmacion), new { codigo = codigoConfirmacion });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar empadronamiento público");
                TempData["ErrorMessage"] = "Ocurrió un error al procesar su empadronamiento. Por favor, intente nuevamente o contacte con soporte.";
                datosCompletos.PasoActual = 5;
                return View("Paso5_Salud", datosCompletos);
            }
        }

        /// <summary>
        /// Página de confirmación final
        /// </summary>
        [HttpGet]
        public IActionResult Confirmacion(string codigo)
        {
            ViewBag.CodigoConfirmacion = codigo;
            return View();
        }

        // ========== MÉTODOS AUXILIARES ==========

        private bool VerificarSistemaHabilitado()
        {
            return _configuration.GetValue<bool>("EmpadronamientoPublico:Habilitado");
        }

        private bool VerificarLimiteIntentos()
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var key = IP_ATTEMPTS_KEY + ip;
            var maxIntentos = _configuration.GetValue<int>("EmpadronamientoPublico:MaxIntentosPorIP");

            if (HttpContext.Session.GetInt32(key) is int intentos)
            {
                return intentos < maxIntentos;
            }

            return true;
        }

        private void RegistrarIntentoExitoso()
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var key = IP_ATTEMPTS_KEY + ip;
            var intentosActuales = HttpContext.Session.GetInt32(key) ?? 0;
            HttpContext.Session.SetInt32(key, intentosActuales + 1);
        }

        private void GuardarEnTempData(EmpadronamientoPublicoViewModel viewModel)
        {
            var json = JsonSerializer.Serialize(viewModel);
            TempData[TEMP_DATA_KEY] = json;
            TempData.Keep(TEMP_DATA_KEY);
        }

        private EmpadronamientoPublicoViewModel? RecuperarDeTempData()
        {
            if (TempData[TEMP_DATA_KEY] is string json)
            {
                TempData.Keep(TEMP_DATA_KEY);
                return JsonSerializer.Deserialize<EmpadronamientoPublicoViewModel>(json);
            }
            return null;
        }

        private async Task CargarDatosFormulario()
        {
            ViewBag.PeriodosAcademicos = new SelectList(
                await _context.PeriodosAcademicos
                    .Where(p => p.Activo)
                    .OrderByDescending(p => p.Anio)
                    .ToListAsync(),
                "IdPeriodo",
                "Nombre"
            );
        }

        private async Task<int> ObtenerPeriodoActualId()
        {
            var periodoActual = await _context.PeriodosAcademicos
                .Where(p => p.Activo)
                .OrderByDescending(p => p.Anio)
                .FirstOrDefaultAsync();

            return periodoActual?.Id ?? 1;
        }
    }
}
