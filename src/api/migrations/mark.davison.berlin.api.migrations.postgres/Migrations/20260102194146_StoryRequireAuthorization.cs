using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mark.davison.berlin.api.migrations.postgres.Migrations
{
    /// <inheritdoc />
    public partial class StoryRequireAuthorization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RequiresAuthentication",
                table: "Stories",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequiresAuthentication",
                table: "Stories");
        }
    }
}
