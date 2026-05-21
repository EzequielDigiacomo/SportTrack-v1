using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SportTrack_v1.AccesoDatos.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PagoAfiliacionAlDia",
                schema: "regatas",
                table: "Participantes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Pagado",
                schema: "regatas",
                table: "Inscripciones",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PagoAfiliacionAlDia",
                schema: "catalogos",
                table: "Clubes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Pagos",
                schema: "regatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TipoPago = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ClubId = table.Column<int>(type: "integer", nullable: true),
                    ParticipanteId = table.Column<int>(type: "integer", nullable: true),
                    InscripcionId = table.Column<int>(type: "integer", nullable: true),
                    Monto = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    FechaPago = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    Referencia = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    RegistradoPor = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Notas = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pagos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pagos_Clubes",
                        column: x => x.ClubId,
                        principalSchema: "catalogos",
                        principalTable: "Clubes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Pagos_Inscripciones",
                        column: x => x.InscripcionId,
                        principalSchema: "regatas",
                        principalTable: "Inscripciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Pagos_Participantes",
                        column: x => x.ParticipanteId,
                        principalSchema: "regatas",
                        principalTable: "Participantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_ClubId",
                schema: "regatas",
                table: "Pagos",
                column: "ClubId");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_InscripcionId",
                schema: "regatas",
                table: "Pagos",
                column: "InscripcionId");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_ParticipanteId",
                schema: "regatas",
                table: "Pagos",
                column: "ParticipanteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pagos",
                schema: "regatas");

            migrationBuilder.DropColumn(
                name: "PagoAfiliacionAlDia",
                schema: "regatas",
                table: "Participantes");

            migrationBuilder.DropColumn(
                name: "Pagado",
                schema: "regatas",
                table: "Inscripciones");

            migrationBuilder.DropColumn(
                name: "PagoAfiliacionAlDia",
                schema: "catalogos",
                table: "Clubes");
        }
    }
}
