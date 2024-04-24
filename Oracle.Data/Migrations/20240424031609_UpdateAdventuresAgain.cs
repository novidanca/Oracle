using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Oracle.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAdventuresAgain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CharacterAdventure");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Adventure",
                table: "Adventure");

            migrationBuilder.RenameTable(
                name: "Adventure",
                newName: "Adventures");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Adventures",
                table: "Adventures",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "AdventureCharacters",
                columns: table => new
                {
                    CharacterId = table.Column<int>(type: "INTEGER", nullable: false),
                    AdventureId = table.Column<int>(type: "INTEGER", nullable: false),
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdventureCharacters", x => new { x.AdventureId, x.CharacterId });
                    table.ForeignKey(
                        name: "FK_AdventureCharacters_Adventures_AdventureId",
                        column: x => x.AdventureId,
                        principalTable: "Adventures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdventureCharacters_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdventureCharacters_CharacterId",
                table: "AdventureCharacters",
                column: "CharacterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdventureCharacters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Adventures",
                table: "Adventures");

            migrationBuilder.RenameTable(
                name: "Adventures",
                newName: "Adventure");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Adventure",
                table: "Adventure",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "CharacterAdventure",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AdventureId = table.Column<int>(type: "INTEGER", nullable: false),
                    CharacterId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterAdventure", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacterAdventure_Adventure_AdventureId",
                        column: x => x.AdventureId,
                        principalTable: "Adventure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterAdventure_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CharacterAdventure_AdventureId",
                table: "CharacterAdventure",
                column: "AdventureId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterAdventure_CharacterId",
                table: "CharacterAdventure",
                column: "CharacterId");
        }
    }
}
