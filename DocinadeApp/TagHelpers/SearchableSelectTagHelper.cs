using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text.Encodings.Web;

namespace RubricasApp.Web.TagHelpers
{
    /// <summary>
    /// TagHelper para crear select con búsqueda usando Tom Select o Select2
    /// </summary>
    [HtmlTargetElement("searchable-select")]
    public class SearchableSelectTagHelper : TagHelper
    {
        private readonly IHtmlGenerator _generator;

        public SearchableSelectTagHelper(IHtmlGenerator generator)
        {
            _generator = generator;
        }

        [ViewContext]
        public ViewContext ViewContext { get; set; } = null!;

        /// <summary>
        /// Expresión del modelo para binding automático
        /// </summary>
        [HtmlAttributeName("asp-for")]
        public ModelExpression? AspFor { get; set; }

        /// <summary>
        /// Nombre del campo (alternativa a asp-for)
        /// </summary>
        [HtmlAttributeName("name")]
        public string? Name { get; set; }

        /// <summary>
        /// Valor seleccionado (alternativa a asp-for)
        /// </summary>
        [HtmlAttributeName("value")]
        public object? Value { get; set; }

        /// <summary>
        /// Lista de elementos para búsqueda local
        /// </summary>
        [HtmlAttributeName("items")]
        public IEnumerable<SelectListItem>? Items { get; set; }

        /// <summary>
        /// Endpoint para búsqueda remota
        /// </summary>
        [HtmlAttributeName("endpoint")]
        public string? DataEndpoint { get; set; }

        /// <summary>
        /// Texto del placeholder
        /// </summary>
        [HtmlAttributeName("placeholder")]
        public string Placeholder { get; set; } = "Buscar...";

        /// <summary>
        /// Permitir limpiar selección
        /// </summary>
        [HtmlAttributeName("allow-clear")]
        public bool AllowClear { get; set; } = true;

        /// <summary>
        /// Mínimo de caracteres para iniciar búsqueda
        /// </summary>
        [HtmlAttributeName("minimum-input-length")]
        public int? MinimumInputLength { get; set; }

        /// <summary>
        /// Selección múltiple
        /// </summary>
        [HtmlAttributeName("multiple")]
        public bool Multiple { get; set; }

        /// <summary>
        /// Campo deshabilitado
        /// </summary>
        [HtmlAttributeName("disabled")]
        public bool Disabled { get; set; }

        /// <summary>
        /// Clases CSS adicionales
        /// </summary>
        [HtmlAttributeName("css-class")]
        public string? CssClass { get; set; }

        /// <summary>
        /// Librería a usar: tomselect (default) o select2
        /// </summary>
        [HtmlAttributeName("library")]
        public string Library { get; set; } = "tomselect";

        /// <summary>
        /// Máximo de opciones para Tom Select
        /// </summary>
        [HtmlAttributeName("max-options")]
        public int? MaxOptions { get; set; }

        /// <summary>
        /// Datos extra para enviar en petición remota (JSON)
        /// </summary>
        [HtmlAttributeName("extra")]
        public string? DataExtra { get; set; }

        /// <summary>
        /// Tiempo de debounce en milisegundos
        /// </summary>
        [HtmlAttributeName("debounce-ms")]
        public int DebounceMs { get; set; } = 250;

        /// <summary>
        /// ID del campo
        /// </summary>
        [HtmlAttributeName("id")]
        public string? Id { get; set; }

        /// <summary>
        /// Atributos requeridos
        /// </summary>
        [HtmlAttributeName("required")]
        public bool Required { get; set; }

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            // Cambiar el tag a select
            output.TagName = "select";

            // Determinar nombre e ID
            var name = AspFor?.Name ?? Name;
            var id = Id ?? AspFor?.Name ?? Name;
            var value = AspFor?.Model ?? Value;

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Debe especificar 'asp-for' o 'name'");
            }

            // Configurar atributos básicos del select
            output.Attributes.SetAttribute("name", name);
            output.Attributes.SetAttribute("id", id);

            // Clases CSS
            var cssClasses = new List<string> { "form-select", "searchable-select" };
            if (!string.IsNullOrEmpty(CssClass))
            {
                cssClasses.AddRange(CssClass.Split(' ', StringSplitOptions.RemoveEmptyEntries));
            }
            output.Attributes.SetAttribute("class", string.Join(" ", cssClasses));

            // Atributos condicionales
            if (Multiple)
            {
                output.Attributes.SetAttribute("multiple", "multiple");
            }

            if (Disabled)
            {
                output.Attributes.SetAttribute("disabled", "disabled");
            }

            if (Required)
            {
                output.Attributes.SetAttribute("required", "required");
            }

            // Configuración del validador MVC
            if (AspFor != null)
            {
                // Note: GetValidationAttributes no está disponible en .NET 8
                // Los atributos de validación se manejarán automáticamente por el framework
                output.Attributes.SetAttribute("data-val", "true");
                output.Attributes.SetAttribute("data-val-required", $"El campo {AspFor.Name} es requerido.");
            }

            // Data attributes para configuración
            output.Attributes.SetAttribute("data-searchable-library", Library.ToLowerInvariant());
            output.Attributes.SetAttribute("data-searchable-placeholder", Placeholder);
            output.Attributes.SetAttribute("data-searchable-allow-clear", AllowClear.ToString().ToLowerInvariant());
            output.Attributes.SetAttribute("data-searchable-debounce", DebounceMs.ToString());

            if (!string.IsNullOrEmpty(DataEndpoint))
            {
                output.Attributes.SetAttribute("data-searchable-endpoint", DataEndpoint);
                output.Attributes.SetAttribute("data-searchable-minimum-input", (MinimumInputLength ?? 2).ToString());
            }
            else
            {
                output.Attributes.SetAttribute("data-searchable-minimum-input", (MinimumInputLength ?? 0).ToString());
            }

            if (MaxOptions.HasValue)
            {
                output.Attributes.SetAttribute("data-searchable-max-options", MaxOptions.Value.ToString());
            }

            if (!string.IsNullOrEmpty(DataExtra))
            {
                output.Attributes.SetAttribute("data-searchable-extra", DataExtra);
            }

            // Generar contenido del select
            GenerateSelectContent(output, name, value);

            // Incluir assets una sola vez
            IncludeAssets(output);

            return Task.CompletedTask;
        }

        private void GenerateSelectContent(TagHelperOutput output, string name, object? value)
        {
            var content = new HtmlContentBuilder();

            // Opción placeholder si está permitido
            if (AllowClear && !Multiple)
            {
                content.AppendHtml($"<option value=\"\">{HtmlEncoder.Default.Encode(Placeholder)}</option>");
            }

            // Si hay items locales, agregarlos
            if (Items != null)
            {
                foreach (var item in Items)
                {
                    var selected = IsSelected(item.Value ?? "", value);
                    var selectedAttr = selected ? " selected=\"selected\"" : "";
                    var disabledAttr = item.Disabled ? " disabled=\"disabled\"" : "";
                    
                    content.AppendHtml($"<option value=\"{HtmlEncoder.Default.Encode(item.Value ?? "")}\"{selectedAttr}{disabledAttr}>");
                    content.AppendHtml(HtmlEncoder.Default.Encode(item.Text ?? ""));
                    content.AppendHtml("</option>");
                }
            }
            // Si es remoto y hay valor preseleccionado, agregar opción temporal
            else if (!string.IsNullOrEmpty(DataEndpoint) && value != null && !IsEmptyValue(value))
            {
                var valueStr = value.ToString();
                if (!string.IsNullOrEmpty(valueStr))
                {
                    content.AppendHtml($"<option value=\"{HtmlEncoder.Default.Encode(valueStr)}\" selected=\"selected\">");
                    content.AppendHtml($"Cargando... (ID: {HtmlEncoder.Default.Encode(valueStr)})");
                    content.AppendHtml("</option>");
                }
            }

            output.Content.SetHtmlContent(content);
        }

        private bool IsSelected(string itemValue, object? currentValue)
        {
            if (currentValue == null && string.IsNullOrEmpty(itemValue))
                return true;
            
            if (currentValue == null)
                return false;

            // Manejar múltiples valores
            if (Multiple && currentValue is IEnumerable<object> enumerable && !(currentValue is string))
            {
                return enumerable.Any(v => string.Equals(v?.ToString(), itemValue, StringComparison.OrdinalIgnoreCase));
            }

            return string.Equals(currentValue.ToString(), itemValue, StringComparison.OrdinalIgnoreCase);
        }

        private bool IsEmptyValue(object? value)
        {
            if (value == null)
                return true;

            if (value is string str)
                return string.IsNullOrEmpty(str);

            if (value is IEnumerable<object> enumerable && !(value is string))
                return !enumerable.Any();

            return false;
        }

        private void IncludeAssets(TagHelperOutput output)
        {
            // Marcar que se necesitan los assets
            ViewContext.HttpContext.Items["SearchableSelectAssetsNeeded"] = true;
            
            // Agregar el ID del elemento a la lista de elementos para inicializar
            var elementId = Id ?? AspFor?.Name ?? Name;
            var elementsToInit = ViewContext.HttpContext.Items["SearchableSelectElementsToInit"] as List<string>;
            if (elementsToInit == null)
            {
                elementsToInit = new List<string>();
                ViewContext.HttpContext.Items["SearchableSelectElementsToInit"] = elementsToInit;
            }
            elementsToInit.Add(elementId!);
            
            // NO agregar scripts aquí - se manejarán en el partial view
        }
    }
}
