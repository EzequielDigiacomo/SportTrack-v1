using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportTrack_v1.AccesoDatos.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCamposCronogramaSmart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CarrilesDisponibles",
                schema: "regatas",
                table: "Eventos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "HoraFinReceso",
                schema: "regatas",
                table: "Eventos",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "HoraInicioEvento",
                schema: "regatas",
                table: "Eventos",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "HoraInicioReceso",
                schema: "regatas",
                table: "Eventos",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<int>(
                name: "PerfilTiempo",
                schema: "regatas",
                table: "Eventos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "PermitirCombinadas",
                schema: "regatas",
                table: "Eventos",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CarrilesDisponibles",
                schema: "regatas",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "HoraFinReceso",
                schema: "regatas",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "HoraInicioEvento",
                schema: "regatas",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "HoraInicioReceso",
                schema: "regatas",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "PerfilTiempo",
                schema: "regatas",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "PermitirCombinadas",
                schema: "regatas",
                table: "Eventos");
        }
    }
}
