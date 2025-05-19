using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RESTfulBookWebsite.Migrations
{
    /// <inheritdoc />
    public partial class AddTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AuthorID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChapterIDs = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReadCount = table.Column<int>(type: "int", nullable: false),
                    Genre = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Chapters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookID = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReadCount = table.Column<int>(type: "int", nullable: false),
                    ChapterNumber = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chapters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BookReadIDs = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BookFavoritedIDs = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BookWroteIDs = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Id", "AuthorID", "ChapterIDs", "CreatedDate", "Description", "Genre", "Name", "Rating", "ReadCount", "Title", "UpdatedDate" },
                values: new object[,]
                {
                    { 1, 1, "[1,2,3]", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Epic fantasy adventure set in the land of Middle Earth starring Froddo Baggings", 0, "Lord of the Rings", 5, 0, "Lord of the Rings", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, 2, "[4]", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Adventures of Harry Potter in a world of Wizardy and Magic", 0, "Harry Potter", 4, 1, "Harry Potter", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, 4, "[5,6]", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Adventures of Artemis Fowl and Holly Short", 0, "Artemis Fowl", 4, 2, "Artemis Fowl", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Chapters",
                columns: new[] { "Id", "BookID", "ChapterNumber", "Content", "CreatedDate", "ReadCount", "Title", "UpdatedDate" },
                values: new object[,]
                {
                    { 1, 1, 1, "Froddo finds the One Ring and meets Gandalf", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "The first step", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, 1, 2, "Froddo and Sam have to flee a Nazgul to escape the Shire", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "The Nazgul", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, 1, 3, "Froddo and Sam meet Aragon", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "The FellowShip of the Ring", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, 2, 1, "Harry gets invited to the Hogwarts School of Wizardy and Magic by Hagrid", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "The Boy who Lived", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, 1, 1, "Artemis Fowl tricks an old fairy into handing him the secret book of the fairies and decodes it", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "The Azure Gem", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 6, 1, 2, "Artemis Fowl and Buttler kidnap Holly Short and demand a randsom from the LFPD for her release", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "The Police Capture", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "BookFavoritedIDs", "BookReadIDs", "BookWroteIDs", "Name", "Password", "Username" },
                values: new object[,]
                {
                    { 1, "[2]", "[2,3]", "[1]", "J.R.R Tolkien", "testpass", "George" },
                    { 2, "[1,3]", "[1,3]", "[2]", "J.K. Rowling", "testpass2", "Margaret" },
                    { 3, "[1]", "[1]", "[3]", "Eoin Colfer", "testpass3", "Colfer" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Books");

            migrationBuilder.DropTable(
                name: "Chapters");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
