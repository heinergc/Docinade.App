namespace RubricasApp.Web.Models
{
    public class MateriaRequisito
    {
        public int MateriaId { get; set; }
        public Materia Materia { get; set; }

        public int RequisitoId { get; set; }
        public Materia Requisito { get; set; }
    }
}
