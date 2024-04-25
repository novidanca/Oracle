using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Oracle.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAll_AddStatuses_AddTimelines : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Characters",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "CharacterStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    CanQuest = table.Column<bool>(type: "INTEGER", nullable: false),
                    StartDay = table.Column<int>(type: "INTEGER", nullable: false),
                    EndDay = table.Column<int>(type: "INTEGER", nullable: true),
                    CharacterId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProjectId = table.Column<int>(type: "INTEGER", nullable: true)
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
                    table.ForeignKey(
                        name: "FK_CharacterStatuses_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CharacterTimelines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CharacterId = table.Column<int>(type: "INTEGER", nullable: false),
                    StartDay = table.Column<int>(type: "INTEGER", nullable: false),
                    EndDay = table.Column<int>(type: "INTEGER", nullable: true),
                    AdventureId = table.Column<int>(type: "INTEGER", nullable: true),
                    ActivityId = table.Column<int>(type: "INTEGER", nullable: true),
                    CharacterStatusId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterTimelines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacterTimelines_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CharacterTimelines_Adventures_AdventureId",
                        column: x => x.AdventureId,
                        principalTable: "Adventures",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CharacterTimelines_CharacterStatuses_CharacterStatusId",
                        column: x => x.CharacterStatusId,
                        principalTable: "CharacterStatuses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CharacterTimelines_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CharacterStatuses_CharacterId",
                table: "CharacterStatuses",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterStatuses_ProjectId",
                table: "CharacterStatuses",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterTimelines_ActivityId",
                table: "CharacterTimelines",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterTimelines_AdventureId",
                table: "CharacterTimelines",
                column: "AdventureId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterTimelines_CharacterId",
                table: "CharacterTimelines",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterTimelines_CharacterStatusId",
                table: "CharacterTimelines",
                column: "CharacterStatusId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CharacterTimelines");

            migrationBuilder.DropTable(
                name: "CharacterStatuses");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Characters",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");
        }
    }
}
