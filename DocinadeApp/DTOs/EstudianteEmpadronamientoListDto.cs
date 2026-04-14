using System.ComponentModel.DataAnnotations;

namespace DocinadeApp.DTOs
{
    public class EstudianteEmpadronamientoListDto
    {
        public int IdEstudiante { get; set; }
        
        [Display(Name = "Nombre Completo")]
        public string NombreCompleto { get; set; } = string.Empty;
        
        [Display(Name = "Número de ID")]
        public string NumeroId { get; set; } = string.Empty;
        
        [Display(Name = "Correo Electrónico")]
        public string DireccionCorreo { get; set; } = string.Empty;
        
        [Display(Name = "Institución")]
        public string Institucion { get; set; } = string.Empty;
        
        [Display(Name = "Período Académico")]
        public string PeriodoNombre { get; set; } = string.Empty;
        
        [Display(Name = "Tiene Empadronamiento")]
        public bool TieneEmpadronamiento { get; set; }
        
        [Display(Name = "Etapa Actual")]
        public string? EtapaActual { get; set; }
    }
}