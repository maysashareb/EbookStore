using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EbookStore.Migrations
{
    /// <inheritdoc />
    public partial class updateBooksTable : Migration
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

            migrationBuilder.AddColumn<bool>(
                name: "IsBuyOnly",
                table: "Books",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastBorrowedDate",
                table: "Books",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "agelimt",
                table: "Books",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "bookrate",
                table: "Books",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Borrowings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BorrowDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReturnDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Borrowings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Borrowings_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WaitingLists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    DateAdded = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BorrowedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SendReminder = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaitingLists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WaitingLists_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WaitingLists_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Borrowings_BookId",
                table: "Borrowings",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_WaitingLists_BookId",
                table: "WaitingLists",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_WaitingLists_UserId",
                table: "WaitingLists",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Borrowings");

            migrationBuilder.DropTable(
                name: "WaitingLists");

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

            migrationBuilder.DropColumn(
                name: "IsBuyOnly",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "LastBorrowedDate",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "agelimt",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "bookrate",
                table: "Books");

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
