using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SomeWeirdApplicationBackend.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "card_hilo",
                incrementBy: 10);

            migrationBuilder.CreateTable(
                name: "Cards",
                columns: table => new
                {
                    CardInternalId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    TypeId = table.Column<int>(nullable: false),
                    Number = table.Column<string>(nullable: false),
                    CVV = table.Column<string>(nullable: false),
                    ExpirationDate = table.Column<DateTime>(maxLength: 100, nullable: false),
                    HolderName = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cards", x => x.CardInternalId);
                });

            migrationBuilder.CreateTable(
                name: "MoneyMovements",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MovementTime = table.Column<DateTime>(maxLength: 100, nullable: false),
                    CardInternalId = table.Column<int>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoneyMovements", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cards");

            migrationBuilder.DropTable(
                name: "MoneyMovements");

            migrationBuilder.DropSequence(
                name: "card_hilo");
        }
    }
}
