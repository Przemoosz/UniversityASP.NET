using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FirstProject.Migrations
{
    public partial class MessagesAndMessageBox : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MessageBox",
                columns: table => new
                {
                    MessageBoxID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageBox", x => x.MessageBoxID);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationUserMessageBox",
                columns: table => new
                {
                    MessageBoxesMessageBoxID = table.Column<int>(type: "int", nullable: false),
                    ParticipantsId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserMessageBox", x => new { x.MessageBoxesMessageBoxID, x.ParticipantsId });
                    table.ForeignKey(
                        name: "FK_ApplicationUserMessageBox_AspNetUsers_ParticipantsId",
                        column: x => x.ParticipantsId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserMessageBox_MessageBox_MessageBoxesMessageBoxID",
                        column: x => x.MessageBoxesMessageBoxID,
                        principalTable: "MessageBox",
                        principalColumn: "MessageBoxID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    MessageID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MessageBoxID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.MessageID);
                    table.ForeignKey(
                        name: "FK_Messages_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Messages_MessageBox_MessageBoxID",
                        column: x => x.MessageBoxID,
                        principalTable: "MessageBox",
                        principalColumn: "MessageBoxID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserMessageBox_ParticipantsId",
                table: "ApplicationUserMessageBox",
                column: "ParticipantsId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_MessageBoxID",
                table: "Messages",
                column: "MessageBoxID");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_UserId",
                table: "Messages",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUserMessageBox");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "MessageBox");
        }
    }
}
