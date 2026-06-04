using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolyStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReservationAndProductStateMemory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LastStatus",
                table: "Products",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ReserveUntil",
                table: "Orders",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastStatus",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ReserveUntil",
                table: "Orders");
        }
    }
}
