using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarPark.Data.Migrations
{
    /// <inheritdoc />
    public partial class VehicleIdIntToIdGuid_4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_driver_vehicle_assignment_driver_assigned_drivers_guid_id",
                table: "driver_vehicle_assignment");

            migrationBuilder.RenameColumn(
                name: "assigned_drivers_guid_id",
                table: "driver_vehicle_assignment",
                newName: "assigned_drivers_id");

            migrationBuilder.RenameColumn(
                name: "guid_id",
                table: "driver",
                newName: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_driver_vehicle_assignment_driver_assigned_drivers_id",
                table: "driver_vehicle_assignment",
                column: "assigned_drivers_id",
                principalTable: "driver",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_driver_vehicle_assignment_driver_assigned_drivers_id",
                table: "driver_vehicle_assignment");

            migrationBuilder.RenameColumn(
                name: "assigned_drivers_id",
                table: "driver_vehicle_assignment",
                newName: "assigned_drivers_guid_id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "driver",
                newName: "guid_id");

            migrationBuilder.AddForeignKey(
                name: "fk_driver_vehicle_assignment_driver_assigned_drivers_guid_id",
                table: "driver_vehicle_assignment",
                column: "assigned_drivers_guid_id",
                principalTable: "driver",
                principalColumn: "guid_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
