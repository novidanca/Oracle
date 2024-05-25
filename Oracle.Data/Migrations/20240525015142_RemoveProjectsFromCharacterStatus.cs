using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Oracle.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveProjectsFromCharacterStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CharacterStatuses_Projects_ProjectId",
                table: "CharacterStatuses");

            migrationBuilder.DropIndex(
                name: "IX_CharacterStatuses_ProjectId",
                table: "CharacterStatuses");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "CharacterStatuses");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "CharacterStatuses",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharacterStatuses_ProjectId",
                table: "CharacterStatuses",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterStatuses_Projects_ProjectId",
                table: "CharacterStatuses",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id");
        }
    }
}
