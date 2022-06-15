using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechExtensions.SchedyBot.DLL.Migrations
{
    public partial class hangFire : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HangFireJobId",
                table: "Bookings",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HangFireJobId",
                table: "Bookings");
        }
    }
}
