using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NextCommerce.Migrations
{
    public partial class _006_BrandLogo_ShouldPromote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LogoId",
                table: "Brands",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "ShouldPromote",
                table: "Brands",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Brands_LogoId",
                table: "Brands",
                column: "LogoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Brands_Images_LogoId",
                table: "Brands",
                column: "LogoId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Brands_Images_LogoId",
                table: "Brands");

            migrationBuilder.DropIndex(
                name: "IX_Brands_LogoId",
                table: "Brands");

            migrationBuilder.DropColumn(
                name: "LogoId",
                table: "Brands");

            migrationBuilder.DropColumn(
                name: "ShouldPromote",
                table: "Brands");
        }
    }
}
