using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RubricasApp.Web.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTablaAsistencias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Asistencias",
                columns: table => new
                {
                    AsistenciaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EstudianteId = table.Column<int>(type: "int", nullable: false),
                    GrupoId = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    Justificacion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    RegistradoPorId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    HoraLlegada = table.Column<TimeSpan>(type: "time", nullable: true),
                    EsModificacion = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificadoPorId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asistencias", x => x.AsistenciaId);
                    table.ForeignKey(
                        name: "FK_Asistencias_AspNetUsers_ModificadoPorId",
                        column: x => x.ModificadoPorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Asistencias_AspNetUsers_RegistradoPorId",
                        column: x => x.RegistradoPorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Asistencias_Estudiantes_EstudianteId",
                        column: x => x.EstudianteId,
                        principalTable: "Estudiantes",
                        principalColumn: "IdEstudiante",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Asistencias_GruposEstudiantes_GrupoId",
                        column: x => x.GrupoId,
                        principalTable: "GruposEstudiantes",
                        principalColumn: "GrupoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Asistencias_Estado",
                table: "Asistencias",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_Asistencias_Fecha",
                table: "Asistencias",
                column: "Fecha");

            migrationBuilder.CreateIndex(
                name: "IX_Asistencias_FechaRegistro",
                table: "Asistencias",
                column: "FechaRegistro");

            migrationBuilder.CreateIndex(
                name: "IX_Asistencias_GrupoId",
                table: "Asistencias",
                column: "GrupoId");

            migrationBuilder.CreateIndex(
                name: "IX_Asistencias_ModificadoPorId",
                table: "Asistencias",
                column: "ModificadoPorId");

            migrationBuilder.CreateIndex(
                name: "IX_Asistencias_RegistradoPorId",
                table: "Asistencias",
                column: "RegistradoPorId");

            migrationBuilder.CreateIndex(
                name: "IX_Asistencias_Unique_Estudiante_Grupo_Fecha",
                table: "Asistencias",
                columns: new[] { "EstudianteId", "GrupoId", "Fecha" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Asistencias");
        }
    }
}
