using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Waifu.Migrations
{
    /// <inheritdoc />
    public partial class SettingsAndModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "models",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Source = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ModelHash = table.Column<string>(type: "TEXT", nullable: true),
                    FilePath = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_models", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "settings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TargetModelId = table.Column<long>(type: "INTEGER", nullable: true),
                    SettingsTarget = table.Column<int>(type: "INTEGER", nullable: false),
                    Temperature = table.Column<float>(type: "REAL", nullable: false),
                    MaxTokens = table.Column<int>(type: "INTEGER", nullable: false),
                    TopK = table.Column<int>(type: "INTEGER", nullable: false),
                    TopP = table.Column<float>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_settings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_settings_models_TargetModelId",
                        column: x => x.TargetModelId,
                        principalTable: "models",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_settings_TargetModelId",
                table: "settings",
                column: "TargetModelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "settings");

            migrationBuilder.DropTable(
                name: "models");
        }
    }
}
