using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolyStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentDateToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "PaymentDate",
                table: "Orders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentIntentId",
                table: "Orders",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentDate",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentIntentId",
                table: "Orders");
        }
    }
}
