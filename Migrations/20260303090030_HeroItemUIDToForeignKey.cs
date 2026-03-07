using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearningServer01.Migrations
{
    /// <inheritdoc />
    public partial class HeroItemUIDToForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "EquippedHeroItemUID",
                table: "Players",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_UserItems_EquippedHeroItemUID",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_Players_EquippedHeroItemUID",
                table: "Players");

            migrationBuilder.AlterColumn<long>(
                name: "EquippedHeroItemUID",
                table: "Players",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);
        }
    }
}
