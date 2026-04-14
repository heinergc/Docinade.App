namespace RubricasApp.Web.ViewModels
{
    /// <summary>
    /// ViewModel para la generación del Curriculum Vitae de profesores
    /// </summary>
    public class ProfesorCurriculumViewModel
    {
        // Información Personal
        public string NombreCompleto { get; set; } = string.Empty;
        public string Cedula { get; set; } = string.Empty;
        public string TipoCedula { get; set; } = string.Empty;
        public string? Sexo { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public int? Edad { get; set; }
        public string? EstadoCivil { get; set; }
        public string Nacionalidad { get; set; } = string.Empty;
        
        // Contacto
        public string EmailPersonal { get; set; } = string.Empty;
        public string? EmailInstitucional { get; set; }
        public string TelefonoCelular { get; set; } = string.Empty;
        public string? TelefonoFijo { get; set; }
        public string? TelefonoOficina { get; set; }
        
        // Dirección
        public string? DireccionExacta { get; set; }
        public string? ProvinciaNombre { get; set; }
        public string? CantonNombre { get; set; }
        public string? DistritoNombre { get; set; }
        public string? CodigoPostal { get; set; }
        
        // Información Académica
        public string? GradoAcademico { get; set; }
        public string? TituloAcademico { get; set; }
        public string? InstitucionGraduacion { get; set; }
        public string? PaisGraduacion { get; set; }
        public int? AnioGraduacion { get; set; }
        public string? NumeroColegiadoProfesional { get; set; }
        
        // Información Laboral
        public string? EscuelaNombre { get; set; }
        public string? FacultadNombre { get; set; }
        public string? InstitucionNombre { get; set; }
        public string? CodigoEmpleado { get; set; }
        public DateTime FechaIngreso { get; set; }
        public DateTime? FechaRetiro { get; set; }
        public string TipoContrato { get; set; } = string.Empty;
        public string? RegimenLaboral { get; set; }
        public string? CategoriaLaboral { get; set; }
        public string TipoJornada { get; set; } = string.Empty;
        public decimal? HorasLaborales { get; set; }
        public int AniosServicio { get; set; }
        
        // Cargos y Responsabilidades
        public bool EsDirector { get; set; }
        public bool EsCoordinador { get; set; }
        public bool EsDecano { get; set; }
        public string? CargoAdministrativo { get; set; }
        public DateTime? FechaInicioCargoAdmin { get; set; }
        
        // Información Adicional
        public string? AreasEspecializacion { get; set; }
        public string? IdiomasHabla { get; set; }
        public string? NivelIngles { get; set; }
        public int? ExperienciaDocente { get; set; }
        
        // Contacto de Emergencia
        public string? ContactoEmergenciaNombre { get; set; }
        public string? ContactoEmergenciaParentesco { get; set; }
        public string? ContactoEmergenciaTelefono { get; set; }
        
        // Listas de datos relacionados
        public List<FormacionAcademicaItem> FormacionAcademica { get; set; } = new();
        public List<ExperienciaLaboralItem> ExperienciaLaboral { get; set; } = new();
        public List<CapacitacionItem> Capacitaciones { get; set; } = new();
        public List<GrupoAsignadoItem> GruposAsignados { get; set; } = new();
        
        // Fecha de generación
        public DateTime FechaGeneracion { get; set; } = DateTime.Now;
    }
    
    public class FormacionAcademicaItem
    {
        public string TipoFormacion { get; set; } = string.Empty;
        public string TituloObtenido { get; set; } = string.Empty;
        public string InstitucionEducativa { get; set; } = string.Empty;
        public string PaisInstitucion { get; set; } = string.Empty;
        public int? AnioInicio { get; set; }
        public int? AnioFinalizacion { get; set; }
        public bool EnCurso { get; set; }
        public decimal? PromedioGeneral { get; set; }
        public bool EsTituloReconocidoCONARE { get; set; }
        public string? NumeroReconocimiento { get; set; }
    }
    
    public class ExperienciaLaboralItem
    {
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
    
    public class CapacitacionItem
    {
        public string NombreCapacitacion { get; set; } = string.Empty;
        public string? InstitucionOrganizadora { get; set; }
        public string? TipoCapacitacion { get; set; }
        public string? Modalidad { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int? HorasCapacitacion { get; set; }
        public bool CertificadoObtenido { get; set; }
        public string? AreaConocimiento { get; set; }
    }
    
    public class GrupoAsignadoItem
    {
        public string NombreGrupo { get; set; } = string.Empty;
        public string NombreMateria { get; set; } = string.Empty;
        public string PeriodoAcademico { get; set; } = string.Empty;
        public bool EsProfesorPrincipal { get; set; }
        public DateTime FechaAsignacion { get; set; }
        public bool Estado { get; set; }
    }
}
