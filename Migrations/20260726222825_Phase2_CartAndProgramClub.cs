using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Picklr.Migrations
{
    /// <inheritdoc />
    public partial class Phase2_CartAndProgramClub : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AvailableDays",
                table: "Programs",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ClubID",
                table: "Programs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    ReservationID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProgramID = table.Column<int>(type: "INTEGER", nullable: false),
                    ProgramDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FeePaid = table.Column<decimal>(type: "TEXT", nullable: false),
                    ReservedOn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.ReservationID);
                    table.ForeignKey(
                        name: "FK_Reservations_Programs_ProgramID",
                        column: x => x.ProgramID,
                        principalTable: "Programs",
                        principalColumn: "ProgramID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Clubs",
                columns: new[] { "ClubID", "Description", "Location", "Name" },
                values: new object[] { 3, "Our newest club, open year-round with 6 indoor courts.", "789 Broadway, New York, NY", "Picklr New York" });

            migrationBuilder.UpdateData(
                table: "Programs",
                keyColumn: "ProgramID",
                keyValue: 1,
                columns: new[] { "AvailableDays", "ClubID" },
                values: new object[] { "Monday,Wednesday,Friday", 1 });

            migrationBuilder.UpdateData(
                table: "Programs",
                keyColumn: "ProgramID",
                keyValue: 2,
                columns: new[] { "AvailableDays", "ClubID" },
                values: new object[] { "Tuesday,Thursday", 1 });

            migrationBuilder.UpdateData(
                table: "Programs",
                keyColumn: "ProgramID",
                keyValue: 3,
                columns: new[] { "AvailableDays", "ClubID" },
                values: new object[] { "Saturday,Sunday", 2 });

            migrationBuilder.InsertData(
                table: "Programs",
                columns: new[] { "ProgramID", "AvailableDays", "ClubID", "Description", "Fee", "Name" },
                values: new object[,]
                {
                    { 5, "Saturday", 2, "Casual weekend social play, all levels welcome.", 0.00m, "Picklr Social" },
                    { 4, "Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday", 3, "The program is designed for the beginners.", 10.00m, "Picklr 101" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Programs_ClubID",
                table: "Programs",
                column: "ClubID");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_ProgramID",
                table: "Reservations",
                column: "ProgramID");

            migrationBuilder.AddForeignKey(
                name: "FK_Programs_Clubs_ClubID",
                table: "Programs",
                column: "ClubID",
                principalTable: "Clubs",
                principalColumn: "ClubID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Programs_Clubs_ClubID",
                table: "Programs");

            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Programs_ClubID",
                table: "Programs");

            migrationBuilder.DeleteData(
                table: "Programs",
                keyColumn: "ProgramID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Programs",
                keyColumn: "ProgramID",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Clubs",
                keyColumn: "ClubID",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "AvailableDays",
                table: "Programs");

            migrationBuilder.DropColumn(
                name: "ClubID",
                table: "Programs");
        }
    }
}
