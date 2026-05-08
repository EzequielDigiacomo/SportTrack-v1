using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportTrack_v1.AccesoDatos.Migrations
{
    /// <inheritdoc />
    public partial class AddHabilitacionesToEvento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BotesHabilitados",
                schema: "regatas",
                table: "Eventos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CategoriasHabilitadas",
                schema: "regatas",
                table: "Eventos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DistanciasHabilitadas",
                schema: "regatas",
                table: "Eventos",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                schema: "catalogos",
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 1,
                column: "EdadMin",
                value: 8);

            migrationBuilder.UpdateData(
                schema: "catalogos",
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "EdadMax", "EdadMin" },
                values: new object[] { 12, 11 });

            migrationBuilder.UpdateData(
                schema: "catalogos",
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "EdadMax", "EdadMin" },
                values: new object[] { 14, 13 });

            migrationBuilder.UpdateData(
                schema: "catalogos",
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "EdadMax", "EdadMin" },
                values: new object[] { 16, 15 });

            migrationBuilder.UpdateData(
                schema: "catalogos",
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "EdadMax", "EdadMin" },
                values: new object[] { 18, 17 });

            migrationBuilder.UpdateData(
                schema: "catalogos",
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "EdadMax", "EdadMin" },
                values: new object[] { 23, 19 });

            migrationBuilder.UpdateData(
                schema: "catalogos",
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 7,
                column: "EdadMin",
                value: 19);

            migrationBuilder.InsertData(
                schema: "catalogos",
                table: "Categorias",
                columns: new[] { "Id", "EdadMax", "EdadMin", "Nombre" },
                values: new object[] { 11, 99, 0, "Control" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "catalogos",
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DropColumn(
                name: "BotesHabilitados",
                schema: "regatas",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "CategoriasHabilitadas",
                schema: "regatas",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "DistanciasHabilitadas",
                schema: "regatas",
                table: "Eventos");

            migrationBuilder.UpdateData(
                schema: "catalogos",
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 1,
                column: "EdadMin",
                value: 9);

            migrationBuilder.UpdateData(
                schema: "catalogos",
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "EdadMax", "EdadMin" },
                values: new object[] { 11, 10 });

            migrationBuilder.UpdateData(
                schema: "catalogos",
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "EdadMax", "EdadMin" },
                values: new object[] { 13, 12 });

            migrationBuilder.UpdateData(
                schema: "catalogos",
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "EdadMax", "EdadMin" },
                values: new object[] { 15, 14 });

            migrationBuilder.UpdateData(
                schema: "catalogos",
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "EdadMax", "EdadMin" },
                values: new object[] { 17, 16 });

            migrationBuilder.UpdateData(
                schema: "catalogos",
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "EdadMax", "EdadMin" },
                values: new object[] { 22, 18 });

            migrationBuilder.UpdateData(
                schema: "catalogos",
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 7,
                column: "EdadMin",
                value: 18);
        }
    }
}
