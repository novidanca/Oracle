using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Oracle.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeStatusToConditionsAndNotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CharacterTimelines_CharacterStatuses_CharacterStatusId",
                table: "CharacterTimelines");

            migrationBuilder.DropTable(
                name: "CharacterStatuses");

            migrationBuilder.DropIndex(
                name: "IX_CharacterTimelines_CharacterStatusId",
                table: "CharacterTimelines");

            migrationBuilder.AddColumn<int>(
                name: "StatusId",
                table: "CharacterTimelines",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CharacterConditions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    StartDay = table.Column<int>(type: "INTEGER", nullable: false),
                    EndDay = table.Column<int>(type: "INTEGER", nullable: true),
                    CharacterId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterConditions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacterConditions_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TimelineNotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Note = table.Column<string>(type: "TEXT", nullable: false),
                    StartDate = table.Column<int>(type: "INTEGER", nullable: false),
                    EndDate = table.Column<int>(type: "INTEGER", nullable: true),
                    CharacterId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimelineNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimelineNotes_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CharacterTimelines_StatusId",
                table: "CharacterTimelines",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterConditions_CharacterId",
                table: "CharacterConditions",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_TimelineNotes_CharacterId",
                table: "TimelineNotes",
                column: "CharacterId");

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterTimelines_CharacterConditions_StatusId",
                table: "CharacterTimelines",
                column: "StatusId",
                principalTable: "CharacterConditions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CharacterTimelines_CharacterConditions_StatusId",
                table: "CharacterTimelines");

            migrationBuilder.DropTable(
                name: "CharacterConditions");

            migrationBuilder.DropTable(
                name: "TimelineNotes");

            migrationBuilder.DropIndex(
                name: "IX_CharacterTimelines_StatusId",
                table: "CharacterTimelines");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "CharacterTimelines");

            migrationBuilder.CreateTable(
                name: "CharacterStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CharacterId = table.Column<int>(type: "INTEGER", nullable: false),
                    CanQuest = table.Column<bool>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    EndDay = table.Column<int>(type: "INTEGER", nullable: true),
                    StartDay = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterStatuses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacterStatuses_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CharacterTimelines_CharacterStatusId",
                table: "CharacterTimelines",
                column: "CharacterStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterStatuses_CharacterId",
                table: "CharacterStatuses",
                column: "CharacterId");

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterTimelines_CharacterStatuses_CharacterStatusId",
                table: "CharacterTimelines",
                column: "CharacterStatusId",
                principalTable: "CharacterStatuses",
                principalColumn: "Id");
        }
    }
}
