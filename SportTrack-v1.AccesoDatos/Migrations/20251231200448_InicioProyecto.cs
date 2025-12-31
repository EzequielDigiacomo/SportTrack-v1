using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SportTrack_v1.AccesoDatos.Migrations
{
    /// <inheritdoc />
    public partial class InicioProyecto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "catalogos");

            migrationBuilder.EnsureSchema(
                name: "regatas");

            migrationBuilder.CreateTable(
                name: "Botes",
                schema: "catalogos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Tipo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Botes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categorias",
                schema: "catalogos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    EdadMin = table.Column<int>(type: "integer", nullable: true),
                    EdadMax = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Distancias",
                schema: "catalogos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DistanciaRegata = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Distancias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Eventos",
                schema: "regatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Ubicacion = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Programada"),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Eventos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sexos",
                schema: "catalogos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sexos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Participantes",
                schema: "regatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Apellido = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FechaNacimiento = table.Column<DateTime>(type: "date", nullable: false),
                    SexoId = table.Column<int>(type: "integer", nullable: false),
                    CategoriaId = table.Column<int>(type: "integer", nullable: true),
                    Pais = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Club = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Telefono = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Direccion = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Participantes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Participantes_Categorias",
                        column: x => x.CategoriaId,
                        principalSchema: "catalogos",
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Participantes_Sexos",
                        column: x => x.SexoId,
                        principalSchema: "catalogos",
                        principalTable: "Sexos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Pruebas",
                schema: "regatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    BoteId = table.Column<int>(type: "integer", nullable: false),
                    CategoriaId = table.Column<int>(type: "integer", nullable: false),
                    DistanciaId = table.Column<int>(type: "integer", nullable: false),
                    SexoId = table.Column<int>(type: "integer", nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pruebas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pruebas_Botes",
                        column: x => x.BoteId,
                        principalSchema: "catalogos",
                        principalTable: "Botes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pruebas_Categorias",
                        column: x => x.CategoriaId,
                        principalSchema: "catalogos",
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pruebas_Distancias",
                        column: x => x.DistanciaId,
                        principalSchema: "catalogos",
                        principalTable: "Distancias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pruebas_Sexos",
                        column: x => x.SexoId,
                        principalSchema: "catalogos",
                        principalTable: "Sexos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EventoPruebas",
                schema: "regatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventoId = table.Column<int>(type: "integer", nullable: false),
                    PruebaId = table.Column<int>(type: "integer", nullable: false),
                    FechaHora = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MaxParticipantes = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    Pista = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Programada")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventoPruebas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventoPruebas_Eventos",
                        column: x => x.EventoId,
                        principalSchema: "regatas",
                        principalTable: "Eventos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventoPruebas_Pruebas",
                        column: x => x.PruebaId,
                        principalSchema: "regatas",
                        principalTable: "Pruebas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Inscripciones",
                schema: "regatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventoPruebaId = table.Column<int>(type: "integer", nullable: false),
                    ParticipanteId = table.Column<int>(type: "integer", nullable: false),
                    FechaInscripcion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    NumeroCompetidor = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Inscrito")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inscripciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inscripciones_EventoPruebas",
                        column: x => x.EventoPruebaId,
                        principalSchema: "regatas",
                        principalTable: "EventoPruebas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Inscripciones_Participantes",
                        column: x => x.ParticipanteId,
                        principalSchema: "regatas",
                        principalTable: "Participantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Resultados",
                schema: "regatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InscripcionId = table.Column<int>(type: "integer", nullable: false),
                    TiempoOficial = table.Column<TimeSpan>(type: "interval", nullable: true),
                    Posicion = table.Column<int>(type: "integer", nullable: true),
                    Puntos = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    VelocidadMedia = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    Estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Pendiente"),
                    Observaciones = table.Column<string>(type: "text", nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UsuarioRegistro = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    UsuarioActualizacion = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
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

            migrationBuilder.CreateTable(
                name: "Penalizaciones",
                schema: "regatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ResultadoId = table.Column<int>(type: "integer", nullable: false),
                    TipoPenalizacion = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    TiempoPenalizacion = table.Column<TimeSpan>(type: "interval", nullable: true),
                    Severidad = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    JuezAsignado = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Penalizaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Penalizaciones_Resultados",
                        column: x => x.ResultadoId,
                        principalSchema: "regatas",
                        principalTable: "Resultados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "catalogos",
                table: "Botes",
                columns: new[] { "Id", "Tipo" },
                values: new object[,]
                {
                    { 1, "Kayak Individual" },
                    { 2, "Kayak Doble" },
                    { 3, "Kayak Cuadruple" },
                    { 4, "Canoa Individual" },
                    { 5, "Canoa Doble" },
                    { 6, "Canoa Cuadruple" }
                });

            migrationBuilder.InsertData(
                schema: "catalogos",
                table: "Categorias",
                columns: new[] { "Id", "EdadMax", "EdadMin", "Nombre" },
                values: new object[,]
                {
                    { 1, 10, 9, "Pre-Infantil" },
                    { 2, 11, 10, "Infantil" },
                    { 3, 13, 12, "Menor" },
                    { 4, 15, 14, "Cadete" },
                    { 5, 17, 16, "Junior" },
                    { 6, 22, 18, "Sub-23" },
                    { 7, 35, 18, "Senior" },
                    { 8, 49, 40, "Master A" },
                    { 9, 59, 50, "Master B" },
                    { 10, 80, 60, "Master C" }
                });

            migrationBuilder.InsertData(
                schema: "catalogos",
                table: "Distancias",
                columns: new[] { "Id", "DistanciaRegata" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 2 },
                    { 3, 3 },
                    { 4, 4 },
                    { 5, 5 },
                    { 6, 6 },
                    { 7, 7 },
                    { 8, 8 },
                    { 9, 9 },
                    { 10, 10 },
                    { 11, 11 },
                    { 12, 12 },
                    { 13, 13 },
                    { 14, 14 },
                    { 15, 15 },
                    { 16, 16 }
                });

            migrationBuilder.InsertData(
                schema: "catalogos",
                table: "Sexos",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { 1, "Masculino" },
                    { 2, "Femenino" },
                    { 3, "Mixto" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Botes_Tipo",
                schema: "catalogos",
                table: "Botes",
                column: "Tipo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_Nombre",
                schema: "catalogos",
                table: "Categorias",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Distancias_DistanciaRegata",
                schema: "catalogos",
                table: "Distancias",
                column: "DistanciaRegata",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventoPruebas_Estado",
                schema: "regatas",
                table: "EventoPruebas",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_EventoPruebas_FechaHora",
                schema: "regatas",
                table: "EventoPruebas",
                column: "FechaHora");

            migrationBuilder.CreateIndex(
                name: "IX_EventoPruebas_PruebaId",
                schema: "regatas",
                table: "EventoPruebas",
                column: "PruebaId");

            migrationBuilder.CreateIndex(
                name: "IX_EventoPruebas_Unica",
                schema: "regatas",
                table: "EventoPruebas",
                columns: new[] { "EventoId", "PruebaId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Eventos_Estado",
                schema: "regatas",
                table: "Eventos",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_Eventos_Fecha",
                schema: "regatas",
                table: "Eventos",
                column: "Fecha");

            migrationBuilder.CreateIndex(
                name: "IX_Eventos_Nombre",
                schema: "regatas",
                table: "Eventos",
                column: "Nombre");

            migrationBuilder.CreateIndex(
                name: "IX_Inscripciones_Estado",
                schema: "regatas",
                table: "Inscripciones",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_Inscripciones_FechaInscripcion",
                schema: "regatas",
                table: "Inscripciones",
                column: "FechaInscripcion");

            migrationBuilder.CreateIndex(
                name: "IX_Inscripciones_NumeroCompetidor",
                schema: "regatas",
                table: "Inscripciones",
                column: "NumeroCompetidor",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Inscripciones_ParticipanteId",
                schema: "regatas",
                table: "Inscripciones",
                column: "ParticipanteId");

            migrationBuilder.CreateIndex(
                name: "IX_Inscripciones_Unica",
                schema: "regatas",
                table: "Inscripciones",
                columns: new[] { "EventoPruebaId", "ParticipanteId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Participantes_CategoriaId",
                schema: "regatas",
                table: "Participantes",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Participantes_Club",
                schema: "regatas",
                table: "Participantes",
                column: "Club");

            migrationBuilder.CreateIndex(
                name: "IX_Participantes_Email",
                schema: "regatas",
                table: "Participantes",
                column: "Email",
                unique: true,
                filter: "\"Email\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Participantes_NombreApellido",
                schema: "regatas",
                table: "Participantes",
                columns: new[] { "Nombre", "Apellido" });

            migrationBuilder.CreateIndex(
                name: "IX_Participantes_Pais",
                schema: "regatas",
                table: "Participantes",
                column: "Pais");

            migrationBuilder.CreateIndex(
                name: "IX_Participantes_SexoId",
                schema: "regatas",
                table: "Participantes",
                column: "SexoId");

            migrationBuilder.CreateIndex(
                name: "IX_Penalizaciones_JuezAsignado",
                schema: "regatas",
                table: "Penalizaciones",
                column: "JuezAsignado");

            migrationBuilder.CreateIndex(
                name: "IX_Penalizaciones_ResultadoId",
                schema: "regatas",
                table: "Penalizaciones",
                column: "ResultadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Penalizaciones_ResultadoTipo",
                schema: "regatas",
                table: "Penalizaciones",
                columns: new[] { "ResultadoId", "TipoPenalizacion" });

            migrationBuilder.CreateIndex(
                name: "IX_Penalizaciones_Severidad",
                schema: "regatas",
                table: "Penalizaciones",
                column: "Severidad");

            migrationBuilder.CreateIndex(
                name: "IX_Penalizaciones_TipoPenalizacion",
                schema: "regatas",
                table: "Penalizaciones",
                column: "TipoPenalizacion");

            migrationBuilder.CreateIndex(
                name: "IX_Pruebas_CategoriaId",
                schema: "regatas",
                table: "Pruebas",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Pruebas_DistanciaId",
                schema: "regatas",
                table: "Pruebas",
                column: "DistanciaId");

            migrationBuilder.CreateIndex(
                name: "IX_Pruebas_Nombre",
                schema: "regatas",
                table: "Pruebas",
                column: "Nombre");

            migrationBuilder.CreateIndex(
                name: "IX_Pruebas_SexoId",
                schema: "regatas",
                table: "Pruebas",
                column: "SexoId");

            migrationBuilder.CreateIndex(
                name: "IX_Pruebas_Unica",
                schema: "regatas",
                table: "Pruebas",
                columns: new[] { "BoteId", "CategoriaId", "DistanciaId", "SexoId" },
                unique: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_Sexos_Nombre",
                schema: "catalogos",
                table: "Sexos",
                column: "Nombre",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Penalizaciones",
                schema: "regatas");

            migrationBuilder.DropTable(
                name: "Resultados",
                schema: "regatas");

            migrationBuilder.DropTable(
                name: "Inscripciones",
                schema: "regatas");

            migrationBuilder.DropTable(
                name: "EventoPruebas",
                schema: "regatas");

            migrationBuilder.DropTable(
                name: "Participantes",
                schema: "regatas");

            migrationBuilder.DropTable(
                name: "Eventos",
                schema: "regatas");

            migrationBuilder.DropTable(
                name: "Pruebas",
                schema: "regatas");

            migrationBuilder.DropTable(
                name: "Botes",
                schema: "catalogos");

            migrationBuilder.DropTable(
                name: "Categorias",
                schema: "catalogos");

            migrationBuilder.DropTable(
                name: "Distancias",
                schema: "catalogos");

            migrationBuilder.DropTable(
                name: "Sexos",
                schema: "catalogos");
        }
    }
}
