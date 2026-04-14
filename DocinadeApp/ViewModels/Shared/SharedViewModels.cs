namespace DocinadeApp.ViewModels.Shared
{
    public class PeriodoAcademicoDropdownViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
    }

    public class DiagnosticoSistemaViewModel
    {
        public bool ConexionDB { get; set; }
        public int TotalGrupos { get; set; }
        public int TotalEstudiantes { get; set; }
        public int TotalMaterias { get; set; }
        public int TotalPeriodos { get; set; }
        public List<GrupoSimpleViewModel> GruposSimples { get; set; } = new List<GrupoSimpleViewModel>();
    }

    public class GrupoSimpleViewModel
    {
        public int GrupoId { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string TipoGrupo { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
    }
}