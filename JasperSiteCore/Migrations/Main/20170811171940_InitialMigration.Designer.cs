using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using JasperSiteCore.Models.Database;

namespace JasperSiteCore.Migrations.Main
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20170811171940_InitialMigration")]
    partial class InitialMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("JasperSiteCore.Models.Database.Article", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("HtmlContent");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<DateTime>("PublishDate");

                    b.HasKey("Id");

                    b.ToTable("Articles");
                });

            modelBuilder.Entity("JasperSiteCore.Models.Database.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Categories");
                });
        }
    }
}
