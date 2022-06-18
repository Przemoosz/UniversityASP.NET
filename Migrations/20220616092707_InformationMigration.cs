using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FirstProject.Migrations
{
    public partial class InformationMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InformationBoxId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Information",
                columns: table => new
                {
                    InformationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Views = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Information", x => x.InformationId);
                });

            migrationBuilder.CreateTable(
                name: "InformationBox",
                columns: table => new
                {
                    InformationBoxId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InformationBox", x => x.InformationBoxId);
                });

            migrationBuilder.CreateTable(
                name: "InformationInformationBox",
                columns: table => new
                {
                    InformationBoxesInformationBoxId = table.Column<int>(type: "int", nullable: false),
                    InformationsInformationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InformationInformationBox", x => new { x.InformationBoxesInformationBoxId, x.InformationsInformationId });
                    table.ForeignKey(
                        name: "FK_InformationInformationBox_Information_InformationsInformationId",
                        column: x => x.InformationsInformationId,
                        principalTable: "Information",
                        principalColumn: "InformationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InformationInformationBox_InformationBox_InformationBoxesInformationBoxId",
                        column: x => x.InformationBoxesInformationBoxId,
                        principalTable: "InformationBox",
                        principalColumn: "InformationBoxId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_InformationBoxId",
                table: "AspNetUsers",
                column: "InformationBoxId",
                unique: true,
                filter: "[InformationBoxId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_InformationInformationBox_InformationsInformationId",
                table: "InformationInformationBox",
                column: "InformationsInformationId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_InformationBox_InformationBoxId",
                table: "AspNetUsers",
                column: "InformationBoxId",
                principalTable: "InformationBox",
                principalColumn: "InformationBoxId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_InformationBox_InformationBoxId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "InformationInformationBox");

            migrationBuilder.DropTable(
                name: "Information");

            migrationBuilder.DropTable(
                name: "InformationBox");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_InformationBoxId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "InformationBoxId",
                table: "AspNetUsers");
        }
    }
}
