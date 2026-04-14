namespace RubricasApp.Web.Models.SEA
{
    /// <summary>
    /// Constantes para los componentes de evaluación del sistema SEA del MEP
    /// Basado en: https://registraprofe.com/documentacion/reportes/reporte-por-periodo-sea/
    /// </summary>
    public static class ComponentesSEA
    {
        public const string TRABAJO_COTIDIANO = "TRABAJO_COTIDIANO";
        public const string TAREAS = "TAREAS";
        public const string PRUEBAS = "PRUEBAS";
        public const string ASISTENCIA = "ASISTENCIA";
        public const string PROYECTO = "PROYECTO";

        /// <summary>
        /// Obtiene todos los componentes disponibles
        /// </summary>
        public static List<string> ObtenerTodos()
        {
            return new List<string>
            {
                TRABAJO_COTIDIANO,
                TAREAS,
                PRUEBAS,
                ASISTENCIA,
                PROYECTO
            };
        }

        /// <summary>
        /// Obtiene el nombre amigable para mostrar en UI
        /// </summary>
        public static string ObtenerNombreAmigable(string componente)
        {
            return componente switch
            {
                TRABAJO_COTIDIANO => "Trabajo Cotidiano",
                TAREAS => "Tareas",
                PRUEBAS => "Pruebas",
                ASISTENCIA => "Asistencia",
                PROYECTO => "Proyecto",
                _ => componente
            };
        }

        /// <summary>
        /// Valida si un componente es válido
        /// </summary>
        public static bool EsValido(string componente)
        {
            return ObtenerTodos().Contains(componente);
        }
    }

    /// <summary>
    /// Opciones de redondeo para notas en el reporte SEA
    /// </summary>
    public enum OpcionRedondeoSEA
    {
        /// <summary>
        /// Sin decimales: 85
        /// </summary>
        SinDecimales = 0,

        /// <summary>
        /// Un decimal: 85.5
        /// </summary>
        UnDecimal = 1,

        /// <summary>
        /// Dos decimales: 85.50
        /// </summary>
        DosDecimales = 2
    }

    /// <summary>
    /// Estados generales para estudiantes en reporte SEA
    /// </summary>
    public static class EstadosEstudianteSEA
    {
        public const string EXCELENTE = "EXCELENTE";      // >= 90
        public const string SATISFACTORIO = "SATISFACTORIO"; // 70-89
        public const string EN_RIESGO = "EN_RIESGO";      // 60-69
        public const string CRITICO = "CRITICO";          // < 60
        public const string SIN_DATOS = "SIN_DATOS";      // Sin evaluaciones
    }
}
