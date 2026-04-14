using System.Data;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using RubricasApp.Web.Data;
using RubricasApp.Web.Models;

namespace RubricasApp.Web.Services
{
    public class EstudianteImportService : IEstudianteImportService
    {
        private readonly RubricasDbContext _context;
        private readonly ILogger<EstudianteImportService> _logger;

        public EstudianteImportService(RubricasDbContext context, ILogger<EstudianteImportService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ImportResult> ImportarEstudiantesAsync(IFormFile archivo, int periodoAcademicoId)
        {
            var resultado = new ImportResult();
            
            try
            {
                // Configurar licencia de EPPlus 8+
                ExcelPackage.License.SetNonCommercialPersonal("RubricasApp");
                
                using var stream = new MemoryStream();
                await archivo.CopyToAsync(stream);
                
                using var package = new ExcelPackage(stream);
                var worksheet = package.Workbook.Worksheets[0];
                
                var rowCount = worksheet.Dimension.Rows;
                var colCount = worksheet.Dimension.Columns;
                
                // Verificar que el archivo tenga al menos las columnas necesarias
                if (colCount < 6)
                {
                    resultado.Errores.Add("El archivo debe tener al menos 6 columnas: Nombre, Apellido(s), Número de ID, Dirección de correo, Institución, Grupos");
                    return resultado;
                }
                
                // Obtener el año del período académico
                var periodo = await _context.PeriodosAcademicos.FindAsync(periodoAcademicoId);
                if (periodo == null)
                {
                    resultado.Errores.Add("Período académico no encontrado");
                    return resultado;
                }
                
                var estudiantes = new List<Estudiante>();
                
                // Leer datos desde la fila 2 (asumiendo que la fila 1 son encabezados)
                for (int row = 2; row <= rowCount; row++)
                {
                    try
                    {
                        var nombre = worksheet.Cells[row, 1].Text?.Trim();
                        var apellidos = worksheet.Cells[row, 2].Text?.Trim();
                        var numeroId = worksheet.Cells[row, 3].Text?.Trim();
                        var direccionCorreo = worksheet.Cells[row, 4].Text?.Trim();
                        var institucion = worksheet.Cells[row, 5].Text?.Trim();
                        var grupos = worksheet.Cells[row, 6].Text?.Trim();
                        
                        // Validar campos obligatorios
                        if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(apellidos) || 
                            string.IsNullOrEmpty(numeroId) || string.IsNullOrEmpty(direccionCorreo) || 
                            string.IsNullOrEmpty(institucion))
                        {
                            resultado.Errores.Add($"Fila {row}: Faltan campos obligatorios");
                            continue;
                        }
                        
                        // Validar email
                        if (!IsValidEmail(direccionCorreo))
                        {
                            resultado.Errores.Add($"Fila {row}: Email inválido - {direccionCorreo}");
                            continue;
                        }
                        
                        // Verificar si el estudiante ya existe por número de ID
                        var existeEstudiante = await _context.Estudiantes
                            .AnyAsync(e => e.NumeroId == numeroId);
                            
                        if (existeEstudiante)
                        {
                            resultado.Advertencias.Add($"Fila {row}: Estudiante con ID {numeroId} ya existe, se omite");
                            continue;
                        }
                        
                        var estudiante = new Estudiante
                        {
                            Nombre = nombre,
                            Apellidos = apellidos,
                            NumeroId = numeroId,
                            DireccionCorreo = direccionCorreo,
                            Institucion = institucion,
                            Grupos = grupos,
                            Anio = periodo.Anio,
                            PeriodoAcademicoId = periodoAcademicoId
                        };
                        
                        estudiantes.Add(estudiante);
                    }
                    catch (Exception ex)
                    {
                        resultado.Errores.Add($"Fila {row}: Error al procesar - {ex.Message}");
                        _logger.LogError(ex, "Error procesando fila {Row}", row);
                    }
                }
                
                // Guardar estudiantes
                if (estudiantes.Any())
                {
                    _context.Estudiantes.AddRange(estudiantes);
                    await _context.SaveChangesAsync();
                    resultado.EstudiantesImportados = estudiantes.Count;
                }
                
                resultado.Exitoso = true;
            }
            catch (Exception ex)
            {
                resultado.Errores.Add($"Error general al importar: {ex.Message}");
                _logger.LogError(ex, "Error general al importar estudiantes");
            }
            
            return resultado;
        }
        
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
    
    public class ImportResult
    {
        public bool Exitoso { get; set; }
        public int EstudiantesImportados { get; set; }
        public List<string> Errores { get; set; } = new List<string>();
        public List<string> Advertencias { get; set; } = new List<string>();
    }
}