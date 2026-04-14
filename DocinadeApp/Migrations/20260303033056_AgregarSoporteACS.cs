using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RubricasApp.Web.Migrations
{
    /// <inheritdoc />
    public partial class AgregarSoporteACS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Estudiantes_PeriodosAcademicos_PeriodoAcademicoId",
                table: "Estudiantes");

            migrationBuilder.AddColumn<bool>(
                name: "AplicarACSPeriodosAnteriores",
                table: "Estudiantes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DetallesACS",
                table: "Estudiantes",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaInicioACS",
                table: "Estudiantes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PeriodoInicioACSId",
                table: "Estudiantes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoAdecuacion",
                table: "Estudiantes",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "NoPresenta");

            migrationBuilder.CreateTable(
                name: "EstudiantesInstrumentosACS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EstudianteId = table.Column<int>(type: "int", nullable: false),
                    InstrumentoEvaluacionId = table.Column<int>(type: "int", nullable: false),
                    PeriodoAcademicoId = table.Column<int>(type: "int", nullable: false),
                    RubricaModificadaId = table.Column<int>(type: "int", nullable: true),
                    PonderacionPersonalizadaPorcentaje = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    Exento = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    MotivoExencion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CriteriosAdaptados = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioCreacion = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UsuarioModificacion = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstudiantesInstrumentosACS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EstudiantesInstrumentosACS_Estudiantes_EstudianteId",
                        column: x => x.EstudianteId,
                        principalTable: "Estudiantes",
                        principalColumn: "IdEstudiante",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EstudiantesInstrumentosACS_InstrumentosEvaluacion_InstrumentoEvaluacionId",
                        column: x => x.InstrumentoEvaluacionId,
                        principalTable: "InstrumentosEvaluacion",
                        principalColumn: "InstrumentoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EstudiantesInstrumentosACS_PeriodosAcademicos_PeriodoAcademicoId",
                        column: x => x.PeriodoAcademicoId,
                        principalTable: "PeriodosAcademicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EstudiantesInstrumentosACS_Rubricas_RubricaModificadaId",
                        column: x => x.RubricaModificadaId,
                        principalTable: "Rubricas",
                        principalColumn: "IdRubrica",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Estudiantes_PeriodoInicioACSId",
                table: "Estudiantes",
                column: "PeriodoInicioACSId");

            migrationBuilder.CreateIndex(
                name: "IX_EstudianteInstrumentoACS_Unique",
                table: "EstudiantesInstrumentosACS",
                columns: new[] { "EstudianteId", "InstrumentoEvaluacionId", "PeriodoAcademicoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EstudiantesInstrumentosACS_EstudianteId",
                table: "EstudiantesInstrumentosACS",
                column: "EstudianteId");

            migrationBuilder.CreateIndex(
                name: "IX_EstudiantesInstrumentosACS_InstrumentoEvaluacionId",
                table: "EstudiantesInstrumentosACS",
                column: "InstrumentoEvaluacionId");

            migrationBuilder.CreateIndex(
                name: "IX_EstudiantesInstrumentosACS_PeriodoAcademicoId",
                table: "EstudiantesInstrumentosACS",
                column: "PeriodoAcademicoId");

            migrationBuilder.CreateIndex(
                name: "IX_EstudiantesInstrumentosACS_RubricaModificadaId",
                table: "EstudiantesInstrumentosACS",
                column: "RubricaModificadaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Estudiantes_PeriodosAcademicos_PeriodoAcademicoId",
                table: "Estudiantes",
                column: "PeriodoAcademicoId",
                principalTable: "PeriodosAcademicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Estudiantes_PeriodosAcademicos_PeriodoInicioACSId",
                table: "Estudiantes",
                column: "PeriodoInicioACSId",
                principalTable: "PeriodosAcademicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Estudiantes_PeriodosAcademicos_PeriodoAcademicoId",
                table: "Estudiantes");

            migrationBuilder.DropForeignKey(
                name: "FK_Estudiantes_PeriodosAcademicos_PeriodoInicioACSId",
                table: "Estudiantes");

            migrationBuilder.DropTable(
                name: "EstudiantesInstrumentosACS");

            migrationBuilder.DropIndex(
                name: "IX_Estudiantes_PeriodoInicioACSId",
                table: "Estudiantes");

            migrationBuilder.DropColumn(
                name: "AplicarACSPeriodosAnteriores",
                table: "Estudiantes");

            migrationBuilder.DropColumn(
                name: "DetallesACS",
                table: "Estudiantes");

            migrationBuilder.DropColumn(
                name: "FechaInicioACS",
                table: "Estudiantes");

            migrationBuilder.DropColumn(
                name: "PeriodoInicioACSId",
                table: "Estudiantes");

            migrationBuilder.DropColumn(
                name: "TipoAdecuacion",
                table: "Estudiantes");

            migrationBuilder.AddForeignKey(
                name: "FK_Estudiantes_PeriodosAcademicos_PeriodoAcademicoId",
                table: "Estudiantes",
                column: "PeriodoAcademicoId",
                principalTable: "PeriodosAcademicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
