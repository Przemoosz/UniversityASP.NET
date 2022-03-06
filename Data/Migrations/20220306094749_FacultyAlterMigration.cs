using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FirstProject.Data.Migrations
{
    public partial class FacultyAlterMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UniveristyID",
                table: "Faculty");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UniveristyID",
                table: "Faculty",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
