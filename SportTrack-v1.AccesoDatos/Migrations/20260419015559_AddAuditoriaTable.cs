using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SportTrack_v1.AccesoDatos.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditoriaTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "LimitacionBotesAB",
                schema: "regatas",
                table: "Eventos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PermitirCompletarK4",
                schema: "regatas",
                table: "Eventos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PermitirMasterBajarASenior",
                schema: "regatas",
                table: "Eventos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PermitirSub23EnSenior",
                schema: "regatas",
                table: "Eventos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RestringirSoloCategoriaPropia",
                schema: "regatas",
                table: "Eventos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Auditoria",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Accion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Detalle = table.Column<string>(type: "text", nullable: false),
                    Usuario = table.Column<string>(type: "text", nullable: false),
                    IP = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Modulo = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auditoria", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Auditoria");

            migrationBuilder.DropColumn(
                name: "LimitacionBotesAB",
                schema: "regatas",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "PermitirCompletarK4",
                schema: "regatas",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "PermitirMasterBajarASenior",
                schema: "regatas",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "PermitirSub23EnSenior",
                schema: "regatas",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "RestringirSoloCategoriaPropia",
                schema: "regatas",
                table: "Eventos");
        }
    }
}
