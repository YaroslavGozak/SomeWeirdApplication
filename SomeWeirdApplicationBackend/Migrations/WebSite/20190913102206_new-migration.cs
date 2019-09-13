using Microsoft.EntityFrameworkCore.Migrations;

namespace SomeWeirdApplicationBackend.Migrations.WebSite
{
    public partial class newmigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "WebSites",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<bool>(
                name: "IsInteresting",
                table: "WebSites",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_WebSites_Url",
                table: "WebSites",
                column: "Url");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WebSites_Url",
                table: "WebSites");

            migrationBuilder.DropColumn(
                name: "IsInteresting",
                table: "WebSites");

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "WebSites",
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}
