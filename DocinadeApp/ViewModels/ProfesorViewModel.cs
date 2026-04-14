using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RubricasApp.Web.ViewModels
{
    /// <summary>
    /// ViewModel para el formulario multi-step de gestión de profesores
    /// </summary>
    public class ProfesorViewModel
    {
        // ======== DATOS PRINCIPALES DEL PROFESOR ========
        public int Id { get; set; }

        // ======== PASO 1: INFORMACIÓN PERSONAL ========
        
        [Required(ErrorMessage = "Los nombres son requeridos")]
        [StringLength(100, ErrorMessage = "Los nombres no pueden exceder 100 caracteres")]
        [Display(Name = "Nombres")]
        public string Nombres { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El primer apellido es requerido")]
        [StringLength(100, ErrorMessage = "El primer apellido no puede exceder 100 caracteres")]
        [Display(Name = "Primer Apellido")]
        public string PrimerApellido { get; set; } = string.Empty;
        
        [StringLength(100, ErrorMessage = "El segundo apellido no puede exceder 100 caracteres")]
        [Display(Name = "Segundo Apellido")]
        public string? SegundoApellido { get; set; }
        
        [Required(ErrorMessage = "La cédula es requerida")]
        [StringLength(20, ErrorMessage = "La cédula no puede exceder 20 caracteres")]
        [Display(Name = "Número de Cédula")]
        public string Cedula { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El tipo de cédula es requerido")]
        [Display(Name = "Tipo de Cédula")]
        public string TipoCedula { get; set; } = "Nacional";
        
        [Display(Name = "Sexo")]
        public string? Sexo { get; set; }
        
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Nacimiento")]
        public DateTime? FechaNacimiento { get; set; }
        
        [StringLength(20)]
        [Display(Name = "Estado Civil")]
        public string? EstadoCivil { get; set; }
        
        [StringLength(50)]
        [Display(Name = "Nacionalidad")]
        public string Nacionalidad { get; set; } = "Costarricense";

        // ======== PASO 2: INFORMACIÓN DE CONTACTO ========
        
        [Required(ErrorMessage = "El email personal es requerido")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        [StringLength(150)]
        [Display(Name = "Email Personal")]
        public string EmailPersonal { get; set; } = string.Empty;
        
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        [StringLength(150)]
        [Display(Name = "Email Institucional")]
        public string? EmailInstitucional { get; set; }
        
        [StringLength(20)]
        [Display(Name = "Teléfono Fijo")]
        public string? TelefonoFijo { get; set; }
        
        [Required(ErrorMessage = "El teléfono celular es requerido")]
        [StringLength(20)]
        [Display(Name = "Teléfono Celular")]
        public string TelefonoCelular { get; set; } = string.Empty;
        
        [StringLength(20)]
        [Display(Name = "Teléfono Oficina")]
        public string? TelefonoOficina { get; set; }
        
        [StringLength(10)]
        [Display(Name = "Extensión")]
        public string? Extension { get; set; }

        // ======== PASO 3: DIRECCIÓN COMPLETA ========
        
        [StringLength(400)]
        [Display(Name = "Dirección Exacta")]
        public string? DireccionExacta { get; set; }
        
        [Display(Name = "Provincia")]
        public int? ProvinciaId { get; set; }
        
        [Display(Name = "Cantón")]
        public int? CantonId { get; set; }
        
        [Display(Name = "Distrito")]
        public int? DistritoId { get; set; }
        
        [StringLength(10)]
        [Display(Name = "Código Postal")]
        public string? CodigoPostal { get; set; }

        // Propiedades adicionales para mostrar nombres (no editables)
        public string? ProvinciaNombre { get; set; }
        public string? CantonNombre { get; set; }
        public string? DistritoNombre { get; set; }

        // ======== PASO 4: INFORMACIÓN ACADÉMICA ========
        
        [StringLength(100)]
        [Display(Name = "Grado Académico")]
        public string? GradoAcademico { get; set; }
        
        [StringLength(200)]
        [Display(Name = "Título Académico")]
        public string? TituloAcademico { get; set; }
        
        [StringLength(200)]
        [Display(Name = "Institución de Graduación")]
        public string? InstitucionGraduacion { get; set; }
        
        [StringLength(50)]
        [Display(Name = "País de Graduación")]
        public string PaisGraduacion { get; set; } = "Costa Rica";
        
        [Display(Name = "Año de Graduación")]
        public int? AnioGraduacion { get; set; }
        
        [StringLength(30)]
        [Display(Name = "Número Colegiado Profesional")]
        public string? NumeroColegiadoProfesional { get; set; }

        // ======== PASO 5: INFORMACIÓN LABORAL ========
        
        [Display(Name = "Escuela")]
        public int? EscuelaId { get; set; }
        
        [StringLength(20)]
        [Display(Name = "Código de Empleado")]
        public string? CodigoEmpleado { get; set; }
        
        [Required(ErrorMessage = "La fecha de ingreso es requerida")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Ingreso")]
        public DateTime FechaIngreso { get; set; } = DateTime.Today;
        
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Retiro")]
        public DateTime? FechaRetiro { get; set; }
        
        [Required(ErrorMessage = "El tipo de contrato es requerido")]
        [StringLength(50)]
        [Display(Name = "Tipo de Contrato")]
        public string TipoContrato { get; set; } = string.Empty;
        
        [StringLength(50)]
        [Display(Name = "Régimen Laboral")]
        public string? RegimenLaboral { get; set; }
        
        [StringLength(50)]
        [Display(Name = "Categoría Laboral")]
        public string? CategoriaLaboral { get; set; }
        
        [Required(ErrorMessage = "El tipo de jornada es requerido")]
        [StringLength(20)]
        [Display(Name = "Tipo de Jornada")]
        public string TipoJornada { get; set; } = string.Empty;
        
        [Range(0, 48, ErrorMessage = "Las horas laborales deben estar entre 0 y 48")]
        [Display(Name = "Horas Laborales")]
        public decimal? HorasLaborales { get; set; }

        // ======== PASO 6: INFORMACIÓN ADICIONAL ========
        
        [StringLength(500)]
        [Display(Name = "Áreas de Especialización")]
        public string? AreasEspecializacion { get; set; }
        
        [StringLength(200)]
        [Display(Name = "Idiomas que Habla")]
        public string? IdiomasHabla { get; set; }
        
        [StringLength(20)]
        [Display(Name = "Nivel de Inglés")]
        public string? NivelIngles { get; set; }
        
        [Range(0, 50, ErrorMessage = "La experiencia docente debe estar entre 0 y 50 años")]
        [Display(Name = "Años de Experiencia Docente")]
        public int? ExperienciaDocente { get; set; }

        // ======== PASO 7: CONTACTO DE EMERGENCIA ========
        
        [StringLength(150)]
        [Display(Name = "Nombre del Contacto de Emergencia")]
        public string? ContactoEmergenciaNombre { get; set; }
        
        [StringLength(50)]
        [Display(Name = "Parentesco")]
        public string? ContactoEmergenciaParentesco { get; set; }
        
        [StringLength(20)]
        [Display(Name = "Teléfono de Emergencia")]
        public string? ContactoEmergenciaTelefono { get; set; }

        // ======== CONFIGURACIONES ========
        
        [Display(Name = "Recibir Notificaciones por Email")]
        public bool NotificacionesEmail { get; set; } = true;
        
        [Display(Name = "Recibir Notificaciones por SMS")]
        public bool NotificacionesSMS { get; set; } = false;
        
        [Display(Name = "Estado")]
        public bool Estado { get; set; } = true;
        
        [StringLength(200)]
        [Display(Name = "Motivo de Inactividad")]
        public string? MotivoInactividad { get; set; }

        // ======== LISTAS PARA DROPDOWNS ========
        public List<SelectListItem> Provincias { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Cantones { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Distritos { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Escuelas { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> GruposDisponibles { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> MateriasDisponibles { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> PeriodosDisponibles { get; set; } = new List<SelectListItem>();

        // ======== PROPIEDADES CALCULADAS ========
        public string NombreCompleto => $"{Nombres} {PrimerApellido} {SegundoApellido}".Trim();

        // ======== DATOS RELACIONADOS (PARA VISTAS) ========
        public List<ProfesorFormacionAcademicaDto> FormacionAcademica { get; set; } = new List<ProfesorFormacionAcademicaDto>();
        public List<ProfesorExperienciaLaboralDto> ExperienciaLaboral { get; set; } = new List<ProfesorExperienciaLaboralDto>();
        public List<ProfesorCapacitacionDto> Capacitaciones { get; set; } = new List<ProfesorCapacitacionDto>();
        public List<ProfesorGrupoDto> GruposAsignados { get; set; } = new List<ProfesorGrupoDto>();
        public List<ProfesorGuiaDto> GruposQueGuia { get; set; } = new List<ProfesorGuiaDto>();
        public List<ProfesorGrupoCreateDto> ProfesorGrupos { get; set; } = new List<ProfesorGrupoCreateDto>();
    }

    // ======== DTOs PARA DATOS RELACIONADOS ========

    public class ProfesorFormacionAcademicaDto
    {
        public int Id { get; set; }
        public string TipoFormacion { get; set; } = string.Empty;
        public string TituloObtenido { get; set; } = string.Empty;
        public string InstitucionEducativa { get; set; } = string.Empty;
        public string PaisInstitucion { get; set; } = "Costa Rica";
        public int? AnioInicio { get; set; }
        public int? AnioFinalizacion { get; set; }
        public bool EnCurso { get; set; }
        public decimal? PromedioGeneral { get; set; }
        public bool EsTituloReconocidoCONARE { get; set; } = true;
        public string? NumeroReconocimiento { get; set; }
    }

    public class ProfesorExperienciaLaboralDto
    {
        public int Id { get; set; }
        public string NombreInstitucion { get; set; } = string.Empty;
        public string CargoDesempenado { get; set; } = string.Empty;
        public string? TipoInstitucion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public bool TrabajandoActualmente { get; set; }
        public string? DescripcionFunciones { get; set; }
        public string? TipoContrato { get; set; }
        public string? JornadaLaboral { get; set; }
        public int MesesExperiencia { get; set; }
    }

    public class ProfesorCapacitacionDto
    {
        public int Id { get; set; }
        public string NombreCapacitacion { get; set; } = string.Empty;
        public string? InstitucionOrganizadora { get; set; }
        public string? TipoCapacitacion { get; set; }
        public string? Modalidad { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int? HorasCapacitacion { get; set; }
        public bool CertificadoObtenido { get; set; }
        public decimal? CalificacionObtenida { get; set; }
        public string? AreaConocimiento { get; set; }
    }

    public class ProfesorGrupoDto
    {
        public int Id { get; set; }
        public string NombreGrupo { get; set; } = string.Empty;
        public string NombreMateria { get; set; } = string.Empty;
        public string PeriodoAcademico { get; set; } = string.Empty;
        public bool EsProfesorPrincipal { get; set; }
        public string? AulaAsignada { get; set; }
        public bool Estado { get; set; }
        public DateTime FechaAsignacion { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
    }

    public class ProfesorGuiaDto
    {
        public int Id { get; set; }
        public string NombreGrupo { get; set; } = string.Empty;
        public DateTime FechaAsignacion { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public bool Estado { get; set; }
        public string? Observaciones { get; set; }
    }

    public class ProfesorGrupoCreateDto
    {
        public int ProfesorGrupoId { get; set; }
        public int ProfessorId { get; set; }
        public int GrupoId { get; set; }
        public int MateriaId { get; set; }
        public int PeriodoId { get; set; }
        public bool EsProfesorGuia { get; set; }
        public bool Estado { get; set; } = true;
    }

    /// <summary>
    /// ViewModel para vincular un usuario a un profesor
    /// </summary>
    public class VincularUsuarioProfesorViewModel
    {
        public int ProfesorId { get; set; }
        
        [Display(Name = "Profesor")]
        public string NombreProfesor { get; set; } = string.Empty;
        
        [Display(Name = "Cédula del Profesor")]
        public string CedulaProfesor { get; set; } = string.Empty;
        
        [Display(Name = "Usuario Vinculado")]
        public string? UsuarioVinculadoId { get; set; }
        
        public string? UsuarioVinculadoNombre { get; set; }
        
        [Display(Name = "Seleccionar Usuario")]
        public string? UsuarioSeleccionadoId { get; set; }
        
        public List<SelectListItem> UsuariosDisponibles { get; set; } = new();
    }
}
