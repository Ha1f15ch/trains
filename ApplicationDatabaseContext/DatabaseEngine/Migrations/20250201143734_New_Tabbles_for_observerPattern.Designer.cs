﻿// <auto-generated />
using System;
using DatabaseEngine;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DatabaseEngine.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250201143734_New_Tabbles_for_observerPattern")]
    partial class New_Tabbles_for_observerPattern
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("DatabaseEngine.Models.Book", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Author")
                        .HasColumnType("text");

                    b.Property<int?>("CountLists")
                        .HasColumnType("integer");

                    b.Property<string>("CreatedAt")
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("Book", "dbo");
                });

            modelBuilder.Entity("DatabaseEngine.Models.NewsChannel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("CountSubscribers")
                        .HasColumnType("integer");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("NewsChannel", "dict");
                });

            modelBuilder.Entity("DatabaseEngine.Models.NewsChannelsPosts", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("AuthorPost")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("BodyPost")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("FooterPost")
                        .HasColumnType("text");

                    b.Property<int>("NewsChannelId")
                        .HasColumnType("integer");

                    b.Property<string>("SurceImage")
                        .HasColumnType("text");

                    b.Property<string>("TitlePost")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("NewsChannelId");

                    b.ToTable("NewsChannelsPosts", "dbo");
                });

            modelBuilder.Entity("DatabaseEngine.Models.NewsChannelsSubscribers", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("NewsChannelId")
                        .HasColumnType("integer");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("NewsChannelId");

                    b.HasIndex("UserId");

                    b.ToTable("NewsChannelsSubscribers", "dbo");
                });

            modelBuilder.Entity("DatabaseEngine.Models.Subscription", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("BookId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("SubscriptionDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("BookId");

                    b.HasIndex("UserId");

                    b.ToTable("Subscription", "dbo");
                });

            modelBuilder.Entity("DatabaseEngine.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("DateCreate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DateDelete")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("DateUpdate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("UserIsActive")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.ToTable("User", "dbo");
                });

            modelBuilder.Entity("DatabaseEngine.Models.NewsChannelsPosts", b =>
                {
                    b.HasOne("DatabaseEngine.Models.NewsChannel", "NewsChannel")
                        .WithMany("ListNewsChannelsPosts")
                        .HasForeignKey("NewsChannelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("NewsChannel");
                });

            modelBuilder.Entity("DatabaseEngine.Models.NewsChannelsSubscribers", b =>
                {
                    b.HasOne("DatabaseEngine.Models.NewsChannel", "NewsChannel")
                        .WithMany("ListNewsChannelsSubscribers")
                        .HasForeignKey("NewsChannelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DatabaseEngine.Models.User", "User")
                        .WithMany("ListNewsChannelsSubscribers")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("NewsChannel");

                    b.Navigation("User");
                });

            modelBuilder.Entity("DatabaseEngine.Models.Subscription", b =>
                {
                    b.HasOne("DatabaseEngine.Models.Book", "Book")
                        .WithMany("Subscriptions")
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DatabaseEngine.Models.User", "User")
                        .WithMany("Subscriptions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Book");

                    b.Navigation("User");
                });

            modelBuilder.Entity("DatabaseEngine.Models.Book", b =>
                {
                    b.Navigation("Subscriptions");
                });

            modelBuilder.Entity("DatabaseEngine.Models.NewsChannel", b =>
                {
                    b.Navigation("ListNewsChannelsPosts");

                    b.Navigation("ListNewsChannelsSubscribers");
                });

            modelBuilder.Entity("DatabaseEngine.Models.User", b =>
                {
                    b.Navigation("ListNewsChannelsSubscribers");

                    b.Navigation("Subscriptions");
                });
#pragma warning restore 612, 618
        }
    }
}
