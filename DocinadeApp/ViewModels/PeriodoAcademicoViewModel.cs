using DocinadeApp.Models;
using System.ComponentModel.DataAnnotations;

namespace DocinadeApp.ViewModels
{
    public class SeleccionarPeriodoViewModel
    {
        [Required(ErrorMessage = "Debe seleccionar un período académico")]
        [Display(Name = "Período Académico")]
        public int? PeriodoSeleccionadoId { get; set; }

        public IEnumerable<PeriodoAcademico> PeriodosDisponibles { get; set; } = new List<PeriodoAcademico>();

        public int? PeriodoActivoId { get; set; }

        public bool MostrarMensajeBienvenida { get; set; } = false;

        public string? UrlRetorno { get; set; }

        public string MensajeBienvenida => MostrarMensajeBienvenida 
            ? "¡Bienvenido! Seleccione el período académico con el que desea trabajar."
            : "Cambiar período académico de trabajo";

        public bool TienePeriodosDisponibles => PeriodosDisponibles.Any();
    }

    public class PeriodoAcademicoSelectorViewModel
    {
        public PeriodoAcademico? PeriodoActivo { get; set; }
        public IEnumerable<PeriodoAcademico> PeriodosDisponibles { get; set; } = new List<PeriodoAcademico>();
        public bool MostrarIndicador { get; set; } = true;
        public string CssClass { get; set; } = "";
        public bool MostrarTexto { get; set; } = true;
    }
}