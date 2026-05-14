using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportTrack_v1.AccesoDatos.Migrations
{
    /// <inheritdoc />
    public partial class AddIntentosFallidos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserAgent",
                table: "Auditoria");

            migrationBuilder.AddColumn<int>(
                name: "IntentosFallidos",
                schema: "seguridad",
                table: "Usuarios",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                schema: "catalogos",
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 7,
                column: "EdadMax",
                value: 39);

            migrationBuilder.UpdateData(
                schema: "catalogos",
                table: "PlanesSaaS",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "MaxAtletas", "MaxTorneosActivos", "Nombre" },
                values: new object[] { 1000, 5, "Bronce" });

            migrationBuilder.UpdateData(
                schema: "catalogos",
                table: "PlanesSaaS",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ExportacionExcel", "MaxAtletas", "MaxTorneosActivos", "Nombre", "Precio", "ResultadosTiempoReal" },
                values: new object[] { false, 4000, 20, "Plata", 99m, false });

            migrationBuilder.UpdateData(
                schema: "catalogos",
                table: "PlanesSaaS",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Nombre", "Precio" },
                values: new object[] { "Oro", 250m });

            migrationBuilder.UpdateData(
                schema: "seguridad",
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "IntentosFallidos",
                value: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IntentosFallidos",
                schema: "seguridad",
                table: "Usuarios");

            migrationBuilder.AddColumn<string>(
                name: "UserAgent",
                table: "Auditoria",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                schema: "catalogos",
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 7,
                column: "EdadMax",
                value: 35);

            migrationBuilder.UpdateData(
                schema: "catalogos",
                table: "PlanesSaaS",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "MaxAtletas", "MaxTorneosActivos", "Nombre" },
                values: new object[] { 500, 1, "Básico" });

            migrationBuilder.UpdateData(
                schema: "catalogos",
                table: "PlanesSaaS",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ExportacionExcel", "MaxAtletas", "MaxTorneosActivos", "Nombre", "Precio", "ResultadosTiempoReal" },
                values: new object[] { true, 2000, 5, "Estándar", 50m, true });

            migrationBuilder.UpdateData(
                schema: "catalogos",
                table: "PlanesSaaS",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Nombre", "Precio" },
                values: new object[] { "Premium", 120m });
        }
    }
}
