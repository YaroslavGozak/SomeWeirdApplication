using Microsoft.EntityFrameworkCore.Migrations;

namespace SomeWeirdApplicationBackend.Migrations.WebSite
{
    public partial class WebSitesUrl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WebSites_WebSites_WebSiteStatisticsInternalId",
                table: "WebSites");

            migrationBuilder.RenameColumn(
                name: "WebSiteStatisticsInternalId",
                table: "WebSites",
                newName: "WebSiteInfoId");

            migrationBuilder.RenameColumn(
                name: "Occurrences",
                table: "WebSites",
                newName: "Count");

            migrationBuilder.RenameColumn(
                name: "InternalId",
                table: "WebSites",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_WebSites_WebSiteStatisticsInternalId",
                table: "WebSites",
                newName: "IX_WebSites_WebSiteInfoId");

            migrationBuilder.AddColumn<string>(
                name: "Domain",
                table: "WebSites",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_WebSites_WebSites_WebSiteInfoId",
                table: "WebSites",
                column: "WebSiteInfoId",
                principalTable: "WebSites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WebSites_WebSites_WebSiteInfoId",
                table: "WebSites");

            migrationBuilder.DropColumn(
                name: "Domain",
                table: "WebSites");

            migrationBuilder.RenameColumn(
                name: "WebSiteInfoId",
                table: "WebSites",
                newName: "WebSiteStatisticsInternalId");

            migrationBuilder.RenameColumn(
                name: "Count",
                table: "WebSites",
                newName: "Occurrences");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "WebSites",
                newName: "InternalId");

            migrationBuilder.RenameIndex(
                name: "IX_WebSites_WebSiteInfoId",
                table: "WebSites",
                newName: "IX_WebSites_WebSiteStatisticsInternalId");

            migrationBuilder.AddForeignKey(
                name: "FK_WebSites_WebSites_WebSiteStatisticsInternalId",
                table: "WebSites",
                column: "WebSiteStatisticsInternalId",
                principalTable: "WebSites",
                principalColumn: "InternalId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
