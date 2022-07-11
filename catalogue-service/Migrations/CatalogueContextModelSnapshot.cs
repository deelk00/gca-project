﻿// <auto-generated />
using System;
using CatalogueService.Model.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CatalogueService.Migrations
{
    [DbContext(typeof(CatalogueContext))]
    partial class CatalogueContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("CatalogueService.Model.Database.Types.Brand", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid>("ImageId")
                        .HasColumnType("uuid")
                        .HasColumnName("image_id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_brands");

                    b.HasIndex("ImageId")
                        .HasDatabaseName("ix_brands_image_id");

                    b.ToTable("brands", (string)null);
                });

            modelBuilder.Entity("CatalogueService.Model.Database.Types.Currency", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)")
                        .HasColumnName("name");

                    b.Property<string>("ShortName")
                        .IsRequired()
                        .HasMaxLength(4)
                        .HasColumnType("character varying(4)")
                        .HasColumnName("short_name");

                    b.Property<string>("Symbol")
                        .IsRequired()
                        .HasMaxLength(4)
                        .HasColumnType("character varying(4)")
                        .HasColumnName("symbol");

                    b.HasKey("Id")
                        .HasName("pk_currencies");

                    b.ToTable("currencies", (string)null);
                });

            modelBuilder.Entity("CatalogueService.Model.Database.Types.DatabaseImage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("ContentType")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("content_type");

                    b.Property<byte[]>("Data")
                        .IsRequired()
                        .HasColumnType("bytea")
                        .HasColumnName("data");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("character varying(512)")
                        .HasColumnName("file_name");

                    b.HasKey("Id")
                        .HasName("pk_database_images");

                    b.ToTable("database_images", (string)null);
                });

            modelBuilder.Entity("CatalogueService.Model.Database.Types.FilterProperty", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid>("FilterPropertyDefinitionId")
                        .HasColumnType("uuid")
                        .HasColumnName("filter_property_definition_id");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uuid")
                        .HasColumnName("product_id");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("value");

                    b.HasKey("Id")
                        .HasName("pk_filter_properties");

                    b.HasIndex("FilterPropertyDefinitionId")
                        .HasDatabaseName("ix_filter_properties_filter_property_definition_id");

                    b.HasIndex("ProductId")
                        .HasDatabaseName("ix_filter_properties_product_id");

                    b.ToTable("filter_properties", (string)null);
                });

            modelBuilder.Entity("CatalogueService.Model.Database.Types.FilterPropertyDefinition", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<int>("ValueType")
                        .HasColumnType("integer")
                        .HasColumnName("value_type");

                    b.HasKey("Id")
                        .HasName("pk_filter_property_definitions");

                    b.ToTable("filter_property_definitions", (string)null);
                });

            modelBuilder.Entity("CatalogueService.Model.Database.Types.Image", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid>("DatabaseImageId")
                        .HasColumnType("uuid")
                        .HasColumnName("database_image_id");

                    b.Property<string>("Hash")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("hash");

                    b.HasKey("Id")
                        .HasName("pk_images");

                    b.HasIndex("DatabaseImageId")
                        .HasDatabaseName("ix_images_database_image_id");

                    b.ToTable("images", (string)null);
                });

            modelBuilder.Entity("CatalogueService.Model.Database.Types.Product", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid>("BrandId")
                        .HasColumnType("uuid")
                        .HasColumnName("brand_id");

                    b.Property<Guid>("CurrencyId")
                        .HasColumnType("uuid")
                        .HasColumnName("currency_id");

                    b.Property<int>("Gender")
                        .HasColumnType("integer")
                        .HasColumnName("gender");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric")
                        .HasColumnName("price");

                    b.Property<Guid>("ProductCategoryId")
                        .HasColumnType("uuid")
                        .HasColumnName("product_category_id");

                    b.Property<long>("Stock")
                        .HasColumnType("bigint")
                        .HasColumnName("stock");

                    b.HasKey("Id")
                        .HasName("pk_products");

                    b.HasIndex("BrandId")
                        .HasDatabaseName("ix_products_brand_id");

                    b.HasIndex("CurrencyId")
                        .HasDatabaseName("ix_products_currency_id");

                    b.HasIndex("ProductCategoryId")
                        .HasDatabaseName("ix_products_product_category_id");

                    b.ToTable("products", (string)null);
                });

            modelBuilder.Entity("CatalogueService.Model.Database.Types.ProductCategory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<Guid?>("ParentCategoryId")
                        .HasColumnType("uuid")
                        .HasColumnName("parent_category_id");

                    b.HasKey("Id")
                        .HasName("pk_product_categories");

                    b.HasIndex("ParentCategoryId")
                        .HasDatabaseName("ix_product_categories_parent_category_id");

                    b.ToTable("product_categories", (string)null);
                });

            modelBuilder.Entity("CatalogueService.Model.Database.Types.Tag", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_tags");

                    b.ToTable("tags", (string)null);
                });

            modelBuilder.Entity("FilterPropertyDefinitionProductCategory", b =>
                {
                    b.Property<Guid>("FilterPropertyDefinitionsId")
                        .HasColumnType("uuid")
                        .HasColumnName("filter_property_definitions_id");

                    b.Property<Guid>("ProductCategoriesId")
                        .HasColumnType("uuid")
                        .HasColumnName("product_categories_id");

                    b.HasKey("FilterPropertyDefinitionsId", "ProductCategoriesId")
                        .HasName("pk_filter_property_definition_product_category");

                    b.HasIndex("ProductCategoriesId")
                        .HasDatabaseName("ix_filter_property_definition_product_category_product_categor");

                    b.ToTable("filter_property_definition_product_category", (string)null);
                });

            modelBuilder.Entity("ProductCategoryTag", b =>
                {
                    b.Property<Guid>("ProductCategoriesId")
                        .HasColumnType("uuid")
                        .HasColumnName("product_categories_id");

                    b.Property<Guid>("TagsId")
                        .HasColumnType("uuid")
                        .HasColumnName("tags_id");

                    b.HasKey("ProductCategoriesId", "TagsId")
                        .HasName("pk_product_category_tag");

                    b.HasIndex("TagsId")
                        .HasDatabaseName("ix_product_category_tag_tags_id");

                    b.ToTable("product_category_tag", (string)null);
                });

            modelBuilder.Entity("ProductTag", b =>
                {
                    b.Property<Guid>("ProductsId")
                        .HasColumnType("uuid")
                        .HasColumnName("products_id");

                    b.Property<Guid>("TagsId")
                        .HasColumnType("uuid")
                        .HasColumnName("tags_id");

                    b.HasKey("ProductsId", "TagsId")
                        .HasName("pk_product_tag");

                    b.HasIndex("TagsId")
                        .HasDatabaseName("ix_product_tag_tags_id");

                    b.ToTable("product_tag", (string)null);
                });

            modelBuilder.Entity("CatalogueService.Model.Database.Types.Brand", b =>
                {
                    b.HasOne("CatalogueService.Model.Database.Types.Image", "Image")
                        .WithMany("Brands")
                        .HasForeignKey("ImageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_brands_images_image_id");

                    b.Navigation("Image");
                });

            modelBuilder.Entity("CatalogueService.Model.Database.Types.FilterProperty", b =>
                {
                    b.HasOne("CatalogueService.Model.Database.Types.FilterPropertyDefinition", "FilterPropertyDefinition")
                        .WithMany("FilterProperties")
                        .HasForeignKey("FilterPropertyDefinitionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_filter_properties_filter_property_definitions_filter_proper");

                    b.HasOne("CatalogueService.Model.Database.Types.Product", "Product")
                        .WithMany("FilterProperties")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_filter_properties_products_product_id");

                    b.Navigation("FilterPropertyDefinition");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("CatalogueService.Model.Database.Types.Image", b =>
                {
                    b.HasOne("CatalogueService.Model.Database.Types.DatabaseImage", "DatabaseImage")
                        .WithMany()
                        .HasForeignKey("DatabaseImageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_images_database_images_database_image_id");

                    b.Navigation("DatabaseImage");
                });

            modelBuilder.Entity("CatalogueService.Model.Database.Types.Product", b =>
                {
                    b.HasOne("CatalogueService.Model.Database.Types.Brand", "Brand")
                        .WithMany("Products")
                        .HasForeignKey("BrandId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_products_brands_brand_id");

                    b.HasOne("CatalogueService.Model.Database.Types.Currency", "Currency")
                        .WithMany("Products")
                        .HasForeignKey("CurrencyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_products_currencies_currency_id");

                    b.HasOne("CatalogueService.Model.Database.Types.ProductCategory", "ProductCategory")
                        .WithMany("Products")
                        .HasForeignKey("ProductCategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_products_product_categories_product_category_id");

                    b.Navigation("Brand");

                    b.Navigation("Currency");

                    b.Navigation("ProductCategory");
                });

            modelBuilder.Entity("CatalogueService.Model.Database.Types.ProductCategory", b =>
                {
                    b.HasOne("CatalogueService.Model.Database.Types.ProductCategory", "ParentCategory")
                        .WithMany("ChildCategories")
                        .HasForeignKey("ParentCategoryId")
                        .HasConstraintName("fk_product_categories_product_categories_parent_category_id");

                    b.Navigation("ParentCategory");
                });

            modelBuilder.Entity("FilterPropertyDefinitionProductCategory", b =>
                {
                    b.HasOne("CatalogueService.Model.Database.Types.FilterPropertyDefinition", null)
                        .WithMany()
                        .HasForeignKey("FilterPropertyDefinitionsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_filter_property_definition_product_category_filter_property");

                    b.HasOne("CatalogueService.Model.Database.Types.ProductCategory", null)
                        .WithMany()
                        .HasForeignKey("ProductCategoriesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_filter_property_definition_product_category_product_categor");
                });

            modelBuilder.Entity("ProductCategoryTag", b =>
                {
                    b.HasOne("CatalogueService.Model.Database.Types.ProductCategory", null)
                        .WithMany()
                        .HasForeignKey("ProductCategoriesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_product_category_tag_product_categories_product_categories_");

                    b.HasOne("CatalogueService.Model.Database.Types.Tag", null)
                        .WithMany()
                        .HasForeignKey("TagsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_product_category_tag_tags_tags_id");
                });

            modelBuilder.Entity("ProductTag", b =>
                {
                    b.HasOne("CatalogueService.Model.Database.Types.Product", null)
                        .WithMany()
                        .HasForeignKey("ProductsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_product_tag_products_products_id");

                    b.HasOne("CatalogueService.Model.Database.Types.Tag", null)
                        .WithMany()
                        .HasForeignKey("TagsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_product_tag_tags_tags_id");
                });

            modelBuilder.Entity("CatalogueService.Model.Database.Types.Brand", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("CatalogueService.Model.Database.Types.Currency", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("CatalogueService.Model.Database.Types.FilterPropertyDefinition", b =>
                {
                    b.Navigation("FilterProperties");
                });

            modelBuilder.Entity("CatalogueService.Model.Database.Types.Image", b =>
                {
                    b.Navigation("Brands");
                });

            modelBuilder.Entity("CatalogueService.Model.Database.Types.Product", b =>
                {
                    b.Navigation("FilterProperties");
                });

            modelBuilder.Entity("CatalogueService.Model.Database.Types.ProductCategory", b =>
                {
                    b.Navigation("ChildCategories");

                    b.Navigation("Products");
                });
#pragma warning restore 612, 618
        }
    }
}
