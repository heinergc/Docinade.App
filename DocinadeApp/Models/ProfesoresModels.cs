using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocinadeApp.Models
{
    // ============================================================
    // MODELOS PARA PROFESORES - COSTA RICA
    // Sistema de Rúbricas Académicas
    // ============================================================

    /// <summary>
    /// Modelo para las Provincias de Costa Rica
    /// </summary>
    public class Provincia
    {
        public int Id { get; set; }
        
        [Required, MaxLength(50)]
        public string Nombre { get; set; } = string.Empty;
        
        [MaxLength(10)]
        public string Codigo { get; set; } = string.Empty;
        
        public bool Estado { get; set; } = true;
        
        // Navegación
        public virtual ICollection<Canton> Cantones { get; set; } = new List<Canton>();
    }

    /// <summary>
    /// Modelo para los Cantones de Costa Rica
    /// </summary>
    public class Canton
    {
        public int Id { get; set; }
        
        public int ProvinciaId { get; set; }
        
        [Required, MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;
        
        [MaxLength(10)]
        public string Codigo { get; set; } = string.Empty;
        
        public bool Estado { get; set; } = true;
        
        // Navegación
        public virtual Provincia Provincia { get; set; } = null!;
        public virtual ICollection<Distrito> Distritos { get; set; } = new List<Distrito>();
    }

    /// <summary>
    /// Modelo para los Distritos de Costa Rica
    /// </summary>
    public class Distrito
    {
        public int Id { get; set; }
        
        public int CantonId { get; set; }
        
        [Required, MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;
        
        [MaxLength(10)]
        public string Codigo { get; set; } = string.Empty;
        
        public bool Estado { get; set; } = true;
        
        // Navegación
        public virtual Canton Canton { get; set; } = null!;
        public virtual ICollection<Institucion> Instituciones { get; set; } = new List<Institucion>();
        public virtual ICollection<Profesor> Profesores { get; set; } = new List<Profesor>();
    }

    /// <summary>
    /// Modelo para Instituciones Educativas
    /// </summary>
    public class Institucion
    {
        public int Id { get; set; }
        
        [Required, MaxLength(200)]
        public string Nombre { get; set; } = string.Empty;
        
        [MaxLength(20)]
        public string? Siglas { get; set; }
        
        [Required, MaxLength(50)]
        public string TipoInstitucion { get; set; } = string.Empty; // Universidad Pública, Universidad Privada, etc.
        
        [MaxLength(20)]
        public string? CodigoMEP { get; set; }
        
        [MaxLength(20)]
        public string? Telefono { get; set; }
        
        [MaxLength(150)]
        public string? Email { get; set; }
        
        [MaxLength(200)]
        public string? SitioWeb { get; set; }
        
        [MaxLength(300)]
        public string? Direccion { get; set; }
        
        [MaxLength(200)]
        public string? DireccionRegional { get; set; }
        
        [MaxLength(100)]
        public string? CircuitoEscolar { get; set; }
        
        public int? DistritoId { get; set; }
        
        public bool Estado { get; set; } = true;
        
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        
        // Navegación
        public virtual Distrito? Distrito { get; set; }
        public virtual ICollection<Facultad> Facultades { get; set; } = new List<Facultad>();
    }

    /// <summary>
    /// Modelo para Facultades
    /// </summary>
    public class Facultad
    {
        public int Id { get; set; }
        
        public int InstitucionId { get; set; }
        
        [Required, MaxLength(200)]
        public string Nombre { get; set; } = string.Empty;
        
        [MaxLength(20)]
        public string? Codigo { get; set; }
        
        [MaxLength(150)]
        public string? Decano { get; set; }
        
        [MaxLength(150)]
        public string? Email { get; set; }
        
        [MaxLength(20)]
        public string? Telefono { get; set; }
        
        public bool Estado { get; set; } = true;
        
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        
        // Navegación
        public virtual Institucion Institucion { get; set; } = null!;
        public virtual ICollection<Escuela> Escuelas { get; set; } = new List<Escuela>();
    }

    /// <summary>
    /// Modelo para Escuelas/Departamentos
    /// </summary>
    public class Escuela
    {
        public int Id { get; set; }
        
        public int FacultadId { get; set; }
        
        [Required, MaxLength(200)]
        public string Nombre { get; set; } = string.Empty;
        
        [MaxLength(20)]
        public string? Codigo { get; set; }
        
        [MaxLength(150)]
        public string? Director { get; set; }
        
        [MaxLength(150)]
        public string? Email { get; set; }
        
        [MaxLength(20)]
        public string? Telefono { get; set; }
        
        public bool Estado { get; set; } = true;
        
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        
        // Navegación
        public virtual Facultad Facultad { get; set; } = null!;
        public virtual ICollection<Profesor> Profesores { get; set; } = new List<Profesor>();
        public virtual ICollection<Materia> Materias { get; set; } = new List<Materia>();
    }

    /// <summary>
    /// Modelo principal para Profesores
    /// </summary>
    public class Profesor
    {
        public int Id { get; set; }
        
        // Información Personal Costa Rica
        [Required, MaxLength(100)]
        public string Nombres { get; set; } = string.Empty;
        
        [Required, MaxLength(100)]
        public string PrimerApellido { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string? SegundoApellido { get; set; }
        
        [Required, MaxLength(20)]
        public string Cedula { get; set; } = string.Empty; // Formato: 1-1234-5678
        
        [Required, MaxLength(10)]
        public string TipoCedula { get; set; } = "Nacional"; // Nacional, Residencia, Dimex
        
        [MaxLength(1)]
        public string? Sexo { get; set; } // M, F
        
        public DateTime? FechaNacimiento { get; set; }
        
        [MaxLength(20)]
        public string? EstadoCivil { get; set; }
        
        [MaxLength(50)]
        public string Nacionalidad { get; set; } = "Costarricense";
        
        // Información de Contacto
        [Required, MaxLength(150)]
        public string EmailPersonal { get; set; } = string.Empty;
        
        [MaxLength(150)]
        public string? EmailInstitucional { get; set; }
        
        [MaxLength(20)]
        public string? TelefonoFijo { get; set; }
        
        [Required, MaxLength(20)]
        public string TelefonoCelular { get; set; } = string.Empty;
        
        [MaxLength(20)]
        public string? TelefonoOficina { get; set; }
        
        [MaxLength(10)]
        public string? Extension { get; set; }
        
        // Dirección
        [MaxLength(400)]
        public string? DireccionExacta { get; set; }
        
        public int? ProvinciaId { get; set; }
        public int? CantonId { get; set; }
        public int? DistritoId { get; set; }
        
        [MaxLength(10)]
        public string? CodigoPostal { get; set; }
        
        // Información Académica
        [MaxLength(100)]
        public string? GradoAcademico { get; set; }
        
        [MaxLength(200)]
        public string? TituloAcademico { get; set; }
        
        [MaxLength(200)]
        public string? InstitucionGraduacion { get; set; }
        
        [MaxLength(50)]
        public string PaisGraduacion { get; set; } = "Costa Rica";
        
        public int? AnioGraduacion { get; set; }
        
        [MaxLength(30)]
        public string? NumeroColegiadoProfesional { get; set; }
        
        // Información Laboral
        public int? EscuelaId { get; set; }
        
        [MaxLength(20)]
        public string? CodigoEmpleado { get; set; }
        
        [Required]
        public DateTime FechaIngreso { get; set; }
        
        public DateTime? FechaRetiro { get; set; }
        
        [Required, MaxLength(50)]
        public string TipoContrato { get; set; } = string.Empty; // Propiedad, Interino, Sustituto, Honorarios
        
        [MaxLength(50)]
        public string? RegimenLaboral { get; set; }
        
        [MaxLength(50)]
        public string? CategoriaLaboral { get; set; }
        
        [Required, MaxLength(20)]
        public string TipoJornada { get; set; } = string.Empty; // Tiempo Completo, Medio Tiempo, etc.
        
        [Column(TypeName = "decimal(4,2)")]
        public decimal? HorasLaborales { get; set; }
        
        [Column(TypeName = "decimal(12,2)")]
        public decimal? SalarioBase { get; set; }
        
        // Información Bancaria
        [MaxLength(30)]
        public string? CuentaBancaria { get; set; }
        
        [MaxLength(20)]
        public string? TipoCuenta { get; set; }
        
        [MaxLength(100)]
        public string? BancoNombre { get; set; }
        
        // Cargos y Responsabilidades
        public bool EsDirector { get; set; } = false;
        public bool EsCoordinador { get; set; } = false;
        public bool EsDecano { get; set; } = false;
        
        [MaxLength(100)]
        public string? CargoAdministrativo { get; set; }
        
        public DateTime? FechaInicioCargoAdmin { get; set; }
        
        // Permisos en el Sistema
        public bool PuedeCrearRubricas { get; set; } = true;
        public bool PuedeEvaluarEstudiantes { get; set; } = true;
        public bool PuedeVerReportes { get; set; } = false;
        public bool EsAdministradorSistema { get; set; } = false;
        public bool PuedeGestionarUsuarios { get; set; } = false;
        
        // Estado y Control
        public bool Estado { get; set; } = true; // true = Activo, false = Inactivo
        
        [MaxLength(200)]
        public string? MotivoInactividad { get; set; }
        
        public bool NotificacionesEmail { get; set; } = true;
        public bool NotificacionesSMS { get; set; } = false;
        
        // Información Adicional Académica
        [MaxLength(500)]
        public string? AreasEspecializacion { get; set; }
        
        [MaxLength(200)]
        public string? IdiomasHabla { get; set; }
        
        [MaxLength(20)]
        public string? NivelIngles { get; set; }
        
        public int? ExperienciaDocente { get; set; }
        
        // Información de Emergencia
        [MaxLength(150)]
        public string? ContactoEmergenciaNombre { get; set; }
        
        [MaxLength(50)]
        public string? ContactoEmergenciaParentesco { get; set; }
        
        [MaxLength(20)]
        public string? ContactoEmergenciaTelefono { get; set; }
        
        // Auditoría
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        
        [MaxLength(100)]
        public string? CreadoPor { get; set; }
        
        public DateTime? FechaModificacion { get; set; }
        
        [MaxLength(100)]
        public string? ModificadoPor { get; set; }
        
        public int Version { get; set; } = 1;
        
        public int? UsuarioId { get; set; }
        
        // Navegación
        public virtual Provincia? Provincia { get; set; }
        public virtual Canton? Canton { get; set; }
        public virtual Distrito? Distrito { get; set; }
        public virtual Escuela? Escuela { get; set; }
        public virtual ICollection<ProfesorGrupo> ProfesorGrupos { get; set; } = new List<ProfesorGrupo>();
        public virtual ICollection<ProfesorGuia> ProfesoresGuia { get; set; } = new List<ProfesorGuia>();
        public virtual ICollection<ProfesorFormacionAcademica> FormacionAcademica { get; set; } = new List<ProfesorFormacionAcademica>();
        public virtual ICollection<ProfesorExperienciaLaboral> ExperienciaLaboral { get; set; } = new List<ProfesorExperienciaLaboral>();
        public virtual ICollection<ProfesorCapacitacion> Capacitaciones { get; set; } = new List<ProfesorCapacitacion>();
        
        // Propiedades calculadas
        [NotMapped]
        public string NombreCompleto => $"{Nombres} {PrimerApellido} {SegundoApellido}".Trim();
        
        [NotMapped]
        public string NombreCompletoConTitulo
        {
            get
            {
                var prefijo = GradoAcademico switch
                {
                    "Doctorado" => "Dr. ",
                    "Maestría" => "M.Sc. ",
                    "Licenciatura" => "Lic. ",
                    _ => ""
                };
                return $"{prefijo}{NombreCompleto}";
            }
        }
        
        [NotMapped]
        public int AniosServicio => FechaRetiro.HasValue 
            ? (int)(FechaRetiro.Value - FechaIngreso).TotalDays / 365
            : (int)(DateTime.Now - FechaIngreso).TotalDays / 365;
        
        [NotMapped]
        public bool EsProfesorGuiaActivo => ProfesoresGuia?.Any(pg => pg.Estado) ?? false;
        
        [NotMapped]
        public IEnumerable<GrupoEstudiante> GruposQueGuia => ProfesoresGuia?.Where(pg => pg.Estado).Select(pg => pg.Grupo) ?? Enumerable.Empty<GrupoEstudiante>();
    }



    /// <summary>
    /// Modelo para la Formación Académica del Profesor
    /// </summary>
    public class ProfesorFormacionAcademica
    {
        public int Id { get; set; }
        
        public int ProfesorId { get; set; }
        
        [Required, MaxLength(50)]
        public string TipoFormacion { get; set; } = string.Empty;
        
        [Required, MaxLength(200)]
        public string TituloObtenido { get; set; } = string.Empty;
        
        [Required, MaxLength(200)]
        public string InstitucionEducativa { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string PaisInstitucion { get; set; } = "Costa Rica";
        
        public int? AnioInicio { get; set; }
        public int? AnioFinalizacion { get; set; }
        
        public bool EnCurso { get; set; } = false;
        
        [Column(TypeName = "decimal(4,2)")]
        public decimal? PromedioGeneral { get; set; }
        
        public bool EsTituloReconocidoCONARE { get; set; } = true;
        
        [MaxLength(50)]
        public string? NumeroReconocimiento { get; set; }
        
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        
        // Navegación
        public virtual Profesor Profesor { get; set; } = null!;
    }

    /// <summary>
    /// Modelo para la Experiencia Laboral del Profesor
    /// </summary>
    public class ProfesorExperienciaLaboral
    {
        public int Id { get; set; }
        
        public int ProfesorId { get; set; }
        
        [Required, MaxLength(200)]
        public string NombreInstitucion { get; set; } = string.Empty;
        
        [Required, MaxLength(150)]
        public string CargoDesempenado { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string? TipoInstitucion { get; set; }
        
        [Required]
        public DateTime FechaInicio { get; set; }
        
        public DateTime? FechaFin { get; set; }
        
        public bool TrabajandoActualmente { get; set; } = false;
        
        [MaxLength(1000)]
        public string? DescripcionFunciones { get; set; }
        
        [MaxLength(50)]
        public string? TipoContrato { get; set; }
        
        [MaxLength(50)]
        public string? JornadaLaboral { get; set; }
        
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        
        // Navegación
        public virtual Profesor Profesor { get; set; } = null!;
        
        // Propiedades calculadas
        [NotMapped]
        public int MesesExperiencia => FechaFin.HasValue 
            ? (int)(FechaFin.Value - FechaInicio).TotalDays / 30
            : (int)(DateTime.Now - FechaInicio).TotalDays / 30;
    }

    /// <summary>
    /// Modelo para las Capacitaciones del Profesor
    /// </summary>
    public class ProfesorCapacitacion
    {
        public int Id { get; set; }
        
        public int ProfesorId { get; set; }
        
        [Required, MaxLength(200)]
        public string NombreCapacitacion { get; set; } = string.Empty;
        
        [MaxLength(200)]
        public string? InstitucionOrganizadora { get; set; }
        
        [MaxLength(50)]
        public string? TipoCapacitacion { get; set; }
        
        [MaxLength(20)]
        public string? Modalidad { get; set; }
        
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        
        public int? HorasCapacitacion { get; set; }
        
        public bool CertificadoObtenido { get; set; } = false;
        
        [Column(TypeName = "decimal(4,2)")]
        public decimal? CalificacionObtenida { get; set; }
        
        [MaxLength(100)]
        public string? AreaConocimiento { get; set; }
        
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        
        // Navegación
        public virtual Profesor Profesor { get; set; } = null!;
    }

    /// <summary>
    /// Modelo para la relación Profesor-Grupo (reemplaza ProfesorMateria)
    /// </summary>
    public class ProfesorGrupo
    {
        public int Id { get; set; }
        
        public int ProfesorId { get; set; }
        public int GrupoId { get; set; } // Relación con GruposEstudiantes existente
        public int PeriodoAcademicoId { get; set; } // Relación con PeriodosAcademicos existente
        public int MateriaId { get; set; } // Relación con Materias existente
        
        public bool EsProfesorPrincipal { get; set; } = true; // Si es el profesor principal o asistente
        
        [MaxLength(50)]
        public string? AulaAsignada { get; set; }
        
        public bool Estado { get; set; } = true; // true = Activo, false = Inactivo
        
        public DateTime FechaAsignacion { get; set; } = DateTime.Now;
        
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        
        [MaxLength(500)]
        public string? Observaciones { get; set; }
        
        // Navegación
        public virtual Profesor Profesor { get; set; } = null!;
        public virtual GrupoEstudiante Grupo { get; set; } = null!;
        public virtual PeriodoAcademico PeriodoAcademico { get; set; } = null!;
        public virtual Materia Materia { get; set; } = null!;
    }

    /// <summary>
    /// Modelo para la relación Profesores-Guía (Muchos a Muchos)
    /// </summary>
    public class ProfesorGuia
    {
        public int Id { get; set; }
        
        public int ProfesorId { get; set; }
        public int GrupoId { get; set; }
        
        public DateTime FechaAsignacion { get; set; } = DateTime.Now;
        
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        
        public bool Estado { get; set; } = true; // true = Activo, false = Inactivo
        
        [MaxLength(500)]
        public string? Observaciones { get; set; }
        
        // Navegación
        public virtual Profesor Profesor { get; set; } = null!;
        public virtual GrupoEstudiante Grupo { get; set; } = null!;
    }
}
