using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EbookStore.Migrations
{
    /// <inheritdoc />
    public partial class FixCartItemsSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Books_BookID",
                table: "CartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Books_BookID",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "AddressLine2",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "StreetAddress",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "BookID",
                table: "OrderItems",
                newName: "BookId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItems_BookID",
                table: "OrderItems",
                newName: "IX_OrderItems_BookId");

            migrationBuilder.RenameColumn(
                name: "BookID",
                table: "CartItems",
                newName: "BookId");

            migrationBuilder.RenameIndex(
                name: "IX_CartItems_BookID",
                table: "CartItems",
                newName: "IX_CartItems_BookId");

            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "Orders",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "UserLibrary",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    BookID = table.Column<int>(type: "int", nullable: false),
                    PurchaseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OrderID = table.Column<int>(type: "int", nullable: false),
                    Format = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLibrary", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserLibrary_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserLibrary_Books_BookID",
                        column: x => x.BookID,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserLibrary_Orders_OrderID",
                        column: x => x.OrderID,
                        principalTable: "Orders",
                        principalColumn: "OrderID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserLibrary_BookID",
                table: "UserLibrary",
                column: "BookID");

            migrationBuilder.CreateIndex(
                name: "IX_UserLibrary_OrderID",
                table: "UserLibrary",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_UserLibrary_UserID",
                table: "UserLibrary",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Books_BookId",
                table: "CartItems",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Books_BookId",
                table: "OrderItems",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Books_BookId",
                table: "CartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Books_BookId",
                table: "OrderItems");

            migrationBuilder.DropTable(
                name: "UserLibrary");

            migrationBuilder.RenameColumn(
                name: "BookId",
                table: "OrderItems",
                newName: "BookID");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItems_BookId",
                table: "OrderItems",
                newName: "IX_OrderItems_BookID");

            migrationBuilder.RenameColumn(
                name: "BookId",
                table: "CartItems",
                newName: "BookID");

            migrationBuilder.RenameIndex(
                name: "IX_CartItems_BookId",
                table: "CartItems",
                newName: "IX_CartItems_BookID");

            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldMaxLength: 450);

            migrationBuilder.AddColumn<string>(
                name: "AddressLine2",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StreetAddress",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Books_BookID",
                table: "CartItems",
                column: "BookID",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Books_BookID",
                table: "OrderItems",
                column: "BookID",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
