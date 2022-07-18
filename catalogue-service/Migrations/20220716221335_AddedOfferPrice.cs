using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogueService.Migrations
{
    public partial class AddedOfferPrice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_products_brands_brand_id",
                table: "products");

            migrationBuilder.AlterColumn<Guid>(
                name: "brand_id",
                table: "products",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<decimal>(
                name: "offer_price",
                table: "products",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_products_brands_brand_id",
                table: "products",
                column: "brand_id",
                principalTable: "brands",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_products_brands_brand_id",
                table: "products");

            migrationBuilder.DropColumn(
                name: "offer_price",
                table: "products");

            migrationBuilder.AlterColumn<Guid>(
                name: "brand_id",
                table: "products",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_products_brands_brand_id",
                table: "products",
                column: "brand_id",
                principalTable: "brands",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
