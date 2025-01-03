using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EbookStore.Migrations
{
    /// <inheritdoc />
    public partial class AddBorrowedDateAndDueDateToWaitingList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Books",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Author",
                table: "Books",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "agelimt",
                table: "Books",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "bookrate",
                table: "Books",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: false);
            // Add BorrowedDate column to the WaitingList table
            migrationBuilder.AddColumn<DateTime>(
                name: "BorrowedDate",
                table: "WaitingList",
                type: "datetime2",
                nullable: true);

            // Add DueDate column to the WaitingList table
            migrationBuilder.AddColumn<DateTime>(
                name: "DueDate",
                table: "WaitingList",
                type: "datetime2",
                nullable: true);

            // Add SendReminder column to the WaitingList table
            migrationBuilder.AddColumn<bool>(
                name: "SendReminder",
                table: "WaitingList",
                type: "bit",
                nullable: false,
                defaultValue: false);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove BorrowedDate column from the WaitingList table
            migrationBuilder.DropColumn(
                name: "BorrowedDate",
                table: "WaitingList");

            // Remove DueDate column from the WaitingList table
            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "WaitingList");

            // Remove SendReminder column from the WaitingList table
            migrationBuilder.DropColumn(
                name: "SendReminder",
                table: "WaitingList");

            migrationBuilder.AlterColumn<int>(
                 name: "agelimt",
                 table: "Books",
                 type: "int",
                 nullable: false,
                 defaultValue: 0,
                 oldClrType: typeof(string),
                 oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<decimal>(
                name: "bookrate",
                table: "Books",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Books",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Author",
                table: "Books",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
