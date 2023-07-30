﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Realert.Data;

#nullable disable

namespace Realert.Migrations
{
    [DbContext(typeof(RealertContext))]
    partial class RealertContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Realert.Models.Job", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsRunning")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastRun")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Job");
                });

            modelBuilder.Entity("Realert.Models.NewPropertyAlertNotification", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("DeleteCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime?>("LastScannedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MaxBeds")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("MaxPrice")
                        .HasColumnType("bigint");

                    b.Property<string>("MinBeds")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("MinPrice")
                        .HasColumnType("bigint");

                    b.Property<byte>("NotificationFrequency")
                        .HasColumnType("tinyint");

                    b.Property<string>("NotificationName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("PropertyType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("SearchRadius")
                        .HasColumnType("real");

                    b.Property<int>("TargetSite")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("NewPropertyAlertNotification");
                });

            modelBuilder.Entity("Realert.Models.PriceAlertNotification", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("DeleteCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ListingLink")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Note")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("NotificationType")
                        .HasColumnType("int");

                    b.Property<bool>("NotifyOnPriceIncrease")
                        .HasColumnType("bit");

                    b.Property<bool>("NotifyOnPropertyDelist")
                        .HasColumnType("bit");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PriceThreshold")
                        .HasColumnType("int");

                    b.Property<int>("TargetSite")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("PriceAlertNotification");
                });

            modelBuilder.Entity("Realert.Models.PriceAlertProperty", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("FirstScannedPrice")
                        .HasColumnType("int");

                    b.Property<DateTime>("LastPriceChangeDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("LastScannedPrice")
                        .HasColumnType("int");

                    b.Property<int>("NumberOfPriceChanges")
                        .HasColumnType("int");

                    b.Property<int>("PriceAlertNotificationId")
                        .HasColumnType("int");

                    b.Property<string>("PropertyName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("PriceAlertNotificationId")
                        .IsUnique();

                    b.ToTable("PriceAlertProperty");
                });

            modelBuilder.Entity("Realert.Models.PriceAlertProperty", b =>
                {
                    b.HasOne("Realert.Models.PriceAlertNotification", "Notification")
                        .WithOne("Property")
                        .HasForeignKey("Realert.Models.PriceAlertProperty", "PriceAlertNotificationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Notification");
                });

            modelBuilder.Entity("Realert.Models.PriceAlertNotification", b =>
                {
                    b.Navigation("Property");
                });
#pragma warning restore 612, 618
        }
    }
}
