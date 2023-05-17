using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NextCommerce.Migrations
{
    public partial class _004_ProductShowcaseCollection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductShowcaseCollections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Caption = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductShowcaseCollections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductShowcaseCollectionItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CollectionId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductShowcaseCollectionItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductShowcaseCollectionItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductShowcaseCollectionItems_ProductShowcaseCollections_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "ProductShowcaseCollections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductShowcaseCollectionItems_CollectionId",
                table: "ProductShowcaseCollectionItems",
                column: "CollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductShowcaseCollectionItems_ProductId",
                table: "ProductShowcaseCollectionItems",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductShowcaseCollectionItems");

            migrationBuilder.DropTable(
                name: "ProductShowcaseCollections");
        }
    }
}
