using System.ComponentModel.DataAnnotations;

namespace RubricasApp.Web.Models.ViewModels
{
    public class AsignarInstrumentosVm
    {
        [Required]
        public int MateriaId { get; set; }
        
        public string NombreMateria { get; set; } = string.Empty;
        
        [Required]
        public List<int> InstrumentoIds { get; set; } = new List<int>();
        
        public List<AsignacionInstrumentoVm> InstrumentosDisponibles { get; set; } = new List<AsignacionInstrumentoVm>();
        
        public string? Observaciones { get; set; }
    }
}