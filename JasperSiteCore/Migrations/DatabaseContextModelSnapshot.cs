﻿// <auto-generated />
using JasperSiteCore.Models.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace JasperSiteCore.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125");

            modelBuilder.Entity("JasperSiteCore.Models.Database.Article", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CategoryId");

                    b.Property<string>("HtmlContent");

                    b.Property<string>("Keywords");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<bool>("Publish");

                    b.Property<DateTime>("PublishDate");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("Articles");
                });

            modelBuilder.Entity("JasperSiteCore.Models.Database.BlockHolder", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("ThemeId");

                    b.HasKey("Id");

                    b.HasIndex("ThemeId");

                    b.ToTable("BlockHolders");
                });

            modelBuilder.Entity("JasperSiteCore.Models.Database.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("JasperSiteCore.Models.Database.Holder_Block", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("BlockHolderId");

                    b.Property<int>("Order");

                    b.Property<int>("TextBlockId");

                    b.HasKey("Id");

                    b.HasIndex("BlockHolderId");

                    b.HasIndex("TextBlockId");

                    b.ToTable("HolderBlocks");
                });

            modelBuilder.Entity("JasperSiteCore.Models.Database.Image", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ImageDataId");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("ImageDataId");

                    b.ToTable("Images");
                });

            modelBuilder.Entity("JasperSiteCore.Models.Database.ImageData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

                    b.Property<byte[]>("Data")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("ImageData");
                });

            modelBuilder.Entity("JasperSiteCore.Models.Database.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("JasperSiteCore.Models.Database.Setting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Key")
                        .IsRequired();

                    b.Property<string>("Value");

                    b.HasKey("Id");

                    b.ToTable("Settings");
                });

            modelBuilder.Entity("JasperSiteCore.Models.Database.TextBlock", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Content");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("TextBlocks");
                });

            modelBuilder.Entity("JasperSiteCore.Models.Database.Theme", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Themes");
                });

            modelBuilder.Entity("JasperSiteCore.Models.Database.UrlRewrite", b =>
                {
                    b.Property<string>("Url")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ArticleId");

                    b.HasKey("Url");

                    b.HasIndex("ArticleId");

                    b.ToTable("UrlRewrite");
                });

            modelBuilder.Entity("JasperSiteCore.Models.Database.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Nickname")
                        .IsRequired();

                    b.Property<string>("Password")
                        .IsRequired();

                    b.Property<int>("RoleId");

                    b.Property<string>("Salt")
                        .IsRequired();

                    b.Property<string>("Username")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("JasperSiteCore.Models.Database.Article", b =>
                {
                    b.HasOne("JasperSiteCore.Models.Database.Category", "Category")
                        .WithMany("Articles")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("JasperSiteCore.Models.Database.BlockHolder", b =>
                {
                    b.HasOne("JasperSiteCore.Models.Database.Theme", "Theme")
                        .WithMany()
                        .HasForeignKey("ThemeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("JasperSiteCore.Models.Database.Holder_Block", b =>
                {
                    b.HasOne("JasperSiteCore.Models.Database.BlockHolder", "BlockHolder")
                        .WithMany()
                        .HasForeignKey("BlockHolderId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("JasperSiteCore.Models.Database.TextBlock", "TextBlock")
                        .WithMany()
                        .HasForeignKey("TextBlockId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("JasperSiteCore.Models.Database.Image", b =>
                {
                    b.HasOne("JasperSiteCore.Models.Database.ImageData", "ImageData")
                        .WithMany()
                        .HasForeignKey("ImageDataId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("JasperSiteCore.Models.Database.UrlRewrite", b =>
                {
                    b.HasOne("JasperSiteCore.Models.Database.Article", "Article")
                        .WithMany()
                        .HasForeignKey("ArticleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("JasperSiteCore.Models.Database.User", b =>
                {
                    b.HasOne("JasperSiteCore.Models.Database.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
