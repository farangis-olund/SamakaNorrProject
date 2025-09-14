using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatingSearchRequestEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_SearchRequests_SearchRequestEntityId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_SearchRequestEntityId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "SearchRequestEntityId",
                table: "Messages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SearchRequestEntityId",
                table: "Messages",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SearchRequestEntityId",
                table: "Messages",
                column: "SearchRequestEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_SearchRequests_SearchRequestEntityId",
                table: "Messages",
                column: "SearchRequestEntityId",
                principalTable: "SearchRequests",
                principalColumn: "Id");
        }
    }
}
