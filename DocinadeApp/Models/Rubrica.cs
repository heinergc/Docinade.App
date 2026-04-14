using DocinadeApp.Models.Identity;
using System.ComponentModel.DataAnnotations;
using DocinadeApp.Models.Academic;
using DocinadeApp.Interfaces;

namespace DocinadeApp.Models
{
    public class Rubrica : IAuditable, IUserOwned
    {
        public int IdRubrica { get; set; }

        [Required(ErrorMessage = "El nombre de la rúbrica es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres.")]
        [Display(Name = "Nombre de la Rúbrica")]
        public string NombreRubrica { get; set; } = string.Empty;

        [StringLength(1000)]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [Display(Name = "Fecha de Creación")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [StringLength(20)]
        [Display(Name = "Estado")]
        public string? Estado { get; set; } = "ACTIVO";

        [Display(Name = "Es Pública")]
        public int EsPublica { get; set; } = 1;

        public int? IdGrupo { get; set; }
        public string? CreadoPorId { get; set; }
        public string? ModificadoPorId { get; set; }
        public DateTime? FechaModificacion { get; set; }

        // Navigation properties
        public virtual GrupoCalificacion? GrupoCalificacion { get; set; }
        public virtual ApplicationUser? CreadoPor { get; set; }
        public virtual ApplicationUser? ModificadoPor { get; set; }
        public virtual ICollection<ItemEvaluacion> ItemsEvaluacion { get; set; } = new List<ItemEvaluacion>();
        public virtual ICollection<ValorRubrica> ValoresRubrica { get; set; } = new List<ValorRubrica>();
        public virtual ICollection<RubricaNivel> RubricaNiveles { get; set; } = new List<RubricaNivel>();
        public virtual ICollection<Evaluacion> Evaluaciones { get; set; } = new List<Evaluacion>();

        // Propiedades de compatibilidad para código existente
        [Display(Name = "Título")]
        public string Titulo 
        { 
            get => NombreRubrica; 
            set => NombreRubrica = value; 
        }

        [Display(Name = "Vigente")]
        public bool Vigente 
        { 
            get => Estado == "ACTIVO"; 
            set => Estado = value ? "ACTIVO" : "INACTIVO"; 
        }

        /// <summary>
        /// Verifica si la rúbrica pertenece al usuario especificado
        /// </summary>
        public bool BelongsToUser(string userId)
        {
            return !string.IsNullOrEmpty(CreadoPorId) && CreadoPorId == userId;
        }

        /// <summary>
        /// Verifica si el usuario puede editar esta rúbrica
        /// </summary>
        public bool CanBeEditedBy(string userId)
        {
            // Lógica adicional: también puede editar si fue modificada por él recientemente
            return BelongsToUser(userId) || 
                   (!string.IsNullOrEmpty(ModificadoPorId) && ModificadoPorId == userId);
        }
        
    }
}


