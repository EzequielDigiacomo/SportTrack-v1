using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportTrack_v1.AccesoDatos.Migrations
{
    /// <inheritdoc />
    public partial class AddTimingFieldsToFase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaHoraFinReal",
                schema: "regatas",
                table: "Fases",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaHoraInicioReal",
                schema: "regatas",
                table: "Fases",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaHoraFinReal",
                schema: "regatas",
                table: "Fases");

            migrationBuilder.DropColumn(
                name: "FechaHoraInicioReal",
                schema: "regatas",
                table: "Fases");
        }
    }
}
