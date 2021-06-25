using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RailGallery.Data.Migrations
{
    public partial class Addedmodels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Albums",
                columns: table => new
                {
                    AlbumID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AlbumTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AlbumDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AlbumPrivacy = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Albums", x => x.AlbumID);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryTitle = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryID);
                });

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    ImageID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImageTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageMetadata = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageTakenDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ImageUploadedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ImageStatus = table.Column<int>(type: "int", nullable: false),
                    ImagePrivacy = table.Column<int>(type: "int", nullable: false),
                    CategoryID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.ImageID);
                    table.ForeignKey(
                        name: "FK_Images_Categories_CategoryID",
                        column: x => x.CategoryID,
                        principalTable: "Categories",
                        principalColumn: "CategoryID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AlbumImage",
                columns: table => new
                {
                    AlbumsAlbumID = table.Column<int>(type: "int", nullable: false),
                    ImagesImageID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlbumImage", x => new { x.AlbumsAlbumID, x.ImagesImageID });
                    table.ForeignKey(
                        name: "FK_AlbumImage_Albums_AlbumsAlbumID",
                        column: x => x.AlbumsAlbumID,
                        principalTable: "Albums",
                        principalColumn: "AlbumID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlbumImage_Images_ImagesImageID",
                        column: x => x.ImagesImageID,
                        principalTable: "Images",
                        principalColumn: "ImageID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    CommentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CommentText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CommentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ImageID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.CommentID);
                    table.ForeignKey(
                        name: "FK_Comments_Images_ImageID",
                        column: x => x.ImageID,
                        principalTable: "Images",
                        principalColumn: "ImageID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Favorites",
                columns: table => new
                {
                    FavoriteID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImageID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favorites", x => x.FavoriteID);
                    table.ForeignKey(
                        name: "FK_Favorites_Images_ImageID",
                        column: x => x.ImageID,
                        principalTable: "Images",
                        principalColumn: "ImageID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Likes",
                columns: table => new
                {
                    LikeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImageID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Likes", x => x.LikeID);
                    table.ForeignKey(
                        name: "FK_Likes_Images_ImageID",
                        column: x => x.ImageID,
                        principalTable: "Images",
                        principalColumn: "ImageID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlbumImage_ImagesImageID",
                table: "AlbumImage",
                column: "ImagesImageID");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ImageID",
                table: "Comments",
                column: "ImageID");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_ImageID",
                table: "Favorites",
                column: "ImageID");

            migrationBuilder.CreateIndex(
                name: "IX_Images_CategoryID",
                table: "Images",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_ImageID",
                table: "Likes",
                column: "ImageID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlbumImage");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "Favorites");

            migrationBuilder.DropTable(
                name: "Likes");

            migrationBuilder.DropTable(
                name: "Albums");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
