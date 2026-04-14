using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RubricasApp.Web.Data;
using RubricasApp.Web.Models;
using RubricasApp.Web.Services;

namespace RubricasApp.Web.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly IPeriodoAcademicoService _periodoService;
        protected readonly RubricasDbContext _context;
        protected PeriodoAcademico? PeriodoActivo { get; private set; }

        protected BaseController(IPeriodoAcademicoService periodoService, RubricasDbContext context)
        {
            _periodoService = periodoService;
            _context = context;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Solo aplicar filtro si el usuario está autenticado
            if (User.Identity?.IsAuthenticated == true)
            {
                try
                {
                    var userId = User.Identity.Name ?? User.FindFirst("sub")?.Value ?? "anonymous";
                    
                    // Obtener período activo del usuario
                    PeriodoActivo = await _periodoService.GetPeriodoActivoAsync(userId);
                    
                    // Pasar datos a las vistas
                    ViewBag.PeriodoActivo = PeriodoActivo;
                    ViewBag.PeriodosDisponibles = await _periodoService.GetPeriodosDisponiblesAsync();
                    
                    // Establecer el período seleccionado por defecto para dropdowns
                    if (PeriodoActivo != null)
                    {
                        ViewBag.PeriodoActivoId = PeriodoActivo.Id;
                        ViewBag.DefaultPeriodoSeleccionado = PeriodoActivo.Id;
                    }
                    
                    // También disponible como ViewData para mayor compatibilidad
                    ViewData["PeriodoActivoId"] = PeriodoActivo?.Id;
                    ViewData["PeriodoActivoNombre"] = PeriodoActivo?.NombreCompleto;
                }
                catch (Exception ex)
                {
                    // Log del error pero continuar con la ejecución
                    // El período activo quedará como null
                    ViewBag.PeriodoActivo = null;
                    ViewBag.PeriodosDisponibles = new List<PeriodoAcademico>();
                }
            }

            await next();
        }

        /// <summary>
        /// Método helper para obtener el ID del período activo
        /// </summary>
        protected int? GetPeriodoActivoId()
        {
            return PeriodoActivo?.Id;
        }

        /// <summary>
        /// Método helper para verificar si hay un período activo
        /// </summary>
        protected bool TienePeriodoActivo()
        {
            return PeriodoActivo != null;
        }

        /// <summary>
        /// Método helper para obtener el nombre del período activo
        /// </summary>
        protected string GetPeriodoActivoNombre()
        {
            return PeriodoActivo?.NombreCompleto ?? "Sin período seleccionado";
        }

        /// <summary>
        /// Método para redirigir a selección de período si es necesario
        /// </summary>
        protected IActionResult RedirectToSeleccionPeriodo()
        {
            return RedirectToAction("Seleccionar", "PeriodoAcademico");
        }

        /// <summary>
        /// Método helper para validar que hay un período activo antes de continuar
        /// </summary>
        protected bool ValidarPeriodoActivo()
        {
            return PeriodoActivo != null;
        }

        /// <summary>
        /// Método helper centralizado para obtener SelectList de períodos académicos
        /// con preselection automática del período activo
        /// </summary>
        protected async Task<SelectList> GetPeriodosAcademicosSelectListAsync(int? selectedValue = null)
        {
            var periodos = await _context.PeriodosAcademicos
                .Where(p => p.Activo) // Solo períodos activos
                .OrderBy(p => p.Anio)
                .ThenBy(p => p.NumeroPeriodo)
                .Select(p => new { p.Id, NombreCompleto = p.Anio + "-" + p.Codigo + " (" + p.Nombre + ")" })
                .ToListAsync();
                
            // Si no se especifica un valor seleccionado, usar el período activo del usuario
            var valorSeleccionado = selectedValue ?? GetPeriodoActivoId();
                
            return new SelectList(periodos, "Id", "NombreCompleto", valorSeleccionado);
        }

        /// <summary>
        /// Método helper para aplicar filtro automático de período en queries
        /// </summary>
        protected int? GetFiltroPeridoParaQuery(int? periodoEspecificado)
        {
            // Si el usuario especifica un período, usar ese. Sino, usar el período activo.
            return periodoEspecificado ?? GetPeriodoActivoId();
        }
    }
}