using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mark.davison.berlin.api.migrations.sqlite.Migrations
{
    /// <inheritdoc />
    public partial class AddChapterTitleAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChapterAddress",
                table: "StoryUpdates",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChapterTitle",
                table: "StoryUpdates",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChapterAddress",
                table: "StoryUpdates");

            migrationBuilder.DropColumn(
                name: "ChapterTitle",
                table: "StoryUpdates");
        }
    }
}
