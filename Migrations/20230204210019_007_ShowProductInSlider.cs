using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NextCommerce.Migrations
{
    public partial class _007_ShowProductInSlider : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ShowInSlider",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShowInSlider",
                table: "Products");
        }
    }
}
