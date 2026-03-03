using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearningServer01.Migrations
{
    /// <inheritdoc />
    public partial class Bounty_ETC : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Bounty",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HeroTableID",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Nickname",
                table: "Players",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "StatusMsg",
                table: "Players",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "StrengthStat",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bounty",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "HeroTableID",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "Nickname",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "StatusMsg",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "StrengthStat",
                table: "Players");
        }
    }
}
