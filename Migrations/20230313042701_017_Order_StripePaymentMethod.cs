using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NextCommerce.Migrations
{
    public partial class _017_Order_StripePaymentMethod : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Method",
                table: "OrderDetails");

            migrationBuilder.RenameColumn(
                name: "PaymentMethod",
                table: "OrderDetails",
                newName: "StripePaymentMethod");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StripePaymentMethod",
                table: "OrderDetails",
                newName: "PaymentMethod");

            migrationBuilder.AddColumn<int>(
                name: "Method",
                table: "OrderDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
