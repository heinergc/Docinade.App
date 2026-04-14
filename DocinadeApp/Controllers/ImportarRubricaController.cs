using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using RubricasApp.Web.Data;
using RubricasApp.Web.Models;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;

namespace RubricasApp.Web.Controllers
{
    [Authorize]
    public class ImportarRubricaController : Controller
    {
        private readonly RubricasDbContext _context;
        private readonly ILogger<ImportarRubricaController> _logger;

        public ImportarRubricaController(RubricasDbContext context, ILogger<ImportarRubricaController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: ImportarRubrica
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// DIAGNOSTIC METHOD: Check database schema for ValoresRubrica table
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> DiagnosticarEsquema()
        {
            try
            {
                var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();
                
                using var command = connection.CreateCommand();
                // Usar consulta compatible con SQL Server
                command.CommandText = @"
                    SELECT 
                        COLUMN_NAME as name,
                        DATA_TYPE as type,
                        IS_NULLABLE as nullable,
                        COLUMN_DEFAULT as dflt_value
                    FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_NAME = 'ValoresRubrica'
                    ORDER BY ORDINAL_POSITION";
                
                var results = new List<object>();
                using var reader = await command.ExecuteReaderAsync();
                
                while (await reader.ReadAsync())
                {
                    results.Add(new
                    {
                        name = reader.GetString(0),
                        type = reader.GetString(1),
                        nullable = reader.GetString(2),
                        dflt_value = reader.IsDBNull(3) ? null : reader.GetValue(3)
                    });
                }

                return Json(new { 
                    success = true, 
                    schema = results,
                    message = "Esquema de la tabla ValoresRubrica en SQL Server"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al diagnosticar esquema");
                return Json(new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// DIAGNOSTIC METHOD: Test ValorRubrica creation with validation
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> TestCrearValor([FromForm] int rubricaId = 0, [FromForm] decimal valorPuntos = 0)
        {
            try
            {
                // Get or create test data
                var rubrica = await _context.Rubricas.FirstOrDefaultAsync();
                if (rubrica == null)
                {
                    rubrica = new Rubrica
                    {
                        NombreRubrica = "Test Diagnóstico",
                        Descripcion = "Rúbrica de prueba",
                        Estado = "ACTIVO",
                        FechaCreacion = DateTime.Now,
                        EsPublica = 1
                    };
                    _context.Rubricas.Add(rubrica);
                    await _context.SaveChangesAsync();
                }

                var item = await _context.ItemsEvaluacion.FirstOrDefaultAsync(i => i.IdRubrica == rubrica.IdRubrica);
                if (item == null)
                {
                    item = new ItemEvaluacion
                    {
                        IdRubrica = rubrica.IdRubrica,
                        NombreItem = "Test Item",
                        OrdenItem = 1,
                        Peso = 1.0m
                    };
                    _context.ItemsEvaluacion.Add(item);
                    await _context.SaveChangesAsync();
                }

                var nivel = await _context.NivelesCalificacion.FirstOrDefaultAsync();
                if (nivel == null)
                {
                    nivel = new NivelCalificacion
                    {
                        NombreNivel = "Test Nivel",
                        Descripcion = "Nivel de prueba"
                    };
                    _context.NivelesCalificacion.Add(nivel);
                    await _context.SaveChangesAsync();
                }

                // Test crear ValorRubrica con different scenarios
                var testCases = new[]
                {
                    new { Name = "Valor 0", Value = 0m },
                    new { Name = "Valor 5.5", Value = 5.5m },
                    new { Name = "Valor del formulario", Value = valorPuntos }
                };

                var results = new List<object>();

                foreach (var testCase in testCases)
                {
                    try
                    {
                        _logger.LogInformation("🧪 Probando crear ValorRubrica: {TestCase} = {Value}", testCase.Name, testCase.Value);

                        var valor = new ValorRubrica
                        {
                            IdRubrica = rubrica.IdRubrica,
                            IdItem = item.IdItem,
                            IdNivel = nivel.IdNivel,
                            ValorPuntos = testCase.Value
                        };

                        // VALIDATE BEFORE SAVING
                        _logger.LogInformation("📊 Propiedades antes de guardar:");
                        _logger.LogInformation("  - IdRubrica: {IdRubrica}", valor.IdRubrica);
                        _logger.LogInformation("  - IdItem: {IdItem}", valor.IdItem);
                        _logger.LogInformation("  - IdNivel: {IdNivel}", valor.IdNivel);
                        _logger.LogInformation("  - ValorPuntos: {ValorPuntos} (Type: {Type})", valor.ValorPuntos, valor.ValorPuntos.GetType().Name);

                        // Use DbContext validation
                        var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
                        var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(valor);
                        bool isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(valor, validationContext, validationResults, true);

                        if (!isValid)
                        {
                            results.Add(new {
                                testCase = testCase.Name,
                                success = false,
                                error = "Validation failed",
                                validationErrors = validationResults.Select(vr => vr.ErrorMessage)
                            });
                            continue;
                        }

                        _context.ValoresRubrica.Add(valor);
                        await _context.SaveChangesAsync();

                        results.Add(new {
                            testCase = testCase.Name,
                            success = true,
                            valorId = valor.IdValor,
                            valorPuntos = valor.ValorPuntos
                        });

                        _logger.LogInformation("✅ Éxito: ValorRubrica creado con ID {IdValor}", valor.IdValor);

                        // Clean up for next test
                        _context.ValoresRubrica.Remove(valor);
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "❌ Error en test case {TestCase}: {Message}", testCase.Name, ex.Message);
                        
                        results.Add(new {
                            testCase = testCase.Name,
                            success = false,
                            error = ex.Message,
                            innerError = ex.InnerException?.Message,
                            stackTrace = ex.StackTrace
                        });
                    }
                }

                return Json(new {
                    success = true,
                    message = "Tests de ValorRubrica completados",
                    testResults = results,
                    testData = new {
                        rubricaId = rubrica.IdRubrica,
                        itemId = item.IdItem,
                        nivelId = nivel.IdNivel
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en test general");
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportarExcel(IFormFile archivo, string nombreRubrica, string descripcionRubrica)
        {
            if (archivo == null || archivo.Length == 0)
            {
                TempData["ErrorMessage"] = "Por favor seleccione un archivo Excel.";
                return View("Index");
            }

            if (string.IsNullOrWhiteSpace(nombreRubrica))
            {
                TempData["ErrorMessage"] = "Por favor ingrese un nombre para la rúbrica.";
                return View("Index");
            }

            // Limpiar la descripción si viene vacía
            if (string.IsNullOrWhiteSpace(descripcionRubrica))
            {
                descripcionRubrica = null;
            }

            try
            {
                using var stream = new MemoryStream();
                await archivo.CopyToAsync(stream);

                // Configurar licencia de EPPlus 8+
                ExcelPackage.License.SetNonCommercialPersonal("RubricasApp");
                using var package = new ExcelPackage(stream);

                if (package.Workbook.Worksheets.Count == 0)
                {
                    TempData["ErrorMessage"] = "El archivo Excel no contiene hojas de trabajo.";
                    return View("Index");
                }

                var worksheet = package.Workbook.Worksheets[0];
                var resultado = await ProcesarExcel(worksheet, nombreRubrica, descripcionRubrica);

                if (resultado.Exito)
                {
                    TempData["SuccessMessage"] = resultado.Mensaje;
                    return RedirectToAction("Details", "Rubricas", new { id = resultado.RubricaId });
                }
                else
                {
                    TempData["ErrorMessage"] = resultado.Mensaje;
                    return View("Index");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar archivo de importación");
                TempData["ErrorMessage"] = $"Error al procesar el archivo: {ex.Message}";
                if (ex.InnerException != null)
                {
                    TempData["ErrorMessage"] += $" Detalle: {ex.InnerException.Message}";
                }
                return View("Index");
            }
        }

        /// <summary>
        /// Trunca un texto a la longitud especificada agregando "..." si es necesario
        /// </summary>
        private string TruncarTexto(string texto, int longitudMaxima)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return string.Empty;

            if (texto.Length <= longitudMaxima)
                return texto;

            // Truncar y agregar puntos suspensivos
            return texto.Substring(0, longitudMaxima - 3) + "...";
        }

        /// <summary>
        /// Normaliza un nombre eliminando tildes, convirtiendo a minúsculas y eliminando espacios extra
        /// </summary>
        private string NormalizarNombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return string.Empty;

            // Convertir a minúsculas y eliminar espacios al inicio y final
            string nombreNormalizado = nombre.Trim().ToLowerInvariant();

            // Eliminar acentos/tildes usando normalización Unicode
            string normalizado = nombreNormalizado.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (char c in normalizado)
            {
                UnicodeCategory categoria = CharUnicodeInfo.GetUnicodeCategory(c);
                if (categoria != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        /// <summary>
        /// Busca un nivel existente por nombre normalizado
        /// </summary>
        private async Task<NivelCalificacion?> BuscarNivelExistente(string nombreNivel)
        {
            if (string.IsNullOrWhiteSpace(nombreNivel))
                return null;

            string nombreNormalizado = NormalizarNombre(nombreNivel);

            // Obtener todos los niveles y buscar por nombre normalizado
            var nivelesExistentes = await _context.NivelesCalificacion.ToListAsync();

            foreach (var nivel in nivelesExistentes)
            {
                if (NormalizarNombre(nivel.NombreNivel) == nombreNormalizado)
                {
                    _logger.LogDebug("Nivel existente encontrado: '{NivelOriginal}' -> '{NivelExistente}' (ID: {Id})", 
                                   nombreNivel, nivel.NombreNivel, nivel.IdNivel);
                    return nivel;
                }
            }

            return null;
        }

        private async Task<ResultadoImportacion> ProcesarExcel(ExcelWorksheet worksheet, string nombreRubrica, string descripcionRubrica)
        {
            try
            {
                _logger.LogInformation("Iniciando procesamiento de Excel para rúbrica: {NombreRubrica}", nombreRubrica);

                // Validar estructura del archivo
                var validacion = ValidarEstructuraExcel(worksheet);
                if (!validacion.EsValida)
                {
                    _logger.LogWarning("Validación de Excel falló: {Error}", validacion.MensajeError);
                    return new ResultadoImportacion { Exito = false, Mensaje = validacion.MensajeError };
                }

                // Verificar que no exista una rúbrica con el mismo nombre
                var rubricaExistente = await _context.Rubricas
                    .FirstOrDefaultAsync(r => r.NombreRubrica.ToLower() == nombreRubrica.ToLower());

                if (rubricaExistente != null)
                {
                    return new ResultadoImportacion
                    {
                        Exito = false,
                        Mensaje = $"Ya existe una rúbrica con el nombre '{nombreRubrica}'"
                    };
                }

                _logger.LogInformation("Creando rúbrica completa con niveles y valores...");

                // Usar estrategia de ejecución de SQL Server para manejar transacciones
                var strategy = _context.Database.CreateExecutionStrategy();
                
                return await strategy.ExecuteAsync(async () =>
                {
                    // Usar transacción dentro de la estrategia de ejecución
                    using var transaction = await _context.Database.BeginTransactionAsync();

                    try
                    {
                        // ===== PASO 1: Crear y guardar la rúbrica PRIMERO =====
                        var rubrica = new Rubrica
                        {
                            NombreRubrica = nombreRubrica,
                            Descripcion = descripcionRubrica, // Ya viene limpio del método ImportarExcel
                            Estado = "ACTIVO",
                            FechaCreacion = DateTime.Now,
                            EsPublica = 1, // Para SQL Server: true → 1
                            Titulo = nombreRubrica, // Agregar propiedad Titulo requerida
                            Vigente = true, // Agregar propiedad Vigente requerida
                            CreadoPorId = null,
                            ModificadoPorId = null
                        };

                        _context.Rubricas.Add(rubrica);
                        
                        _logger.LogDebug("Rubrica EsPublica value before save: {EsPublica}", rubrica.EsPublica);
                        
                        await _context.SaveChangesAsync(); // CRÍTICO: Guardar para obtener el IdRubrica
                        _logger.LogInformation("Rúbrica creada con ID: {IdRubrica}", rubrica.IdRubrica);

                        // ===== PASO 2: Procesar y crear niveles de calificación =====
                        var nivelesCalificacionFinales = new List<NivelCalificacion>();
                        var nivelesNombres = new HashSet<string>();

                        // Buscar índice de columna "Descripción" (case-insensitive) ANTES de procesar niveles
                        int indiceColumnaDescripcion = -1;
                        for (int i = 0; i < validacion.Encabezados.Count; i++)
                        {
                            if (validacion.Encabezados[i].Trim().ToLower() == "descripción" ||
                                validacion.Encabezados[i].Trim().ToLower() == "descripcion")
                            {
                                indiceColumnaDescripcion = i; // Índice 0-based para excluir de niveles
                                _logger.LogInformation("✅ Columna 'Descripción' encontrada en índice {Indice} (0-based)", indiceColumnaDescripcion);
                                break;
                            }
                        }

                        // Extraer nombres de niveles únicos de los encabezados (columna 2 en adelante)
                        // EXCLUYENDO la columna "Descripción" si existe
                        for (int col = 2; col <= validacion.Encabezados.Count; col++)
                        {
                            var indiceEncabezado = col - 1; // Convertir a índice 0-based

                            // SALTAR la columna "Descripción" si existe
                            if (indiceColumnaDescripcion >= 0 && indiceEncabezado == indiceColumnaDescripcion)
                            {
                                _logger.LogDebug("⏭️ Saltando columna 'Descripción' en índice {Indice}", indiceEncabezado);
                                continue;
                            }

                            var nivelNombre = validacion.Encabezados[indiceEncabezado];
                            if (!string.IsNullOrWhiteSpace(nivelNombre) && !nivelesNombres.Contains(nivelNombre))
                            {
                                nivelesNombres.Add(nivelNombre);
                                _logger.LogDebug("📊 Nivel de calificación identificado: '{NivelNombre}'", nivelNombre);
                            }
                        }

                        // Buscar niveles existentes o crear nuevos automáticamente
                        int ordenNivel = 1;
                        
                        foreach (var nombreNivel in nivelesNombres)
                        {
                            var nivelExistente = await BuscarNivelExistente(nombreNivel);
                            
                            if (nivelExistente != null)
                            {
                                nivelesCalificacionFinales.Add(nivelExistente);
                                _logger.LogInformation("✅ Nivel encontrado: '{NombreNivel}' (ID: {Id})", 
                                                     nombreNivel, nivelExistente.IdNivel);
                            }
                            else
                            {
                                // Crear el nivel automáticamente para no interrumpir el proceso
                                var nuevoNivel = new NivelCalificacion
                                {
                                    NombreNivel = nombreNivel,
                                    Descripcion = $"Nivel {nombreNivel} importado automáticamente desde Excel",
                                    OrdenNivel = ordenNivel
                                };
                                
                                _context.NivelesCalificacion.Add(nuevoNivel);
                                await _context.SaveChangesAsync(); // Guardar para obtener el ID
                                
                                nivelesCalificacionFinales.Add(nuevoNivel);
                                _logger.LogInformation("➕ Nivel CREADO automáticamente: '{NombreNivel}' (ID: {Id})", 
                                                     nombreNivel, nuevoNivel.IdNivel);
                            }
                            
                            ordenNivel++;
                        }

                        // Verificar nivel "No lo hizo" (opcional, crear si no existe)
                        // var nivelNoLoHizo = await BuscarNivelExistente("No lo hizo");
                        // if (nivelNoLoHizo == null)
                        // {
                        //     // Crear "No lo hizo" automáticamente
                        //     nivelNoLoHizo = new NivelCalificacion
                        //     {
                        //         NombreNivel = "No lo hizo",
                        //         Descripcion = "Nivel para estudiantes que no presentaron el trabajo",
                        //         OrdenNivel = ordenNivel
                        //     };
                            
                        //     _context.NivelesCalificacion.Add(nivelNoLoHizo);
                        //     await _context.SaveChangesAsync();
                            
                        //     _logger.LogInformation("➕ Nivel 'No lo hizo' CREADO automáticamente (ID: {Id})", nivelNoLoHizo.IdNivel);
                        // }
                        
                        // if (!nivelesCalificacionFinales.Any(n => n.IdNivel == nivelNoLoHizo.IdNivel))
                        // {
                        //     nivelesCalificacionFinales.Add(nivelNoLoHizo);
                        //     _logger.LogInformation("✅ Nivel 'No lo hizo' agregado (ID: {Id})", nivelNoLoHizo.IdNivel);
                        // }

                        // _logger.LogInformation("✅ Niveles de calificación validados: {Count}", nivelesCalificacionFinales.Count);

                        // ===== PASO 3: Crear items de evaluación =====
                        var itemsEvaluacion = new List<ItemEvaluacion>();
                        var criteriosData = new List<(string nombre, string? descripcion, int fila)>();
                        int ordenItem = 1;

                        // La columna "Descripción" ya fue identificada arriba (indiceColumnaDescripcion)
                        // Convertir a índice 1-based para Excel (si existe)
                        int indiceColumnaDescripcionExcel = indiceColumnaDescripcion >= 0 ? indiceColumnaDescripcion + 1 : -1;

                        // Extraer criterios del Excel
                        for (int fila = validacion.FilaInicioDatos; fila <= worksheet.Dimension.End.Row; fila++)
                        {
                            var criterio = worksheet.Cells[fila, 1].Value?.ToString()?.Trim();

                            if (string.IsNullOrWhiteSpace(criterio) || criterio.ToUpper() == "TOTAL")
                                continue;

                            // Extraer descripción si existe la columna
                            string? descripcionItem = null;
                            if (indiceColumnaDescripcionExcel > 0 && indiceColumnaDescripcionExcel <= worksheet.Dimension.End.Column)
                            {
                                var valorDescripcion = worksheet.Cells[fila, indiceColumnaDescripcionExcel].Value?.ToString()?.Trim();
                                if (!string.IsNullOrWhiteSpace(valorDescripcion))
                                {
                                    descripcionItem = valorDescripcion;
                                    _logger.LogDebug("📝 Descripción encontrada para criterio '{Criterio}': '{Descripcion}'",
                                                   criterio, descripcionItem);
                                }
                            }

                            criteriosData.Add((criterio, descripcionItem, fila));

                            var item = new ItemEvaluacion
                            {
                                IdRubrica = rubrica.IdRubrica,
                                NombreItem = TruncarTexto(criterio, 200),
                                OrdenItem = ordenItem++,
                                Peso = 1.0m,
                                Descripcion = descripcionItem // Solo asignar si existe la columna "Descripción"
                            };

                            _context.ItemsEvaluacion.Add(item);
                            itemsEvaluacion.Add(item);
                        }

                        // Guardar todos los items
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("Items de evaluación creados: {Count}", itemsEvaluacion.Count);

                        // ===== PASO 4: Crear valores de rúbrica =====
                        var valoresRubrica = new List<ValorRubrica>();

                        for (int i = 0; i < itemsEvaluacion.Count; i++)
                        {
                            var item = itemsEvaluacion[i];
                            var (nombreCriterio, descripcionCriterio, filaExcel) = criteriosData[i];

                            _logger.LogDebug("Procesando valores para item '{NombreItem}' (ID: {IdItem}, Fila: {Fila})", 
                                           nombreCriterio, item.IdItem, filaExcel);

                            // Procesar cada columna de niveles (empezando desde columna 2)
                            // SALTAR la columna "Descripción" si existe
                            for (int col = 2; col <= validacion.Encabezados.Count; col++)
                            {
                                var indiceEncabezado = col - 1; // Convertir a índice 0-based

                                // SALTAR la columna "Descripción" si existe
                                if (indiceColumnaDescripcion >= 0 && indiceEncabezado == indiceColumnaDescripcion)
                                {
                                    _logger.LogDebug("⏭️ Saltando columna 'Descripción' en índice {Indice} al procesar valores", indiceEncabezado);
                                    continue;
                                }

                                var nombreNivel = validacion.Encabezados[indiceEncabezado];
                                var valorTexto = worksheet.Cells[filaExcel, col].Value?.ToString()?.Trim();

                                // Encontrar el nivel correspondiente
                                var nivel = nivelesCalificacionFinales.FirstOrDefault(n => n.NombreNivel == nombreNivel);
                                if (nivel == null)
                                {
                                    // Buscar por nombre normalizado si no se encuentra exacto
                                    var nombreNormalizado = NormalizarNombre(nombreNivel);
                                    nivel = nivelesCalificacionFinales.FirstOrDefault(n => 
                                        NormalizarNombre(n.NombreNivel) == nombreNormalizado);
                                }

                                if (nivel == null)
                                {
                                    _logger.LogWarning("No se encontró el nivel '{NombreNivel}' para el item '{NombreItem}'", 
                                                     nombreNivel, nombreCriterio);
                                    continue;
                                }

                                // Extraer valor numérico con GARANTÍA de no-null
                                decimal puntaje = 0; // ALWAYS initialize to 0
                                if (!string.IsNullOrWhiteSpace(valorTexto))
                                {
                                    var match = Regex.Match(valorTexto, @"([0-9]+(?:[,.]?[0-9]+)?)");
                                    if (match.Success)
                                    {
                                        var valorNumerico = match.Groups[1].Value.Replace(',', '.');
                                        if (decimal.TryParse(valorNumerico, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal temp))
                                        {
                                            puntaje = temp;
                                        }
                                    }
                                }

                                // GARANTIZAR que puntaje nunca sea negativo o fuera de rango
                                puntaje = Math.Max(0, Math.Min(999.99m, puntaje));

                                // VALIDATION LOG antes de crear objeto
                                _logger.LogDebug("📊 Creando ValorRubrica: Rubrica={IdRubrica}, Item={IdItem}, Nivel={IdNivel}, Puntos={Puntos}", 
                                               rubrica.IdRubrica, item.IdItem, nivel.IdNivel, puntaje);

                                var valorRubrica = new ValorRubrica
                                {
                                    IdRubrica = rubrica.IdRubrica,
                                    IdItem = item.IdItem,
                                    IdNivel = nivel.IdNivel,
                                    ValorPuntos = puntaje  // GARANTIZADO: nunca null, siempre >= 0
                                };

                                // FINAL SAFETY CHECK: Si el objeto por alguna razón tiene null, forzar a 0
                                if (valorRubrica.ValorPuntos == default(decimal) && puntaje != 0)
                                {
                                    _logger.LogWarning("⚠️ ValorPuntos se reseteó a default, reasignando: {OriginalValue}", puntaje);
                                    valorRubrica.ValorPuntos = puntaje;
                                }

                                // LOG FINAL antes de agregar al contexto
                                _logger.LogDebug("✅ ValorRubrica final: IdRubrica={IdRubrica}, IdItem={IdItem}, IdNivel={IdNivel}, ValorPuntos={ValorPuntos}", 
                                               valorRubrica.IdRubrica, valorRubrica.IdItem, valorRubrica.IdNivel, valorRubrica.ValorPuntos);

                                _context.ValoresRubrica.Add(valorRubrica);
                                valoresRubrica.Add(valorRubrica);

                                _logger.LogDebug("Valor creado: Item={IdItem}, Nivel={IdNivel} ({NombreNivel}), Puntos={Puntos}", 
                                               item.IdItem, nivel.IdNivel, nivel.NombreNivel, puntaje);
                            }
                        }

                        foreach (var valor in valoresRubrica)
                        {
                            _logger.LogInformation("ValorRubrica creado: Id={IdValor}, Rubrica={IdRubrica}, Item={IdItem}, Nivel={IdNivel}, Puntos={ValorPuntos}", 
                                           valor.IdValor, valor.IdRubrica, valor.IdItem, valor.IdNivel, valor.ValorPuntos);
                        }

                        // Guardar todos los valores
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("Valores de rúbrica creados: {Count}", valoresRubrica.Count);

                        // ===== PASO 5: Crear relaciones RubricaNivel =====
                        var rubricaNiveles = new List<RubricaNivel>();
                        int orden = 1;

                        foreach (var nivel in nivelesCalificacionFinales.OrderBy(n => n.OrdenNivel))
                        {
                            // Verificar si ya existe la relación
                            var relacionExistente = await _context.RubricaNiveles
                                .FirstOrDefaultAsync(rn => rn.IdRubrica == rubrica.IdRubrica && rn.IdNivel == nivel.IdNivel);

                            if (relacionExistente == null)
                            {
                                var rubricaNivel = new RubricaNivel
                                {
                                    IdRubrica = rubrica.IdRubrica,
                                    IdNivel = nivel.IdNivel,
                                    OrdenEnRubrica = orden++
                                };

                                _context.RubricaNiveles.Add(rubricaNivel);
                                rubricaNiveles.Add(rubricaNivel);
                            }
                        }

                        await _context.SaveChangesAsync();
                        _logger.LogInformation("Relaciones RubricaNivel creadas: {Count}", rubricaNiveles.Count);

                        // Confirmar transacción
                        await transaction.CommitAsync();

                        var mensaje = $"Rúbrica '{nombreRubrica}' importada exitosamente:\n" +
                                     $"- {itemsEvaluacion.Count} criterios de evaluación\n" +
                                     $"- {nivelesCalificacionFinales.Count} niveles de calificación\n" +
                                     $"- {valoresRubrica.Count} valores de evaluación\n" +
                                     $"- {rubricaNiveles.Count} relaciones nivel-rúbrica";

                        // Contar criterios truncados y descripciones procesadas
                        var criteriosTruncados = itemsEvaluacion.Count(i => i.NombreItem.Length < (i.Descripcion?.Length ?? 0));
                        var descripcionesAgregadas = itemsEvaluacion.Count(i => !string.IsNullOrEmpty(i.Descripcion));

                        if (criteriosTruncados > 0)
                        {
                            mensaje += $"\n\n⚠️ Nota: {criteriosTruncados} criterio(s) fueron truncados a 200 caracteres.";
                        }

                        if (descripcionesAgregadas > 0)
                        {
                            mensaje += $"\n\n📝 Descripciones: {descripcionesAgregadas} item(s) incluyen descripción adicional.";
                        }
                        else if (indiceColumnaDescripcionExcel > 0)
                        {
                            mensaje += $"\n\n📝 Nota: La columna 'Descripción' estaba presente pero no contenía valores.";
                        }

                        _logger.LogInformation("Importación completada exitosamente para rúbrica ID: {IdRubrica}", rubrica.IdRubrica);

                        return new ResultadoImportacion
                        {
                            Exito = true,
                            RubricaId = rubrica.IdRubrica,
                            Mensaje = mensaje
                        };
                    }
                    catch
                    {
                        // Revertir transacción en caso de error
                        await transaction.RollbackAsync();
                        throw; // Re-lanzar para que sea manejado por la estrategia de ejecución
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante el procesamiento de Excel: {Message}", ex.Message);

                return new ResultadoImportacion
                {
                    Exito = false,
                    Mensaje = $"Error durante la importación: {ex.Message}" +
                             (ex.InnerException != null ? $"\nDetalle: {ex.InnerException.Message}" : "")
                };
            }
        }

        private ValidacionExcel ValidarEstructuraExcel(ExcelWorksheet worksheet)
        {
            var resultado = new ValidacionExcel();

            if (worksheet.Dimension == null)
            {
                resultado.EsValida = false;
                resultado.MensajeError = "El archivo Excel está vacío.";
                return resultado;
            }

            // Buscar la fila de encabezados
            int filaEncabezados = -1;
            for (int fila = 1; fila <= Math.Min(5, worksheet.Dimension.End.Row); fila++)
            {
                var primeraColumna = worksheet.Cells[fila, 1].Value?.ToString()?.Trim()?.ToLower();
                if (primeraColumna == "criterio" || primeraColumna == "criterios" ||
                    primeraColumna == "item" || primeraColumna == "items" ||
                    primeraColumna == "items / criterios")
                {
                    filaEncabezados = fila;
                    break;
                }
            }

            if (filaEncabezados == -1)
            {
                resultado.EsValida = false;
                resultado.MensajeError = "No se encontró la fila de encabezados. La primera columna debe ser 'Criterio'.";
                return resultado;
            }

            // Extraer encabezados
            var encabezados = new List<string>();
            for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
            {
                var valor = worksheet.Cells[filaEncabezados, col].Value?.ToString()?.Trim();
                if (!string.IsNullOrEmpty(valor))
                {
                    encabezados.Add(valor);
                }
                else
                {
                    break; // Parar cuando encuentre una columna vacía
                }
            }

            if (encabezados.Count < 3)
            {
                resultado.EsValida = false;
                resultado.MensajeError = "Debe haber al menos 3 columnas (Criterio + al menos 2 niveles de calificación).";
                return resultado;
            }

            // Validar que hay datos después de los encabezados
            bool hayDatos = false;
            var criteriosLargos = new List<string>();
            
            for (int fila = filaEncabezados + 1; fila <= worksheet.Dimension.End.Row; fila++)
            {
                var criterio = worksheet.Cells[fila, 1].Value?.ToString()?.Trim();
                if (!string.IsNullOrEmpty(criterio) && criterio.ToUpper() != "TOTAL")
                {
                    hayDatos = true;
                    
                    // Validar longitud del criterio
                    if (criterio.Length > 200)
                    {
                        criteriosLargos.Add($"Fila {fila}: '{criterio.Substring(0, 50)}...' ({criterio.Length} caracteres)");
                    }
                }
            }

            if (!hayDatos)
            {
                resultado.EsValida = false;
                resultado.MensajeError = "No se encontraron criterios válidos en el archivo.";
                return resultado;
            }

            // Advertir sobre criterios largos
            if (criteriosLargos.Any())
            {
                _logger.LogWarning("Se encontraron {Count} criterios que serán truncados a 200 caracteres:", criteriosLargos.Count);
                foreach (var criterio in criteriosLargos)
                {
                    _logger.LogWarning("  - {Criterio}", criterio);
                }
            }

            resultado.EsValida = true;
            resultado.FilaEncabezados = filaEncabezados;
            resultado.FilaInicioDatos = filaEncabezados + 1;
            resultado.Encabezados = encabezados;

            return resultado;
        }

        private string GetDescripcionPorNivel(string nombreNivel)
        {
            return nombreNivel.ToLower() switch
            {
                "excelente" => "supera las expectativas establecidas",
                "muy bueno" => "supera satisfactoriamente las expectativas",
                "bueno" => "cumple satisfactoriamente las expectativas",
                "regular" => "cumple parcialmente las expectativas, necesita mejoras",
                "deficiente" => "no cumple las expectativas mínimas",
                "malo" => "desempeño muy por debajo de las expectativas",
                "insuficiente" => "desempeño inadecuado que requiere intervención",
                "no lo hizo" => "criterio no evaluado o tarea no completada",
                _ => "nivel de desempeño estándar"
            };
        }
    }

    public class ResultadoImportacion
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public int RubricaId { get; set; }
    }

    public class ValidacionExcel
    {
        public bool EsValida { get; set; }
        public string MensajeError { get; set; } = string.Empty;
        public int FilaEncabezados { get; set; }
        public int FilaInicioDatos { get; set; }
        public List<string> Encabezados { get; set; } = new();
    }
}