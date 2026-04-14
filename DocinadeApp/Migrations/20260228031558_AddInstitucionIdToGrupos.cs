using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocinadeApp.Migrations
{
    /// <inheritdoc />
    public partial class AddInstitucionIdToGrupos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InstitucionId",
                table: "GruposEstudiantes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GruposEstudiantes_InstitucionId",
                table: "GruposEstudiantes",
                column: "InstitucionId");

            migrationBuilder.AddForeignKey(
                name: "FK_GruposEstudiantes_Instituciones_InstitucionId",
                table: "GruposEstudiantes",
                column: "InstitucionId",
                principalTable: "Instituciones",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GruposEstudiantes_Instituciones_InstitucionId",
                table: "GruposEstudiantes");

            migrationBuilder.DropIndex(
                name: "IX_GruposEstudiantes_InstitucionId",
                table: "GruposEstudiantes");

            migrationBuilder.DropColumn(
                name: "InstitucionId",
                table: "GruposEstudiantes");
        }
    }
}
