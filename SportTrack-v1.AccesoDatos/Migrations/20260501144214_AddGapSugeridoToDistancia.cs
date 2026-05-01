using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportTrack_v1.AccesoDatos.Migrations
{
    /// <inheritdoc />
    public partial class AddGapSugeridoToDistancia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GapSugerido",
                schema: "catalogos",
                table: "Distancias",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                schema: "catalogos",
                table: "Distancias",
                keyColumn: "Id",
                keyValue: 1,
                column: "GapSugerido",
                value: 10);

            migrationBuilder.UpdateData(
                schema: "catalogos",
                table: "Distancias",
                keyColumn: "Id",
                keyValue: 2,
                column: "GapSugerido",
                value: 10);

            migrationBuilder.UpdateData(
                schema: "catalogos",
                table: "Distancias",
                keyColumn: "Id",
                keyValue: 3,
                column: "GapSugerido",
                value: 10);

            migrationBuilder.UpdateData(
                schema: "catalogos",
                table: "Distancias",
                keyColumn: "Id",
                keyValue: 4,
                column: "GapSugerido",
                value: 10);

            migrationBuilder.UpdateData(
                schema: "catalogos",
                table: "Distancias",
                keyColumn: "Id",
                keyValue: 5,
                column: "GapSugerido",
                value: 10);

            migrationBuilder.UpdateData(
                schema: "catalogos",
                table: "Distancias",
                keyColumn: "Id",
                keyValue: 6,
                column: "GapSugerido",
                value: 10);

            migrationBuilder.UpdateData(
                schema: "catalogos",
                table: "Distancias",
                keyColumn: "Id",
                keyValue: 7,
                column: "GapSugerido",
                value: 10);

            migrationBuilder.UpdateData(
                schema: "catalogos",
                table: "Distancias",
                keyColumn: "Id",
                keyValue: 8,
                column: "GapSugerido",
                value: 10);

            migrationBuilder.UpdateData(
                schema: "catalogos",
                table: "Distancias",
                keyColumn: "Id",
                keyValue: 9,
                column: "GapSugerido",
                value: 10);

            migrationBuilder.UpdateData(
                schema: "catalogos",
                table: "Distancias",
                keyColumn: "Id",
                keyValue: 10,
                column: "GapSugerido",
                value: 10);

            migrationBuilder.UpdateData(
                schema: "catalogos",
                table: "Distancias",
                keyColumn: "Id",
                keyValue: 11,
                column: "GapSugerido",
                value: 10);

            migrationBuilder.UpdateData(
                schema: "catalogos",
                table: "Distancias",
                keyColumn: "Id",
                keyValue: 12,
                column: "GapSugerido",
                value: 10);

            migrationBuilder.UpdateData(
                schema: "catalogos",
                table: "Distancias",
                keyColumn: "Id",
                keyValue: 13,
                column: "GapSugerido",
                value: 10);

            migrationBuilder.UpdateData(
                schema: "catalogos",
                table: "Distancias",
                keyColumn: "Id",
                keyValue: 14,
                column: "GapSugerido",
                value: 10);

            migrationBuilder.UpdateData(
                schema: "catalogos",
                table: "Distancias",
                keyColumn: "Id",
                keyValue: 15,
                column: "GapSugerido",
                value: 10);

            migrationBuilder.UpdateData(
                schema: "catalogos",
                table: "Distancias",
                keyColumn: "Id",
                keyValue: 16,
                column: "GapSugerido",
                value: 10);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GapSugerido",
                schema: "catalogos",
                table: "Distancias");
        }
    }
}
