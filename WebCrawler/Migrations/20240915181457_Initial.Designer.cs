﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using WebCrawler.Models;

#nullable disable

namespace WebCrawler.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240915181457_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("WebCrawler.Models.NodeModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("CrawlTime")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("UrlMain")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("WebsiteRecordId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("WebsiteRecordId");

                    b.ToTable("Node");
                });

            modelBuilder.Entity("WebCrawler.Models.NodeNeighbourModel", b =>
                {
                    b.Property<int>("NodeId")
                        .HasColumnType("integer");

                    b.Property<int>("NeighbourNodeId")
                        .HasColumnType("integer");

                    b.HasKey("NodeId", "NeighbourNodeId");

                    b.HasIndex("NeighbourNodeId");

                    b.ToTable("NodeNeighbour");
                });

            modelBuilder.Entity("WebCrawler.Models.WebsiteRecordModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("BoundaryRegExp")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Days")
                        .HasColumnType("integer");

                    b.Property<int>("Hours")
                        .HasColumnType("integer");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<string>("Label")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("LastChange")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("LastExecution")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool?>("LastStatus")
                        .HasColumnType("boolean");

                    b.Property<int>("Minutes")
                        .HasColumnType("integer");

                    b.Property<string>("Tags")
                        .HasColumnType("text");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("WebsiteRecord");
                });

            modelBuilder.Entity("WebCrawler.Models.NodeModel", b =>
                {
                    b.HasOne("WebCrawler.Models.WebsiteRecordModel", null)
                        .WithMany()
                        .HasForeignKey("WebsiteRecordId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("WebCrawler.Models.NodeNeighbourModel", b =>
                {
                    b.HasOne("WebCrawler.Models.NodeModel", null)
                        .WithMany()
                        .HasForeignKey("NeighbourNodeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("WebCrawler.Models.NodeModel", null)
                        .WithMany()
                        .HasForeignKey("NodeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
