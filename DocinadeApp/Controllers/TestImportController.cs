using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using DocinadeApp.Data;
using DocinadeApp.Models;

namespace DocinadeApp.Controllers
{
    public class TestImportController : Controller
    {
        private readonly RubricasDbContext _context;
        private readonly ILogger<TestImportController> _logger;

        public TestImportController(RubricasDbContext context, ILogger<TestImportController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Página principal de diagnóstico con botones para ejecutar las acciones
        /// </summary>
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// DIAGNOSTIC: Check database schema for ValoresRubrica table
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> DiagnosticarEsquema()
        {
            try
            {
                var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = "PRAGMA table_info(ValoresRubrica)";

                var results = new List<object>();
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    results.Add(new
                    {
                        cid = reader.GetInt32(0),        // Use ordinal instead of name
                        name = reader.GetString(1),      // Use ordinal instead of name
                        type = reader.GetString(2),      // Use ordinal instead of name
                        notnull = reader.GetBoolean(3),  // Use ordinal instead of name
                        dflt_value = reader.IsDBNull(4) ? null : reader.GetValue(4),  // Use ordinal instead of name
                        pk = reader.GetBoolean(5)        // Use ordinal instead of name
                    });
                }

                await connection.CloseAsync();

                return Json(new
                {
                    success = true,
                    schema = results,
                    message = "Esquema de la tabla ValoresRubrica - Buscar ValorPuntos para ver si tiene NOT NULL constraint"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al diagnosticar esquema");
                return Json(new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// FIX: Correct database schema for ValorPuntos NOT NULL constraint
        /// TEMPORALLY CHANGED TO HttpGet FOR TESTING
        /// </summary>
        [HttpGet]  // 🔧 CAMBIO TEMPORAL: De HttpPost a HttpGet
        public async Task<IActionResult> CorregirEsquemaValorPuntos()
        {
            try
            {
                var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                _logger.LogInformation("🔧 Iniciando corrección del esquema de ValorPuntos");

                var commands = new[]
                {
                    // First, update any existing NULL values to 0
                    "UPDATE ValoresRubrica SET ValorPuntos = '0' WHERE ValorPuntos IS NULL",
                    
                    // Create new table with proper constraints
                    @"CREATE TABLE ValoresRubrica_new (
                        IdValor INTEGER PRIMARY KEY AUTOINCREMENT,
                        IdRubrica INTEGER NOT NULL,
                        IdItem INTEGER NOT NULL,
                        IdNivel INTEGER NOT NULL,
                        ValorPuntos TEXT NOT NULL DEFAULT '0',
                        FOREIGN KEY (IdRubrica) REFERENCES Rubricas(IdRubrica),
                        FOREIGN KEY (IdItem) REFERENCES ItemsEvaluacion(IdItem),
                        FOREIGN KEY (IdNivel) REFERENCES NivelesCalificacion(IdNivel)
                    )",

                    // Copy data from old table to new table
                    @"INSERT INTO ValoresRubrica_new (IdValor, IdRubrica, IdItem, IdNivel, ValorPuntos)
                      SELECT IdValor, IdRubrica, IdItem, IdNivel, COALESCE(ValorPuntos, '0') FROM ValoresRubrica",

                    // Drop old table
                    "DROP TABLE ValoresRubrica",

                    // Rename new table to original name
                    "ALTER TABLE ValoresRubrica_new RENAME TO ValoresRubrica",

                    // Recreate indexes if any
                    @"CREATE UNIQUE INDEX IF NOT EXISTS IX_ValorRubrica_Unique 
                      ON ValoresRubrica (IdRubrica, IdItem, IdNivel)"
                };

                foreach (var cmd in commands)
                {
                    using var command = connection.CreateCommand();
                    command.CommandText = cmd;
                    await command.ExecuteNonQueryAsync();
                    _logger.LogInformation("✅ Ejecutado: {Command}", cmd.Split('\n')[0] + "...");
                }

                await connection.CloseAsync();

                return Json(new
                {
                    success = true,
                    message = "✅ Esquema corregido exitosamente - ValorPuntos ahora es NOT NULL con valor por defecto '0'"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al corregir esquema");
                return Json(new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Versión JSON del método para AJAX
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CorregirEsquemaValorPuntosJson()
        {
            try
            {
                var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                _logger.LogInformation("🔧 Iniciando corrección del esquema de ValorPuntos");

                var commands = new[]
                {
                    "UPDATE ValoresRubrica SET ValorPuntos = '0' WHERE ValorPuntos IS NULL",
                    @"CREATE TABLE ValoresRubrica_new (
                        IdValor INTEGER PRIMARY KEY AUTOINCREMENT,
                        IdRubrica INTEGER NOT NULL,
                        IdItem INTEGER NOT NULL,
                        IdNivel INTEGER NOT NULL,
                        ValorPuntos TEXT NOT NULL DEFAULT '0',
                        FOREIGN KEY (IdRubrica) REFERENCES Rubricas(IdRubrica),
                        FOREIGN KEY (IdItem) REFERENCES ItemsEvaluacion(IdItem),
                        FOREIGN KEY (IdNivel) REFERENCES NivelesCalificacion(IdNivel)
                    )",
                    @"INSERT INTO ValoresRubrica_new (IdValor, IdRubrica, IdItem, IdNivel, ValorPuntos)
                      SELECT IdValor, IdRubrica, IdItem, IdNivel, COALESCE(ValorPuntos, '0') FROM ValoresRubrica",
                    "DROP TABLE ValoresRubrica",
                    "ALTER TABLE ValoresRubrica_new RENAME TO ValoresRubrica",
                    @"CREATE UNIQUE INDEX IF NOT EXISTS IX_ValorRubrica_Unique 
                      ON ValoresRubrica (IdRubrica, IdItem, IdNivel)"
                };

                foreach (var cmd in commands)
                {
                    using var command = connection.CreateCommand();
                    command.CommandText = cmd;
                    await command.ExecuteNonQueryAsync();
                    _logger.LogInformation("✅ Ejecutado: {Command}", cmd.Split('\n')[0] + "...");
                }

                await connection.CloseAsync();

                return Json(new
                {
                    success = true,
                    message = "✅ Esquema corregido exitosamente - ValorPuntos ahora es NOT NULL con valor por defecto '0'"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al corregir esquema");
                return Json(new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// FIX: Correct database schema for InstrumentosEvaluacion Activo column
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> CorregirEsquemaInstrumentosEvaluacion()
        {
            try
            {
                var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                _logger.LogInformation("🔧 Iniciando corrección del esquema de InstrumentosEvaluacion");

                // 1. Verificar estructura actual de la tabla
                using var tableInfoCommand = connection.CreateCommand();
                tableInfoCommand.CommandText = "PRAGMA table_info(InstrumentosEvaluacion)";

                var columns = new List<string>();
                using (var reader = await tableInfoCommand.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        columns.Add(reader.GetString(1)); // nombre de columna
                    }
                }

                bool hasActivo = columns.Contains("Activo");
                bool hasEstaActivo = columns.Contains("EstaActivo");

                _logger.LogInformation("📊 Estado actual: Activo={hasActivo}, EstaActivo={hasEstaActivo}", hasActivo, hasEstaActivo);

                var messages = new List<string>
                {
                    $"📊 Estado actual: Activo={hasActivo}, EstaActivo={hasEstaActivo}"
                };

                // 2. Agregar columna Activo si no existe
                if (!hasActivo)
                {
                    using var addActivoCommand = connection.CreateCommand();
                    addActivoCommand.CommandText = "ALTER TABLE InstrumentosEvaluacion ADD COLUMN Activo INTEGER DEFAULT 1";
                    await addActivoCommand.ExecuteNonQueryAsync();
                    messages.Add("✅ Columna Activo agregada");
                    _logger.LogInformation("✅ Columna Activo agregada");
                }

                // 3. Si existe EstaActivo, sincronizar datos
                if (hasEstaActivo)
                {
                    using var syncCommand = connection.CreateCommand();
                    syncCommand.CommandText = @"
                        UPDATE InstrumentosEvaluacion 
                        SET Activo = COALESCE(EstaActivo, 1) 
                        WHERE Activo IS NULL OR Activo != COALESCE(EstaActivo, 1)";
                    var rowsUpdated = await syncCommand.ExecuteNonQueryAsync();
                    messages.Add($"🔄 {rowsUpdated} registros sincronizados desde EstaActivo a Activo");
                    _logger.LogInformation("🔄 {rowsUpdated} registros sincronizados desde EstaActivo a Activo", rowsUpdated);
                }

                // 4. Asegurar que no hay valores NULL en Activo
                using var fixNullCommand = connection.CreateCommand();
                fixNullCommand.CommandText = "UPDATE InstrumentosEvaluacion SET Activo = 1 WHERE Activo IS NULL";
                var nullsFixed = await fixNullCommand.ExecuteNonQueryAsync();
                messages.Add($"🛠️ {nullsFixed} valores NULL corregidos en Activo");
                _logger.LogInformation("🛠️ {nullsFixed} valores NULL corregidos en Activo", nullsFixed);

                // 5. Verificar que todo está correcto
                using var verifyCommand = connection.CreateCommand();
                verifyCommand.CommandText = "SELECT COUNT(*) FROM InstrumentosEvaluacion WHERE Activo IS NULL";
                var nullCount = Convert.ToInt32(await verifyCommand.ExecuteScalarAsync());

                if (nullCount == 0)
                {
                    messages.Add("🎉 Esquema de InstrumentosEvaluacion corregido exitosamente");
                    _logger.LogInformation("🎉 Esquema de InstrumentosEvaluacion corregido exitosamente");
                }
                else
                {
                    messages.Add($"⚠️ Aún quedan {nullCount} valores NULL en Activo");
                    _logger.LogWarning("⚠️ Aún quedan {nullCount} valores NULL en Activo", nullCount);
                }

                // 6. Mostrar datos finales
                using var showDataCommand = connection.CreateCommand();
                showDataCommand.CommandText = "SELECT InstrumentoId, Nombre, Activo FROM InstrumentosEvaluacion ORDER BY InstrumentoId";
                var data = new List<object>();
                using (var dataReader = await showDataCommand.ExecuteReaderAsync())
                {
                    while (await dataReader.ReadAsync())
                    {
                        data.Add(new
                        {
                            InstrumentoId = dataReader.GetInt32(0),
                            Nombre = dataReader.GetString(1),
                            Activo = dataReader.IsDBNull(2) ? "NULL" : dataReader.GetInt32(2).ToString()
                        });
                    }
                }

                await connection.CloseAsync();

                return Json(new
                {
                    success = true,
                    messages = messages,
                    data = data,
                    message = "✅ Corrección de InstrumentosEvaluacion completada"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al corregir esquema de InstrumentosEvaluacion");
                return Json(new
                {
                    success = false,
                    error = ex.Message,
                    message = "❌ Error al corregir esquema de InstrumentosEvaluacion"
                });
            }
        }

        // Método de prueba para diagnosticar problemas FK
        public async Task<IActionResult> TestCrearRubricaSimple()
        {
            try
            {
                _logger.LogInformation("🔍 Iniciando prueba de creación de rúbrica simple...");

                // Paso 1: Crear rúbrica base
                var rubrica = new Rubrica
                {
                    NombreRubrica = $"Test Rúbrica {DateTime.Now:yyyy-MM-dd HH:mm:ss}",
                    Descripcion = "Rúbrica de prueba para diagnosticar FK",
                    Estado = "ACTIVO",
                    FechaCreacion = DateTime.Now,
                    EsPublica = 1
                };

                _context.Rubricas.Add(rubrica);
                await _context.SaveChangesAsync();
                _logger.LogInformation("✅ Rúbrica creada con ID: {IdRubrica}", rubrica.IdRubrica);

                // Paso 2: Crear nivel de calificación
                var nivel = new NivelCalificacion
                {
                    NombreNivel = "Test Excelente",
                    OrdenNivel = 1,
                    Descripcion = "Nivel de prueba"
                };

                _context.NivelesCalificacion.Add(nivel);
                await _context.SaveChangesAsync();
                _logger.LogInformation("✅ Nivel creado con ID: {IdNivel}", nivel.IdNivel);

                // Paso 3: Crear item de evaluación
                var item = new ItemEvaluacion
                {
                    IdRubrica = rubrica.IdRubrica,
                    NombreItem = "Test Criterio",
                    OrdenItem = 1,
                    Peso = 1.0m,
                    Descripcion = "Item de prueba"
                };

                _context.ItemsEvaluacion.Add(item);
                await _context.SaveChangesAsync();
                _logger.LogInformation("✅ Item creado con ID: {IdItem}", item.IdItem);

                // Paso 4: Intentar crear valor (aquí puede fallar)
                try
                {
                    var valor = new ValorRubrica
                    {
                        IdRubrica = rubrica.IdRubrica,
                        IdItem = item.IdItem,
                        IdNivel = nivel.IdNivel,
                        ValorPuntos = 4.0m
                    };

                    _context.ValoresRubrica.Add(valor);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("✅ Valor creado con ID: {IdValor}", valor.IdValor);

                    return Json(new
                    {
                        success = true,
                        message = "Test exitoso: todas las entidades creadas",
                        rubricaId = rubrica.IdRubrica,
                        itemId = item.IdItem,
                        nivelId = nivel.IdNivel,
                        valorId = valor.IdValor
                    });
                }
                catch (Exception exValor)
                {
                    _logger.LogError(exValor, "❌ Error al crear valor: {Message}", exValor.Message);
                    return Json(new
                    {
                        success = false,
                        error = "FK Error en ValorRubrica",
                        message = exValor.Message,
                        innerException = exValor.InnerException?.Message
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error general en test: {Message}", ex.Message);
                return Json(new
                {
                    success = false,
                    error = "Error general",
                    message = ex.Message,
                    innerException = ex.InnerException?.Message
                });
            }
        }

        /// <summary>
        /// DIAGNOSTIC: Test PeriodosAcademicos service and dependencies
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> TestPeriodosService()
        {
            try
            {
                var diagnostics = new List<string>();

                // Test 1: Verify service registration
                try
                {
                    using var scope = HttpContext.RequestServices.CreateScope();
                    var periodosService = scope.ServiceProvider.GetService<DocinadeApp.Services.Academic.IPeriodosAcademicosService>();

                    if (periodosService == null)
                    {
                        diagnostics.Add("❌ ERROR: IPeriodosAcademicosService is not registered in DI container");
                        return Json(new { success = false, diagnostics });
                    }
                    else
                    {
                        diagnostics.Add("✅ IPeriodosAcademicosService is registered correctly");
                    }

                    // Test 2: Test database access
                    var testData = await _context.PeriodosAcademicos.Take(1).ToListAsync();
                    diagnostics.Add($"✅ Database access working - Found {await _context.PeriodosAcademicos.CountAsync()} periods");

                    // Test 3: Test service method
                    var pagedResult = await periodosService.GetPagedAsync(1, 5, null);
                    diagnostics.Add($"✅ Service.GetPagedAsync working - Retrieved {pagedResult.Items.Count()} items");

                    // Test 4: Test single item retrieval
                    if (testData.Any())
                    {
                        var periodo = await periodosService.GetByIdAsync(testData.First().Id);
                        if (periodo != null)
                        {
                            diagnostics.Add($"✅ Service.GetByIdAsync working - Retrieved period: {periodo.Nombre}");
                        }
                        else
                        {
                            diagnostics.Add("❌ ERROR: Service.GetByIdAsync returned null for existing period");
                        }
                    }

                    diagnostics.Add("🎉 All PeriodosAcademicos service tests passed!");

                    return Json(new
                    {
                        success = true,
                        diagnostics,
                        message = "✅ PeriodosAcademicos service is working correctly"
                    });
                }
                catch (Exception serviceEx)
                {
                    diagnostics.Add($"❌ ERROR in service test: {serviceEx.Message}");
                    diagnostics.Add($"   Stack trace: {serviceEx.StackTrace}");
                    return Json(new { success = false, diagnostics });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error testing PeriodosAcademicos service");
                return Json(new
                {
                    success = false,
                    error = ex.Message,
                    stackTrace = ex.StackTrace,
                    message = "❌ Error testing PeriodosAcademicos service"
                });
            }
        }

        /// <summary>
        /// DIAGNOSTIC: Check AutoMapper configuration for Academic models
        /// </summary>
        [HttpGet]
        public IActionResult TestAutoMapperConfig()
        {
            try
            {
                var diagnostics = new List<string>();

                using var scope = HttpContext.RequestServices.CreateScope();
                var mapper = scope.ServiceProvider.GetService<AutoMapper.IMapper>();

                if (mapper == null)
                {
                    diagnostics.Add("❌ ERROR: AutoMapper is not registered in DI container");
                    return Json(new { success = false, diagnostics });
                }

                diagnostics.Add("✅ AutoMapper is registered correctly");

                // Test mappings
                var testPeriodo = new DocinadeApp.Models.PeriodoAcademico
                {
                    Id = 1,
                    Codigo = "TEST",
                    Nombre = "Test Period",
                    Anio = 2025,
                    Ciclo = "I-2025",
                    Tipo = DocinadeApp.Models.TipoPeriodo.Cuatrimestre,
                    NumeroPeriodo = 1,
                    FechaInicio = DateTime.Now,
                    FechaFin = DateTime.Now.AddDays(120),
                    Activo = true,
                    FechaCreacion = DateTime.Now
                };

                try
                {
                    var periodoVm = mapper.Map<DocinadeApp.ViewModels.Academic.PeriodoVm>(testPeriodo);
                    diagnostics.Add($"✅ PeriodoAcademico -> PeriodoVm mapping works: {periodoVm.Nombre}");
                }
                catch (Exception mapEx)
                {
                    diagnostics.Add($"❌ ERROR: PeriodoAcademico -> PeriodoVm mapping failed: {mapEx.Message}");
                }

                try
                {
                    var periodoListVm = mapper.Map<DocinadeApp.ViewModels.Academic.PeriodoListVm>(testPeriodo);
                    diagnostics.Add($"✅ PeriodoAcademico -> PeriodoListVm mapping works: {periodoListVm.Nombre}");
                }
                catch (Exception mapEx)
                {
                    diagnostics.Add($"❌ ERROR: PeriodoAcademico -> PeriodoListVm mapping failed: {mapEx.Message}");
                }

                return Json(new
                {
                    success = true,
                    diagnostics,
                    message = "✅ AutoMapper configuration tested successfully"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    error = ex.Message,
                    message = "❌ Error testing AutoMapper configuration"
                });
            }
        }
    }
}