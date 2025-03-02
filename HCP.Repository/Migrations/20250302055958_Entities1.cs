using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HCP.Repository.Migrations
{
    /// <inheritdoc />
    public partial class Entities1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateStart",
                table: "ServiceTimeSlots");

            migrationBuilder.DropColumn(
                name: "Zipcode",
                table: "Addresses");

            migrationBuilder.RenameColumn(
                name: "Province",
                table: "CleaningServices",
                newName: "PlaceId");

            migrationBuilder.RenameColumn(
                name: "Province",
                table: "Bookings",
                newName: "PlaceId");

            migrationBuilder.RenameColumn(
                name: "Province",
                table: "Addresses",
                newName: "District");

            migrationBuilder.AddColumn<string>(
                name: "District",
                table: "CleaningServices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<double>(
                name: "Rating",
                table: "Bookings",
                type: "float",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Feedback",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "District",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PlaceId",
                table: "Addresses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "District",
                table: "CleaningServices");

            migrationBuilder.DropColumn(
                name: "District",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "PlaceId",
                table: "Addresses");

            migrationBuilder.RenameColumn(
                name: "PlaceId",
                table: "CleaningServices",
                newName: "Province");

            migrationBuilder.RenameColumn(
                name: "PlaceId",
                table: "Bookings",
                newName: "Province");

            migrationBuilder.RenameColumn(
                name: "District",
                table: "Addresses",
                newName: "Province");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateStart",
                table: "ServiceTimeSlots",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Rating",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Feedback",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Zipcode",
                table: "Addresses",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }
    }
}
