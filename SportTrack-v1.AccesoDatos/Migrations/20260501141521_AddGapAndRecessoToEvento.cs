using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportTrack_v1.AccesoDatos.Migrations
{
    /// <inheritdoc />
    public partial class AddGapAndRecessoToEvento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GapEntrePruebas",
                schema: "regatas",
                table: "Eventos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SinReceso",
                schema: "regatas",
                table: "Eventos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TimeZoneId",
                schema: "regatas",
                table: "Eventos",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GapEntrePruebas",
                schema: "regatas",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "SinReceso",
                schema: "regatas",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "TimeZoneId",
                schema: "regatas",
                table: "Eventos");
        }
    }
}
