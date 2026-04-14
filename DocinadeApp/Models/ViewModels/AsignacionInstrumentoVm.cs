using System.ComponentModel.DataAnnotations;

namespace RubricasApp.Web.Models.ViewModels
{
    public class AsignacionInstrumentoVm
    {
        [Required]
        public int InstrumentoEvaluacionId { get; set; }
        
        public string NombreInstrumento { get; set; } = string.Empty;
        
        public string? Descripcion { get; set; }
        
        public DateTime FechaAsignacion { get; set; } = DateTime.Now;
        
        public bool EsObligatorio { get; set; }
        
        public bool Seleccionado { get; set; }
    }
}