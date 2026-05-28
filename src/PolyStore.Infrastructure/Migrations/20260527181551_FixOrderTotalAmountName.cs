using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolyStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixOrderTotalAmountName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalAmout",
                table: "Orders",
                newName: "TotalAmount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalAmount",
                table: "Orders",
                newName: "TotalAmout");
        }
    }
}
