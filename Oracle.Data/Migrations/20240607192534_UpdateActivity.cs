using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Oracle.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateActivity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CharacterTimelines_CharacterConditions_StatusId",
                table: "CharacterTimelines");

            migrationBuilder.DropIndex(
                name: "IX_CharacterTimelines_StatusId",
                table: "CharacterTimelines");

            migrationBuilder.DropColumn(
                name: "CharacterStatusId",
                table: "CharacterTimelines");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "CharacterTimelines");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Activities",
                newName: "StartDate");

            migrationBuilder.AddColumn<int>(
                name: "EndDate",
                table: "Activities",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Activities");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Activities",
                newName: "Date");

            migrationBuilder.AddColumn<int>(
                name: "CharacterStatusId",
                table: "CharacterTimelines",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StatusId",
                table: "CharacterTimelines",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharacterTimelines_StatusId",
                table: "CharacterTimelines",
                column: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterTimelines_CharacterConditions_StatusId",
                table: "CharacterTimelines",
                column: "StatusId",
                principalTable: "CharacterConditions",
                principalColumn: "Id");
        }
    }
}
