using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearningServer01.Migrations
{
    /// <inheritdoc />
    public partial class AddBattleLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BattleLogs",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SessionId = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DefenderId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AttackerId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AttackerNickname = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BattleResult = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    LogTimeUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ModeType = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    IsRevenged = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    LootedGold = table.Column<int>(type: "int", nullable: false),
                    LootedWood = table.Column<int>(type: "int", nullable: false),
                    LootedFood = table.Column<int>(type: "int", nullable: false),
                    PlayerInfoID = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BattleLogs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BattleLogs_Players_AttackerId",
                        column: x => x.AttackerId,
                        principalTable: "Players",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BattleLogs_Players_DefenderId",
                        column: x => x.DefenderId,
                        principalTable: "Players",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BattleLogs_Players_PlayerInfoID",
                        column: x => x.PlayerInfoID,
                        principalTable: "Players",
                        principalColumn: "ID");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_BattleLogs_AttackerId",
                table: "BattleLogs",
                column: "AttackerId");

            migrationBuilder.CreateIndex(
                name: "IX_BattleLogs_DefenderId",
                table: "BattleLogs",
                column: "DefenderId");

            migrationBuilder.CreateIndex(
                name: "IX_BattleLogs_PlayerInfoID",
                table: "BattleLogs",
                column: "PlayerInfoID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BattleLogs");
        }
    }
}
