using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using RubricasApp.Web.Data;

namespace RubricasApp.Web.Controllers
{
    [Authorize]
    public class DebugGruposController : Controller
    {
        private readonly RubricasDbContext _context;
        private readonly ILogger<DebugGruposController> _logger;

        public DebugGruposController(RubricasDbContext context, ILogger<DebugGruposController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var debug = new
                {
                    TotalGrupos = await _context.GruposEstudiantes.CountAsync(),
                    TotalEstudiantes = await _context.Estudiantes.CountAsync(),
                    TotalPeriodos = await _context.PeriodosAcademicos.CountAsync(),
                    Grupos = await _context.GruposEstudiantes
                        .Take(5)
                        .Select(g => new {
                            g.GrupoId,
                            g.Codigo,
                            g.Nombre,
                            g.Estado
                        })
                        .ToListAsync()
                };

                ViewBag.Debug = debug;
                ViewBag.PruebaUrl = "/GruposEstudiantes/AsignarEstudiantes/1";
                
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        public async Task<IActionResult> TestUrl(int id = 1)
        {
            try
            {
                var grupo = await _context.GruposEstudiantes.FindAsync(id);
                if (grupo == null)
                {
                    return Json(new { success = false, message = $"Grupo {id} no encontrado" });
                }

                return Json(new { 
                    success = true, 
                    message = "Grupo encontrado",
                    grupo = new {
                        grupo.GrupoId,
                        grupo.Codigo,
                        grupo.Nombre
                    },
                    urls = new {
                        asignar = $"/GruposEstudiantes/AsignarEstudiantes/{id}",
                        details = $"/GruposEstudiantes/Details/{id}",
                        test = $"/TestGrupos/AsignarEstudiantes/{id}"
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}