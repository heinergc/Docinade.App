using RubricasApp.Web.Models;

namespace RubricasApp.Web.ViewModels
{
    public class GestionarValoresItemsViewModel
    {
        public int RubricaId { get; set; }
        public string RubricaNombre { get; set; } = string.Empty;
        public Rubrica Rubrica { get; set; } = new Rubrica(); // Para compatibilidad con la vista
        public List<ItemEvaluacion> Items { get; set; } = new List<ItemEvaluacion>();
        public List<NivelCalificacion> Niveles { get; set; } = new List<NivelCalificacion>();
        public List<NivelCalificacion> NivelesDisponibles { get; set; } = new List<NivelCalificacion>(); // Para compatibilidad
        public List<ValorRubrica> ValoresExistentes { get; set; } = new List<ValorRubrica>();
        public List<ItemEvaluacion> ItemsSinConfigurar { get; set; } = new List<ItemEvaluacion>();
        public List<ItemEvaluacion> ItemsSinValores { get; set; } = new List<ItemEvaluacion>(); // Para compatibilidad
        public Dictionary<int, Dictionary<int, ValorRubrica>> MatrizValores { get; set; } = new Dictionary<int, Dictionary<int, ValorRubrica>>(); // Para compatibilidad
        
        // Propiedades calculadas
        public int TotalItems => Items.Count;
        public int ItemsConfigurados => Items.Count - ItemsSinConfigurar.Count;
        public int ItemsSinConfiguracion => ItemsSinConfigurar.Count;
        
        // Método helper para verificar si un item tiene valor para un nivel
        public bool TieneValor(int itemId, int nivelId)
        {
            return ValoresExistentes.Any(v => v.IdItem == itemId && v.IdNivel == nivelId);
        }
        
        // Método helper para obtener el valor de un item para un nivel
        public decimal? ObtenerValor(int itemId, int nivelId)
        {
            var valor = ValoresExistentes.FirstOrDefault(v => v.IdItem == itemId && v.IdNivel == nivelId);
            return valor?.ValorPuntos;
        }
    }
    
    public class RubricaConfiguracionViewModel
    {
        public Rubrica Rubrica { get; set; } = new Rubrica();
        public List<NivelCalificacion> TodosLosNiveles { get; set; } = new List<NivelCalificacion>();
        public List<int> NivelesSeleccionados { get; set; } = new List<int>();
    }
    
    public class ItemConfiguracionViewModel
    {
        public int ItemId { get; set; }
        public string ItemNombre { get; set; } = string.Empty;
        public List<NivelValorViewModel> Niveles { get; set; } = new List<NivelValorViewModel>();
    }
    
    public class NivelValorViewModel
    {
        public int NivelId { get; set; }
        public string NivelNombre { get; set; } = string.Empty;
        public decimal? Valor { get; set; }
    }
    
    public class DiagnosticoItemsViewModel
    {
        public int RubricaId { get; set; }
        public string RubricaNombre { get; set; } = string.Empty;
        public int TotalItems { get; set; }
        public int ItemsConfigurados { get; set; }
        public int ItemsSinConfigurar { get; set; }
        public List<ItemDiagnosticoViewModel> DetalleItems { get; set; } = new List<ItemDiagnosticoViewModel>();
        public List<string> Problemas { get; set; } = new List<string>();
        public List<string> Recomendaciones { get; set; } = new List<string>();
    }
    
    public class ItemDiagnosticoViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int Orden { get; set; }
        public bool TieneValores { get; set; }
        public int NivelesConfigurados { get; set; }
        public int TotalNiveles { get; set; }
        public List<string> NivelesFaltantes { get; set; } = new List<string>();
        public string Estado => TieneValores ? "Configurado" : "Sin configurar";
    }
    
    public class AsignarNivelesRequest
    {
        public int ItemId { get; set; }
        public Dictionary<int, decimal?> Valores { get; set; } = new Dictionary<int, decimal?>();
    }
    
    public class AsignarNivelesResponse
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public int ValoresCreados { get; set; }
        public int ValoresActualizados { get; set; }
        public List<string> Errores { get; set; } = new List<string>();
    }
    
    // ViewModels para resolver el problema de binding dinámico
    public class GrupoViewModel
    {
        public int IdGrupo { get; set; }
        public string NombreGrupo { get; set; } = "";
        public int CantidadNiveles { get; set; }
    }

    public class NivelViewModel
    {
        public int IdNivel { get; set; }
        public string NombreNivel { get; set; } = "";
        public string Descripcion { get; set; } = "";
    }

    /// <summary>
    /// ViewModel para mostrar rúbricas con información de permisos del usuario
    /// </summary>
    public class RubricaWithPermissionsViewModel
    {
        public Rubrica Rubrica { get; set; } = null!;
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool CanView { get; set; }
        public bool IsOwner { get; set; }
        public string? CreatedByName { get; set; }
        public string? ModifiedByName { get; set; }
    }

    /// <summary>
    /// ViewModel para la lista de rúbricas con permisos
    /// </summary>
    public class RubricasIndexViewModel
    {
        public List<RubricaWithPermissionsViewModel> Rubricas { get; set; } = new();
        public bool CanCreateNew { get; set; }
        public string CurrentUserRole { get; set; } = string.Empty;
        public int TotalRubricas { get; set; }
        public int MisRubricas { get; set; }
        public int RubricasPublicas { get; set; }
    }
}