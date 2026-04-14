using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace DocinadeApp.ViewModels
{
    public class AsignarInstrumentosVm
    {
        public int MateriaId { get; set; }
        public string MateriaNombre { get; set; } = string.Empty;
        
        [Display(Name = "Instrumentos Disponibles")]
        public List<SelectListItem> InstrumentosDisponibles { get; set; } = new List<SelectListItem>();
        
        public List<InstrumentoAsignadoVm> InstrumentosAsignados { get; set; } = new List<InstrumentoAsignadoVm>();
        
        // Para la asignación
        [Required(ErrorMessage = "Debe seleccionar un instrumento")]
        [Display(Name = "Instrumento")]
        public int? InstrumentoIdSeleccionado { get; set; }
        
        [Display(Name = "Período Académico")]
        public int? PeriodoAcademicoIdSeleccionado { get; set; }
        
        [Display(Name = "Es Obligatorio")]
        public bool EsObligatorio { get; set; } = false;
    }
}