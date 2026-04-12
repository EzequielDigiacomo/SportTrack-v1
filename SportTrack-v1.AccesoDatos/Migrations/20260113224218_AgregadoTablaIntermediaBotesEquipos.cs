using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SportTrack_v1.AccesoDatos.Migrations
{
    /// <inheritdoc />
    public partial class AgregadoTablaIntermediaBotesEquipos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Inscripciones_Unica",
                schema: "regatas",
                table: "Inscripciones");

            migrationBuilder.AlterColumn<int>(
                name: "ParticipanteId",
                schema: "regatas",
                table: "Inscripciones",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "Carril",
                schema: "regatas",
                table: "Inscripciones",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "InscripcionTripulantes",
                schema: "regatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InscripcionId = table.Column<int>(type: "integer", nullable: false),
                    ParticipanteId = table.Column<int>(type: "integer", nullable: false),
                    PosicionEnBote = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InscripcionTripulantes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InscripcionTripulantes_Inscripciones",
                        column: x => x.InscripcionId,
                        principalSchema: "regatas",
                        principalTable: "Inscripciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InscripcionTripulantes_Participantes",
                        column: x => x.ParticipanteId,
                        principalSchema: "regatas",
                        principalTable: "Participantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Inscripciones_Carril",
                schema: "regatas",
                table: "Inscripciones",
                column: "Carril");

            migrationBuilder.CreateIndex(
                name: "IX_Inscripciones_Unica",
                schema: "regatas",
                table: "Inscripciones",
                columns: new[] { "EventoPruebaId", "ParticipanteId" },
                unique: true,
                filter: "\"ParticipanteId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_InscripcionTripulantes_ParticipanteId",
                schema: "regatas",
                table: "InscripcionTripulantes",
                column: "ParticipanteId");

            migrationBuilder.CreateIndex(
                name: "IX_InscripcionTripulantes_Unica",
                schema: "regatas",
                table: "InscripcionTripulantes",
                columns: new[] { "InscripcionId", "ParticipanteId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InscripcionTripulantes",
                schema: "regatas");

            migrationBuilder.DropIndex(
                name: "IX_Inscripciones_Carril",
                schema: "regatas",
                table: "Inscripciones");

            migrationBuilder.DropIndex(
                name: "IX_Inscripciones_Unica",
                schema: "regatas",
                table: "Inscripciones");

            migrationBuilder.DropColumn(
                name: "Carril",
                schema: "regatas",
                table: "Inscripciones");

            migrationBuilder.AlterColumn<int>(
                name: "ParticipanteId",
                schema: "regatas",
                table: "Inscripciones",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Inscripciones_Unica",
                schema: "regatas",
                table: "Inscripciones",
                columns: new[] { "EventoPruebaId", "ParticipanteId" },
                unique: true);
        }
    }
}
