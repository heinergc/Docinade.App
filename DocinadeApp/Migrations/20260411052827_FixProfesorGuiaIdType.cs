using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocinadeApp.Migrations
{
    /// <inheritdoc />
    public partial class FixProfesorGuiaIdType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // TipoFaltaIdTipoFalta was a phantom shadow property that existed only in the
            // model snapshot but was never created in the actual database. No-op migration.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Nothing to undo.
        }
    }
}
