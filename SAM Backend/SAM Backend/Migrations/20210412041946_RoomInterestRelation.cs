using Microsoft.EntityFrameworkCore.Migrations;

namespace SAM_Backend.Migrations
{
    public partial class RoomInterestRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InterestsId",
                table: "Rooms",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_InterestsId",
                table: "Rooms",
                column: "InterestsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Interests_InterestsId",
                table: "Rooms",
                column: "InterestsId",
                principalTable: "Interests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Interests_InterestsId",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_InterestsId",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "InterestsId",
                table: "Rooms");
        }
    }
}
