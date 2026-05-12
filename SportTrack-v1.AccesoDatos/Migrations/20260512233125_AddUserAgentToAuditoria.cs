using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportTrack_v1.AccesoDatos.Migrations
{
    /// <inheritdoc />
    public partial class AddUserAgentToAuditoria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                schema: "catalogos",
                table: "Clubes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ParentClubId",
                schema: "catalogos",
                table: "Clubes",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserAgent",
                table: "Auditoria",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Clubes_ParentClubId",
                schema: "catalogos",
                table: "Clubes",
                column: "ParentClubId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clubes_Clubes_ParentClubId",
                schema: "catalogos",
                table: "Clubes",
                column: "ParentClubId",
                principalSchema: "catalogos",
                principalTable: "Clubes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clubes_Clubes_ParentClubId",
                schema: "catalogos",
                table: "Clubes");

            migrationBuilder.DropIndex(
                name: "IX_Clubes_ParentClubId",
                schema: "catalogos",
                table: "Clubes");

            migrationBuilder.DropColumn(
                name: "Activo",
                schema: "catalogos",
                table: "Clubes");

            migrationBuilder.DropColumn(
                name: "ParentClubId",
                schema: "catalogos",
                table: "Clubes");

            migrationBuilder.DropColumn(
                name: "UserAgent",
                table: "Auditoria");
        }
    }
}
