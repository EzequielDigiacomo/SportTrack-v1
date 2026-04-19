using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SportTrack_v1.AccesoDatos.Migrations
{
    /// <inheritdoc />
    public partial class ReestructuracionMangas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Penalizaciones_Resultados",
                schema: "regatas",
                table: "Penalizaciones");

            migrationBuilder.DropTable(
                name: "Resultados",
                schema: "regatas");

            migrationBuilder.DropIndex(
                name: "IX_Inscripciones_Carril",
                schema: "regatas",
                table: "Inscripciones");

            migrationBuilder.DropIndex(
                name: "IX_Inscripciones_Unica_Fase",
                schema: "regatas",
                table: "Inscripciones");

            migrationBuilder.DropColumn(
                name: "BoteIdentificador",
                schema: "regatas",
                table: "Inscripciones");

            migrationBuilder.DropColumn(
                name: "Carril",
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

            migrationBuilder.RenameColumn(
                name: "ResultadoId",
                schema: "regatas",
                table: "Penalizaciones",
                newName: "MangaResultadoId");

            migrationBuilder.RenameIndex(
                name: "IX_Penalizaciones_ResultadoTipo",
                schema: "regatas",
                table: "Penalizaciones",
                newName: "IX_Penalizaciones_MangaResultadoTipo");

            migrationBuilder.RenameIndex(
                name: "IX_Penalizaciones_ResultadoId",
                schema: "regatas",
                table: "Penalizaciones",
                newName: "IX_Penalizaciones_MangaResultadoId");

            migrationBuilder.CreateTable(
                name: "Mangas",
                schema: "regatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventoPruebaId = table.Column<int>(type: "integer", nullable: false),
                    Fase = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    NumeroManga = table.Column<int>(type: "integer", nullable: false),
                    FechaHoraProgramada = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Estado = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mangas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mangas_EventoPruebas",
                        column: x => x.EventoPruebaId,
                        principalSchema: "regatas",
                        principalTable: "EventoPruebas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MangaResultados",
                schema: "regatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MangaId = table.Column<int>(type: "integer", nullable: false),
                    InscripcionId = table.Column<int>(type: "integer", nullable: false),
                    Carril = table.Column<int>(type: "integer", nullable: true),
                    EsCabezaDeSerie = table.Column<bool>(type: "boolean", nullable: false),
                    TiempoOficial = table.Column<TimeSpan>(type: "interval", nullable: true),
                    Posicion = table.Column<int>(type: "integer", nullable: true),
                    Puntos = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    VelocidadMedia = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    Estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Pendiente"),
                    Observaciones = table.Column<string>(type: "text", nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UsuarioRegistro = table.Column<string>(type: "text", nullable: true),
                    UsuarioActualizacion = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MangaResultados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MangaResultados_Inscripciones",
                        column: x => x.InscripcionId,
                        principalSchema: "regatas",
                        principalTable: "Inscripciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MangaResultados_Mangas",
                        column: x => x.MangaId,
                        principalSchema: "regatas",
                        principalTable: "Mangas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Inscripciones_EventoPruebaId",
                schema: "regatas",
                table: "Inscripciones",
                column: "EventoPruebaId");

            migrationBuilder.CreateIndex(
                name: "IX_MangaResultados_Carril",
                schema: "regatas",
                table: "MangaResultados",
                columns: new[] { "MangaId", "Carril" });

            migrationBuilder.CreateIndex(
                name: "IX_MangaResultados_InscripcionId",
                schema: "regatas",
                table: "MangaResultados",
                column: "InscripcionId");

            migrationBuilder.CreateIndex(
                name: "IX_Mangas_EventoPruebaId",
                schema: "regatas",
                table: "Mangas",
                column: "EventoPruebaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Penalizaciones_MangaResultados",
                schema: "regatas",
                table: "Penalizaciones",
                column: "MangaResultadoId",
                principalSchema: "regatas",
                principalTable: "MangaResultados",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Penalizaciones_MangaResultados",
                schema: "regatas",
                table: "Penalizaciones");

            migrationBuilder.DropTable(
                name: "MangaResultados",
                schema: "regatas");

            migrationBuilder.DropTable(
                name: "Mangas",
                schema: "regatas");

            migrationBuilder.DropIndex(
                name: "IX_Inscripciones_EventoPruebaId",
                schema: "regatas",
                table: "Inscripciones");

            migrationBuilder.RenameColumn(
                name: "MangaResultadoId",
                schema: "regatas",
                table: "Penalizaciones",
                newName: "ResultadoId");

            migrationBuilder.RenameIndex(
                name: "IX_Penalizaciones_MangaResultadoTipo",
                schema: "regatas",
                table: "Penalizaciones",
                newName: "IX_Penalizaciones_ResultadoTipo");

            migrationBuilder.RenameIndex(
                name: "IX_Penalizaciones_MangaResultadoId",
                schema: "regatas",
                table: "Penalizaciones",
                newName: "IX_Penalizaciones_ResultadoId");

            migrationBuilder.AddColumn<string>(
                name: "BoteIdentificador",
                schema: "regatas",
                table: "Inscripciones",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Carril",
                schema: "regatas",
                table: "Inscripciones",
                type: "integer",
                nullable: true);

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

            migrationBuilder.CreateTable(
                name: "Resultados",
                schema: "regatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InscripcionId = table.Column<int>(type: "integer", nullable: false),
                    Estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Pendiente"),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    Observaciones = table.Column<string>(type: "text", nullable: true),
                    Posicion = table.Column<int>(type: "integer", nullable: true),
                    Puntos = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    TiempoOficial = table.Column<TimeSpan>(type: "interval", nullable: true),
                    UsuarioActualizacion = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    UsuarioRegistro = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    VelocidadMedia = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resultados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Resultados_Inscripciones",
                        column: x => x.InscripcionId,
                        principalSchema: "regatas",
                        principalTable: "Inscripciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Inscripciones_Carril",
                schema: "regatas",
                table: "Inscripciones",
                column: "Carril");

            migrationBuilder.CreateIndex(
                name: "IX_Inscripciones_Unica_Fase",
                schema: "regatas",
                table: "Inscripciones",
                columns: new[] { "EventoPruebaId", "ParticipanteId", "Fase" },
                unique: true,
                filter: "\"ParticipanteId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Resultados_Estado",
                schema: "regatas",
                table: "Resultados",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_Resultados_InscripcionId",
                schema: "regatas",
                table: "Resultados",
                column: "InscripcionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Resultados_InscripcionPosicion",
                schema: "regatas",
                table: "Resultados",
                columns: new[] { "InscripcionId", "Posicion" });

            migrationBuilder.CreateIndex(
                name: "IX_Resultados_Posicion",
                schema: "regatas",
                table: "Resultados",
                column: "Posicion");

            migrationBuilder.CreateIndex(
                name: "IX_Resultados_Puntos",
                schema: "regatas",
                table: "Resultados",
                column: "Puntos");

            migrationBuilder.AddForeignKey(
                name: "FK_Penalizaciones_Resultados",
                schema: "regatas",
                table: "Penalizaciones",
                column: "ResultadoId",
                principalSchema: "regatas",
                principalTable: "Resultados",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
