using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Alzheimer_Detection.Migrations
{
    /// <inheritdoc />
    public partial class AddMRI_ImageToUser1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RésultatTest",
                table: "Users",
                newName: "ResultatTest");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ResultatTest",
                table: "Users",
                newName: "RésultatTest");
        }
    }
}
