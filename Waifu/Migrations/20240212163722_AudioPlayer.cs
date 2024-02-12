using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Waifu.Migrations
{
    /// <inheritdoc />
    public partial class AudioPlayer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AudioPlayerDeviceId",
                table: "settings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "EnableElevenlabs",
                table: "settings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AudioPlayerDeviceId",
                table: "settings");

            migrationBuilder.DropColumn(
                name: "EnableElevenlabs",
                table: "settings");
        }
    }
}
