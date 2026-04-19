using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportTrack_v1.AccesoDatos.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEventoPruebaIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EventoPruebas_Unica",
                schema: "regatas",
                table: "EventoPruebas");

            migrationBuilder.CreateIndex(
                name: "IX_EventoPruebas_EventoPrueba_Fecha",
                schema: "regatas",
                table: "EventoPruebas",
                columns: new[] { "EventoId", "PruebaId", "FechaHora" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EventoPruebas_EventoPrueba_Fecha",
                schema: "regatas",
                table: "EventoPruebas");

            migrationBuilder.CreateIndex(
                name: "IX_EventoPruebas_Unica",
                schema: "regatas",
                table: "EventoPruebas",
                columns: new[] { "EventoId", "PruebaId" },
                unique: true);
        }
    }
}
