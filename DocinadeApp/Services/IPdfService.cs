namespace DocinadeApp.Services
{
    /// <summary>
    /// Interfaz para el servicio de generación de PDFs desde HTML
    /// </summary>
    public interface IPdfService
    {
        /// <summary>
        /// Genera un PDF a partir de contenido HTML
        /// </summary>
        /// <param name="htmlContent">Contenido HTML completo</param>
        /// <param name="orientation">Orientación del documento (Portrait/Landscape)</param>
        /// <returns>Bytes del archivo PDF generado</returns>
        byte[] GeneratePdfFromHtml(string htmlContent, string orientation = "Portrait");
        
        /// <summary>
        /// Genera un PDF a partir de una vista Razor
        /// </summary>
        /// <param name="viewName">Nombre de la vista</param>
        /// <param name="model">Modelo para la vista</param>
        /// <param name="orientation">Orientación del documento</param>
        /// <returns>Bytes del archivo PDF generado</returns>
        Task<byte[]> GeneratePdfFromViewAsync(string viewName, object model, string orientation = "Portrait");
    }
}
