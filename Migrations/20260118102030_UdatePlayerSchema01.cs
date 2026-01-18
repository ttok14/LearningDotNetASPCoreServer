using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearningServer01.Migrations
{
    /// <inheritdoc />
    public partial class UdatePlayerSchema01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NickName",
                table: "Players",
                newName: "ID");

            migrationBuilder.AddColumn<int>(
                name: "Food",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Players",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "SkillID01",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SkillID02",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SkillID03",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SpellID01",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SpellID02",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SpellID03",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Wood",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Food",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "SkillID01",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "SkillID02",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "SkillID03",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "SpellID01",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "SpellID02",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "SpellID03",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "Wood",
                table: "Players");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Players",
                newName: "NickName");
        }
    }
}
