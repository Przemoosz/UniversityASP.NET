using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FirstProject.Migrations
{
    public partial class RelationWithStudents : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "CourseStudentModel",
                columns: table => new
                {
                    CoursesCourseID = table.Column<int>(type: "int", nullable: false),
                    StudentsID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseStudentModel", x => new { x.CoursesCourseID, x.StudentsID });
                    table.ForeignKey(
                        name: "FK_CourseStudentModel_Course_CoursesCourseID",
                        column: x => x.CoursesCourseID,
                        principalTable: "Course",
                        principalColumn: "CourseID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseStudentModel_Person_StudentsID",
                        column: x => x.StudentsID,
                        principalTable: "Person",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseStudentModel_StudentsID",
                table: "CourseStudentModel",
                column: "StudentsID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseStudentModel");

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
    }
}
