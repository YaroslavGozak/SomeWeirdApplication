using Microsoft.EntityFrameworkCore.Migrations;

namespace SomeWeirdApplicationBackend.Migrations
{
    public partial class AmountColumnSpecification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "MoneyMovements",
                type: "money",
                nullable: false,
                oldClrType: typeof(decimal));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "MoneyMovements",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "money");
        }
    }
}
