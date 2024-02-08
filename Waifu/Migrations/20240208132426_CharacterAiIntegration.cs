using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Waifu.Migrations
{
    /// <inheritdoc />
    public partial class CharacterAiIntegration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UseCharacterAi",
                table: "settings");

            migrationBuilder.AddColumn<string>(
                name: "CharacterAiToken",
                table: "settings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CharacterAiId",
                table: "characters",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCharacterAi",
                table: "characters",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CharacterAiToken",
                table: "settings");

            migrationBuilder.DropColumn(
                name: "CharacterAiId",
                table: "characters");

            migrationBuilder.DropColumn(
                name: "IsCharacterAi",
                table: "characters");

            migrationBuilder.AddColumn<bool>(
                name: "UseCharacterAi",
                table: "settings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}
