using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Alzheimer_Detection.Migrations
{
    /// <inheritdoc />
    public partial class AddMRI_ImageToUser121 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "PdfReport",
                table: "Users",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReportGeneratedDate",
                table: "Users",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PdfReport",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ReportGeneratedDate",
                table: "Users");
        }
    }
}
