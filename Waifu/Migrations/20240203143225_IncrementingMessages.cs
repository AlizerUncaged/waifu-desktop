using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Waifu.Migrations
{
    /// <inheritdoc />
    public partial class IncrementingMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HuggingFaceId",
                table: "models",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "IncrementId",
                table: "messages",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HuggingFaceId",
                table: "models");

            migrationBuilder.DropColumn(
                name: "IncrementId",
                table: "messages");
        }
    }
}
