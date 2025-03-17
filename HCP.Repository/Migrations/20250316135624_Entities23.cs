using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HCP.Repository.Migrations
{
    /// <inheritdoc />
    public partial class Entities23 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefundRequests_AspNetUsers_AcceptBy",
                table: "RefundRequests");

            migrationBuilder.AlterColumn<string>(
                name: "AcceptBy",
                table: "RefundRequests",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_RefundRequests_AspNetUsers_AcceptBy",
                table: "RefundRequests",
                column: "AcceptBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefundRequests_AspNetUsers_AcceptBy",
                table: "RefundRequests");

            migrationBuilder.AlterColumn<string>(
                name: "AcceptBy",
                table: "RefundRequests",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RefundRequests_AspNetUsers_AcceptBy",
                table: "RefundRequests",
                column: "AcceptBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
