using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Oracle.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProgressToProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Progress",
                table: "Projects",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Progress",
                table: "Projects");
        }
    }
}
