using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogueService.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "currencies",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    short_name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_currencies", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "database_images",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    file_name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    content_type = table.Column<string>(type: "text", nullable: false),
                    data = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_database_images", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "filter_property_definitions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    value_type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_filter_property_definitions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "product_categories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    parent_category_id = table.Column<Guid>(type: "uuid", nullable: true),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_product_categories", x => x.id);
                    table.ForeignKey(
                        name: "fk_product_categories_product_categories_parent_category_id",
                        column: x => x.parent_category_id,
                        principalTable: "product_categories",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "tags",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tags", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "images",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    hash = table.Column<string>(type: "text", nullable: false),
                    database_image_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_images", x => x.id);
                    table.ForeignKey(
                        name: "fk_images_database_images_database_image_id",
                        column: x => x.database_image_id,
                        principalTable: "database_images",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "filter_property_definition_product_category",
                columns: table => new
                {
                    filter_property_definitions_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_categories_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_filter_property_definition_product_category", x => new { x.filter_property_definitions_id, x.product_categories_id });
                    table.ForeignKey(
                        name: "fk_filter_property_definition_product_category_filter_property",
                        column: x => x.filter_property_definitions_id,
                        principalTable: "filter_property_definitions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_filter_property_definition_product_category_product_categor",
                        column: x => x.product_categories_id,
                        principalTable: "product_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "product_category_tag",
                columns: table => new
                {
                    product_categories_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tags_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_product_category_tag", x => new { x.product_categories_id, x.tags_id });
                    table.ForeignKey(
                        name: "fk_product_category_tag_product_categories_product_categories_",
                        column: x => x.product_categories_id,
                        principalTable: "product_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_product_category_tag_tags_tags_id",
                        column: x => x.tags_id,
                        principalTable: "tags",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "brands",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    image_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brands", x => x.id);
                    table.ForeignKey(
                        name: "fk_brands_images_image_id",
                        column: x => x.image_id,
                        principalTable: "images",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    currency_id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    stock = table.Column<long>(type: "bigint", nullable: false),
                    gender = table.Column<int>(type: "integer", nullable: false),
                    price = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_products", x => x.id);
                    table.ForeignKey(
                        name: "fk_products_brands_brand_id",
                        column: x => x.brand_id,
                        principalTable: "brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_products_currencies_currency_id",
                        column: x => x.currency_id,
                        principalTable: "currencies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_products_product_categories_product_category_id",
                        column: x => x.product_category_id,
                        principalTable: "product_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "filter_properties",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    filter_property_definition_id = table.Column<Guid>(type: "uuid", nullable: false),
                    value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_filter_properties", x => x.id);
                    table.ForeignKey(
                        name: "fk_filter_properties_filter_property_definitions_filter_proper",
                        column: x => x.filter_property_definition_id,
                        principalTable: "filter_property_definitions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_filter_properties_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "product_tag",
                columns: table => new
                {
                    products_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tags_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_product_tag", x => new { x.products_id, x.tags_id });
                    table.ForeignKey(
                        name: "fk_product_tag_products_products_id",
                        column: x => x.products_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_product_tag_tags_tags_id",
                        column: x => x.tags_id,
                        principalTable: "tags",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_brands_image_id",
                table: "brands",
                column: "image_id");

            migrationBuilder.CreateIndex(
                name: "ix_filter_properties_filter_property_definition_id",
                table: "filter_properties",
                column: "filter_property_definition_id");

            migrationBuilder.CreateIndex(
                name: "ix_filter_properties_product_id",
                table: "filter_properties",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "ix_filter_property_definition_product_category_product_categor",
                table: "filter_property_definition_product_category",
                column: "product_categories_id");

            migrationBuilder.CreateIndex(
                name: "ix_images_database_image_id",
                table: "images",
                column: "database_image_id");

            migrationBuilder.CreateIndex(
                name: "ix_product_categories_parent_category_id",
                table: "product_categories",
                column: "parent_category_id");

            migrationBuilder.CreateIndex(
                name: "ix_product_category_tag_tags_id",
                table: "product_category_tag",
                column: "tags_id");

            migrationBuilder.CreateIndex(
                name: "ix_product_tag_tags_id",
                table: "product_tag",
                column: "tags_id");

            migrationBuilder.CreateIndex(
                name: "ix_products_brand_id",
                table: "products",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                name: "ix_products_currency_id",
                table: "products",
                column: "currency_id");

            migrationBuilder.CreateIndex(
                name: "ix_products_product_category_id",
                table: "products",
                column: "product_category_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "filter_properties");

            migrationBuilder.DropTable(
                name: "filter_property_definition_product_category");

            migrationBuilder.DropTable(
                name: "product_category_tag");

            migrationBuilder.DropTable(
                name: "product_tag");

            migrationBuilder.DropTable(
                name: "filter_property_definitions");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "tags");

            migrationBuilder.DropTable(
                name: "brands");

            migrationBuilder.DropTable(
                name: "currencies");

            migrationBuilder.DropTable(
                name: "product_categories");

            migrationBuilder.DropTable(
                name: "images");

            migrationBuilder.DropTable(
                name: "database_images");
        }
    }
}
