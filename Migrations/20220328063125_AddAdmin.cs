using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FirstProject.Migrations
{
    public partial class AddAdmin : Migration
    {
        private string AdminRoleId = Guid.NewGuid().ToString();
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            AdminAddition(migrationBuilder);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }

        private void AdminAddition(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @$"INSERT INTO [dbo].[AspNetUserRoles]([UserId],[RoleId]) VALUES ('9edf37ba-af5b-4bb4-a7d8-b48264c91a97','0a7892be-983c-4da3-9af9-2e7607c8d3f9')");

        }
    }
}
