using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Waifu.Migrations
{
    /// <inheritdoc />
    public partial class ElevellabsVoice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ElevenlabsSelectedVoice",
                table: "characters",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ElevenlabsSelectedVoice",
                table: "characters");
        }
    }
}
