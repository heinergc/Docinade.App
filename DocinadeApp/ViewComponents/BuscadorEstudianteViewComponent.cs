using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DocinadeApp.Data;
using Microsoft.EntityFrameworkCore;

namespace DocinadeApp.ViewComponents
{
    /// <summary>
    /// ViewComponent para el buscador avanzado de estudiantes
    /// Encapsula la funcionalidad de b�squeda modal con Bootstrap 5
    /// </summary>
    public class BuscadorEstudianteViewComponent : ViewComponent
    {
        private readonly RubricasDbContext _context;

        public BuscadorEstudianteViewComponent(RubricasDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Renderiza el modal de b�squeda de estudiantes
        /// </summary>
        /// <param name="selectId">ID del select que recibir� el valor seleccionado</param>
        /// <param name="modalId">ID �nico del modal (por defecto: modalBusquedaEstudiante)</param>
        /// <param name="controllerName">Nombre del controlador que contiene el endpoint de b�squeda</param>
        /// <param name="actionName">Nombre de la acci�n para la b�squeda (por defecto: BuscarEstudiantes)</param>
        /// <param name="incluirScripts">Si debe incluir los scripts JS autom�ticamente</param>
        /// <returns>Vista del componente</returns>
        public async Task<IViewComponentResult> InvokeAsync(
            string selectId = "estudianteId",
            string modalId = "modalBusquedaEstudiante",
            string controllerName = "Evaluaciones",
            string actionName = "BuscarEstudiantes",
            bool incluirScripts = true,
            bool cargarEstudiantesIniciales = false)
        {
            var model = new BuscadorEstudianteViewModel
            {
                SelectId = selectId,
                ModalId = modalId,
                ControllerName = controllerName,
                ActionName = actionName,
                IncluirScripts = incluirScripts,
                BuscarUrl = Url.Action(actionName, controllerName) ?? $"/{controllerName}/{actionName}"
            };

            // Opcionalmente cargar estudiantes iniciales para el dropdown
            if (cargarEstudiantesIniciales)
            {
                model.EstudiantesIniciales = await _context.Estudiantes
                    .OrderBy(e => e.Apellidos)
                    .ThenBy(e => e.Nombre)
                    .Take(100) // Limitar para performance
                    .Select(e => new SelectListItem
                    {
                        Value = e.IdEstudiante.ToString(),
                        Text = $"{e.Apellidos}, {e.Nombre} ({e.NumeroId})"
                    })
                    .ToListAsync();
            }

            return View(model);
        }
    }

    /// <summary>
    /// ViewModel para el ViewComponent de b�squeda de estudiantes
    /// </summary>
    public class BuscadorEstudianteViewModel
    {
        public string SelectId { get; set; } = "estudianteId";
        public string ModalId { get; set; } = "modalBusquedaEstudiante";
        public string ControllerName { get; set; } = "Evaluaciones";
        public string ActionName { get; set; } = "BuscarEstudiantes";
        public string BuscarUrl { get; set; } = "/Evaluaciones/BuscarEstudiantes";
        public bool IncluirScripts { get; set; } = true;
        public List<SelectListItem> EstudiantesIniciales { get; set; } = new List<SelectListItem>();
    }
}