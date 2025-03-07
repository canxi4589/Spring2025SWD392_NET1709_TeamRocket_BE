using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HCP.Repository.Migrations
{
    /// <inheritdoc />
    public partial class Entities12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Checkout",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CleaningServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ServicePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DistancePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AdditionalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    District = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlaceId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddressLine = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Checkout", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Checkout_AspNetUsers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CheckoutAdditionalService",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdditionalServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CheckoutId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckoutAdditionalService", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CheckoutAdditionalService_AdditionalServices_AdditionalServiceId",
                        column: x => x.AdditionalServiceId,
                        principalTable: "AdditionalServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CheckoutAdditionalService_Checkout_CheckoutId",
                        column: x => x.CheckoutId,
                        principalTable: "Checkout",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Checkout_CustomerId",
                table: "Checkout",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckoutAdditionalService_AdditionalServiceId",
                table: "CheckoutAdditionalService",
                column: "AdditionalServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckoutAdditionalService_CheckoutId",
                table: "CheckoutAdditionalService",
                column: "CheckoutId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CheckoutAdditionalService");

            migrationBuilder.DropTable(
                name: "Checkout");
        }
    }
}
