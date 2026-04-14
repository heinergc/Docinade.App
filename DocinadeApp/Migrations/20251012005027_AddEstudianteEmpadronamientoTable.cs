using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RubricasApp.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddEstudianteEmpadronamientoTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EstudiantesEmpadronamiento",
                columns: table => new
                {
                    IdEstudiante = table.Column<int>(type: "int", nullable: false),
                    NumeroId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    FechaNacimiento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Genero = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Nacionalidad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EstadoCivil = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Provincia = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Canton = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Distrito = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Barrio = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Senas = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TelefonoAlterno = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CorreoAlterno = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NombrePadre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NombreMadre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NombreTutor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ContactoEmergencia = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TelefonoEmergencia = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    RelacionEmergencia = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Alergias = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CondicionesMedicas = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Medicamentos = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SeguroMedico = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CentroMedicoHabitual = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    InstitucionProcedencia = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UltimoNivelCursado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PromedioAnterior = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    AdaptacionesPrevias = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DocumentosRecibidosJson = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    DocumentosPendientesJson = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    FechaEntregaDocumentos = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaVencimientoPoliza = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EtapaActual = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FechaEtapa = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioEtapa = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NotasInternas = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioCreacion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UsuarioModificacion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstudiantesEmpadronamiento", x => x.IdEstudiante);
                    table.ForeignKey(
                        name: "FK_EstudiantesEmpadronamiento_Estudiantes_IdEstudiante",
                        column: x => x.IdEstudiante,
                        principalTable: "Estudiantes",
                        principalColumn: "IdEstudiante",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EstudianteEmpadronamiento_EtapaActual",
                table: "EstudiantesEmpadronamiento",
                column: "EtapaActual");

            migrationBuilder.CreateIndex(
                name: "IX_EstudianteEmpadronamiento_FechaCreacion",
                table: "EstudiantesEmpadronamiento",
                column: "FechaCreacion");

            migrationBuilder.CreateIndex(
                name: "IX_EstudianteEmpadronamiento_NumeroId",
                table: "EstudiantesEmpadronamiento",
                column: "NumeroId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EstudiantesEmpadronamiento");
        }
    }
}
