using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddingSearchMessageTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_SearchRequests_SearchRequestId",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "SearchRequestId",
                table: "Messages",
                newName: "SearchRequestEntityId");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_SearchRequestId",
                table: "Messages",
                newName: "IX_Messages_SearchRequestEntityId");

            migrationBuilder.CreateTable(
                name: "SearchMessages",
                columns: table => new
                {
                    MessageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SenderId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MessageContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RideId = table.Column<int>(type: "int", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    SearchRequestId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchMessages", x => x.MessageId);
                    table.ForeignKey(
                        name: "FK_SearchMessages_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SearchMessages_Rides_RideId",
                        column: x => x.RideId,
                        principalTable: "Rides",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SearchMessages_SearchRequests_SearchRequestId",
                        column: x => x.SearchRequestId,
                        principalTable: "SearchRequests",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SearchMessages_RideId",
                table: "SearchMessages",
                column: "RideId");

            migrationBuilder.CreateIndex(
                name: "IX_SearchMessages_SearchRequestId",
                table: "SearchMessages",
                column: "SearchRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_SearchMessages_UserId",
                table: "SearchMessages",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_SearchRequests_SearchRequestEntityId",
                table: "Messages",
                column: "SearchRequestEntityId",
                principalTable: "SearchRequests",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_SearchRequests_SearchRequestEntityId",
                table: "Messages");

            migrationBuilder.DropTable(
                name: "SearchMessages");

            migrationBuilder.RenameColumn(
                name: "SearchRequestEntityId",
                table: "Messages",
                newName: "SearchRequestId");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_SearchRequestEntityId",
                table: "Messages",
                newName: "IX_Messages_SearchRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_SearchRequests_SearchRequestId",
                table: "Messages",
                column: "SearchRequestId",
                principalTable: "SearchRequests",
                principalColumn: "Id");
        }
    }
}
