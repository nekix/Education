using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarPark.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangedRidersStartEndPointsOnDeleteBehaviour : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_ride_vehicle_vehicle_id",
                table: "ride");

            migrationBuilder.AddForeignKey(
                name: "fk_ride_vehicle_vehicle_id",
                table: "ride",
                column: "vehicle_id",
                principalTable: "vehicle",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_ride_vehicle_vehicle_id",
                table: "ride");

            migrationBuilder.AddForeignKey(
                name: "fk_ride_vehicle_vehicle_id",
                table: "ride",
                column: "vehicle_id",
                principalTable: "vehicle",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
