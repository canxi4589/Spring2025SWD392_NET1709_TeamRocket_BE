using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HCP.Repository.Migrations
{
    /// <inheritdoc />
    public partial class Entities26 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsHousekeeperVerified",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "HousekeeperStatus",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HousekeeperStatus",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<bool>(
                name: "IsHousekeeperVerified",
                table: "AspNetUsers",
                type: "bit",
                nullable: true);
        }
    }
}
