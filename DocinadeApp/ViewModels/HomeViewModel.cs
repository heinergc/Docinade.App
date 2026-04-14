namespace DocinadeApp.ViewModels
{
    public class HomeIndexViewModel
    {
        public bool EmpadronamientoPublicoHabilitado { get; set; }
        
        // Estadísticas del sistema
        public int RubricasCreadas { get; set; }
        public int EvaluacionesCompletadas { get; set; }
        public int EstudiantesEvaluados { get; set; }
        public int ReportesGenerados { get; set; }
    }
}
