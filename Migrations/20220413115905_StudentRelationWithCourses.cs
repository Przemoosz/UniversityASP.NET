using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FirstProject.Migrations
{
    public partial class StudentRelationWithCourses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StudentModelID",
                table: "Course",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Course_StudentModelID",
                table: "Course",
                column: "StudentModelID");

            migrationBuilder.AddForeignKey(
                name: "FK_Course_Person_StudentModelID",
                table: "Course",
                column: "StudentModelID",
                principalTable: "Person",
                principalColumn: "ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Course_Person_StudentModelID",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_StudentModelID",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "StudentModelID",
                table: "Course");
        }
    }
}
