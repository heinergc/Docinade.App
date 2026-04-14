using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DocinadeApp.Data;
using Microsoft.EntityFrameworkCore;

namespace DocinadeApp.ViewComponents
{
    public class BusquedaEstudianteViewComponent : ViewComponent
    {
        private readonly RubricasDbContext _context;

        public BusquedaEstudianteViewComponent(RubricasDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(
            string selectName = "estudianteId", 
            string selectId = "estudianteId",
            string modalId = "modalBusquedaEstudiante",
            string placeholder = "-- Seleccione un estudiante --",
            int? selectedValue = null,
            string cssClass = "form-select",
            bool disabled = false,
            bool required = false,
            string label = "Estudiante:")
        {
            var model = new BusquedaEstudianteViewModel
            {
                SelectName = selectName,
                SelectId = selectId,
                ModalId = modalId,
                Placeholder = placeholder,
                SelectedValue = selectedValue,
                CssClass = cssClass,
                Disabled = disabled,
                Required = required,
                Label = label
            };

            // Cargar estudiantes existentes para el select
            var estudiantes = await _context.Estudiantes
                .OrderBy(e => e.Apellidos)
                .ThenBy(e => e.Nombre)
                .Select(e => new SelectListItem
                {
                    Value = e.IdEstudiante.ToString(),
                    Text = $"{e.Apellidos}, {e.Nombre} ({e.NumeroId})",
                    Selected = selectedValue.HasValue && e.IdEstudiante == selectedValue.Value
                })
                .ToListAsync();

            model.Estudiantes = estudiantes;

            return View(model);
        }
    }

    public class BusquedaEstudianteViewModel
    {
        public string SelectName { get; set; } = "estudianteId";
        public string SelectId { get; set; } = "estudianteId";
        public string ModalId { get; set; } = "modalBusquedaEstudiante";
        public string Placeholder { get; set; } = "-- Seleccione un estudiante --";
        public int? SelectedValue { get; set; }
        public string CssClass { get; set; } = "form-select";
        public bool Disabled { get; set; }
        public bool Required { get; set; }
        public string Label { get; set; } = "Estudiante:";
        public List<SelectListItem> Estudiantes { get; set; } = new();
    }
}