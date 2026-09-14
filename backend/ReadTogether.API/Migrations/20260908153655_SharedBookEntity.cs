using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReadTogether.API.Migrations
{
    /// <inheritdoc />
    public partial class SharedBookEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Dev data is disposable: existing rows cannot be re-pointed at the new Books
            // table (they only store title/thumbnail, not a valid book ID), so wipe them.
            migrationBuilder.Sql("DELETE FROM \"BookshelfBooks\";");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookshelfBooks",
                table: "BookshelfBooks");

            migrationBuilder.DropColumn(
                name: "VolumeId",
                table: "BookshelfBooks");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "BookshelfBooks");

            migrationBuilder.DropColumn(
                name: "ThumbnailUrl",
                table: "BookshelfBooks");

            migrationBuilder.AddColumn<string>(
                name: "BookId",
                table: "BookshelfBooks",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "AddedAt",
                table: "BookshelfBooks",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now() at time zone 'utc'");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookshelfBooks",
                table: "BookshelfBooks",
                columns: new[] { "BookshelfId", "BookId" });

            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    AuthorName = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    FirstPublishedYear = table.Column<int>(type: "integer", nullable: true),
                    CoverImageUrl = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookshelfBooks_BookId",
                table: "BookshelfBooks",
                column: "BookId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookshelfBooks_Books_BookId",
                table: "BookshelfBooks",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookshelfBooks_Books_BookId",
                table: "BookshelfBooks");

            migrationBuilder.DropTable(
                name: "Books");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookshelfBooks",
                table: "BookshelfBooks");

            migrationBuilder.DropIndex(
                name: "IX_BookshelfBooks_BookId",
                table: "BookshelfBooks");

            migrationBuilder.DropColumn(
                name: "AddedAt",
                table: "BookshelfBooks");

            migrationBuilder.DropColumn(
                name: "BookId",
                table: "BookshelfBooks");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "BookshelfBooks",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VolumeId",
                table: "BookshelfBooks",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ThumbnailUrl",
                table: "BookshelfBooks",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookshelfBooks",
                table: "BookshelfBooks",
                columns: new[] { "BookshelfId", "VolumeId" });
        }
    }
}
