using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNewFieldToMessageEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RideId",
                table: "Messages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_RideId",
                table: "Messages",
                column: "RideId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Rides_RideId",
                table: "Messages",
                column: "RideId",
                principalTable: "Rides",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Rides_RideId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_RideId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "RideId",
                table: "Messages");
        }
    }
}
