using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportTrack_v1.AccesoDatos.Migrations
{
    /// <inheritdoc />
    public partial class AgregadoEstructuraStartListFases : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Inscripciones_NumeroCompetidor",
                schema: "regatas",
                table: "Inscripciones");

            migrationBuilder.DropIndex(
                name: "IX_Inscripciones_Unica",
                schema: "regatas",
                table: "Inscripciones");

            migrationBuilder.AddColumn<bool>(
                name: "EsCabezaDeSerie",
                schema: "regatas",
                table: "Inscripciones",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Fase",
                schema: "regatas",
                table: "Inscripciones",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "NumeroManga",
                schema: "regatas",
                table: "Inscripciones",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Inscripciones_NumeroCompetidor",
                schema: "regatas",
                table: "Inscripciones",
                column: "NumeroCompetidor");

            migrationBuilder.CreateIndex(
                name: "IX_Inscripciones_Unica_Fase",
                schema: "regatas",
                table: "Inscripciones",
                columns: new[] { "EventoPruebaId", "ParticipanteId", "Fase" },
                unique: true,
                filter: "\"ParticipanteId\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Inscripciones_NumeroCompetidor",
                schema: "regatas",
                table: "Inscripciones");

            migrationBuilder.DropIndex(
                name: "IX_Inscripciones_Unica_Fase",
                schema: "regatas",
                table: "Inscripciones");

            migrationBuilder.DropColumn(
                name: "EsCabezaDeSerie",
                schema: "regatas",
                table: "Inscripciones");

            migrationBuilder.DropColumn(
                name: "Fase",
                schema: "regatas",
                table: "Inscripciones");

            migrationBuilder.DropColumn(
                name: "NumeroManga",
                schema: "regatas",
                table: "Inscripciones");

            migrationBuilder.CreateIndex(
                name: "IX_Inscripciones_NumeroCompetidor",
                schema: "regatas",
                table: "Inscripciones",
                column: "NumeroCompetidor",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Inscripciones_Unica",
                schema: "regatas",
                table: "Inscripciones",
                columns: new[] { "EventoPruebaId", "ParticipanteId" },
                unique: true,
                filter: "\"ParticipanteId\" IS NOT NULL");
        }
    }
}
