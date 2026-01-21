using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearningServer01.Migrations
{
    /// <inheritdoc />
    public partial class StructureTask01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PositionY",
                table: "Structures");

            migrationBuilder.AlterColumn<int>(
                name: "TableID",
                table: "Structures",
                type: "int",
                nullable: false,
                oldClrType: typeof(uint),
                oldType: "int unsigned");

            migrationBuilder.AlterColumn<float>(
                name: "RotationY",
                table: "Structures",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<float>(
                name: "PositionZ",
                table: "Structures",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<float>(
                name: "PositionX",
                table: "Structures",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<uint>(
                name: "TableID",
                table: "Structures",
                type: "int unsigned",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "RotationY",
                table: "Structures",
                type: "int",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "float");

            migrationBuilder.AlterColumn<int>(
                name: "PositionZ",
                table: "Structures",
                type: "int",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "float");

            migrationBuilder.AlterColumn<int>(
                name: "PositionX",
                table: "Structures",
                type: "int",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "float");

            migrationBuilder.AddColumn<int>(
                name: "PositionY",
                table: "Structures",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
