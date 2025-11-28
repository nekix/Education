using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarPark.Data.Migrations
{
    /// <inheritdoc />
    public partial class VehiclesIdIntToIdGuid_11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_driver_vehicle_new_active_assigned_vehicle_id",
                table: "driver");

            migrationBuilder.RenameColumn(
                name: "new_active_assigned_vehicle_id",
                table: "driver",
                newName: "assigned_vehicle_id");

            migrationBuilder.RenameIndex(
                name: "ix_driver_new_active_assigned_vehicle_id",
                table: "driver",
                newName: "ix_driver_assigned_vehicle_id");

            migrationBuilder.AddForeignKey(
                name: "fk_driver_vehicle_assigned_vehicle_id",
                table: "driver",
                column: "assigned_vehicle_id",
                principalTable: "vehicle",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_driver_vehicle_assigned_vehicle_id",
                table: "driver");

            migrationBuilder.RenameColumn(
                name: "assigned_vehicle_id",
                table: "driver",
                newName: "new_active_assigned_vehicle_id");

            migrationBuilder.RenameIndex(
                name: "ix_driver_assigned_vehicle_id",
                table: "driver",
                newName: "ix_driver_new_active_assigned_vehicle_id");

            migrationBuilder.AddForeignKey(
                name: "fk_driver_vehicle_new_active_assigned_vehicle_id",
                table: "driver",
                column: "new_active_assigned_vehicle_id",
                principalTable: "vehicle",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
