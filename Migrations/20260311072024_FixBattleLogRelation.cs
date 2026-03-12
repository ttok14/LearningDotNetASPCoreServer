using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearningServer01.Migrations
{
    /// <inheritdoc />
    public partial class FixBattleLogRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BattleLogs_Players_PlayerInfoID",
                table: "BattleLogs");

            migrationBuilder.DropIndex(
                name: "IX_BattleLogs_PlayerInfoID",
                table: "BattleLogs");

            migrationBuilder.DropColumn(
                name: "PlayerInfoID",
                table: "BattleLogs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PlayerInfoID",
                table: "BattleLogs",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_BattleLogs_PlayerInfoID",
                table: "BattleLogs",
                column: "PlayerInfoID");

            migrationBuilder.AddForeignKey(
                name: "FK_BattleLogs_Players_PlayerInfoID",
                table: "BattleLogs",
                column: "PlayerInfoID",
                principalTable: "Players",
                principalColumn: "ID");
        }
    }
}
