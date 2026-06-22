using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportTrack_v1.AccesoDatos.Migrations
{
    /// <inheritdoc />
    public partial class AddProgressionTraceability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FaseOrigenId",
                schema: "regatas",
                table: "Resultados",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReglaClasificacionAplicada",
                schema: "regatas",
                table: "Resultados",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlanProgresionAsignado",
                schema: "regatas",
                table: "EventoPruebas",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FaseOrigenId",
                schema: "regatas",
                table: "Resultados");

            migrationBuilder.DropColumn(
                name: "ReglaClasificacionAplicada",
                schema: "regatas",
                table: "Resultados");

            migrationBuilder.DropColumn(
                name: "PlanProgresionAsignado",
                schema: "regatas",
                table: "EventoPruebas");
        }
    }
}
