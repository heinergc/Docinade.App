using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DocinadeApp.Models.ViewModels
{
    public class MateriaViewModel
    {
        [Required]
        public Materia Materia { get; set; } = new Materia();
        
        [Display(Name = "Prerrequisitos")]
        public List<int>? SelectedPrerequisitos { get; set; } = new List<int>();
        
        public MultiSelectList? PrerequisitosList { get; set; }
        
        [Display(Name = "Rúbricas Asignadas")]
        public List<int>? SelectedRubricas { get; set; } = new List<int>();
        
        public MultiSelectList? RubricasList { get; set; }
        
        // Información adicional de las rúbricas asignadas para la vista
        public List<RubricaAsignadaInfo>? RubricasAsignadas { get; set; } = new List<RubricaAsignadaInfo>();
    }
    
    public class RubricaAsignadaInfo
    {
        public int IdRubrica { get; set; }
        public string NombreRubrica { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public string? CreadoPor { get; set; }
        public int EsPublica { get; set; }
        public string? GrupoCalificacion { get; set; }
        public int CantidadItems { get; set; }
    }
}
