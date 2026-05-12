using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SportTrack_v1.AccesoDatos.Migrations
{
    /// <inheritdoc />
    public partial class AddSaaSPlans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PlanSaaSId",
                schema: "catalogos",
                table: "Clubes",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PlanesSaaS",
                schema: "catalogos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Precio = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    MaxAtletas = table.Column<int>(type: "integer", nullable: false),
                    MaxTorneosActivos = table.Column<int>(type: "integer", nullable: false),
                    ResultadosTiempoReal = table.Column<bool>(type: "boolean", nullable: false),
                    ExportacionExcel = table.Column<bool>(type: "boolean", nullable: false),
                    SoportePrioritario = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanesSaaS", x => x.Id);
                });

            migrationBuilder.InsertData(
                schema: "catalogos",
                table: "PlanesSaaS",
                columns: new[] { "Id", "ExportacionExcel", "MaxAtletas", "MaxTorneosActivos", "Nombre", "Precio", "ResultadosTiempoReal", "SoportePrioritario" },
                values: new object[,]
                {
                    { 1, false, 500, 1, "Básico", 0m, false, false },
                    { 2, true, 2000, 5, "Estándar", 50m, true, false },
                    { 3, true, -1, -1, "Premium", 120m, true, true }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clubes_PlanSaaSId",
                schema: "catalogos",
                table: "Clubes",
                column: "PlanSaaSId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanesSaaS_Nombre",
                schema: "catalogos",
                table: "PlanesSaaS",
                column: "Nombre",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Clubes_PlanesSaaS_PlanSaaSId",
                schema: "catalogos",
                table: "Clubes",
                column: "PlanSaaSId",
                principalSchema: "catalogos",
                principalTable: "PlanesSaaS",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clubes_PlanesSaaS_PlanSaaSId",
                schema: "catalogos",
                table: "Clubes");

            migrationBuilder.DropTable(
                name: "PlanesSaaS",
                schema: "catalogos");

            migrationBuilder.DropIndex(
                name: "IX_Clubes_PlanSaaSId",
                schema: "catalogos",
                table: "Clubes");

            migrationBuilder.DropColumn(
                name: "PlanSaaSId",
                schema: "catalogos",
                table: "Clubes");
        }
    }
}
