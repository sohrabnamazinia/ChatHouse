using Microsoft.EntityFrameworkCore.Migrations;

namespace SAM_Backend.Migrations
{
    public partial class Interests : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InterestsId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Interests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Wellness = table.Column<int>(type: "int", nullable: false),
                    Identity = table.Column<int>(type: "int", nullable: false),
                    Places = table.Column<int>(type: "int", nullable: false),
                    WorldAffairs = table.Column<int>(type: "int", nullable: false),
                    Tech = table.Column<int>(type: "int", nullable: false),
                    HangingOut = table.Column<int>(type: "int", nullable: false),
                    KnowLedge = table.Column<int>(type: "int", nullable: false),
                    Hustle = table.Column<int>(type: "int", nullable: false),
                    Sports = table.Column<int>(type: "int", nullable: false),
                    Arts = table.Column<int>(type: "int", nullable: false),
                    Life = table.Column<int>(type: "int", nullable: false),
                    Languages = table.Column<int>(type: "int", nullable: false),
                    Entertainment = table.Column<int>(type: "int", nullable: false),
                    Faith = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interests", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_InterestsId",
                table: "AspNetUsers",
                column: "InterestsId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Interests_InterestsId",
                table: "AspNetUsers",
                column: "InterestsId",
                principalTable: "Interests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Interests_InterestsId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Interests");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_InterestsId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "InterestsId",
                table: "AspNetUsers");
        }
    }
}
