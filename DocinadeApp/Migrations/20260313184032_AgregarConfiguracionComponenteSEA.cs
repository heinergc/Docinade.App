using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocinadeApp.Migrations
{
    /// <inheritdoc />
    public partial class AgregarConfiguracionComponenteSEA : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConfiguracionesComponenteSEA",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MateriaId = table.Column<int>(type: "int", nullable: false),
                    InstrumentoEvaluacionId = table.Column<int>(type: "int", nullable: false),
                    ComponenteSEA = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Porcentaje = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaConfiguracion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UsuarioConfiguracion = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionesComponenteSEA", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConfiguracionesComponenteSEA_InstrumentosEvaluacion_InstrumentoEvaluacionId",
                        column: x => x.InstrumentoEvaluacionId,
                        principalTable: "InstrumentosEvaluacion",
                        principalColumn: "InstrumentoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConfiguracionesComponenteSEA_Materias_MateriaId",
                        column: x => x.MateriaId,
                        principalTable: "Materias",
                        principalColumn: "MateriaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracionesComponenteSEA_InstrumentoEvaluacionId",
                table: "ConfiguracionesComponenteSEA",
                column: "InstrumentoEvaluacionId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracionesComponenteSEA_MateriaId_ComponenteSEA",
                table: "ConfiguracionesComponenteSEA",
                columns: new[] { "MateriaId", "ComponenteSEA" });

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracionesComponenteSEA_MateriaId_InstrumentoEvaluacionId",
                table: "ConfiguracionesComponenteSEA",
                columns: new[] { "MateriaId", "InstrumentoEvaluacionId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfiguracionesComponenteSEA");
        }
    }
}
