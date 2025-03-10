using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HCP.Repository.Migrations
{
    /// <inheritdoc />
    public partial class Entities14 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "CheckoutAdditionalService",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Duration",
                table: "CheckoutAdditionalService",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "CheckoutAdditionalService",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "CheckoutAdditionalService");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "CheckoutAdditionalService");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "CheckoutAdditionalService");
        }
    }
}
