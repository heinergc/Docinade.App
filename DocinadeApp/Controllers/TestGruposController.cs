using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using RubricasApp.Web.Data;

namespace RubricasApp.Web.Controllers
{
    [Authorize]
    public class TestGruposController : Controller
    {
        private readonly RubricasDbContext _context;
        private readonly ILogger<TestGruposController> _logger;

        public TestGruposController(RubricasDbContext context, ILogger<TestGruposController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Test simple para verificar que el routing funciona
        public IActionResult AsignarEstudiantes(int id)
        {
            try
            {
                _logger.LogInformation("?? Test: AsignarEstudiantes llamado con ID: {Id}", id);
                
                ViewBag.GrupoId = id;
                ViewBag.Mensaje = $"? Ruta funcionando correctamente para grupo ID: {id}";
                
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "? Error en test AsignarEstudiantes");
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        // Test para verificar que el grupo existe
        public async Task<IActionResult> VerificarGrupo(int id)
        {
            try
            {
                var grupo = await _context.GruposEstudiantes.FindAsync(id);
                
                if (grupo == null)
                {
                    return Json(new { 
                        success = false, 
                        message = $"? Grupo con ID {id} no encontrado",
                        id = id 
                    });
                }

                return Json(new { 
                    success = true, 
                    message = $"? Grupo encontrado: {grupo.Codigo} - {grupo.Nombre}",
                    grupo = new {
                        grupo.GrupoId,
                        grupo.Codigo,
                        grupo.Nombre,
                        grupo.Estado,
                        grupo.TipoGrupo
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verificando grupo {Id}", id);
                return Json(new { 
                    success = false, 
                    message = $"? Error: {ex.Message}",
                    id = id 
                });
            }
        }

        // Test para verificar el servicio
        public async Task<IActionResult> TestServicio()
        {
            try
            {
                var totalGrupos = await _context.GruposEstudiantes.CountAsync();
                var totalEstudiantes = await _context.Estudiantes.CountAsync();
                
                return Json(new { 
                    success = true,
                    totalGrupos = totalGrupos,
                    totalEstudiantes = totalEstudiantes,
                    message = "? Conexión a base de datos funcionando"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en test de servicio");
                return Json(new { 
                    success = false, 
                    message = $"? Error: {ex.Message}"
                });
            }
        }
    }
}