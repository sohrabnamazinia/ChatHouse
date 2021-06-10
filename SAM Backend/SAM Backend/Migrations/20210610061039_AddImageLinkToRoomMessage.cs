using Microsoft.EntityFrameworkCore.Migrations;

namespace SAM_Backend.Migrations
{
    public partial class AddImageLinkToRoomMessage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LinkIfImage",
                table: "RoomsMessages",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LinkIfImage",
                table: "RoomsMessages");
        }
    }
}
