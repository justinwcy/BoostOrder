using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoostOrder.Migrations
{
    /// <inheritdoc />
    public partial class Initial3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SrcSmall",
                table: "ProductImage",
                newName: "Src_Small");

            migrationBuilder.RenameColumn(
                name: "SrcMedium",
                table: "ProductImage",
                newName: "Src_Medium");

            migrationBuilder.RenameColumn(
                name: "SrcLarge",
                table: "ProductImage",
                newName: "Src_Large");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Src_Small",
                table: "ProductImage",
                newName: "SrcSmall");

            migrationBuilder.RenameColumn(
                name: "Src_Medium",
                table: "ProductImage",
                newName: "SrcMedium");

            migrationBuilder.RenameColumn(
                name: "Src_Large",
                table: "ProductImage",
                newName: "SrcLarge");
        }
    }
}
