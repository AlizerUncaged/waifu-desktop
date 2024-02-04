using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Waifu.Migrations
{
    /// <inheritdoc />
    public partial class CharacterChannelsNowStored : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_channels_characters_CharacterId",
                table: "channels");

            migrationBuilder.DropIndex(
                name: "IX_channels_CharacterId",
                table: "channels");

            migrationBuilder.DropColumn(
                name: "CharacterId",
                table: "channels");

            migrationBuilder.CreateTable(
                name: "ChatChannelRoleplayCharacter",
                columns: table => new
                {
                    CharacterChannelsId = table.Column<long>(type: "INTEGER", nullable: false),
                    CharactersId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatChannelRoleplayCharacter", x => new { x.CharacterChannelsId, x.CharactersId });
                    table.ForeignKey(
                        name: "FK_ChatChannelRoleplayCharacter_channels_CharacterChannelsId",
                        column: x => x.CharacterChannelsId,
                        principalTable: "channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatChannelRoleplayCharacter_characters_CharactersId",
                        column: x => x.CharactersId,
                        principalTable: "characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatChannelRoleplayCharacter_CharactersId",
                table: "ChatChannelRoleplayCharacter",
                column: "CharactersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatChannelRoleplayCharacter");

            migrationBuilder.AddColumn<long>(
                name: "CharacterId",
                table: "channels",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_channels_CharacterId",
                table: "channels",
                column: "CharacterId");

            migrationBuilder.AddForeignKey(
                name: "FK_channels_characters_CharacterId",
                table: "channels",
                column: "CharacterId",
                principalTable: "characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
