using System.ComponentModel.DataAnnotations;

namespace DocinadeApp.DTOs.Calificador
{
    /// <summary>
    /// DTO que representa una fila del cuaderno calificador para un estudiante
    /// </summary>
    public class CalificadorRowDto
    {
        public int EstudianteId { get; set; }
        
        [Display(Name = "Estudiante")]
        public string EstudianteNombre { get; set; } = string.Empty;
        
        [Display(Name = "N�mero ID")]
        public string NumeroId { get; set; } = string.Empty;
        
        /// <summary>
        /// Diccionario din�mico que contiene las calificaciones por cada combinaci�n Instrumento-R�brica
        /// Key: "InstrumentoId_RubricaId" (ej: "1_3")
        /// Value: Calificaci�n obtenida (0-100)
        /// </summary>
        public Dictionary<string, decimal> CalificacionesPorInstrumentoRubrica { get; set; } = new();
        
        /// <summary>
        /// Diccionario con las calificaciones agregadas por instrumento
        /// Key: InstrumentoId como string
        /// Value: Calificaci�n promedio/total del instrumento
        /// </summary>
        public Dictionary<string, decimal> CalificacionesPorInstrumento { get; set; } = new();
        
        [Display(Name = "Total Final")]
        [DisplayFormat(DataFormatString = "{0:F2}")]
        public decimal TotalFinal { get; set; }
        
        /// <summary>
        /// Estado del estudiante en la materia
        /// </summary>
        public string Estado { get; set; } = "ACTIVO";
    }
    
    /// <summary>
    /// Metadatos de una columna del cuaderno calificador
    /// </summary>
    public class CalificadorColumnDto
    {
        public int InstrumentoId { get; set; }
        public string InstrumentoNombre { get; set; } = string.Empty;
        public int RubricaId { get; set; }
        public string RubricaNombre { get; set; } = string.Empty;
        public decimal Ponderacion { get; set; }
        public int OrdenPresentacion { get; set; }
        
        /// <summary>
        /// Clave �nica para identificar la columna: "InstrumentoId_RubricaId"
        /// </summary>
        public string ClaveColumna => $"{InstrumentoId}_{RubricaId}";
        
        /// <summary>
        /// Nombre de la columna para mostrar en el UI
        /// </summary>
        public string NombreColumna => $"{InstrumentoNombre} ? {RubricaNombre}";
    }
    
    /// <summary>
    /// DTO principal que contiene todo el cuaderno calificador
    /// </summary>
    public class CuadernoCalificadorDto
    {
        public int MateriaId { get; set; }
        public string MateriaNombre { get; set; } = string.Empty;
        public int PeriodoAcademicoId { get; set; }
        public string PeriodoAcademicoNombre { get; set; } = string.Empty;
        
        /// <summary>
        /// Lista de columnas din�micas (Instrumento ? R�brica)
        /// </summary>
        public List<CalificadorColumnDto> Columnas { get; set; } = new();
        
        /// <summary>
        /// Filas del cuaderno (una por estudiante)
        /// </summary>
        public List<CalificadorRowDto> Filas { get; set; } = new();
        
        /// <summary>
        /// Estad�sticas del cuaderno
        /// </summary>
        public CalificadorEstadisticasDto Estadisticas { get; set; } = new();
        
        /// <summary>
        /// Fecha de generaci�n del cuaderno
        /// </summary>
        public DateTime FechaGeneracion { get; set; } = DateTime.Now;
    }
    
    /// <summary>
    /// Estad�sticas generales del cuaderno calificador
    /// </summary>
    public class CalificadorEstadisticasDto
    {
        public int TotalEstudiantes { get; set; }
        public int TotalInstrumentos { get; set; }
        public int TotalRubricas { get; set; }
        public decimal PromedioGeneral { get; set; }
        public decimal NotaMaxima { get; set; }
        public decimal NotaMinima { get; set; }
        public int EstudiantesConTodasLasNotas { get; set; }
        public int EstudiantesConNotasPendientes { get; set; }
    }
    
    /// <summary>
    /// Par�metros de consulta para generar el cuaderno calificador
    /// </summary>
    public class CalificadorQueryDto
    {
        [Required]
        public int MateriaId { get; set; }
        
        [Required]
        public int PeriodoAcademicoId { get; set; }
        
        /// <summary>
        /// Incluir estudiantes inactivos o dados de baja
        /// </summary>
        public bool IncluirInactivos { get; set; } = false;
        
        /// <summary>
        /// Formato de c�lculo para m�ltiples r�bricas por instrumento
        /// PROMEDIO, SUMA, MEJOR_NOTA
        /// </summary>
        public string ModoCalculo { get; set; } = "PROMEDIO";
        
        /// <summary>
        /// Valor por defecto para calificaciones faltantes
        /// </summary>
        public decimal ValorPorDefecto { get; set; } = 0m;
    }
}