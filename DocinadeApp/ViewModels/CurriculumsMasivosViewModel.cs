namespace RubricasApp.Web.ViewModels
{
    /// <summary>
    /// ViewModel para generación masiva de currículums
    /// </summary>
    public class CurriculumsMasivosViewModel
    {
        public List<ProfesorCurriculumViewModel> Curriculums { get; set; } = new List<ProfesorCurriculumViewModel>();
    }
}
