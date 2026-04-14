namespace DocinadeApp.Models
{
    /// <summary>
    /// Tipos de período académico
    /// </summary>
    public enum TipoPeriodo
    {
        /// <summary>
        /// Período semestral (6 meses)
        /// </summary>
        Semestre = 1,

        /// <summary>
        /// Período cuatrimestral (4 meses)
        /// </summary>
        Cuatrimestre = 2,

        /// <summary>
        /// Período trimestral (3 meses)
        /// </summary>
        Trimestre = 3,

        /// <summary>
        /// Período anual (12 meses)
        /// </summary>
        Anual = 4,

        /// <summary>
        /// Período personalizado
        /// </summary>
        Personalizado = 5
    }
}