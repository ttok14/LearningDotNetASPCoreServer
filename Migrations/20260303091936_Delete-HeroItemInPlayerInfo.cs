using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearningServer01.Migrations
{
    /// <inheritdoc />
    public partial class DeleteHeroItemInPlayerInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_UserItems_EquippedHeroItemUID",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_Players_EquippedHeroItemUID",
                table: "Players");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Players_EquippedHeroItemUID",
                table: "Players",
                column: "EquippedHeroItemUID");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_UserItems_EquippedHeroItemUID",
                table: "Players",
                column: "EquippedHeroItemUID",
                principalTable: "UserItems",
                principalColumn: "UID");
        }
    }
}
