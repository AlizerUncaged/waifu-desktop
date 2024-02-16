using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Waifu.Migrations
{
    /// <inheritdoc />
    public partial class VTubeConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VtubeStudioIp",
                table: "settings",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "VtubeStudioPort",
                table: "settings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VtubeStudioIp",
                table: "settings");

            migrationBuilder.DropColumn(
                name: "VtubeStudioPort",
                table: "settings");
        }
    }
}
