using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NextCommerce.Migrations
{
    public partial class _010_CartItemUnitTaxes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "UnitTaxes",
                table: "CartItems",
                type: "money",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnitTaxes",
                table: "CartItems");
        }
    }
}
