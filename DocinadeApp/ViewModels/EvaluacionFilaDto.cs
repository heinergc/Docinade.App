namespace DocinadeApp.ViewModels
{
    // TODO: Filtro cascada - DTO para mostrar evaluaciones con datos completos
    public record EvaluacionFilaDto(
        int IdEvaluacion,
        string Grupo,
        string Estudiante,
        string Materia,
        string Instrumento,
        string Rubrica,
        DateTime Fecha,
        decimal? Total,
        string Estado
    );
}