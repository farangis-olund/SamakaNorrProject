using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddingSearchMessageRemoveRide : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SearchMessages_Rides_RideId",
                table: "SearchMessages");

            migrationBuilder.DropIndex(
                name: "IX_SearchMessages_RideId",
                table: "SearchMessages");

            migrationBuilder.DropColumn(
                name: "RideId",
                table: "SearchMessages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RideId",
                table: "SearchMessages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SearchMessages_RideId",
                table: "SearchMessages",
                column: "RideId");

            migrationBuilder.AddForeignKey(
                name: "FK_SearchMessages_Rides_RideId",
                table: "SearchMessages",
                column: "RideId",
                principalTable: "Rides",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
