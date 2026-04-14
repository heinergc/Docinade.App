using System.ComponentModel.DataAnnotations;

namespace RubricasApp.Web.DTOs
{
    /// <summary>Datos planos de rúbrica para selects y respuestas JSON.</summary>
    public record RubricaDto(
        int IdRubrica,
        string NombreRubrica,
        string? Descripcion,
        string? Estado,
        bool EsPublica
    );

    /// <summary>Datos para crear una rúbrica.</summary>
    public class RubricaCreateDto
    {
        [Required(ErrorMessage = "El nombre de la rúbrica es obligatorio.")]
        [StringLength(100)]
        public string NombreRubrica { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Descripcion { get; set; }

        public int? IdGrupo { get; set; }
        public int EsPublica { get; set; } = 1;
    }

    /// <summary>Datos para actualizar una rúbrica existente.</summary>
    public class RubricaUpdateDto : RubricaCreateDto
    {
        public int IdRubrica { get; set; }
    }

    /// <summary>Item de evaluación para respuestas JSON (endpoint ObtenerItemsPorRubrica).</summary>
    public record ItemEvaluacionDto(
        int IdItem,
        string NombreItem,
        string? Descripcion,
        decimal Peso,
        int OrdenItem
    );

    /// <summary>Valor de nivel para respuestas JSON (endpoint ObtenerItemsPorRubrica).</summary>
    public record ValorNivelDto(
        int IdNivel,
        string NombreNivel,
        int IdItem,
        decimal Valor
    );

    /// <summary>Respuesta tipada del endpoint ObtenerItemsPorRubrica.</summary>
    public record RubricaItemsResponse(
        IEnumerable<ItemEvaluacionDto> Items,
        IEnumerable<ValorNivelDto> Niveles
    );
}
