using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SomeWeirdApplicationBackend.Migrations.WebSite
{
    public partial class website : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WebSites",
                columns: table => new
                {
                    InternalId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Url = table.Column<string>(nullable: false),
                    Occurrences = table.Column<int>(nullable: false),
                    WebSiteStatisticsInternalId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebSites", x => x.InternalId);
                    table.ForeignKey(
                        name: "FK_WebSites_WebSites_WebSiteStatisticsInternalId",
                        column: x => x.WebSiteStatisticsInternalId,
                        principalTable: "WebSites",
                        principalColumn: "InternalId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WebSites_WebSiteStatisticsInternalId",
                table: "WebSites",
                column: "WebSiteStatisticsInternalId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WebSites");
        }
    }
}
