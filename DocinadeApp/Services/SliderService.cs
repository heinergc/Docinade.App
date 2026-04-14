using Microsoft.EntityFrameworkCore;
using DocinadeApp.Data;
using DocinadeApp.Models;

namespace DocinadeApp.Services
{
    public class SliderService : ISliderService
    {
        private readonly RubricasDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<SliderService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string UPLOAD_FOLDER = "uploads/sliders";

        public SliderService(
            RubricasDbContext context,
            IWebHostEnvironment environment,
            ILogger<SliderService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _environment = environment;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<SliderItem>> GetAllAsync()
        {
            return await _context.SliderItems
                .Include(s => s.UsuarioCreacion)
                .Include(s => s.UsuarioModificacion)
                .OrderBy(s => s.Orden)
                .ToListAsync();
        }

        public async Task<IEnumerable<SliderItem>> GetActiveAsync()
        {
            return await _context.SliderItems
                .Where(s => s.Activo)
                .OrderBy(s => s.Orden)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<SliderItem?> GetByIdAsync(int id)
        {
            return await _context.SliderItems
                .Include(s => s.UsuarioCreacion)
                .Include(s => s.UsuarioModificacion)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<SliderItem> CreateAsync(SliderItem sliderItem, IFormFile? imageFile, string userId)
        {
            try
            {
                // Guardar imagen si se proporcionó
                if (imageFile != null && imageFile.Length > 0)
                {
                    sliderItem.ImagenUrl = await SaveImageAsync(imageFile);
                }

                sliderItem.UsuarioCreacionId = userId;
                sliderItem.FechaCreacion = DateTime.Now;

                _context.SliderItems.Add(sliderItem);
                await _context.SaveChangesAsync();

                _logger.LogInformation("[SUCCESS] SliderItem creado: {Titulo} por usuario {UserId}", 
                    sliderItem.Titulo, userId);

                return sliderItem;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ERROR] Error creando SliderItem: {Titulo}", sliderItem.Titulo);
                throw;
            }
        }

        public async Task<SliderItem> UpdateAsync(SliderItem sliderItem, IFormFile? imageFile, string userId)
        {
            try
            {
                var existing = await _context.SliderItems.FindAsync(sliderItem.Id);
                if (existing == null)
                {
                    throw new InvalidOperationException($"SliderItem con ID {sliderItem.Id} no encontrado");
                }

                // Si hay nueva imagen, eliminar la anterior y guardar la nueva
                if (imageFile != null && imageFile.Length > 0)
                {
                    // Eliminar imagen anterior
                    if (!string.IsNullOrEmpty(existing.ImagenUrl))
                    {
                        await DeleteImageAsync(existing.ImagenUrl);
                    }

                    // Guardar nueva imagen
                    existing.ImagenUrl = await SaveImageAsync(imageFile);
                }

                // Actualizar propiedades
                existing.Titulo = sliderItem.Titulo;
                existing.Subtitulo = sliderItem.Subtitulo;
                existing.EnlaceUrl = sliderItem.EnlaceUrl;
                existing.TextoBoton = sliderItem.TextoBoton;
                existing.Orden = sliderItem.Orden;
                existing.Activo = sliderItem.Activo;
                existing.FechaModificacion = DateTime.Now;
                existing.UsuarioModificacionId = userId;

                await _context.SaveChangesAsync();

                _logger.LogInformation("[SUCCESS] SliderItem actualizado: {Titulo} por usuario {UserId}", 
                    existing.Titulo, userId);

                return existing;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ERROR] Error actualizando SliderItem ID: {Id}", sliderItem.Id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var sliderItem = await _context.SliderItems.FindAsync(id);
                if (sliderItem == null)
                {
                    return false;
                }

                // Eliminar imagen del servidor
                if (!string.IsNullOrEmpty(sliderItem.ImagenUrl))
                {
                    await DeleteImageAsync(sliderItem.ImagenUrl);
                }

                _context.SliderItems.Remove(sliderItem);
                await _context.SaveChangesAsync();

                _logger.LogInformation("[SUCCESS] SliderItem eliminado: {Titulo} (ID: {Id})", 
                    sliderItem.Titulo, id);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ERROR] Error eliminando SliderItem ID: {Id}", id);
                throw;
            }
        }

        public async Task<bool> ToggleActiveAsync(int id)
        {
            try
            {
                var sliderItem = await _context.SliderItems.FindAsync(id);
                if (sliderItem == null)
                {
                    return false;
                }

                sliderItem.Activo = !sliderItem.Activo;
                sliderItem.FechaModificacion = DateTime.Now;

                await _context.SaveChangesAsync();

                _logger.LogInformation("[INFO] SliderItem {Id} cambiado a {Status}", 
                    id, sliderItem.Activo ? "Activo" : "Inactivo");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ERROR] Error cambiando estado de SliderItem ID: {Id}", id);
                throw;
            }
        }

        public async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            try
            {
                // Validar tipo de archivo
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    throw new InvalidOperationException(
                        $"Tipo de archivo no permitido. Use: {string.Join(", ", allowedExtensions)}");
                }

                // Validar tamaño (máximo 5MB)
                if (imageFile.Length > 5 * 1024 * 1024)
                {
                    throw new InvalidOperationException("El archivo no debe exceder 5MB");
                }

                // Crear directorio si no existe
                var uploadPath = Path.Combine(_environment.WebRootPath, UPLOAD_FOLDER);
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                // Generar nombre único para el archivo
                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadPath, fileName);

                // Guardar archivo
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                // Obtener PathBase si existe (para IIS en subcarpeta)
                var pathBase = _httpContextAccessor.HttpContext?.Request.PathBase.Value ?? string.Empty;
                
                // Retornar ruta relativa con PathBase
                var relativePath = $"{pathBase}/{UPLOAD_FOLDER}/{fileName}";

                _logger.LogInformation("[SUCCESS] Imagen guardada: {FilePath} (PathBase: {PathBase})", relativePath, pathBase);

                return relativePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ERROR] Error guardando imagen: {FileName}", imageFile.FileName);
                throw;
            }
        }

        public async Task DeleteImageAsync(string imageUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(imageUrl))
                {
                    return;
                }

                // Convertir URL relativa a ruta física
                var fileName = Path.GetFileName(imageUrl);
                var filePath = Path.Combine(_environment.WebRootPath, UPLOAD_FOLDER, fileName);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    _logger.LogInformation("[SUCCESS] Imagen eliminada: {FilePath}", filePath);
                }

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "[WARNING] Error eliminando imagen: {ImageUrl}", imageUrl);
                // No lanzamos excepción para no interrumpir el flujo
            }
        }
    }
}
