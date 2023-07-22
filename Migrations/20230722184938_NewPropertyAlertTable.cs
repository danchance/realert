using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Realert.Migrations
{
    /// <inheritdoc />
    public partial class NewPropertyAlertTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NewPropertyAlertNotification",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NotificationName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NotificationFrequency = table.Column<byte>(type: "tinyint", nullable: false),
                    TargetSite = table.Column<int>(type: "int", nullable: false),
                    PropertyType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SearchRadius = table.Column<float>(type: "real", nullable: false),
                    MinPrice = table.Column<long>(type: "bigint", nullable: false),
                    MaxPrice = table.Column<long>(type: "bigint", nullable: false),
                    MinBeds = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaxBeds = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeleteCode = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewPropertyAlertNotification", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NewPropertyAlertNotification");
        }
    }
}
