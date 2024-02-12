using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Waifu.Migrations
{
    /// <inheritdoc />
    public partial class ElevellabsToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ElevenlabsApiKey",
                table: "settings",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ElevenlabsApiKey",
                table: "settings");
        }
    }
}
