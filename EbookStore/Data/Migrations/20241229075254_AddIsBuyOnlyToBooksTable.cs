using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EbookStore.Migrations
{
    /// <inheritdoc />
    public partial class AddIsBuyOnlyToBooksTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BookId",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "BookTitle",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "BorrowDate",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DueDate",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPurchase",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "OrderPrice",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

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

            // Alter the column to ensure it exists with the correct type if it wasn't there before
            migrationBuilder.AlterColumn<DateTime>(
                name: "LastBorrowedDate",
                table: "Books",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
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

            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.DropColumn(
                name: "BookId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "BookTitle",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "BorrowDate",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "IsPurchase",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "OrderPrice",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "Orders");

            // Revert the column back to the previous state (in this case, drop the column)
            migrationBuilder.AlterColumn<DateTime>(
                name: "LastBorrowedDate",
                table: "Books",
                type: "datetime2",
                nullable: false, // or any other previous settings you want to revert to
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

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
