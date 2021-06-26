using Microsoft.EntityFrameworkCore.Migrations;

namespace RailGallery.Data.Migrations
{
    public partial class NewImageModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ImageTitle",
                table: "Images",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Images",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Images");

            migrationBuilder.AlterColumn<string>(
                name: "ImageTitle",
                table: "Images",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
