using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RailGallery.Data.Migrations
{
    public partial class Newseeddata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Locations",
                columns: new[] { "LocationID", "LocationName" },
                values: new object[,]
                {
                    { 1, "Ontario, Canada" },
                    { 2, "New York, USA" },
                    { 3, "New Jersey, USA" },
                    { 4, "British Columbia, Canada" },
                    { 5, "Nova Scotia, Canada" },
                    { 6, "Virginia, USA" },
                    { 7, "Manitoba, Canada" },
                    { 8, "Alberta, Canada" }
                });

            migrationBuilder.InsertData(
                table: "Locomotives",
                columns: new[] { "LocomotiveID", "LocomotiveBuilt", "LocomotiveModel" },
                values: new object[,]
                {
                    { 1, new DateTime(1964, 8, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Steam 4-6-0" },
                    { 2, new DateTime(1997, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "GE ES44AC" },
                    { 3, new DateTime(2000, 2, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), "GE AC4400CWM" },
                    { 4, new DateTime(1956, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "R160" },
                    { 5, new DateTime(1994, 1, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "GE P42DC" },
                    { 6, new DateTime(2007, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "GC 546" },
                    { 7, new DateTime(1956, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Steam 2-8-4" },
                    { 8, new DateTime(2009, 6, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "AMT 1325" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "LocationID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "LocationID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "LocationID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "LocationID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "LocationID",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "LocationID",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "LocationID",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "LocationID",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Locomotives",
                keyColumn: "LocomotiveID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Locomotives",
                keyColumn: "LocomotiveID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Locomotives",
                keyColumn: "LocomotiveID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Locomotives",
                keyColumn: "LocomotiveID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Locomotives",
                keyColumn: "LocomotiveID",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Locomotives",
                keyColumn: "LocomotiveID",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Locomotives",
                keyColumn: "LocomotiveID",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Locomotives",
                keyColumn: "LocomotiveID",
                keyValue: 8);
        }
    }
}
