using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HCP.Repository.Migrations
{
    /// <inheritdoc />
    public partial class oops : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CleaningMethods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CleaningMethods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PictureUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HomeServices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PictureUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BasePrice = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeServices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HomeServices_ServiceCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "ServiceCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceAddons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceAddons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceAddons_HomeServices_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "HomeServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceCleaningMethods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CleaningMethodId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceCleaningMethods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceCleaningMethods_CleaningMethods_CleaningMethodId",
                        column: x => x.CleaningMethodId,
                        principalTable: "CleaningMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceCleaningMethods_HomeServices_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "HomeServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceOptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceOptions_HomeServices_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "HomeServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceStep",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StepOrder = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceStep", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceStep_HomeServices_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "HomeServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceOptionValues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PriceFactor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ServiceOptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceOptionValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceOptionValues_ServiceOptions_ServiceOptionId",
                        column: x => x.ServiceOptionId,
                        principalTable: "ServiceOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HomeServices_CategoryId",
                table: "HomeServices",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceAddons_ServiceId",
                table: "ServiceAddons",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCleaningMethods_CleaningMethodId",
                table: "ServiceCleaningMethods",
                column: "CleaningMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCleaningMethods_ServiceId",
                table: "ServiceCleaningMethods",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOptions_ServiceId",
                table: "ServiceOptions",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOptionValues_ServiceOptionId",
                table: "ServiceOptionValues",
                column: "ServiceOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceStep_ServiceId",
                table: "ServiceStep",
                column: "ServiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceAddons");

            migrationBuilder.DropTable(
                name: "ServiceCleaningMethods");

            migrationBuilder.DropTable(
                name: "ServiceOptionValues");

            migrationBuilder.DropTable(
                name: "ServiceStep");

            migrationBuilder.DropTable(
                name: "CleaningMethods");

            migrationBuilder.DropTable(
                name: "ServiceOptions");

            migrationBuilder.DropTable(
                name: "HomeServices");

            migrationBuilder.DropTable(
                name: "ServiceCategories");
        }
    }
}
