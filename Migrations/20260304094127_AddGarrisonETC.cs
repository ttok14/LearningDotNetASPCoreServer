using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearningServer01.Migrations
{
    /// <inheritdoc />
    public partial class AddGarrisonETC : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SpecificDataJson",
                table: "Entities");

            migrationBuilder.CreateTable(
                name: "EntityGarrisonInfo",
                columns: table => new
                {
                    UID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SlotIdx = table.Column<int>(type: "int", nullable: false),
                    OwnerEntityUID = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    EquippedItemUID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityGarrisonInfo", x => x.UID);
                    table.ForeignKey(
                        name: "FK_EntityGarrisonInfo_Entities_OwnerEntityUID",
                        column: x => x.OwnerEntityUID,
                        principalTable: "Entities",
                        principalColumn: "UID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_EntityGarrisonInfo_OwnerEntityUID",
                table: "EntityGarrisonInfo",
                column: "OwnerEntityUID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EntityGarrisonInfo");

            migrationBuilder.AddColumn<string>(
                name: "SpecificDataJson",
                table: "Entities",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
