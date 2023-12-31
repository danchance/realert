﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Realert.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PriceAlertNotification",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ListingLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TargetSite = table.Column<int>(type: "int", nullable: false),
                    PriceThreshold = table.Column<int>(type: "int", nullable: false),
                    NotifyOnPriceIncrease = table.Column<bool>(type: "bit", nullable: false),
                    NotifyOnPropertyDelist = table.Column<bool>(type: "bit", nullable: false),
                    NotificationType = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeleteCode = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceAlertNotification", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PriceAlertProperty",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PropertyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstScannedPrice = table.Column<int>(type: "int", nullable: false),
                    LastScannedPrice = table.Column<int>(type: "int", nullable: false),
                    NumberOfPriceChanges = table.Column<int>(type: "int", nullable: false),
                    LastPriceChangeDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PriceAlertNotificationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceAlertProperty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PriceAlertProperty_PriceAlertNotification_PriceAlertNotificationId",
                        column: x => x.PriceAlertNotificationId,
                        principalTable: "PriceAlertNotification",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PriceAlertProperty_PriceAlertNotificationId",
                table: "PriceAlertProperty",
                column: "PriceAlertNotificationId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PriceAlertProperty");

            migrationBuilder.DropTable(
                name: "PriceAlertNotification");
        }
    }
}
