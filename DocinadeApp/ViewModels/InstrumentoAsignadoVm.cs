using System.ComponentModel.DataAnnotations;

namespace RubricasApp.Web.ViewModels
{
    public class InstrumentoAsignadoVm
    {
        public int InstrumentoId { get; set; }
        
        [Display(Name = "Instrumento")]
        public string InstrumentoNombre { get; set; } = string.Empty;
        
        public int? PeriodoAcademicoId { get; set; }
        
        [Display(Name = "Período Académico")]
        public string PeriodoAcademicoNombre { get; set; } = string.Empty;
        
        [Display(Name = "Fecha de Asignación")]
        public DateTime FechaAsignacion { get; set; }
        
        [Display(Name = "Es Obligatorio")]
        public bool EsObligatorio { get; set; }
        
        [Display(Name = "Orden")]
        public int? OrdenPresentacion { get; set; }
    }
}