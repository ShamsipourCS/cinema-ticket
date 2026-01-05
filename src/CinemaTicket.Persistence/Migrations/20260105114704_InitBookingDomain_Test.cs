using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinemaTicket.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitBookingDomain_Test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Ticket_ShowtimeId",
                table: "Ticket");

            migrationBuilder.AlterColumn<string>(
                name: "TicketNumber",
                table: "Ticket",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "HolderName",
                table: "Ticket",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "Ticket",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "HallId1",
                table: "Showtime",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MovieId1",
                table: "Showtime",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "StripePaymentIntentId",
                table: "Payment",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Currency",
                table: "Payment",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_ShowtimeId_SeatId",
                table: "Ticket",
                columns: new[] { "ShowtimeId", "SeatId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_UserId1",
                table: "Ticket",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_Showtime_HallId1",
                table: "Showtime",
                column: "HallId1");

            migrationBuilder.CreateIndex(
                name: "IX_Showtime_MovieId1",
                table: "Showtime",
                column: "MovieId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Showtime_Hall_HallId1",
                table: "Showtime",
                column: "HallId1",
                principalTable: "Hall",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Showtime_Movie_MovieId1",
                table: "Showtime",
                column: "MovieId1",
                principalTable: "Movie",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Ticket_Users_UserId1",
                table: "Ticket",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Showtime_Hall_HallId1",
                table: "Showtime");

            migrationBuilder.DropForeignKey(
                name: "FK_Showtime_Movie_MovieId1",
                table: "Showtime");

            migrationBuilder.DropForeignKey(
                name: "FK_Ticket_Users_UserId1",
                table: "Ticket");

            migrationBuilder.DropIndex(
                name: "IX_Ticket_ShowtimeId_SeatId",
                table: "Ticket");

            migrationBuilder.DropIndex(
                name: "IX_Ticket_UserId1",
                table: "Ticket");

            migrationBuilder.DropIndex(
                name: "IX_Showtime_HallId1",
                table: "Showtime");

            migrationBuilder.DropIndex(
                name: "IX_Showtime_MovieId1",
                table: "Showtime");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "HallId1",
                table: "Showtime");

            migrationBuilder.DropColumn(
                name: "MovieId1",
                table: "Showtime");

            migrationBuilder.AlterColumn<string>(
                name: "TicketNumber",
                table: "Ticket",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "HolderName",
                table: "Ticket",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "StripePaymentIntentId",
                table: "Payment",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Currency",
                table: "Payment",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_ShowtimeId",
                table: "Ticket",
                column: "ShowtimeId");
        }
    }
}
