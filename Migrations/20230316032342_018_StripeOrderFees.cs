using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NextCommerce.Migrations
{
    public partial class _018_StripeOrderFees : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Fees",
                table: "Orders",
                type: "money",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Fees",
                table: "Orders");
        }
    }
}
