using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DocinadeApp.Models;
using DocinadeApp.Services;
using System.Security.Claims;

namespace DocinadeApp.Controllers
{
    [Authorize]
    public class SliderController : Controller
    {
        private readonly ISliderService _sliderService;
        private readonly ILogger<SliderController> _logger;

        public SliderController(ISliderService sliderService, ILogger<SliderController> logger)
        {
            _sliderService = sliderService;
            _logger = logger;
        }

        // GET: Slider
        public async Task<IActionResult> Index()
        {
            try
            {
                var sliders = await _sliderService.GetAllAsync();
                return View(sliders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ERROR] Error cargando lista de sliders");
                TempData["Error"] = "Error al cargar la lista de sliders";
                return View(new List<SliderItem>());
            }
        }

        // GET: Slider/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Slider/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SliderItem sliderItem, IFormFile? imageFile)
        {
            _logger.LogInformation("[DEBUG] POST Create recibido - Titulo: {Titulo}, ImageFile: {HasImage}", 
                sliderItem.Titulo ?? "NULL", imageFile != null ? $"{imageFile.FileName} ({imageFile.Length} bytes)" : "NULL");

            try
            {
                if (imageFile == null || imageFile.Length == 0)
                {
                    _logger.LogWarning("[VALIDATION] Imagen no proporcionada");
                    ModelState.AddModelError("imageFile", "Debe seleccionar una imagen para el slider");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("[VALIDATION] ModelState inválido. Errores: {Errors}", 
                        string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                    return View(sliderItem);
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
                _logger.LogInformation("[INFO] Intentando crear slider. UserId: {UserId}", userId);
                
                var created = await _sliderService.CreateAsync(sliderItem, imageFile, userId);

                _logger.LogInformation("[SUCCESS] Slider creado exitosamente: ID={Id}, Titulo={Titulo}", 
                    created.Id, created.Titulo);
                    
                TempData["Success"] = $"Slider '{created.Titulo}' creado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ERROR] Error creando slider: {Titulo}", sliderItem.Titulo);
                ModelState.AddModelError("", $"Error al crear el slider: {ex.Message}");
                return View(sliderItem);
            }
        }

        // GET: Slider/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sliderItem = await _sliderService.GetByIdAsync(id.Value);
            if (sliderItem == null)
            {
                return NotFound();
            }

            return View(sliderItem);
        }

        // POST: Slider/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SliderItem sliderItem, IFormFile? imageFile)
        {
            if (id != sliderItem.Id)
            {
                return NotFound();
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    return View(sliderItem);
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
                var updated = await _sliderService.UpdateAsync(sliderItem, imageFile, userId);

                TempData["Success"] = $"Slider '{updated.Titulo}' actualizado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ERROR] Error actualizando slider ID: {Id}", id);
                ModelState.AddModelError("", $"Error al actualizar el slider: {ex.Message}");
                return View(sliderItem);
            }
        }

        // GET: Slider/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sliderItem = await _sliderService.GetByIdAsync(id.Value);
            if (sliderItem == null)
            {
                return NotFound();
            }

            return View(sliderItem);
        }

        // POST: Slider/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var success = await _sliderService.DeleteAsync(id);
                if (success)
                {
                    TempData["Success"] = "Slider eliminado exitosamente";
                }
                else
                {
                    TempData["Error"] = "No se encontró el slider a eliminar";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ERROR] Error eliminando slider ID: {Id}", id);
                TempData["Error"] = $"Error al eliminar el slider: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Slider/ToggleActive/5
        [HttpPost]
        public async Task<IActionResult> ToggleActive(int id)
        {
            try
            {
                var success = await _sliderService.ToggleActiveAsync(id);
                if (success)
                {
                    return Json(new { success = true, message = "Estado cambiado exitosamente" });
                }
                else
                {
                    return Json(new { success = false, message = "No se encontró el slider" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ERROR] Error cambiando estado del slider ID: {Id}", id);
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }
    }
}
