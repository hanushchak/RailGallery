using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RailGallery.Data.Migrations
{
    public partial class ImprovedModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Categories_CategoryID",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "ImageMetadata",
                table: "Images");

            migrationBuilder.AlterColumn<string>(
                name: "ImageDescription",
                table: "Images",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(120)",
                oldMaxLength: 120,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CategoryID",
                table: "Images",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LocationID",
                table: "Images",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LocomotiveID",
                table: "Images",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CategoryTitle",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AlbumTitle",
                table: "Albums",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    LocationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LocationName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.LocationID);
                });

            migrationBuilder.CreateTable(
                name: "Locomotives",
                columns: table => new
                {
                    LocomotiveID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LocomotiveModel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LocomotiveBuilt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locomotives", x => x.LocomotiveID);
                });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 3,
                column: "CategoryTitle",
                value: "Accidents");

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryID", "CategoryTitle" },
                values: new object[,]
                {
                    { 4, "In-Cab Photos" },
                    { 5, "Passenger Trains" },
                    { 6, "Freight Trains" },
                    { 7, "Steam Locomotives" },
                    { 8, "Bridges" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Images_LocationID",
                table: "Images",
                column: "LocationID");

            migrationBuilder.CreateIndex(
                name: "IX_Images_LocomotiveID",
                table: "Images",
                column: "LocomotiveID");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Categories_CategoryID",
                table: "Images",
                column: "CategoryID",
                principalTable: "Categories",
                principalColumn: "CategoryID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Locations_LocationID",
                table: "Images",
                column: "LocationID",
                principalTable: "Locations",
                principalColumn: "LocationID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Locomotives_LocomotiveID",
                table: "Images",
                column: "LocomotiveID",
                principalTable: "Locomotives",
                principalColumn: "LocomotiveID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Categories_CategoryID",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_Locations_LocationID",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_Locomotives_LocomotiveID",
                table: "Images");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "Locomotives");

            migrationBuilder.DropIndex(
                name: "IX_Images_LocationID",
                table: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Images_LocomotiveID",
                table: "Images");

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 8);

            migrationBuilder.DropColumn(
                name: "LocationID",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "LocomotiveID",
                table: "Images");

            migrationBuilder.AlterColumn<string>(
                name: "ImageDescription",
                table: "Images",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(120)",
                oldMaxLength: 120);

            migrationBuilder.AlterColumn<int>(
                name: "CategoryID",
                table: "Images",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "ImageMetadata",
                table: "Images",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CategoryTitle",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "AlbumTitle",
                table: "Albums",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(32)",
                oldMaxLength: 32);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 3,
                column: "CategoryTitle",
                value: "Passanger Trains");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Categories_CategoryID",
                table: "Images",
                column: "CategoryID",
                principalTable: "Categories",
                principalColumn: "CategoryID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
