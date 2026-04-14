using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Hosting;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Text;
using DocinadeApp.ViewModels;
using DocinadeApp.ViewModels.Conducta;

namespace DocinadeApp.Services
{
    /// <summary>
    /// Servicio para la generación de documentos PDF utilizando QuestPDF
    /// </summary>
    public class PdfService : IPdfService
    {
        private readonly IRazorViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PdfService> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PdfService(
            IRazorViewEngine viewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider,
            ILogger<PdfService> logger,
            IWebHostEnvironment webHostEnvironment)
        {
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// Genera un PDF a partir de contenido HTML - DEPRECATED para QuestPDF
        /// QuestPDF no soporta HTML directamente. Use GeneratePdfFromViewAsync con el modelo.
        /// </summary>
        public byte[] GeneratePdfFromHtml(string htmlContent, string orientation = "Portrait")
        {
            throw new NotSupportedException("QuestPDF no soporta generación desde HTML. Use GeneratePdfFromViewAsync con el ViewModel apropiado.");
        }

        /// <summary>
        /// Genera un PDF a partir de un modelo usando QuestPDF
        /// </summary>
        public async Task<byte[]> GeneratePdfFromViewAsync(string viewName, object model, string orientation = "Portrait")
        {
            try
            {
                // Configurar QuestPDF para uso comercial gratuito
                QuestPDF.Settings.License = LicenseType.Community;

                // Determinar qué tipo de documento generar basado en el viewName
                if (viewName.Contains("_CurriculumTemplate"))
                {
                    var curriculumModel = model as ProfesorCurriculumViewModel;
                    if (curriculumModel == null)
                        throw new ArgumentException("El modelo debe ser ProfesorCurriculumViewModel para CurriculumTemplate");
                    
                    return await GenerateCurriculumPdfAsync(curriculumModel, orientation);
                }
                else if (viewName.Contains("_CurriculumMasivoTemplate"))
                {
                    // Aceptar tanto CurriculumsMasivosViewModel como List<ProfesorCurriculumViewModel>
                    List<ProfesorCurriculumViewModel>? masiveModel = null;
                    
                    if (model is CurriculumsMasivosViewModel viewModel)
                    {
                        masiveModel = viewModel.Curriculums;
                    }
                    else if (model is List<ProfesorCurriculumViewModel> list)
                    {
                        masiveModel = list;
                    }
                    
                    if (masiveModel == null || masiveModel.Count == 0)
                        throw new ArgumentException("El modelo debe ser CurriculumsMasivosViewModel o List<ProfesorCurriculumViewModel> con al menos un elemento");
                    
                    return await GenerateMasiveCurriculumPdfAsync(masiveModel, orientation);
                }
                else if (viewName.Contains("ReporteDecisionProfesional"))
                {
                    var reporteModel = model as ReporteDecisionProfesionalViewModel;
                    if (reporteModel == null)
                        throw new ArgumentException("El modelo debe ser ReporteDecisionProfesionalViewModel para ReporteDecisionProfesional");
                    
                    return await GenerateReporteDecisionProfesionalPdfAsync(reporteModel, orientation);
                }
                else if (viewName.Contains("ReporteMasivoProgramas"))
                {
                    var reporteMasivoModel = model as ReporteMasivoProgramasViewModel;
                    if (reporteMasivoModel == null)
                        throw new ArgumentException("El modelo debe ser ReporteMasivoProgramasViewModel para ReporteMasivoProgramas");
                    
                    return await GenerateReporteMasivoProgramasPdfAsync(reporteMasivoModel, orientation);
                }
                else if (viewName.Contains("ReporteMasivoDecisiones"))
                {
                    var reporteMasivoDecisionesModel = model as ReporteMasivoDecisionesViewModel;
                    if (reporteMasivoDecisionesModel == null)
                        throw new ArgumentException("El modelo debe ser ReporteMasivoDecisionesViewModel para ReporteMasivoDecisiones");
                    
                    return await GenerateReporteMasivoDecisionesPdfAsync(reporteMasivoDecisionesModel, orientation);
                }
                else
                {
                    throw new NotSupportedException($"El template {viewName} no está soportado en QuestPDF");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error generando PDF desde vista: {viewName}");
                throw;
            }
        }

        /// <summary>
        /// Genera PDF de curriculum individual
        /// </summary>
        private Task<byte[]> GenerateCurriculumPdfAsync(ProfesorCurriculumViewModel model, string orientation)
        {
            return Task.Run(() =>
            {
                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(20);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(11).FontColor(Colors.Black));

                        page.Header().Element(c => ComposeHeader(c, model));
                        page.Content().Element(c => ComposeContent(c, model));
                        page.Footer().AlignCenter().Text(t =>
                        {
                            t.CurrentPageNumber();
                            t.Span(" / ");
                            t.TotalPages();
                        });
                    });
                });

                return document.GeneratePdf();
            });
        }

        /// <summary>
        /// Genera PDF de múltiples curriculums
        /// </summary>
        private Task<byte[]> GenerateMasiveCurriculumPdfAsync(List<ProfesorCurriculumViewModel> models, string orientation)
        {
            return Task.Run(() =>
            {
                var document = Document.Create(container =>
                {
                    foreach (var model in models)
                    {
                        container.Page(page =>
                        {
                            page.Size(PageSizes.A4);
                            page.Margin(20);
                            page.PageColor(Colors.White);
                            page.DefaultTextStyle(x => x.FontSize(11).FontColor(Colors.Black));

                            page.Header().Element(c => ComposeHeader(c, model));
                            page.Content().Element(c => ComposeContent(c, model));
                            page.Footer().AlignCenter().Text(t =>
                            {
                                t.CurrentPageNumber();
                                t.Span(" / ");
                                t.TotalPages();
                            });
                        });
                    }
                });

                return document.GeneratePdf();
            });
        }

        /// <summary>
        /// Genera PDF de Reporte de Decisión Profesional
        /// </summary>
        private Task<byte[]> GenerateReporteDecisionProfesionalPdfAsync(ReporteDecisionProfesionalViewModel model, string orientation)
        {
            return Task.Run(() =>
            {
                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(40);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(11).FontColor(Colors.Black));

                        page.Header().Element(c => ComposeReporteHeader(c, model));
                        page.Content().Element(c => ComposeReporteContent(c, model));
                        page.Footer().AlignCenter().Text(t =>
                        {
                            t.DefaultTextStyle(x => x.FontSize(9).FontColor(Colors.Grey.Darken1));
                            t.Span("Página ");
                            t.CurrentPageNumber();
                            t.Span(" de ");
                            t.TotalPages();
                            t.Span($" - Generado: {model.FechaGeneracion:dd/MM/yyyy HH:mm}");
                        });
                    });
                });

                return document.GeneratePdf();
            });
        }

        /// <summary>
        /// Genera PDF de Reporte Masivo de Programas de Acciones
        /// </summary>
        private Task<byte[]> GenerateReporteMasivoProgramasPdfAsync(ReporteMasivoProgramasViewModel model, string orientation)
        {
            return Task.Run(() =>
            {
                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        // Usar orientación horizontal para tablas anchas
                        page.Size(orientation == "Landscape" ? PageSizes.A4.Landscape() : PageSizes.A4);
                        page.Margin(30);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.Black));

                        page.Header().Element(c => ComposeReporteMasivoHeader(c, 
                            "REPORTE MASIVO DE PROGRAMAS DE ACCIONES INSTITUCIONALES", 
                            model.NombreInstitucion,
                            model.PeriodoAcademico,
                            model.EstadoFiltro,
                            model.TotalRegistros));
                        
                        page.Content().Element(c => ComposeProgramasTable(c, model));
                        
                        page.Footer().AlignCenter().Text(t =>
                        {
                            t.DefaultTextStyle(x => x.FontSize(8).FontColor(Colors.Grey.Darken1));
                            t.Span("Página ");
                            t.CurrentPageNumber();
                            t.Span(" de ");
                            t.TotalPages();
                            t.Span($" - Generado: {model.FechaGeneracion:dd/MM/yyyy HH:mm}");
                        });
                    });
                });

                return document.GeneratePdf();
            });
        }

        /// <summary>
        /// Genera PDF de Reporte Masivo de Decisiones Profesionales
        /// </summary>
        private Task<byte[]> GenerateReporteMasivoDecisionesPdfAsync(ReporteMasivoDecisionesViewModel model, string orientation)
        {
            return Task.Run(() =>
            {
                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        // Usar orientación horizontal para tablas anchas
                        page.Size(orientation == "Landscape" ? PageSizes.A4.Landscape() : PageSizes.A4);
                        page.Margin(30);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.Black));

                        page.Header().Element(c => ComposeReporteMasivoHeader(c, 
                            "REPORTE MASIVO DE DECISIONES PROFESIONALES", 
                            model.NombreInstitucion,
                            model.PeriodoAcademico,
                            model.EstadoFiltro,
                            model.TotalRegistros));
                        
                        page.Content().Element(c => ComposeDecisionesTable(c, model));
                        
                        page.Footer().AlignCenter().Text(t =>
                        {
                            t.DefaultTextStyle(x => x.FontSize(8).FontColor(Colors.Grey.Darken1));
                            t.Span("Página ");
                            t.CurrentPageNumber();
                            t.Span(" de ");
                            t.TotalPages();
                            t.Span($" - Generado: {model.FechaGeneracion:dd/MM/yyyy HH:mm}");
                        });
                    });
                });

                return document.GeneratePdf();
            });
        }

        /// <summary>
        /// Compone el encabezado para reportes masivos
        /// </summary>
        private void ComposeReporteMasivoHeader(IContainer container, string titulo, string institucion, string periodo, string filtroEstado, int totalRegistros)
        {
            container.Column(column =>
            {
                column.Item().AlignCenter().Text(institucion)
                    .FontSize(14).Bold().FontColor(Colors.Blue.Darken2);
                
                column.Item().PaddingTop(3).AlignCenter().Text(titulo)
                    .FontSize(12).Bold().FontColor(Colors.Blue.Medium);
                
                column.Item().PaddingTop(8).Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text($"Período: {periodo}").FontSize(9);
                        col.Item().Text($"Estado: {filtroEstado}").FontSize(9);
                    });
                    
                    row.RelativeItem().AlignRight().Column(col =>
                    {
                        col.Item().Text($"Total de Registros: {totalRegistros}").FontSize(9).Bold();
                    });
                });
                
                column.Item().PaddingTop(5).BorderBottom(1).BorderColor(Colors.Blue.Medium);
            });
        }

        /// <summary>
        /// Compone la tabla de programas de acciones
        /// </summary>
        private void ComposeProgramasTable(IContainer container, ReporteMasivoProgramasViewModel model)
        {
            container.PaddingTop(10).Table(table =>
            {
                // Definir columnas
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(60);  // N° Programa
                    columns.ConstantColumn(70);  // Fecha
                    columns.RelativeColumn(2);   // Estudiante
                    columns.ConstantColumn(70);  // Carnet
                    columns.RelativeColumn(1);   // Grado
                    columns.RelativeColumn(1);   // Estado
                    columns.RelativeColumn(1.5f); // Autorizado Por
                    columns.ConstantColumn(70);  // Fecha Autorización
                });

                // Encabezado
                table.Header(header =>
                {
                    header.Cell().Background(Colors.Blue.Darken1).Padding(5)
                        .Text("N° Programa").FontSize(9).Bold().FontColor(Colors.White);
                    header.Cell().Background(Colors.Blue.Darken1).Padding(5)
                        .Text("Fecha Elab.").FontSize(9).Bold().FontColor(Colors.White);
                    header.Cell().Background(Colors.Blue.Darken1).Padding(5)
                        .Text("Estudiante").FontSize(9).Bold().FontColor(Colors.White);
                    header.Cell().Background(Colors.Blue.Darken1).Padding(5)
                        .Text("Carnet").FontSize(9).Bold().FontColor(Colors.White);
                    header.Cell().Background(Colors.Blue.Darken1).Padding(5)
                        .Text("Grado/Sección").FontSize(9).Bold().FontColor(Colors.White);
                    header.Cell().Background(Colors.Blue.Darken1).Padding(5)
                        .Text("Estado").FontSize(9).Bold().FontColor(Colors.White);
                    header.Cell().Background(Colors.Blue.Darken1).Padding(5)
                        .Text("Autorizado Por").FontSize(9).Bold().FontColor(Colors.White);
                    header.Cell().Background(Colors.Blue.Darken1).Padding(5)
                        .Text("Fecha Aut.").FontSize(9).Bold().FontColor(Colors.White);
                });

                // Filas de datos
                foreach (var programa in model.Programas)
                {
                    var bgColor = model.Programas.IndexOf(programa) % 2 == 0 ? Colors.White : Colors.Grey.Lighten4;
                    
                    table.Cell().Background(bgColor).Padding(4).Text(programa.NumeroPrograma).FontSize(8);
                    table.Cell().Background(bgColor).Padding(4).Text(programa.FechaElaboracion.ToString("dd/MM/yyyy")).FontSize(8);
                    table.Cell().Background(bgColor).Padding(4).Text(programa.NombreEstudiante).FontSize(8);
                    table.Cell().Background(bgColor).Padding(4).Text(programa.Carnet).FontSize(8);
                    table.Cell().Background(bgColor).Padding(4).Text(programa.GradoSeccion).FontSize(8);
                    table.Cell().Background(bgColor).Padding(4)
                        .Text(programa.Estado).FontSize(8).Bold()
                        .FontColor(programa.Estado == "Completado" ? Colors.Green.Darken2 : Colors.Orange.Darken2);
                    table.Cell().Background(bgColor).Padding(4).Text(programa.AutorizadoPor).FontSize(8);
                    table.Cell().Background(bgColor).Padding(4)
                        .Text(programa.FechaAutorizacion?.ToString("dd/MM/yyyy") ?? "-").FontSize(8);
                }
            });
        }

        /// <summary>
        /// Compone la tabla de decisiones profesionales
        /// </summary>
        private void ComposeDecisionesTable(IContainer container, ReporteMasivoDecisionesViewModel model)
        {
            container.PaddingTop(10).Table(table =>
            {
                // Definir columnas
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(60);  // N° Acta
                    columns.ConstantColumn(70);  // Fecha Reunión
                    columns.RelativeColumn(2);   // Estudiante
                    columns.ConstantColumn(70);  // Carnet
                    columns.RelativeColumn(1);   // Grado
                    columns.RelativeColumn(2);   // Decisión
                    columns.RelativeColumn(1.5f); // Tomada Por
                    columns.ConstantColumn(60);  // Estado
                });

                // Encabezado
                table.Header(header =>
                {
                    header.Cell().Background(Colors.Blue.Darken1).Padding(5)
                        .Text("N° Acta").FontSize(9).Bold().FontColor(Colors.White);
                    header.Cell().Background(Colors.Blue.Darken1).Padding(5)
                        .Text("Fecha Reunión").FontSize(9).Bold().FontColor(Colors.White);
                    header.Cell().Background(Colors.Blue.Darken1).Padding(5)
                        .Text("Estudiante").FontSize(9).Bold().FontColor(Colors.White);
                    header.Cell().Background(Colors.Blue.Darken1).Padding(5)
                        .Text("Carnet").FontSize(9).Bold().FontColor(Colors.White);
                    header.Cell().Background(Colors.Blue.Darken1).Padding(5)
                        .Text("Grado/Sección").FontSize(9).Bold().FontColor(Colors.White);
                    header.Cell().Background(Colors.Blue.Darken1).Padding(5)
                        .Text("Decisión Tomada").FontSize(9).Bold().FontColor(Colors.White);
                    header.Cell().Background(Colors.Blue.Darken1).Padding(5)
                        .Text("Tomada Por").FontSize(9).Bold().FontColor(Colors.White);
                    header.Cell().Background(Colors.Blue.Darken1).Padding(5)
                        .Text("Estado").FontSize(9).Bold().FontColor(Colors.White);
                });

                // Filas de datos
                foreach (var decision in model.Decisiones)
                {
                    var bgColor = model.Decisiones.IndexOf(decision) % 2 == 0 ? Colors.White : Colors.Grey.Lighten4;
                    
                    table.Cell().Background(bgColor).Padding(4).Text(decision.NumeroActa).FontSize(8);
                    table.Cell().Background(bgColor).Padding(4).Text(decision.FechaReunion.ToString("dd/MM/yyyy")).FontSize(8);
                    table.Cell().Background(bgColor).Padding(4).Text(decision.NombreEstudiante).FontSize(8);
                    table.Cell().Background(bgColor).Padding(4).Text(decision.Carnet).FontSize(8);
                    table.Cell().Background(bgColor).Padding(4).Text(decision.GradoSeccion).FontSize(8);
                    table.Cell().Background(bgColor).Padding(4).Text(decision.DecisionTomada).FontSize(8);
                    table.Cell().Background(bgColor).Padding(4).Text(decision.TomaDecisionPor).FontSize(8);
                    table.Cell().Background(bgColor).Padding(4)
                        .Text(decision.Estado).FontSize(8).Bold()
                        .FontColor(decision.Estado == "Registrado" ? Colors.Green.Darken2 : Colors.Orange.Darken2);
                }
            });
        }

        /// <summary>
        /// Compone el encabezado del reporte de decisión profesional
        /// </summary>
        private void ComposeReporteHeader(IContainer container, ReporteDecisionProfesionalViewModel model)
        {
            container.Column(column =>
            {
                // Logo y nombre institución
                column.Item().AlignCenter().Text(model.NombreInstitucion)
                    .FontSize(16).Bold().FontColor(Colors.Blue.Darken2);
                
                column.Item().PaddingTop(5).AlignCenter().Text("ACTA DE DECISIÓN PROFESIONAL")
                    .FontSize(14).Bold().FontColor(Colors.Blue.Medium);
                
                column.Item().PaddingTop(3).AlignCenter().Text($"N° {model.NumeroActa}")
                    .FontSize(12).FontColor(Colors.Grey.Darken2);
                
                // Línea divisoria
                column.Item().PaddingTop(10).BorderBottom(2).BorderColor(Colors.Blue.Medium);
            });
        }

        /// <summary>
        /// Compone el contenido del reporte de decisión profesional
        /// </summary>
        private void ComposeReporteContent(IContainer container, ReporteDecisionProfesionalViewModel model)
        {
            container.PaddingTop(15).Column(column =>
            {
                // Información General
                column.Item().Element(c => ComposeInfoSection(c, "INFORMACIÓN GENERAL", infoColumn =>
                {
                    infoColumn.Item().Row(row =>
                    {
                        row.RelativeItem().Text("Fecha de Reunión: ").Bold();
                        row.RelativeItem().Text(model.FechaReunion.ToString("dd/MM/yyyy"));
                    });
                    
                    infoColumn.Item().Row(row =>
                    {
                        row.RelativeItem().Text("Periodo Académico: ").Bold();
                        row.RelativeItem().Text(model.PeriodoAcademico);
                    });
                    
                    infoColumn.Item().PaddingTop(8).Text("Datos del Estudiante").Bold().FontSize(12);
                    
                    infoColumn.Item().Row(row =>
                    {
                        row.RelativeItem().Text("Nombre: ").Bold();
                        row.RelativeItem().Text(model.NombreEstudiante);
                    });
                    
                    infoColumn.Item().Row(row =>
                    {
                        row.RelativeItem().Text("Carnet: ").Bold();
                        row.RelativeItem().Text(model.CarnetEstudiante);
                    });
                    
                    if (!string.IsNullOrEmpty(model.GradoSeccion))
                    {
                        infoColumn.Item().Row(row =>
                        {
                            row.RelativeItem().Text("Grado/Sección: ").Bold();
                            row.RelativeItem().Text(model.GradoSeccion);
                        });
                    }
                }));

                // Boletas Relacionadas (si hay)
                if (model.Boletas?.Any() == true)
                {
                    column.Item().Element(c => ComposeInfoSection(c, "ANTECEDENTES - BOLETAS DE CONDUCTA", boletasColumn =>
                    {
                        foreach (var boleta in model.Boletas)
                        {
                            boletasColumn.Item().PaddingBottom(8).Background(Colors.Grey.Lighten3)
                                .Padding(8).Column(boletaColumn =>
                            {
                                boletaColumn.Item().Row(row =>
                                {
                                    row.RelativeItem().Text($"Fecha: {boleta.FechaIncidente:dd/MM/yyyy}").FontSize(10);
                                    row.RelativeItem().Text($"Rebajo: {boleta.RebajoAplicado} puntos").FontSize(10).Bold();
                                });
                                
                                boletaColumn.Item().Text($"Tipo de Falta: {boleta.TipoFalta}").FontSize(10).Bold();
                                
                                if (!string.IsNullOrEmpty(boleta.DescripcionIncidente))
                                {
                                    boletaColumn.Item().Text(boleta.DescripcionIncidente).FontSize(10);
                                }
                                
                                boletaColumn.Item().Text($"Reportado por: {boleta.ReportadoPor}").FontSize(9).FontColor(Colors.Grey.Darken2);
                            });
                        }
                    }));
                }

                // Antecedentes y Análisis
                if (!string.IsNullOrEmpty(model.AntecedentesExpuestos))
                {
                    column.Item().Element(c => ComposeInfoSection(c, "ANTECEDENTES EXPUESTOS", antColumn =>
                    {
                        antColumn.Item().Text(model.AntecedentesExpuestos).FontSize(11).LineHeight(1.5f);
                    }));
                }

                if (!string.IsNullOrEmpty(model.VersionEstudiante))
                {
                    column.Item().Element(c => ComposeInfoSection(c, "VERSIÓN DEL ESTUDIANTE", verColumn =>
                    {
                        verColumn.Item().Text(model.VersionEstudiante).FontSize(11).LineHeight(1.5f);
                    }));
                }

                if (!string.IsNullOrEmpty(model.VersionPadres))
                {
                    column.Item().Element(c => ComposeInfoSection(c, "VERSIÓN DE LOS PADRES/ENCARGADOS", padresColumn =>
                    {
                        padresColumn.Item().Text(model.VersionPadres).FontSize(11).LineHeight(1.5f);
                    }));
                }

                if (!string.IsNullOrEmpty(model.AnalisisComite))
                {
                    column.Item().Element(c => ComposeInfoSection(c, "ANÁLISIS DEL COMITÉ", analisisColumn =>
                    {
                        analisisColumn.Item().Text(model.AnalisisComite).FontSize(11).LineHeight(1.5f);
                    }));
                }

                // Decisión Tomada
                column.Item().Element(c => ComposeInfoSection(c, "DECISIÓN TOMADA", decisionColumn =>
                {
                    decisionColumn.Item().Background(Colors.Yellow.Lighten3).Padding(10)
                        .Text(model.DecisionTomada).FontSize(12).Bold().LineHeight(1.5f);
                    
                    if (!string.IsNullOrEmpty(model.AccionesSeguimiento))
                    {
                        decisionColumn.Item().PaddingTop(10).Column(seguimientoCol =>
                        {
                            seguimientoCol.Item().Text("Acciones de Seguimiento:").Bold().FontSize(11);
                            seguimientoCol.Item().PaddingTop(5).Text(model.AccionesSeguimiento).FontSize(11);
                        });
                    }

                    if (!string.IsNullOrEmpty(model.ObservacionesComite))
                    {
                        decisionColumn.Item().PaddingTop(10).Column(obsCol =>
                        {
                            obsCol.Item().Text("Observaciones del Comité:").Bold().FontSize(11);
                            obsCol.Item().PaddingTop(5).Text(model.ObservacionesComite).FontSize(11);
                        });
                    }
                }));

                // Firmas
                column.Item().PaddingTop(30).Column(firmasColumn =>
                {
                    firmasColumn.Item().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Decisión tomada por:").FontSize(10).FontColor(Colors.Grey.Darken2);
                            col.Item().PaddingTop(20).BorderBottom(1).BorderColor(Colors.Grey.Darken1);
                            col.Item().PaddingTop(3).AlignCenter().Text(model.TomaDecisionPor).FontSize(10).Bold();
                            col.Item().AlignCenter().Text($"Fecha: {model.FechaDecision:dd/MM/yyyy}").FontSize(9);
                        });
                        
                        row.Spacing(20);
                        
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Estado del Registro:").FontSize(10).FontColor(Colors.Grey.Darken2);
                            col.Item().PaddingTop(20).Background(model.Estado == "Registrado" ? Colors.Green.Lighten3 : Colors.Orange.Lighten3)
                                .Padding(10).AlignCenter().Text(model.Estado).FontSize(11).Bold();
                        });
                    });
                });
            });
        }

        /// <summary>
        /// Compone una sección de información con título
        /// </summary>
        private void ComposeInfoSection(IContainer container, string title, Action<ColumnDescriptor> composeContent)
        {
            container.PaddingBottom(15).Column(column =>
            {
                // Título de sección
                column.Item().Background(Colors.Blue.Darken1).Padding(6)
                    .Text(title).FontSize(12).Bold().FontColor(Colors.White);
                
                // Contenido de sección
                column.Item().PaddingTop(8).PaddingHorizontal(5)
                    .Column(composeContent);
            });
        }

        /// <summary>
        /// Compone el encabezado del curriculum
        /// </summary>
        private void ComposeHeader(IContainer container, ProfesorCurriculumViewModel model)
        {
            container.Column(column =>
            {
                column.Item().BorderBottom(3).BorderColor(Colors.Blue.Medium).PaddingBottom(10).Column(headerContent =>
                {
                    headerContent.Item().AlignCenter().Text(model.NombreCompleto)
                        .FontSize(24).Bold().FontColor(Colors.Blue.Medium);
                    
                    headerContent.Item().AlignCenter().Text($"Cédula {model.TipoCedula}: {model.Cedula}")
                        .FontSize(11).FontColor(Colors.Grey.Darken2);
                    
                    if (!string.IsNullOrEmpty(model.NumeroColegiadoProfesional))
                    {
                        headerContent.Item().AlignCenter().Text($"Colegiado: {model.NumeroColegiadoProfesional}")
                            .FontSize(11).FontColor(Colors.Grey.Darken2);
                    }
                });
            });
        }

        /// <summary>
        /// Compone el contenido principal del curriculum
        /// </summary>
        private void ComposeContent(IContainer container, ProfesorCurriculumViewModel model)
        {
            container.PaddingVertical(10).Column(column =>
            {
                // Datos Personales
                column.Item().Element(c => ComposeSection(c, "DATOS PERSONALES", sectionColumn =>
                {
                    sectionColumn.Item().Row(row =>
                    {
                        row.RelativeItem().Text($"Fecha de Nacimiento: ").Bold();
                        row.RelativeItem().Text($"{model.FechaNacimiento?.ToString("dd/MM/yyyy") ?? "No especificada"} {(model.Edad != null ? $"({model.Edad} años)" : "")}");
                    });
                    
                    if (model.Sexo != null)
                    {
                        sectionColumn.Item().Row(row =>
                        {
                            row.RelativeItem().Text("Sexo: ").Bold();
                            row.RelativeItem().Text(model.Sexo == "M" ? "Masculino" : "Femenino");
                        });
                    }
                    
                    if (model.EstadoCivil != null)
                    {
                        sectionColumn.Item().Row(row =>
                        {
                            row.RelativeItem().Text("Estado Civil: ").Bold();
                            row.RelativeItem().Text(model.EstadoCivil);
                        });
                    }
                    
                    sectionColumn.Item().Row(row =>
                    {
                        row.RelativeItem().Text("Nacionalidad: ").Bold();
                        row.RelativeItem().Text(model.Nacionalidad ?? "No especificada");
                    });
                }));

                // Información de Contacto
                column.Item().Element(c => ComposeSection(c, "INFORMACIÓN DE CONTACTO", sectionColumn =>
                {
                    sectionColumn.Item().Row(row =>
                    {
                        row.RelativeItem().Text("Email Personal: ").Bold();
                        row.RelativeItem().Text(model.EmailPersonal ?? "No especificado");
                    });
                    
                    if (!string.IsNullOrEmpty(model.EmailInstitucional))
                    {
                        sectionColumn.Item().Row(row =>
                        {
                            row.RelativeItem().Text("Email Institucional: ").Bold();
                            row.RelativeItem().Text(model.EmailInstitucional);
                        });
                    }
                    
                    sectionColumn.Item().Row(row =>
                    {
                        row.RelativeItem().Text("Teléfono Celular: ").Bold();
                        row.RelativeItem().Text(model.TelefonoCelular ?? "No especificado");
                    });
                    
                    if (!string.IsNullOrEmpty(model.TelefonoFijo))
                    {
                        sectionColumn.Item().Row(row =>
                        {
                            row.RelativeItem().Text("Teléfono Fijo: ").Bold();
                            row.RelativeItem().Text(model.TelefonoFijo);
                        });
                    }
                    
                    if (!string.IsNullOrEmpty(model.DireccionExacta) || !string.IsNullOrEmpty(model.ProvinciaNombre))
                    {
                        sectionColumn.Item().PaddingTop(5).Column(dirColumn =>
                        {
                            dirColumn.Item().Text("Dirección:").Bold();
                            if (!string.IsNullOrEmpty(model.DireccionExacta))
                            {
                                dirColumn.Item().Text(model.DireccionExacta);
                            }
                            if (!string.IsNullOrEmpty(model.ProvinciaNombre))
                            {
                                var direccion = $"{model.ProvinciaNombre}, {model.CantonNombre}, {model.DistritoNombre}";
                                if (!string.IsNullOrEmpty(model.CodigoPostal))
                                    direccion += $" - CP: {model.CodigoPostal}";
                                dirColumn.Item().Text(direccion);
                            }
                        });
                    }
                }));

                // Formación Académica
                if (model.FormacionAcademica?.Any() == true)
                {
                    column.Item().Element(c => ComposeSection(c, "FORMACIÓN ACADÉMICA", sectionColumn =>
                    {
                        foreach (var formacion in model.FormacionAcademica)
                        {
                            sectionColumn.Item().PaddingBottom(10).Background(Colors.Grey.Lighten3)
                                .Padding(10).Column(itemColumn =>
                            {
                                itemColumn.Item().Text(formacion.TituloObtenido ?? "Sin título")
                                    .FontSize(12).Bold().FontColor(Colors.Blue.Medium);
                                
                                itemColumn.Item().Text($"{formacion.TipoFormacion} - {formacion.InstitucionEducativa}")
                                    .FontColor(Colors.Grey.Darken2);
                                
                                var periodo = formacion.EnCurso 
                                    ? $"{formacion.AnioInicio} - En curso" 
                                    : $"{formacion.AnioInicio} - {formacion.AnioFinalizacion}";
                                itemColumn.Item().Text(periodo).FontSize(10).FontColor(Colors.Grey.Darken1);
                                
                                if (!string.IsNullOrEmpty(formacion.PaisInstitucion))
                                {
                                    itemColumn.Item().Text($"País: {formacion.PaisInstitucion}").FontSize(10);
                                }
                            });
                        }
                    }));
                }

                // Experiencia Laboral
                if (model.ExperienciaLaboral?.Any() == true)
                {
                    column.Item().Element(c => ComposeSection(c, "EXPERIENCIA LABORAL", sectionColumn =>
                    {
                        foreach (var experiencia in model.ExperienciaLaboral)
                        {
                            sectionColumn.Item().PaddingBottom(10).Background(Colors.Grey.Lighten3)
                                .Padding(10).Column(itemColumn =>
                            {
                                itemColumn.Item().Text(experiencia.CargoDesempenado ?? "Sin cargo")
                                    .FontSize(12).Bold().FontColor(Colors.Blue.Medium);
                                
                                itemColumn.Item().Text(experiencia.NombreInstitucion ?? "Sin institución")
                                    .FontColor(Colors.Grey.Darken2);
                                
                                var fechaInicio = experiencia.FechaInicio != DateTime.MinValue 
                                    ? experiencia.FechaInicio.ToString("MM/yyyy", System.Globalization.CultureInfo.InvariantCulture)
                                    : "";
                                var fechaFin = experiencia.FechaFin.HasValue 
                                    ? experiencia.FechaFin.Value.ToString("MM/yyyy", System.Globalization.CultureInfo.InvariantCulture)
                                    : "";
                                var fechas = experiencia.TrabajandoActualmente
                                    ? $"{fechaInicio} - Actualmente"
                                    : $"{fechaInicio} - {fechaFin}";
                                itemColumn.Item().Text(fechas).FontSize(10).FontColor(Colors.Grey.Darken1);
                                
                                if (!string.IsNullOrEmpty(experiencia.DescripcionFunciones))
                                {
                                    itemColumn.Item().PaddingTop(5).Text(experiencia.DescripcionFunciones).FontSize(10);
                                }
                            });
                        }
                    }));
                }

                // Capacitaciones
                if (model.Capacitaciones?.Any() == true)
                {
                    column.Item().Element(c => ComposeSection(c, "CAPACITACIONES Y CERTIFICACIONES", sectionColumn =>
                    {
                        foreach (var capacitacion in model.Capacitaciones)
                        {
                            sectionColumn.Item().PaddingBottom(8).Column(itemColumn =>
                            {
                                itemColumn.Item().Text(capacitacion.NombreCapacitacion ?? "Sin nombre")
                                    .Bold().FontColor(Colors.Blue.Medium);
                                
                                itemColumn.Item().Text($"{capacitacion.InstitucionOrganizadora} - {capacitacion.TipoCapacitacion}")
                                    .FontSize(10);
                                
                                string fechaInicio = capacitacion.FechaInicio.HasValue ? capacitacion.FechaInicio.Value.ToString("MM/yyyy") : "";
                                string fechaFin = capacitacion.FechaFin.HasValue ? capacitacion.FechaFin.Value.ToString("MM/yyyy") : "";
                                var periodo = $"{fechaInicio} - {fechaFin}";
                                itemColumn.Item().Text($"{periodo} ({capacitacion.HorasCapacitacion} horas)")
                                    .FontSize(9).FontColor(Colors.Grey.Darken1);
                            });
                        }
                    }));
                }
            });
        }

        /// <summary>
        /// Compone una sección con título y contenido
        /// </summary>
        private void ComposeSection(IContainer container, string title, Action<ColumnDescriptor> composeContent)
        {
            container.PaddingBottom(15).Column(column =>
            {
                // Título de sección
                column.Item().Background(Colors.Blue.Medium).Padding(8)
                    .Text(title).FontSize(13).Bold().FontColor(Colors.White);
                
                // Contenido de sección
                column.Item().PaddingTop(10).PaddingHorizontal(5)
                    .Column(composeContent);
            });
        }
    }

    /// <summary>
    /// Controlador temporal para compatibilidad
    /// </summary>
    public class TempController : Controller
    {
        // Controlador vacío para compatibilidad
    }
}
