using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVCWebAppIsmane.Migrations
{
    /// <inheritdoc />
    public partial class AddingQunatityStockToProductTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QuantityStock",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 20);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuantityStock",
                table: "Products");
        }
    }
}
