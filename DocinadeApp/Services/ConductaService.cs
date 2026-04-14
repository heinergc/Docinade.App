using Microsoft.EntityFrameworkCore;
using RubricasApp.Web.Data;
using RubricasApp.Web.Models;
using RubricasApp.Web.ViewModels.Conducta;

namespace RubricasApp.Web.Services
{
    public class ConductaService : IConductaService
    {
        private readonly RubricasDbContext _context;
        private readonly ILogger<ConductaService> _logger;
        private readonly IEmailService _emailService;
        private const string PARAMETRO_NOTA_MINIMA = "NotaMinimaAprobacionConducta";
        private const decimal NOTA_MINIMA_DEFAULT = 65m;
        private const decimal NOTA_INICIAL = 100m;

        public ConductaService(RubricasDbContext context, ILogger<ConductaService> logger, IEmailService emailService)
        {
            _context = context;
            _logger = logger;
            _emailService = emailService;
        }

        #region Configuración

        public async Task<decimal> ObtenerNotaMinimaAprobacionAsync()
        {
            var parametro = await _context.ParametrosInstitucion
                .FirstOrDefaultAsync(p => p.Clave == PARAMETRO_NOTA_MINIMA && p.Activo);

            if (parametro != null && decimal.TryParse(parametro.Valor, out decimal valor))
            {
                return valor;
            }

            return NOTA_MINIMA_DEFAULT;
        }

        public async Task ActualizarNotaMinimaAprobacionAsync(decimal notaMinima)
        {
            var parametro = await _context.ParametrosInstitucion
                .FirstOrDefaultAsync(p => p.Clave == PARAMETRO_NOTA_MINIMA);

            if (parametro == null)
            {
                parametro = new ParametroInstitucion
                {
                    Clave = PARAMETRO_NOTA_MINIMA,
                    Nombre = "Nota Mínima de Aprobación en Conducta",
                    Descripcion = "Nota mínima para aprobar conducta según REA 40862-V21",
                    Valor = notaMinima.ToString(),
                    TipoDato = "Decimal",
                    Categoria = "Conducta",
                    Activo = true
                };
                _context.ParametrosInstitucion.Add(parametro);
            }
            else
            {
                parametro.Valor = notaMinima.ToString();
                parametro.FechaModificacion = DateTime.Now;
            }

            await _context.SaveChangesAsync();
        }

        #endregion

        #region Boletas de Conducta

        public async Task<int> RegistrarBoletaAsync(BoletaConducta boleta)
        {
            // Validar que el rebajo esté dentro del rango del tipo de falta
            var tipoFalta = await _context.TiposFalta.FindAsync(boleta.IdTipoFalta);
            if (tipoFalta == null)
            {
                throw new InvalidOperationException("Tipo de falta no encontrado");
            }

            if (boleta.RebajoAplicado < tipoFalta.RebajoMinimo || boleta.RebajoAplicado > tipoFalta.RebajoMaximo)
            {
                throw new InvalidOperationException($"El rebajo debe estar entre {tipoFalta.RebajoMinimo} y {tipoFalta.RebajoMaximo} puntos");
            }

            // Obtener el profesor guía del estudiante
            var profesorGuiaId = await ObtenerProfesorGuiaDeEstudianteAsync(boleta.IdEstudiante);
            boleta.ProfesorGuiaId = profesorGuiaId;

            _logger.LogInformation($"Registrando boleta para estudiante {boleta.IdEstudiante}. Profesor guía asignado: {profesorGuiaId?.ToString() ?? "NULL"}");

            _context.BoletasConducta.Add(boleta);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Boleta {boleta.IdBoleta} guardada con ProfesorGuiaId: {boleta.ProfesorGuiaId?.ToString() ?? "NULL"}");

            // Recalcular la nota de conducta
            await CalcularNotaConductaAsync(boleta.IdEstudiante, boleta.IdPeriodo);

            // Notificar al profesor guía (se hace asíncrono)
            if (profesorGuiaId.HasValue)
            {
                _ = NotificarProfesorGuiaAsync(boleta.IdBoleta);
            }

            return boleta.IdBoleta;
        }

        public async Task<BoletaConducta?> ObtenerBoletaPorIdAsync(int idBoleta)
        {
            return await _context.BoletasConducta
                .Include(b => b.Estudiante)
                .Include(b => b.TipoFalta)
                .Include(b => b.Periodo)
                .Include(b => b.DocenteEmisor)
                .Include(b => b.ProfesorGuia)
                .FirstOrDefaultAsync(b => b.IdBoleta == idBoleta);
        }

        public async Task<List<BoletaConducta>> ObtenerBoletasPorEstudianteAsync(int idEstudiante, int idPeriodo)
        {
            return await _context.BoletasConducta
                .Include(b => b.TipoFalta)
                .Include(b => b.DocenteEmisor)
                .Where(b => b.IdEstudiante == idEstudiante && b.IdPeriodo == idPeriodo)
                .OrderByDescending(b => b.FechaEmision)
                .ToListAsync();
        }

        public async Task<List<BoletaConducta>> ObtenerBoletasPorGrupoAsync(int idGrupo, int idPeriodo)
        {
            // Obtener estudiantes del grupo
            var estudiantesIds = await _context.EstudianteGrupos
                .Where(eg => eg.GrupoId == idGrupo && eg.Estado == EstadoAsignacion.Activo)
                .Select(eg => eg.EstudianteId)
                .ToListAsync();

            return await _context.BoletasConducta
                .Include(b => b.Estudiante)
                .Include(b => b.TipoFalta)
                .Include(b => b.DocenteEmisor)
                .Where(b => estudiantesIds.Contains(b.IdEstudiante) && b.IdPeriodo == idPeriodo && b.Estado == "Activa")
                .OrderByDescending(b => b.FechaEmision)
                .ToListAsync();
        }

        public async Task AnularBoletaAsync(int idBoleta, string usuarioId, string motivo)
        {
            var boleta = await _context.BoletasConducta.FindAsync(idBoleta);
            if (boleta == null)
            {
                throw new InvalidOperationException("Boleta no encontrada");
            }

            boleta.Estado = "Anulada";
            boleta.MotivoAnulacion = motivo;
            boleta.FechaAnulacion = DateTime.Now;
            boleta.AnuladaPorId = usuarioId;

            await _context.SaveChangesAsync();

            // Recalcular la nota de conducta
            await CalcularNotaConductaAsync(boleta.IdEstudiante, boleta.IdPeriodo);
        }

        #endregion

        #region Nota de Conducta

        public async Task<NotaConducta> CalcularNotaConductaAsync(int idEstudiante, int idPeriodo)
        {
            // Obtener todas las boletas del estudiante (activas y anuladas)
            var todasBoletas = await ObtenerBoletasPorEstudianteAsync(idEstudiante, idPeriodo);
            
            // FILTRAR SOLO LAS BOLETAS ACTIVAS para el cálculo
            var boletas = todasBoletas.Where(b => b.Estado == "Activa").ToList();
            
            // Calcular total de rebajos (solo boletas activas)
            var totalRebajos = boletas.Sum(b => b.RebajoAplicado);
            var notaFinal = NOTA_INICIAL - totalRebajos;
            
            // Asegurar que no sea negativa
            if (notaFinal < 0) notaFinal = 0;

            // Obtener nota mínima de aprobación
            var notaMinima = await ObtenerNotaMinimaAprobacionAsync();
            
            // Verificar si existe un programa de acciones exitoso para este estudiante y periodo
            var programaExitoso = await _context.ProgramasAccionesInstitucional
                .FirstOrDefaultAsync(p => 
                    p.IdEstudiante == idEstudiante && 
                    p.IdPeriodo == idPeriodo &&
                    p.Estado == "Completado" &&
                    !string.IsNullOrEmpty(p.ResultadoFinal) &&
                    (p.ResultadoFinal.Contains("Satisfactorio") || p.ResultadoFinal.Contains("Exitoso")));
            
            // Determinar estado
            string estado;
            bool requiereProgramaAcciones = false;

            // Si hay un programa exitoso, el estudiante se considera aprobado automáticamente
            if (programaExitoso != null)
            {
                estado = "Aprobado";
                requiereProgramaAcciones = false;
                notaFinal = notaMinima; // Ajustar la nota a la mínima de aprobación
                
                _logger.LogInformation(
                    "Estudiante {IdEstudiante} aprobado por programa de acciones exitoso (ID: {IdPrograma}) en periodo {IdPeriodo}",
                    idEstudiante, programaExitoso.IdPrograma, idPeriodo
                );
            }
            // Si no hay boletas activas, el estudiante está aprobado (sin incidencias)
            else if (!boletas.Any() || totalRebajos == 0)
            {
                estado = "Aprobado";
                requiereProgramaAcciones = false;
            }
            else if (notaFinal >= notaMinima)
            {
                estado = "Aprobado";
            }
            else if (notaFinal >= (notaMinima - 5))
            {
                estado = "Riesgo";
            }
            else
            {
                estado = "Aplazado";
                requiereProgramaAcciones = true;
            }

            // Buscar o crear la nota de conducta
            var notaConducta = await _context.NotasConducta
                .Include(n => n.ProgramaAcciones)
                .FirstOrDefaultAsync(n => n.IdEstudiante == idEstudiante && n.IdPeriodo == idPeriodo);

            if (notaConducta == null)
            {
                notaConducta = new NotaConducta
                {
                    IdEstudiante = idEstudiante,
                    IdPeriodo = idPeriodo,
                    NotaInicial = NOTA_INICIAL,
                    FechaCalculo = DateTime.Now
                };
                _context.NotasConducta.Add(notaConducta);
            }

            // Guardar estado anterior para logging
            var estadoAnterior = notaConducta.Estado;

            notaConducta.TotalRebajos = totalRebajos;
            notaConducta.NotaFinal = notaFinal;
            notaConducta.Estado = estado;
            notaConducta.RequiereProgramaAcciones = requiereProgramaAcciones;
            notaConducta.FechaUltimaActualizacion = DateTime.Now;

            // Si el estudiante mejora su estado a "Aprobado" (ej: por anulación de boletas)
            // y NO requiere programa de acciones, limpiar la referencia si existía
            if (estado == "Aprobado" && !requiereProgramaAcciones)
            {
                if (notaConducta.IdProgramaAcciones.HasValue)
                {
                    _logger.LogInformation(
                        "Estudiante {IdEstudiante} mejoró de {EstadoAnterior} a Aprobado en periodo {IdPeriodo}. " +
                        "Se mantiene referencia al programa de acciones {IdPrograma} como historial.",
                        idEstudiante, estadoAnterior, idPeriodo, notaConducta.IdProgramaAcciones
                    );
                    // No eliminamos la referencia para mantener el historial
                    // pero el RequiereProgramaAcciones = false indica que ya no es necesario
                }
            }

            await _context.SaveChangesAsync();

            // Log del cambio de estado
            if (estadoAnterior != estado)
            {
                _logger.LogInformation(
                    "Nota de conducta actualizada para estudiante {IdEstudiante} en periodo {IdPeriodo}: " +
                    "{EstadoAnterior} → {EstadoNuevo} (Nota: {NotaFinal}, Rebajos: {TotalRebajos})",
                    idEstudiante, idPeriodo, estadoAnterior, estado, notaFinal, totalRebajos
                );
            }

            return notaConducta;
        }

        public async Task<NotaConducta?> ObtenerNotaConductaAsync(int idEstudiante, int idPeriodo)
        {
            var notaConducta = await _context.NotasConducta
                .Include(n => n.Estudiante)
                .Include(n => n.Periodo)
                .Include(n => n.ProgramaAcciones)
                .Include(n => n.DecisionProfesional)
                .FirstOrDefaultAsync(n => n.IdEstudiante == idEstudiante && n.IdPeriodo == idPeriodo);

            if (notaConducta == null)
            {
                // Si no existe, calcularla
                notaConducta = await CalcularNotaConductaAsync(idEstudiante, idPeriodo);
            }

            return notaConducta;
        }

        public async Task<List<EstudianteRiesgoItem>> ObtenerEstudiantesEnRiesgoAsync(int idPeriodo)
        {
            // Obtener todos los estudiantes con boletas activas en el período
            var estudiantesConBoletas = await _context.BoletasConducta
                .Where(b => b.IdPeriodo == idPeriodo && b.Estado == "Activa")
                .Select(b => b.IdEstudiante)
                .Distinct()
                .ToListAsync();

            var resultado = new List<EstudianteRiesgoItem>();

            // Procesar cada estudiante
            foreach (var idEstudiante in estudiantesConBoletas)
            {
                // Calcular o actualizar la nota de conducta
                var nota = await CalcularNotaConductaAsync(idEstudiante, idPeriodo);

                // Solo incluir si está en riesgo
                if (nota.Estado == "Riesgo")
                {
                    var estudiante = await _context.Estudiantes.FindAsync(idEstudiante);
                    if (estudiante == null) continue;

                    var boletas = await _context.BoletasConducta
                        .Where(b => b.IdEstudiante == idEstudiante && b.IdPeriodo == idPeriodo && b.Estado == "Activa")
                        .ToListAsync();

                    var cantidadBoletas = boletas.Count;
                    var totalRebajos = boletas.Sum(b => b.RebajoAplicado);
                    var fechaUltimaBoleta = boletas.Any() ? boletas.Max(b => b.FechaEmision) : (DateTime?)null;

                    var profesorGuia = await ObtenerNombreProfesorGuiaAsync(idEstudiante);
                    var grupo = await ObtenerGrupoEstudianteAsync(idEstudiante);

                    resultado.Add(new EstudianteRiesgoItem
                    {
                        IdEstudiante = idEstudiante,
                        NombreCompleto = $"{estudiante.Nombre} {estudiante.Apellidos}",
                        NumeroId = estudiante.NumeroId,
                        Grupo = grupo ?? "Sin grupo",
                        ProfesorGuia = profesorGuia ?? "Sin asignar",
                        NotaFinal = nota.NotaFinal,
                        Estado = nota.Estado,
                        CantidadFaltas = cantidadBoletas,
                        CantidadBoletas = cantidadBoletas,
                        TotalRebajos = totalRebajos,
                        FechaUltimaBoleta = fechaUltimaBoleta,
                        TieneProgramaAcciones = nota.IdProgramaAcciones.HasValue,
                        EstadoPrograma = nota.ProgramaAcciones?.Estado,
                        DecisionProfesionalAplicada = nota.DecisionProfesionalAplicada
                    });
                }
            }

            return resultado.OrderBy(e => e.NotaFinal).ToList();
        }

        public async Task<List<EstudianteRiesgoItem>> ObtenerEstudiantesAplazadosAsync(int idPeriodo)
        {
            // Obtener todos los estudiantes con boletas activas en el período
            var estudiantesConBoletas = await _context.BoletasConducta
                .Where(b => b.IdPeriodo == idPeriodo && b.Estado == "Activa")
                .Select(b => b.IdEstudiante)
                .Distinct()
                .ToListAsync();

            var resultado = new List<EstudianteRiesgoItem>();

            // Procesar cada estudiante
            foreach (var idEstudiante in estudiantesConBoletas)
            {
                // Calcular o actualizar la nota de conducta
                var nota = await CalcularNotaConductaAsync(idEstudiante, idPeriodo);

                // Solo incluir si está aplazado
                if (nota.Estado == "Aplazado")
                {
                    var estudiante = await _context.Estudiantes.FindAsync(idEstudiante);
                    if (estudiante == null) continue;

                    var boletas = await _context.BoletasConducta
                        .Where(b => b.IdEstudiante == idEstudiante && b.IdPeriodo == idPeriodo && b.Estado == "Activa")
                        .ToListAsync();

                    var cantidadBoletas = boletas.Count;
                    var totalRebajos = boletas.Sum(b => b.RebajoAplicado);
                    var fechaUltimaBoleta = boletas.Any() ? boletas.Max(b => b.FechaEmision) : (DateTime?)null;

                    var profesorGuia = await ObtenerNombreProfesorGuiaAsync(idEstudiante);
                    var grupo = await ObtenerGrupoEstudianteAsync(idEstudiante);

                    // Recargar la nota con las relaciones
                    nota = await _context.NotasConducta
                        .Include(n => n.ProgramaAcciones)
                        .FirstOrDefaultAsync(n => n.IdEstudiante == idEstudiante && n.IdPeriodo == idPeriodo) ?? nota;

                    resultado.Add(new EstudianteRiesgoItem
                    {
                        IdEstudiante = idEstudiante,
                        NombreCompleto = $"{estudiante.Nombre} {estudiante.Apellidos}",
                        NumeroId = estudiante.NumeroId,
                        Grupo = grupo ?? "Sin grupo",
                        ProfesorGuia = profesorGuia ?? "Sin asignar",
                        NotaFinal = nota.NotaFinal,
                        Estado = nota.Estado,
                        CantidadFaltas = cantidadBoletas,
                        CantidadBoletas = cantidadBoletas,
                        TotalRebajos = totalRebajos,
                        FechaUltimaBoleta = fechaUltimaBoleta,
                        TieneProgramaAcciones = nota.IdProgramaAcciones.HasValue,
                        EstadoPrograma = nota.ProgramaAcciones?.Estado,
                        DecisionProfesionalAplicada = nota.DecisionProfesionalAplicada
                    });
                }
            }

            return resultado.OrderBy(e => e.NotaFinal).ToList();
        }

        #endregion

        #region Programas de Acciones

        public async Task<int> CrearProgramaAccionesAsync(ProgramaAccionesInstitucional programa)
        {
            _context.ProgramasAccionesInstitucional.Add(programa);
            await _context.SaveChangesAsync();

            // Vincular con la nota de conducta
            var notaConducta = await _context.NotasConducta.FindAsync(programa.IdNotaConducta);
            if (notaConducta != null)
            {
                notaConducta.IdProgramaAcciones = programa.IdPrograma;
                await _context.SaveChangesAsync();
            }

            return programa.IdPrograma;
        }

        public async Task ActualizarProgramaAccionesAsync(ProgramaAccionesInstitucional programa)
        {
            _context.ProgramasAccionesInstitucional.Update(programa);
            await _context.SaveChangesAsync();
        }

        public async Task<ProgramaAccionesInstitucional?> ObtenerProgramaAccionesPorIdAsync(int idPrograma)
        {
            return await _context.ProgramasAccionesInstitucional
                .Include(p => p.Estudiante)
                .Include(p => p.Periodo)
                .Include(p => p.ResponsableSupervision)
                .Include(p => p.VerificadoPor)
                .FirstOrDefaultAsync(p => p.IdPrograma == idPrograma);
        }

        public async Task VerificarProgramaAccionesAsync(int idPrograma, string resultadoFinal, bool aprobarConducta, string conclusiones, string verificadoPorId)
        {
            var programa = await _context.ProgramasAccionesInstitucional.FindAsync(idPrograma);
            if (programa == null)
            {
                throw new InvalidOperationException("Programa no encontrado");
            }

            programa.Estado = "Completado";
            programa.ResultadoFinal = resultadoFinal;
            programa.AprobarConducta = aprobarConducta;
            programa.ConclusionesComite = conclusiones;
            programa.FechaVerificacion = DateTime.Now;
            programa.VerificadoPorId = verificadoPorId;
            programa.FechaFinReal = DateTime.Now;

            await _context.SaveChangesAsync();

            // Si se aprueba la conducta, actualizar la nota
            if (aprobarConducta)
            {
                var notaConducta = await _context.NotasConducta.FindAsync(programa.IdNotaConducta);
                if (notaConducta != null)
                {
                    var notaMinima = await ObtenerNotaMinimaAprobacionAsync();
                    notaConducta.NotaFinal = notaMinima;
                    notaConducta.Estado = "Aprobado";
                    notaConducta.RequiereProgramaAcciones = false;
                    await _context.SaveChangesAsync();
                }
            }
        }

        #endregion

        #region Decisión Profesional

        public async Task<int> AplicarDecisionProfesionalAsync(DecisionProfesionalConducta decision)
        {
            _context.DecisionesProfesionalesConducta.Add(decision);
            await _context.SaveChangesAsync();

            // Actualizar la nota de conducta
            var notaConducta = await _context.NotasConducta.FindAsync(decision.IdNotaConducta);
            if (notaConducta != null)
            {
                notaConducta.IdDecisionProfesional = decision.IdDecision;
                notaConducta.DecisionProfesionalAplicada = true;

                if (decision.DecisionTomada == "Asignar Aprobado")
                {
                    var notaMinima = await ObtenerNotaMinimaAprobacionAsync();
                    notaConducta.NotaFinal = notaMinima;
                    notaConducta.Estado = "Aprobado";
                    notaConducta.RequiereProgramaAcciones = false;
                }

                await _context.SaveChangesAsync();
            }

            return decision.IdDecision;
        }

        public async Task<DecisionProfesionalConducta?> ObtenerDecisionProfesionalPorIdAsync(int idDecision)
        {
            return await _context.DecisionesProfesionalesConducta
                .Include(d => d.Estudiante)
                .Include(d => d.Periodo)
                .Include(d => d.TomaDecisionPor)
                .FirstOrDefaultAsync(d => d.IdDecision == idDecision);
        }

        #endregion

        #region Notificaciones

        public async Task NotificarProfesorGuiaAsync(int idBoleta)
        {
            try
            {
                var boleta = await ObtenerBoletaPorIdAsync(idBoleta);
                if (boleta == null)
                {
                    _logger.LogWarning($"No se pudo notificar: boleta {idBoleta} no encontrada");
                    return;
                }

                // Paso 1: Obtener el grupo principal del estudiante
                var grupoInfo = await _context.EstudianteGrupos
                    .Where(eg => eg.EstudianteId == boleta.IdEstudiante && 
                                 eg.Estado == EstadoAsignacion.Activo && 
                                 eg.EsGrupoPrincipal)
                    .Select(eg => new { eg.GrupoId })
                    .FirstOrDefaultAsync();

                if (grupoInfo == null)
                {
                    _logger.LogWarning($"No se pudo notificar: estudiante {boleta.IdEstudiante} no tiene grupo principal asignado");
                    return;
                }

                _logger.LogInformation($"Estudiante {boleta.IdEstudiante} pertenece al grupo {grupoInfo.GrupoId}");

                // Paso 2: Obtener el profesor guía del grupo
                var profesorGuiaInfo = await _context.ProfesorGuia
                    .Where(pg => pg.GrupoId == grupoInfo.GrupoId && pg.Estado == true)
                    .Select(pg => new { pg.ProfesorId })
                    .FirstOrDefaultAsync();

                if (profesorGuiaInfo == null)
                {
                    _logger.LogWarning($"No se pudo notificar: grupo {grupoInfo.GrupoId} no tiene profesor guía asignado");
                    return;
                }

                _logger.LogInformation($"Grupo {grupoInfo.GrupoId} tiene asignado al profesor {profesorGuiaInfo.ProfesorId}");

                // Paso 3: Obtener datos del profesor desde la tabla Profesores
                var profesor = await _context.Profesores
                    .Where(p => p.Id == profesorGuiaInfo.ProfesorId)
                    .Select(p => new { 
                        p.Id, 
                        p.Nombres, 
                        p.PrimerApellido, 
                        p.SegundoApellido,
                        p.EmailInstitucional,
                        p.EmailPersonal
                    })
                    .FirstOrDefaultAsync();

                if (profesor == null)
                {
                    _logger.LogError($"No se pudo notificar: profesor {profesorGuiaInfo.ProfesorId} no encontrado en tabla Profesores");
                    return;
                }

                // Determinar el email a usar (prioridad: institucional > personal)
                var emailProfesor = !string.IsNullOrEmpty(profesor.EmailInstitucional) 
                    ? profesor.EmailInstitucional 
                    : profesor.EmailPersonal;

                if (string.IsNullOrEmpty(emailProfesor))
                {
                    _logger.LogWarning($"No se pudo notificar: profesor {profesor.Nombres} {profesor.PrimerApellido} no tiene email configurado");
                    return;
                }

                // Preparar datos para el correo
                var nombreProfesor = $"{profesor.Nombres} {profesor.PrimerApellido} {profesor.SegundoApellido}".Trim();
                var nombreEstudiante = $"{boleta.Estudiante.Nombre} {boleta.Estudiante.Apellidos}";
                var numeroId = boleta.Estudiante.NumeroId;
                var tipoFalta = boleta.TipoFalta.Nombre;
                var rebajo = boleta.RebajoAplicado;
                var descripcion = boleta.Descripcion;
                var fechaEmision = boleta.FechaEmision;
                var docenteEmisor = $"{boleta.DocenteEmisor.Nombre} {boleta.DocenteEmisor.Apellidos}";

                _logger.LogInformation($"Enviando notificación a: {nombreProfesor} ({emailProfesor})");

                // Enviar correo electrónico
                var emailEnviado = await _emailService.SendBoletaConductaNotificationAsync(
                    emailProfesor,
                    nombreProfesor,
                    nombreEstudiante,
                    numeroId,
                    tipoFalta,
                    rebajo,
                    descripcion,
                    fechaEmision,
                    docenteEmisor
                );

                if (emailEnviado)
                {
                    // Actualizar el flag de notificación
                    boleta.NotificacionEnviada = true;
                    boleta.FechaNotificacion = DateTime.Now;
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Notificación enviada exitosamente al profesor guía {emailProfesor} por boleta {idBoleta}");
                }
                else
                {
                    _logger.LogWarning($"No se pudo enviar el correo de notificación al profesor guía {emailProfesor} por boleta {idBoleta}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al notificar profesor guía para boleta {idBoleta}");
            }
        }

        public async Task<int?> ObtenerProfesorGuiaDeEstudianteAsync(int idEstudiante)
        {
            try
            {
                // Obtener el grupo principal del estudiante
                var grupoInfo = await _context.EstudianteGrupos
                    .Where(eg => eg.EstudianteId == idEstudiante && eg.Estado == EstadoAsignacion.Activo && eg.EsGrupoPrincipal)
                    .Select(eg => new { eg.GrupoId, eg.EstudianteId })
                    .FirstOrDefaultAsync();

                if (grupoInfo == null || grupoInfo.GrupoId == 0)
                {
                    _logger.LogWarning($"No se encontró grupo principal activo para estudiante {idEstudiante}");
                    return null;
                }

                _logger.LogInformation($"Estudiante {idEstudiante} pertenece al grupo {grupoInfo.GrupoId}");

                // Obtener el profesor guía del grupo
                var profesorGuiaInfo = await _context.ProfesorGuia
                    .Where(pg => pg.GrupoId == grupoInfo.GrupoId && pg.Estado == true)
                    .Select(pg => new { pg.ProfesorId, pg.GrupoId })
                    .FirstOrDefaultAsync();

                if (profesorGuiaInfo == null || profesorGuiaInfo.ProfesorId == 0)
                {
                    _logger.LogWarning($"No se encontró profesor guía activo para grupo {grupoInfo.GrupoId}");
                    return null;
                }

                _logger.LogInformation($"Grupo {grupoInfo.GrupoId} tiene asignado al profesor {profesorGuiaInfo.ProfesorId}");

                // Retornar directamente el ProfesorId de la tabla Profesores
                return profesorGuiaInfo.ProfesorId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener profesor guía para estudiante {idEstudiante}");
                return null;
            }
        }

        public async Task<string?> ObtenerNombreProfesorGuiaAsync(int idEstudiante)
        {
            var profesorId = await ObtenerProfesorGuiaDeEstudianteAsync(idEstudiante);
            if (!profesorId.HasValue) return null;

            // Obtener el nombre desde la tabla Profesores
            var profesor = await _context.Profesores
                .Where(p => p.Id == profesorId.Value)
                .Select(p => new { p.Nombres, p.PrimerApellido, p.SegundoApellido })
                .FirstOrDefaultAsync();

            return profesor != null ? $"{profesor.Nombres} {profesor.PrimerApellido} {profesor.SegundoApellido}".Trim() : null;
        }

        private async Task<string?> ObtenerGrupoEstudianteAsync(int idEstudiante)
        {
            var grupoId = await _context.EstudianteGrupos
                .Where(eg => eg.EstudianteId == idEstudiante && eg.Estado == EstadoAsignacion.Activo && eg.EsGrupoPrincipal)
                .Select(eg => eg.GrupoId)
                .FirstOrDefaultAsync();

            if (grupoId == 0) return null;

            var grupo = await _context.GruposEstudiantes.FindAsync(grupoId);
            return grupo?.Nombre;
        }

        #endregion

        #region Reportes

        public async Task<DashboardConductaViewModel> ObtenerDashboardConductaAsync(int idPeriodo)
        {
            var periodo = await _context.PeriodosAcademicos.FindAsync(idPeriodo);
            
            var totalEstudiantes = await _context.Estudiantes.CountAsync(e => e.PeriodoAcademicoId == idPeriodo);
            var totalBoletas = await _context.BoletasConducta.CountAsync(b => b.IdPeriodo == idPeriodo && b.Estado == "Activa");
            var estudiantesConBoletas = await _context.BoletasConducta
                .Where(b => b.IdPeriodo == idPeriodo && b.Estado == "Activa")
                .Select(b => b.IdEstudiante)
                .Distinct()
                .CountAsync();

            var notas = await _context.NotasConducta
                .Where(n => n.IdPeriodo == idPeriodo)
                .ToListAsync();

            var boletasPorTipo = await _context.BoletasConducta
                .Where(b => b.IdPeriodo == idPeriodo && b.Estado == "Activa")
                .Include(b => b.TipoFalta)
                .GroupBy(b => b.TipoFalta.Nombre)
                .Select(g => new { TipoFalta = g.Key, Cantidad = g.Count() })
                .ToDictionaryAsync(x => x.TipoFalta, x => x.Cantidad);

            var programasActivos = await _context.ProgramasAccionesInstitucional
                .CountAsync(p => p.IdPeriodo == idPeriodo && (p.Estado == "Pendiente" || p.Estado == "En Proceso"));

            var decisionesAplicadas = await _context.DecisionesProfesionalesConducta
                .CountAsync(d => d.IdPeriodo == idPeriodo);

            var ultimasBoletas = await _context.BoletasConducta
                .Include(b => b.Estudiante)
                .Include(b => b.TipoFalta)
                .Include(b => b.DocenteEmisor)
                .Where(b => b.IdPeriodo == idPeriodo && b.Estado == "Activa")
                .OrderByDescending(b => b.FechaEmision)
                .Take(10)
                .Select(b => new BoletaConductaListViewModel
                {
                    IdBoleta = b.IdBoleta,
                    IdEstudiante = b.IdEstudiante,
                    NombreEstudiante = $"{b.Estudiante.Nombre} {b.Estudiante.Apellidos}",
                    NumeroId = b.Estudiante.NumeroId,
                    TipoFalta = b.TipoFalta.Nombre,
                    RebajoAplicado = b.RebajoAplicado,
                    Descripcion = b.Descripcion,
                    DocenteEmisor = $"{b.DocenteEmisor.Nombre} {b.DocenteEmisor.Apellidos}",
                    FechaEmision = b.FechaEmision,
                    Estado = b.Estado,
                    NotificacionEnviada = b.NotificacionEnviada
                })
                .ToListAsync();

            // Obtener estudiantes que requieren atención (En Riesgo o Aplazados)
            var estudiantesAtencion = await (from nc in _context.NotasConducta
                where nc.IdPeriodo == idPeriodo && (nc.Estado == "Riesgo" || nc.Estado == "Aplazado")
                join e in _context.Estudiantes on nc.IdEstudiante equals e.IdEstudiante
                join eg in _context.EstudianteGrupos on e.IdEstudiante equals eg.EstudianteId into gruposEst
                from eg in gruposEst.DefaultIfEmpty()
                join g in _context.GruposEstudiantes on eg.GrupoId equals g.GrupoId into grupos
                from g in grupos.DefaultIfEmpty()
                select new
                {
                    nc.IdEstudiante,
                    e.Nombre,
                    e.Apellidos,
                    e.NumeroId,
                    GrupoNombre = g != null ? g.Nombre : "Sin grupo",
                    nc.NotaFinal,
                    nc.Estado
                })
                .Distinct()
                .OrderBy(x => x.NotaFinal)
                .ToListAsync();

            var estudiantesAtencionViewModel = estudiantesAtencion.Select(ea => new EstudianteAtencionViewModel
            {
                IdEstudiante = ea.IdEstudiante,
                NombreEstudiante = $"{ea.Nombre} {ea.Apellidos}",
                NombreCompleto = $"{ea.Nombre} {ea.Apellidos}",
                NumeroIdentificacion = ea.NumeroId,
                Grupo = ea.GrupoNombre,
                NombreGrupo = ea.GrupoNombre,
                Nota = ea.NotaFinal,
                NotaFinal = ea.NotaFinal,
                Estado = ea.Estado,
                CantidadBoletas = _context.BoletasConducta.Count(b => b.IdEstudiante == ea.IdEstudiante && b.IdPeriodo == idPeriodo && b.Estado == "Activa"),
                TienePrograma = _context.ProgramasAccionesInstitucional.Any(p => p.IdEstudiante == ea.IdEstudiante && p.IdPeriodo == idPeriodo),
                TieneDecision = _context.DecisionesProfesionalesConducta.Any(d => d.IdEstudiante == ea.IdEstudiante && d.IdPeriodo == idPeriodo)
            }).ToList();

            return new DashboardConductaViewModel
            {
                IdPeriodo = idPeriodo,
                Periodo = periodo?.Nombre ?? "Período no encontrado",
                TotalEstudiantes = totalEstudiantes,
                TotalBoletas = totalBoletas,
                TotalBoletasEmitidas = totalBoletas,
                EstudiantesConBoletas = estudiantesConBoletas,
                EstudiantesSinBoletas = totalEstudiantes - estudiantesConBoletas,
                TotalPuntosRebajados = notas.Sum(n => n.TotalRebajos),
                PromedioGeneralConducta = notas.Any() ? notas.Average(n => n.NotaFinal) : 100,
                EstudiantesAprobados = notas.Count(n => n.Estado == "Aprobado"),
                PorcentajeAprobados = totalEstudiantes > 0 ? ((decimal)notas.Count(n => n.Estado == "Aprobado") / totalEstudiantes * 100) : 0,
                EstudiantesRiesgo = notas.Count(n => n.Estado == "Riesgo"),
                EstudiantesEnRiesgo = notas.Count(n => n.Estado == "Riesgo"),
                PorcentajeEnRiesgo = totalEstudiantes > 0 ? ((decimal)notas.Count(n => n.Estado == "Riesgo") / totalEstudiantes * 100) : 0,
                EstudiantesAplazados = notas.Count(n => n.Estado == "Aplazado"),
                PorcentajeAplazados = totalEstudiantes > 0 ? ((decimal)notas.Count(n => n.Estado == "Aplazado") / totalEstudiantes * 100) : 0,
                BoletasPorTipoFalta = boletasPorTipo,
                ProgramasAccionesActivos = programasActivos,
                DecisionesProfesionalesAplicadas = decisionesAplicadas,
                EstudiantesRequierenAtencion = estudiantesAtencionViewModel,
                UltimasBoletas = ultimasBoletas
            };
        }

        public async Task<HistorialBoletasEstudianteViewModel> ObtenerHistorialBoletasEstudianteAsync(int idEstudiante, int idPeriodo)
        {
            var estudiante = await _context.Estudiantes.FindAsync(idEstudiante);
            var periodo = await _context.PeriodosAcademicos.FindAsync(idPeriodo);
            var notaConducta = await ObtenerNotaConductaAsync(idEstudiante, idPeriodo);

            var boletas = await _context.BoletasConducta
                .Include(b => b.TipoFalta)
                .Include(b => b.DocenteEmisor)
                .Where(b => b.IdEstudiante == idEstudiante && b.IdPeriodo == idPeriodo)
                .OrderByDescending(b => b.FechaEmision)
                .Select(b => new BoletaConductaListViewModel
                {
                    IdBoleta = b.IdBoleta,
                    IdEstudiante = b.IdEstudiante,
                    NombreEstudiante = $"{estudiante!.Nombre} {estudiante.Apellidos}",
                    NumeroId = estudiante.NumeroId,
                    TipoFalta = b.TipoFalta.Nombre,
                    RebajoAplicado = b.RebajoAplicado,
                    Descripcion = b.Descripcion,
                    DocenteEmisor = $"{b.DocenteEmisor.Nombre} {b.DocenteEmisor.Apellidos}",
                    FechaEmision = b.FechaEmision,
                    Estado = b.Estado,
                    NotificacionEnviada = b.NotificacionEnviada
                })
                .ToListAsync();

            return new HistorialBoletasEstudianteViewModel
            {
                TituloReporte = "Historial de Boletas de Conducta",
                Periodo = periodo?.Nombre,
                NombreEstudiante = $"{estudiante?.Nombre} {estudiante?.Apellidos}",
                NumeroId = estudiante?.NumeroId ?? "",
                Boletas = boletas,
                NotaFinalConducta = notaConducta?.NotaFinal ?? 100,
                EstadoConducta = notaConducta?.Estado ?? "Aprobado"
            };
        }

        public async Task<RegistroFaltasGrupoViewModel> ObtenerRegistroFaltasGrupoAsync(int idGrupo, int idPeriodo)
        {
            var grupo = await _context.GruposEstudiantes.FindAsync(idGrupo);
            var periodo = await _context.PeriodosAcademicos.FindAsync(idPeriodo);

            var estudiantesIds = await _context.EstudianteGrupos
                .Where(eg => eg.GrupoId == idGrupo && eg.Estado == EstadoAsignacion.Activo)
                .Select(eg => eg.EstudianteId)
                .ToListAsync();

            var totalEstudiantes = estudiantesIds.Count();

            var boletas = await _context.BoletasConducta
                .Include(b => b.Estudiante)
                .Include(b => b.TipoFalta)
                .Include(b => b.DocenteEmisor)
                .Where(b => estudiantesIds.Contains(b.IdEstudiante) && b.IdPeriodo == idPeriodo && b.Estado == "Activa")
                .OrderByDescending(b => b.FechaEmision)
                .Select(b => new BoletaConductaListViewModel
                {
                    IdBoleta = b.IdBoleta,
                    IdEstudiante = b.IdEstudiante,
                    NombreEstudiante = $"{b.Estudiante.Nombre} {b.Estudiante.Apellidos}",
                    NumeroId = b.Estudiante.NumeroId,
                    TipoFalta = b.TipoFalta.Nombre,
                    RebajoAplicado = b.RebajoAplicado,
                    Descripcion = b.Descripcion,
                    DocenteEmisor = $"{b.DocenteEmisor.Nombre} {b.DocenteEmisor.Apellidos}",
                    FechaEmision = b.FechaEmision,
                    Estado = b.Estado,
                    NotificacionEnviada = b.NotificacionEnviada
                })
                .ToListAsync();

            var faltasPorTipo = await _context.BoletasConducta
                .Where(b => estudiantesIds.Contains(b.IdEstudiante) && b.IdPeriodo == idPeriodo && b.Estado == "Activa")
                .Include(b => b.TipoFalta)
                .GroupBy(b => b.TipoFalta.Nombre)
                .Select(g => new { TipoFalta = g.Key, Cantidad = g.Count() })
                .ToDictionaryAsync(x => x.TipoFalta, x => x.Cantidad);

            return new RegistroFaltasGrupoViewModel
            {
                TituloReporte = "Registro de Faltas por Grupo",
                Periodo = periodo?.Nombre,
                Grupo = grupo?.Nombre,
                NombreGrupo = grupo?.Nombre ?? "Grupo no encontrado",
                TotalEstudiantes = totalEstudiantes,
                TotalBoletas = boletas.Count,
                FaltasPorTipo = faltasPorTipo,
                Boletas = boletas
            };
        }

        #endregion
    }
}
