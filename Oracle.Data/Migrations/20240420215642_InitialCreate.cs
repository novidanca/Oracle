using Microsoft.EntityFrameworkCore.Migrations;
using static Oracle.Data.Migrations.MigrationExtensions;

#nullable disable

namespace Oracle.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Adventure",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    StartDay = table.Column<int>(type: "INTEGER", nullable: false),
                    Duration = table.Column<int>(type: "INTEGER", nullable: false),
                    IsComplete = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Adventure", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectContributionTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectContributionTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Characters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    AdventureId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Characters_Adventure_AdventureId",
                        column: x => x.AdventureId,
                        principalTable: "Adventure",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ActivityTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ProjectContributionTypeId = table.Column<int>(type: "INTEGER", nullable: true),
                    ProjectContributionAmount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityTypes_ProjectContributionTypes_ProjectContributionTypeId",
                        column: x => x.ProjectContributionTypeId,
                        principalTable: "ProjectContributionTypes",
                        principalColumn: "Id");
                });

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

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ProjectContributionTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Goal = table.Column<int>(type: "INTEGER", nullable: false),
                    IsComplete = table.Column<bool>(type: "INTEGER", nullable: false),
                    OwningCharacterId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_Characters_OwningCharacterId",
                        column: x => x.OwningCharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Projects_ProjectContributionTypes_ProjectContributionTypeId",
                        column: x => x.ProjectContributionTypeId,
                        principalTable: "ProjectContributionTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Activities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ActivityTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    CharacterId = table.Column<int>(type: "INTEGER", nullable: false),
                    Date = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Activities_ActivityTypes_ActivityTypeId",
                        column: x => x.ActivityTypeId,
                        principalTable: "ActivityTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Activities_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityTypes_ProjectContributionTypeId",
                table: "ActivityTypes",
                column: "ProjectContributionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_ActivityTypeId",
                table: "Activities",
                column: "ActivityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_CharacterId",
                table: "Activities",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterAdventure_AdventureId",
                table: "CharacterAdventure",
                column: "AdventureId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterAdventure_CharacterId",
                table: "CharacterAdventure",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_AdventureId",
                table: "Characters",
                column: "AdventureId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_OwningCharacterId",
                table: "Projects",
                column: "OwningCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ProjectContributionTypeId",
                table: "Projects",
                column: "ProjectContributionTypeId");

            AddDefaultProjectContributionTypes(migrationBuilder);
            AddDefaultActivityTypes(migrationBuilder);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Activities");

            migrationBuilder.DropTable(
                name: "CharacterAdventure");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "ActivityTypes");

            migrationBuilder.DropTable(
                name: "Characters");

            migrationBuilder.DropTable(
                name: "ProjectContributionTypes");

            migrationBuilder.DropTable(
                name: "Adventure");
        }


        private static void AddDefaultProjectContributionTypes(MigrationBuilder migrationBuilder)
        {
	        var defaultValues = new List<string>
	        {
		        "Gold",
		        "Day"
	        };

	        foreach (var item in defaultValues)
	        {
		        var sql = $"INSERT INTO ProjectContributionTypes (Name) VALUES ({EscapeSqlValue(item)})";
		        migrationBuilder.Sql(sql);
	        }
        }

        private static void AddDefaultActivityTypes(MigrationBuilder migrationBuilder)
        {
	        // Name, ProjectContributionType Id, Contribution Amount per Activity
	        var defaultValues = new List<(string,int, int)>
	        {
		        ("Craft", 1, 5)
	        };

	        foreach (var item in defaultValues)
	        {
		        var sql = $@"
			INSERT INTO ActivityTypes (Name, ProjectContributionTypeId, ProjectContributionAmount) 
			VALUES ({EscapeSqlValue(item.Item1)}, {NullableIntToSql(item.Item2)}, {item.Item3})";

		        migrationBuilder.Sql(sql);
	        }
        }
    }
}
