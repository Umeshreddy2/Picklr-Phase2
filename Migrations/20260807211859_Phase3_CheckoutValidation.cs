using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Picklr.Migrations
{
    /// <inheritdoc />
    public partial class Phase3_CheckoutValidation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CustomerDOB",
                table: "Reservations",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerEmail",
                table: "Reservations",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "Reservations",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CustomerPhone",
                table: "Reservations",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerDOB",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "CustomerEmail",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "CustomerPhone",
                table: "Reservations");
        }
    }
}
