using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HySound.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class PlaylistCover : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CoverImage",
                table: "Playlists",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoverImage",
                table: "Playlists");
        }
    }
}
