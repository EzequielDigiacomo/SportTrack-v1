using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportTrack_v1.AccesoDatos.Migrations
{
    /// <inheritdoc />
    public partial class AddSubscriptionFieldsToClub : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "BloqueadoPorFaltaDePago",
                schema: "catalogos",
                table: "Clubes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaAltaPlan",
                schema: "catalogos",
                table: "Clubes",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaVencimientoPlan",
                schema: "catalogos",
                table: "Clubes",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FrecuenciaPago",
                schema: "catalogos",
                table: "Clubes",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BloqueadoPorFaltaDePago",
                schema: "catalogos",
                table: "Clubes");

            migrationBuilder.DropColumn(
                name: "FechaAltaPlan",
                schema: "catalogos",
                table: "Clubes");

            migrationBuilder.DropColumn(
                name: "FechaVencimientoPlan",
                schema: "catalogos",
                table: "Clubes");

            migrationBuilder.DropColumn(
                name: "FrecuenciaPago",
                schema: "catalogos",
                table: "Clubes");
        }
    }
}
