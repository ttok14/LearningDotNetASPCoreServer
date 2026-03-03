using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearningServer01.Migrations
{
    /// <inheritdoc />
    public partial class Add_UserItem_DeploymentSlot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserItems",
                columns: table => new
                {
                    UID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OwnerID = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ItemType = table.Column<int>(type: "int", nullable: false),
                    TableID = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    InventorySlotIdx = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserItems", x => x.UID);
                    table.ForeignKey(
                        name: "FK_UserItems_Players_OwnerID",
                        column: x => x.OwnerID,
                        principalTable: "Players",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DeploymentSlots",
                columns: table => new
                {
                    SlotUID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OwnerID = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SlotIdx = table.Column<int>(type: "int", nullable: false),
                    EquippedItemUID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeploymentSlots", x => x.SlotUID);
                    table.ForeignKey(
                        name: "FK_DeploymentSlots_Players_OwnerID",
                        column: x => x.OwnerID,
                        principalTable: "Players",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeploymentSlots_UserItems_EquippedItemUID",
                        column: x => x.EquippedItemUID,
                        principalTable: "UserItems",
                        principalColumn: "UID");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_DeploymentSlots_EquippedItemUID",
                table: "DeploymentSlots",
                column: "EquippedItemUID");

            migrationBuilder.CreateIndex(
                name: "IX_DeploymentSlots_OwnerID",
                table: "DeploymentSlots",
                column: "OwnerID");

            migrationBuilder.CreateIndex(
                name: "IX_UserItems_OwnerID",
                table: "UserItems",
                column: "OwnerID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeploymentSlots");

            migrationBuilder.DropTable(
                name: "UserItems");
        }
    }
}
