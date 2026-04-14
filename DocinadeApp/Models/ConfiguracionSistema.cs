using System.ComponentModel.DataAnnotations;

namespace DocinadeApp.Models
{
    public class ConfiguracionSistema
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Clave { get; set; } = null!;
        
        [Required]
        [MaxLength(200)]
        public string Valor { get; set; } = null!;
        
        [MaxLength(500)]
        public string? Descripcion { get; set; }
        
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        
        public DateTime FechaModificacion { get; set; } = DateTime.UtcNow;
        
        [MaxLength(100)]
        public string? UsuarioModificacion { get; set; }
    }

    public enum ModoRegistroUsuarios
    {
        Abierto,
        Cerrado
    }


}