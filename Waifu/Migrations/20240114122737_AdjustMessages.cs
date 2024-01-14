using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Waifu.Migrations
{
    /// <inheritdoc />
    public partial class AdjustMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TargetChannelId",
                table: "messages",
                newName: "SentByUser");

            migrationBuilder.AddColumn<long>(
                name: "ChatChannelId",
                table: "messages",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_messages_ChatChannelId",
                table: "messages",
                column: "ChatChannelId");

            migrationBuilder.AddForeignKey(
                name: "FK_messages_channels_ChatChannelId",
                table: "messages",
                column: "ChatChannelId",
                principalTable: "channels",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_messages_channels_ChatChannelId",
                table: "messages");

            migrationBuilder.DropIndex(
                name: "IX_messages_ChatChannelId",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "ChatChannelId",
                table: "messages");

            migrationBuilder.RenameColumn(
                name: "SentByUser",
                table: "messages",
                newName: "TargetChannelId");
        }
    }
}
