namespace DocinadeApp.DTOs
{
    /// <summary>Datos planos de estudiante para listados y selects.</summary>
    public record EstudianteDto(
        int IdEstudiante,
        string Nombre,
        string Apellidos,
        string NumeroId,
        string DireccionCorreo,
        string Institucion,
        int Anio,
        bool EstaActivo
    )
    {
        public string NombreCompleto => $"{Apellidos}, {Nombre}";
        public string NombreConId => $"{Apellidos}, {Nombre} ({NumeroId})";
    }
}
