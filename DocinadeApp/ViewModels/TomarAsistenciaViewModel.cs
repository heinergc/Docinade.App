using DocinadeApp.Models;
using System.ComponentModel.DataAnnotations;

namespace DocinadeApp.ViewModels
{
    /// <summary>
    /// ViewModel para tomar asistencia por lección (nuevo método)
    /// </summary>
    public class TomarAsistenciaViewModel
    {
        // Propiedades para el método nuevo (por lección)
        public int IdLeccion { get; set; }
        public Leccion? Leccion { get; set; }
        
        [Required(ErrorMessage = "La fecha es requerida")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha")]
        public DateTime FechaAsistencia { get; set; }
        
        public List<RegistroAsistenciaViewModel> RegistrosAsistencia { get; set; } = new List<RegistroAsistenciaViewModel>();
        
        // Propiedades para compatibilidad con AsistenciasController legacy (por grupo/materia)
        public int? GrupoId { get; set; }
        public int? MateriaId { get; set; }
        
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }
        
        public string? NombreGrupo { get; set; }
        public string? NombreMateria { get; set; }
        
        public List<EstudianteAsistenciaViewModel> Estudiantes { get; set; } = new List<EstudianteAsistenciaViewModel>();
    }

    public class RegistroAsistenciaViewModel
    {
        public int AsistenciaId { get; set; }
        public int EstudianteId { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string? Identificacion { get; set; }
        
        [Required(ErrorMessage = "El estado es requerido")]
        public string Estado { get; set; } = "N";
        
        public string? Observaciones { get; set; }
        public string? Justificacion { get; set; }
    }

    public class HistorialAsistenciaViewModel
    {
        public int EstudianteId { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public List<Asistencia> Asistencias { get; set; } = new List<Asistencia>();
        public Dictionary<string, int> Estadisticas { get; set; } = new Dictionary<string, int>();
        public decimal PorcentajeAsistencia { get; set; }
    }

    public class EstadisticasAsistenciaViewModel
    {
        public int? EstudianteId { get; set; }
        public int? GrupoId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public Dictionary<string, int> Estadisticas { get; set; } = new Dictionary<string, int>();
        public decimal PorcentajeAsistencia { get; set; }
        public List<Asistencia> Ausencias { get; set; } = new List<Asistencia>();
        public Dictionary<int, Dictionary<string, int>>? ResumenGrupo { get; set; }
    }
}