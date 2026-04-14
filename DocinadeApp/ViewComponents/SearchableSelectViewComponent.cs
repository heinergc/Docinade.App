using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace DocinadeApp.ViewComponents
{
    /// <summary>
    /// ViewComponent para crear select con búsqueda usando Tom Select o Select2
    /// </summary>
    public class SearchableSelectViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(SearchableSelectViewModel model)
        {
            // Validar parámetros requeridos
            if (model.AspFor == null && string.IsNullOrEmpty(model.Name))
            {
                throw new ArgumentException("Debe especificar 'AspFor' o 'Name'");
            }

            // Establecer valores por defecto
            if (string.IsNullOrEmpty(model.Library))
                model.Library = "tomselect";
            
            if (string.IsNullOrEmpty(model.Placeholder))
                model.Placeholder = "Buscar...";
            
            if (!model.MinimumInputLength.HasValue)
                model.MinimumInputLength = !string.IsNullOrEmpty(model.DataEndpoint) ? 2 : 0;
            
            if (model.DebounceMs <= 0)
                model.DebounceMs = 250;

            // Determinar nombre, ID y valor
            model.ActualName = model.AspFor?.Name ?? model.Name;
            model.ActualId = model.Id ?? model.ActualName;
            model.ActualValue = model.AspFor?.Model ?? model.Value;

            // Marcar que se necesitan los assets
            ViewContext.HttpContext.Items["SearchableSelectAssetsNeeded"] = true;

            return View(model);
        }
    }

    /// <summary>
    /// Modelo para el ViewComponent SearchableSelect
    /// </summary>
    public class SearchableSelectViewModel
    {
        /// <summary>
        /// Expresión del modelo para binding automático
        /// </summary>
        public ModelExpression AspFor { get; set; }

        /// <summary>
        /// Nombre del campo (alternativa a AspFor)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Valor seleccionado (alternativa a AspFor)
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Lista de elementos para búsqueda local
        /// </summary>
        public IEnumerable<SelectListItem> Items { get; set; }

        /// <summary>
        /// Endpoint para búsqueda remota
        /// </summary>
        public string DataEndpoint { get; set; }

        /// <summary>
        /// Texto del placeholder
        /// </summary>
        public string Placeholder { get; set; } = "Buscar...";

        /// <summary>
        /// Permitir limpiar selección
        /// </summary>
        public bool AllowClear { get; set; } = true;

        /// <summary>
        /// Mínimo de caracteres para iniciar búsqueda
        /// </summary>
        public int? MinimumInputLength { get; set; }

        /// <summary>
        /// Selección múltiple
        /// </summary>
        public bool Multiple { get; set; }

        /// <summary>
        /// Campo deshabilitado
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// Clases CSS adicionales
        /// </summary>
        public string CssClass { get; set; }

        /// <summary>
        /// Librería a usar: tomselect (default) o select2
        /// </summary>
        public string Library { get; set; } = "tomselect";

        /// <summary>
        /// Máximo de opciones para Tom Select
        /// </summary>
        public int? MaxOptions { get; set; }

        /// <summary>
        /// Datos extra para enviar en petición remota (JSON)
        /// </summary>
        public string DataExtra { get; set; }

        /// <summary>
        /// Tiempo de debounce en milisegundos
        /// </summary>
        public int DebounceMs { get; set; } = 250;

        /// <summary>
        /// ID del campo
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Atributos requeridos
        /// </summary>
        public bool Required { get; set; }

        // Propiedades calculadas internamente
        public string ActualName { get; set; }
        public string ActualId { get; set; }
        public object ActualValue { get; set; }

        /// <summary>
        /// Verifica si un valor está seleccionado
        /// </summary>
        public bool IsSelected(string itemValue)
        {
            if (ActualValue == null && string.IsNullOrEmpty(itemValue))
                return true;
            
            if (ActualValue == null)
                return false;

            // Manejar múltiples valores
            if (Multiple && ActualValue is IEnumerable<object> enumerable && !(ActualValue is string))
            {
                return enumerable.Any(v => string.Equals(v?.ToString(), itemValue, StringComparison.OrdinalIgnoreCase));
            }

            return string.Equals(ActualValue.ToString(), itemValue, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Verifica si el valor actual está vacío
        /// </summary>
        public bool IsEmptyValue()
        {
            if (ActualValue == null)
                return true;

            if (ActualValue is string str)
                return string.IsNullOrEmpty(str);

            if (ActualValue is IEnumerable<object> enumerable && !(ActualValue is string))
                return !enumerable.Any();

            return false;
        }
    }
}
