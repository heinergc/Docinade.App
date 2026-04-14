using Microsoft.AspNetCore.Mvc;
using DocinadeApp.Services;
using DocinadeApp.ViewModels;

namespace DocinadeApp.ViewComponents
{
    public class PeriodoAcademicoSelectorViewComponent : ViewComponent
    {
        private readonly IPeriodoAcademicoService _periodoService;

        public PeriodoAcademicoSelectorViewComponent(IPeriodoAcademicoService periodoService)
        {
            _periodoService = periodoService;
        }

        public async Task<IViewComponentResult> InvokeAsync(
            bool mostrarTexto = true, 
            bool mostrarIndicador = true, 
            string cssClass = "")
        {
            try
            {
                var viewModel = new PeriodoAcademicoSelectorViewModel
                {
                    PeriodoActivo = ViewBag.PeriodoActivo as Models.PeriodoAcademico,
                    PeriodosDisponibles = ViewBag.PeriodosDisponibles as IEnumerable<Models.PeriodoAcademico> ?? 
                                        new List<Models.PeriodoAcademico>(),
                    MostrarTexto = mostrarTexto,
                    MostrarIndicador = mostrarIndicador,
                    CssClass = cssClass
                };

                // Si no tenemos los datos en ViewBag, obtenerlos del servicio
                if (viewModel.PeriodoActivo == null && User.Identity?.IsAuthenticated == true)
                {
                    var userId = User.Identity.Name ?? "";
                    viewModel.PeriodoActivo = await _periodoService.GetPeriodoActivoAsync(userId);
                    viewModel.PeriodosDisponibles = await _periodoService.GetPeriodosDisponiblesAsync();
                }

                return View(viewModel);
            }
            catch (Exception)
            {
                // En caso de error, retornar vista vacía
                return View(new PeriodoAcademicoSelectorViewModel
                {
                    PeriodoActivo = null,
                    PeriodosDisponibles = new List<Models.PeriodoAcademico>(),
                    MostrarTexto = mostrarTexto,
                    MostrarIndicador = mostrarIndicador,
                    CssClass = cssClass
                });
            }
        }
    }
}