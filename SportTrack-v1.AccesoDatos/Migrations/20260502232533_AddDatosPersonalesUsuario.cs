using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportTrack_v1.AccesoDatos.Migrations
{
    /// <inheritdoc />
    public partial class AddDatosPersonalesUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Apellido",
                schema: "seguridad",
                table: "Usuarios",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Dni",
                schema: "seguridad",
                table: "Usuarios",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Nombre",
                schema: "seguridad",
                table: "Usuarios",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Telefono",
                schema: "seguridad",
                table: "Usuarios",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                schema: "seguridad",
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Apellido", "Dni", "Nombre", "Telefono" },
                values: new object[] { null, null, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Apellido",
                schema: "seguridad",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Dni",
                schema: "seguridad",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Nombre",
                schema: "seguridad",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Telefono",
                schema: "seguridad",
                table: "Usuarios");
        }
    }
}
