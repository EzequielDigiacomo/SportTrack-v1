using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportTrack_v1.AccesoDatos.Migrations
{
    /// <inheritdoc />
    public partial class AddClubIdToEvento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClubId",
                schema: "regatas",
                table: "Eventos",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Eventos_ClubId",
                schema: "regatas",
                table: "Eventos",
                column: "ClubId");

            migrationBuilder.AddForeignKey(
                name: "FK_Eventos_Clubes_ClubId",
                schema: "regatas",
                table: "Eventos",
                column: "ClubId",
                principalSchema: "catalogos",
                principalTable: "Clubes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Eventos_Clubes_ClubId",
                schema: "regatas",
                table: "Eventos");

            migrationBuilder.DropIndex(
                name: "IX_Eventos_ClubId",
                schema: "regatas",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "ClubId",
                schema: "regatas",
                table: "Eventos");
        }
    }
}
