using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeaSlugAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueValue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_SeaSlugs_Label",
                table: "SeaSlugs",
                column: "Label");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_SeaSlugs_Label",
                table: "SeaSlugs");
        }
    }
}
