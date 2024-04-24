using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Oracle.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAdventure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Characters_Adventure_AdventureId",
                table: "Characters");

            migrationBuilder.DropIndex(
                name: "IX_Characters_AdventureId",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "AdventureId",
                table: "Characters");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AdventureId",
                table: "Characters",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Characters_AdventureId",
                table: "Characters",
                column: "AdventureId");

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_Adventure_AdventureId",
                table: "Characters",
                column: "AdventureId",
                principalTable: "Adventure",
                principalColumn: "Id");
        }
    }
}
