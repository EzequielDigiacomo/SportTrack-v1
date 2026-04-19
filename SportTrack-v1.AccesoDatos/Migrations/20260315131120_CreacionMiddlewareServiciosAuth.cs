using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SportTrack_v1.AccesoDatos.Migrations
{
    /// <inheritdoc />
    public partial class CreacionMiddlewareServiciosAuth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Participantes_Club",
                schema: "regatas",
                table: "Participantes");

            migrationBuilder.DropColumn(
                name: "Club",
                schema: "regatas",
                table: "Participantes");

            migrationBuilder.EnsureSchema(
                name: "seguridad");

            migrationBuilder.AddColumn<int>(
                name: "ClubId",
                schema: "regatas",
                table: "Participantes",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Clubes",
                schema: "catalogos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Sigla = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Telefono = table.Column<string>(type: "text", nullable: true),
                    Direccion = table.Column<string>(type: "text", nullable: true),
                    Ubicacion = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clubes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                schema: "seguridad",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Rol = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ClubId = table.Column<int>(type: "integer", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Usuarios_Clubes_ClubId",
                        column: x => x.ClubId,
                        principalSchema: "catalogos",
                        principalTable: "Clubes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Participantes_ClubId",
                schema: "regatas",
                table: "Participantes",
                column: "ClubId");

            migrationBuilder.CreateIndex(
                name: "IX_Clubes_Nombre",
                schema: "catalogos",
                table: "Clubes",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_ClubId",
                schema: "seguridad",
                table: "Usuarios",
                column: "ClubId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                schema: "seguridad",
                table: "Usuarios",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Username",
                schema: "seguridad",
                table: "Usuarios",
                column: "Username",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Participantes_Clubes",
                schema: "regatas",
                table: "Participantes",
                column: "ClubId",
                principalSchema: "catalogos",
                principalTable: "Clubes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Participantes_Clubes",
                schema: "regatas",
                table: "Participantes");

            migrationBuilder.DropTable(
                name: "Usuarios",
                schema: "seguridad");

            migrationBuilder.DropTable(
                name: "Clubes",
                schema: "catalogos");

            migrationBuilder.DropIndex(
                name: "IX_Participantes_ClubId",
                schema: "regatas",
                table: "Participantes");

            migrationBuilder.DropColumn(
                name: "ClubId",
                schema: "regatas",
                table: "Participantes");

            migrationBuilder.AddColumn<string>(
                name: "Club",
                schema: "regatas",
                table: "Participantes",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Participantes_Club",
                schema: "regatas",
                table: "Participantes",
                column: "Club");
        }
    }
}
