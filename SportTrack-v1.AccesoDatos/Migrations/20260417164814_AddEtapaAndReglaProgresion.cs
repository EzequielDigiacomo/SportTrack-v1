using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SportTrack_v1.AccesoDatos.Migrations
{
    /// <inheritdoc />
    public partial class AddEtapaAndReglaProgresion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM regatas.\"Resultados\"");
            migrationBuilder.Sql("DELETE FROM regatas.\"Fases\"");

            migrationBuilder.DropForeignKey(
                name: "FK_Fases_EventoPruebas",
                schema: "regatas",
                table: "Fases");

            migrationBuilder.RenameColumn(
                name: "EventoPruebaId",
                schema: "regatas",
                table: "Fases",
                newName: "EtapaId");

            migrationBuilder.RenameIndex(
                name: "IX_Fases_EventoPruebaId",
                schema: "regatas",
                table: "Fases",
                newName: "IX_Fases_EtapaId");

            migrationBuilder.CreateTable(
                name: "Etapas",
                schema: "regatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventoPruebaId = table.Column<int>(type: "integer", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    Orden = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Etapas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Etapas_EventoPruebas",
                        column: x => x.EventoPruebaId,
                        principalSchema: "regatas",
                        principalTable: "EventoPruebas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReglasProgresion",
                schema: "regatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventoPruebaId = table.Column<int>(type: "integer", nullable: false),
                    EtapaOrigenId = table.Column<int>(type: "integer", nullable: false),
                    EtapaDestinoId = table.Column<int>(type: "integer", nullable: false),
                    PosicionDesde = table.Column<int>(type: "integer", nullable: false),
                    PosicionHasta = table.Column<int>(type: "integer", nullable: false),
                    PorTiempo = table.Column<bool>(type: "boolean", nullable: false),
                    CantidadATomar = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReglasProgresion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReglasProgresion_Etapas_Destino",
                        column: x => x.EtapaDestinoId,
                        principalSchema: "regatas",
                        principalTable: "Etapas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReglasProgresion_Etapas_Origen",
                        column: x => x.EtapaOrigenId,
                        principalSchema: "regatas",
                        principalTable: "Etapas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReglasProgresion_EventoPruebas",
                        column: x => x.EventoPruebaId,
                        principalSchema: "regatas",
                        principalTable: "EventoPruebas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Etapas_EventoPruebaId",
                schema: "regatas",
                table: "Etapas",
                column: "EventoPruebaId");

            migrationBuilder.CreateIndex(
                name: "IX_ReglasProgresion_EtapaDestinoId",
                schema: "regatas",
                table: "ReglasProgresion",
                column: "EtapaDestinoId");

            migrationBuilder.CreateIndex(
                name: "IX_ReglasProgresion_EtapaOrigenId",
                schema: "regatas",
                table: "ReglasProgresion",
                column: "EtapaOrigenId");

            migrationBuilder.CreateIndex(
                name: "IX_ReglasProgresion_EventoPruebaId",
                schema: "regatas",
                table: "ReglasProgresion",
                column: "EventoPruebaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Fases_Etapas",
                schema: "regatas",
                table: "Fases",
                column: "EtapaId",
                principalSchema: "regatas",
                principalTable: "Etapas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fases_Etapas",
                schema: "regatas",
                table: "Fases");

            migrationBuilder.DropTable(
                name: "ReglasProgresion",
                schema: "regatas");

            migrationBuilder.DropTable(
                name: "Etapas",
                schema: "regatas");

            migrationBuilder.RenameColumn(
                name: "EtapaId",
                schema: "regatas",
                table: "Fases",
                newName: "EventoPruebaId");

            migrationBuilder.RenameIndex(
                name: "IX_Fases_EtapaId",
                schema: "regatas",
                table: "Fases",
                newName: "IX_Fases_EventoPruebaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Fases_EventoPruebas",
                schema: "regatas",
                table: "Fases",
                column: "EventoPruebaId",
                principalSchema: "regatas",
                principalTable: "EventoPruebas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
