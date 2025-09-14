using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddingMessageIntoSearchRequst : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SearchRequestId",
                table: "Messages",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SearchRequestId",
                table: "Messages",
                column: "SearchRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_SearchRequests_SearchRequestId",
                table: "Messages",
                column: "SearchRequestId",
                principalTable: "SearchRequests",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_SearchRequests_SearchRequestId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_SearchRequestId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "SearchRequestId",
                table: "Messages");
        }
    }
}
