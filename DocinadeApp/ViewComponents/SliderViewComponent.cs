using Microsoft.AspNetCore.Mvc;
using DocinadeApp.Services;

namespace DocinadeApp.ViewComponents
{
    /// <summary>
    /// ViewComponent para mostrar el carrusel de sliders activos en el frontend público
    /// </summary>
    public class SliderViewComponent : ViewComponent
    {
        private readonly ISliderService _sliderService;
        private readonly ILogger<SliderViewComponent> _logger;

        public SliderViewComponent(
            ISliderService sliderService,
            ILogger<SliderViewComponent> logger)
        {
            _sliderService = sliderService;
            _logger = logger;
        }

        /// <summary>
        /// Invoca el componente y devuelve la vista con los sliders activos
        /// </summary>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            try
            {
                var sliders = await _sliderService.GetActiveAsync();
                
                if (sliders == null || !sliders.Any())
                {
                    _logger.LogInformation("[INFO] No hay sliders activos para mostrar");
                    return Content(string.Empty); // No renderiza nada si no hay sliders
                }

                _logger.LogInformation($"[INFO] Mostrando {sliders.Count()} sliders activos en carrusel");
                return View(sliders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ERROR] Error al cargar sliders activos");
                return Content(string.Empty); // Falla silenciosamente en el frontend
            }
        }
    }
}
