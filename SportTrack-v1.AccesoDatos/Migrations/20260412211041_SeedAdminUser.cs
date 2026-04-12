using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportTrack_v1.AccesoDatos.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdminUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "seguridad",
                table: "Usuarios",
                columns: new[] { "Id", "Activo", "ClubId", "Email", "FechaCreacion", "PasswordHash", "Rol", "Username" },
                values: new object[] { 1, true, null, "admin@sporttrack.com", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "$2a$12$R9h/lSAbvI125hcnyqvQDu9fAKDLn6Y8yK/.Vz0uI3492M0h0mY3.", "Admin", "admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "seguridad",
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
