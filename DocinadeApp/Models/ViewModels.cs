using RubricasApp.Web.Models;

namespace RubricasApp.Web.Models
{
    // ViewModels para la configuración masiva
    public class ConfigurarRubricaViewModel
    {
        public Rubrica Rubrica { get; set; } = null!;
        public List<NivelCalificacion> Niveles { get; set; } = new();
        public List<ValorRubrica> ValoresExistentes { get; set; } = new();
    }

    public class ValorRubricaConfiguracion
    {
        public int IdItem { get; set; }
        public int IdNivel { get; set; }
        public decimal? ValorPuntos { get; set; }
    }
}