using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Waifu.Migrations
{
    /// <inheritdoc />
    public partial class Personas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_settings_models_TargetModelId",
                table: "settings");

            migrationBuilder.DropTable(
                name: "models");

            migrationBuilder.DropIndex(
                name: "IX_settings_TargetModelId",
                table: "settings");

            migrationBuilder.DropColumn(
                name: "TargetModelId",
                table: "settings");

            migrationBuilder.AddColumn<string>(
                name: "LocalModel",
                table: "settings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "personas",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StageName = table.Column<string>(type: "TEXT", nullable: true),
                    CharacterDescription = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_personas", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "personas");

            migrationBuilder.DropColumn(
                name: "LocalModel",
                table: "settings");

            migrationBuilder.AddColumn<long>(
                name: "TargetModelId",
                table: "settings",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "models",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FilePath = table.Column<string>(type: "TEXT", nullable: true),
                    HuggingFaceId = table.Column<string>(type: "TEXT", nullable: true),
                    ModelHash = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Source = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_models", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_settings_TargetModelId",
                table: "settings",
                column: "TargetModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_settings_models_TargetModelId",
                table: "settings",
                column: "TargetModelId",
                principalTable: "models",
                principalColumn: "Id");
        }
    }
}
