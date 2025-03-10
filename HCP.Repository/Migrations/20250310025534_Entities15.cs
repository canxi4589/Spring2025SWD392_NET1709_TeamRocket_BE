using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HCP.Repository.Migrations
{
    /// <inheritdoc />
    public partial class Entities15 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdditionalServiceName",
                table: "CheckoutAdditionalService",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DayOfWeek",
                table: "Checkout",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "EndTime",
                table: "Checkout",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<string>(
                name: "ServiceName",
                table: "Checkout",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "StartTime",
                table: "Checkout",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalServiceName",
                table: "CheckoutAdditionalService");

            migrationBuilder.DropColumn(
                name: "DayOfWeek",
                table: "Checkout");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "Checkout");

            migrationBuilder.DropColumn(
                name: "ServiceName",
                table: "Checkout");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "Checkout");
        }
    }
}
