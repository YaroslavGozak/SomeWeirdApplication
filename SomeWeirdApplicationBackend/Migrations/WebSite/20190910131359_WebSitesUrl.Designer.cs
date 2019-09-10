﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SomeWeirdApplicationBackend.Infrastructure;

namespace SomeWeirdApplicationBackend.Migrations.WebSite
{
    [DbContext(typeof(WebSiteContext))]
    [Migration("20190910131359_WebSitesUrl")]
    partial class WebSitesUrl
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("SomeWeirdApplicationBackend.Models.WebCrawler.WebSiteInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Count");

                    b.Property<string>("Domain");

                    b.Property<string>("Url")
                        .IsRequired();

                    b.Property<int?>("WebSiteInfoId");

                    b.HasKey("Id");

                    b.HasIndex("WebSiteInfoId");

                    b.ToTable("WebSites");
                });

            modelBuilder.Entity("SomeWeirdApplicationBackend.Models.WebCrawler.WebSiteInfo", b =>
                {
                    b.HasOne("SomeWeirdApplicationBackend.Models.WebCrawler.WebSiteInfo")
                        .WithMany("LinkedSites")
                        .HasForeignKey("WebSiteInfoId");
                });
#pragma warning restore 612, 618
        }
    }
}
