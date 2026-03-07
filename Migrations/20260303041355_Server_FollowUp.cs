using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearningServer01.Migrations
{
    /// <inheritdoc />
    public partial class Server_FollowUp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemType",
                table: "UserItems");

            migrationBuilder.DropColumn(
                name: "HeroTableID",
                table: "Players");

            migrationBuilder.AddColumn<long>(
                name: "EquippedHeroItemUID",
                table: "Players",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EquippedHeroItemUID",
                table: "Players");

            migrationBuilder.AddColumn<int>(
                name: "ItemType",
                table: "UserItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HeroTableID",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
