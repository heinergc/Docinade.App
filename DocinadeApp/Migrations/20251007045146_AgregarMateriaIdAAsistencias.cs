using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocinadeApp.Migrations
{
    /// <inheritdoc />
    public partial class AgregarMateriaIdAAsistencias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Asistencias_Unique_Estudiante_Grupo_Fecha",
                table: "Asistencias");

            migrationBuilder.AddColumn<int>(
                name: "MateriaId",
                table: "Asistencias",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Asistencias_MateriaId",
                table: "Asistencias",
                column: "MateriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Asistencias_Unique_Estudiante_Grupo_Materia_Fecha",
                table: "Asistencias",
                columns: new[] { "EstudianteId", "GrupoId", "MateriaId", "Fecha" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Asistencias_Materias_MateriaId",
                table: "Asistencias",
                column: "MateriaId",
                principalTable: "Materias",
                principalColumn: "MateriaId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asistencias_Materias_MateriaId",
                table: "Asistencias");

            migrationBuilder.DropIndex(
                name: "IX_Asistencias_MateriaId",
                table: "Asistencias");

            migrationBuilder.DropIndex(
                name: "IX_Asistencias_Unique_Estudiante_Grupo_Materia_Fecha",
                table: "Asistencias");

            migrationBuilder.DropColumn(
                name: "MateriaId",
                table: "Asistencias");

            migrationBuilder.CreateIndex(
                name: "IX_Asistencias_Unique_Estudiante_Grupo_Fecha",
                table: "Asistencias",
                columns: new[] { "EstudianteId", "GrupoId", "Fecha" },
                unique: true);
        }
    }
}
