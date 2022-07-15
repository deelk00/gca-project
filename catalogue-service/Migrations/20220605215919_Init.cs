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
                    name = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    parent_category_id = table.Column<Guid>(type: "uuid", nullable: true)
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
                    name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tags", x => x.id);
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
                name: "products",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    stock = table.Column<int>(type: "integer", nullable: false),
                    price = table.Column<decimal>(type: "numeric", nullable: false),
                    gender = table.Column<int>(type: "integer", nullable: false),
                    product_category_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_products", x => x.id);
                    table.ForeignKey(
                        name: "fk_products_product_categories_product_category_id",
                        column: x => x.product_category_id,
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
                name: "filter_properties",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    value = table.Column<string>(type: "text", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    filter_property_definition_id = table.Column<Guid>(type: "uuid", nullable: false)
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
                name: "product_categories");
        }
    }
}
