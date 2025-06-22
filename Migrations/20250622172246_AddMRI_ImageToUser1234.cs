using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Alzheimer_Detection.Migrations
{
    /// <inheritdoc />
    public partial class AddMRI_ImageToUser1234 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PdfReport",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ReportGeneratedDate",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "ResultatTest",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "MRI_Image",
                table: "Users",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0],
                oldClrType: typeof(byte[]),
                oldType: "varbinary(max)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ResultatTest",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<byte[]>(
                name: "MRI_Image",
                table: "Users",
                type: "varbinary(max)",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(max)");

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
    }
}
