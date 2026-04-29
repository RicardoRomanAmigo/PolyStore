using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolyStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStylePropertiesToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccentColor",
                table: "Products",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BackgroundImageUrl",
                table: "Products",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomCss",
                table: "Products",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FontFamily",
                table: "Products",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimaryColor",
                table: "Products",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccentColor",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "BackgroundImageUrl",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CustomCss",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "FontFamily",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PrimaryColor",
                table: "Products");
        }
    }
}
