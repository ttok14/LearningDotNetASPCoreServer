using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearningServer01.Migrations
{
    /// <inheritdoc />
    public partial class Structures_To_Entities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Structures_Players_OwnerID",
                table: "Structures");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Structures",
                table: "Structures");

            migrationBuilder.RenameTable(
                name: "Structures",
                newName: "Entities");

            migrationBuilder.RenameIndex(
                name: "IX_Structures_OwnerID",
                table: "Entities",
                newName: "IX_Entities_OwnerID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Entities",
                table: "Entities",
                column: "UID");

            migrationBuilder.AddForeignKey(
                name: "FK_Entities_Players_OwnerID",
                table: "Entities",
                column: "OwnerID",
                principalTable: "Players",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Entities_Players_OwnerID",
                table: "Entities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Entities",
                table: "Entities");

            migrationBuilder.RenameTable(
                name: "Entities",
                newName: "Structures");

            migrationBuilder.RenameIndex(
                name: "IX_Entities_OwnerID",
                table: "Structures",
                newName: "IX_Structures_OwnerID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Structures",
                table: "Structures",
                column: "UID");

            migrationBuilder.AddForeignKey(
                name: "FK_Structures_Players_OwnerID",
                table: "Structures",
                column: "OwnerID",
                principalTable: "Players",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
