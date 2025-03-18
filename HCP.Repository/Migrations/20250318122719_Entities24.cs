using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HCP.Repository.Migrations
{
    /// <inheritdoc />
    public partial class Entities24 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "HousekeeperVerifiedBy",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HousekeeperVerifiedBy",
                table: "AspNetUsers");
        }
    }
}
