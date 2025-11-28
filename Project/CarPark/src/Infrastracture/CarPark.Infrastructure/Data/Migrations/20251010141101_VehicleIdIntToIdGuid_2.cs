using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarPark.Data.Migrations
{
    /// <inheritdoc />
    public partial class VehicleIdIntToIdGuid_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "assigned_drivers_guid",
                table: "driver_vehicle_assignment",
                type: "uuid",
                nullable: false,
                defaultValue: Guid.Empty);

            migrationBuilder.Sql(@"
                UPDATE driver_vehicle_assignment AS dva
                SET assigned_drivers_guid = d.guid_id
                FROM driver d
                WHERE dva.assigned_drivers_id = d.id;
            ");

            migrationBuilder.DropColumn(
                name: "assigned_drivers_id",
                table: "driver_vehicle_assignment");

            migrationBuilder.RenameColumn(
                name: "assigned_drivers_guid",
                table: "driver_vehicle_assignment",
                newName: "assigned_drivers_id");

            migrationBuilder.AddForeignKey(
                name: "fk_driver_vehicle_assignment_driver_assigned_drivers_id",
                table: "driver_vehicle_assignment",
                column: "assigned_drivers_id",
                principalTable: "driver",
                principalColumn: "guid_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "assigned_drivers_int",
                table: "driver_vehicle_assignment",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(@"
                UPDATE driver_vehicle_assignment AS dva
                SET assigned_drivers_int = d.id
                FROM driver d
                WHERE dva.assigned_drivers_id = d.guid_id;
            ");

            migrationBuilder.DropColumn(
                name: "assigned_drivers_id",
                table: "driver_vehicle_assignment");

            migrationBuilder.RenameColumn(
                name: "assigned_drivers_int",
                table: "driver_vehicle_assignment",
                newName: "assigned_drivers_id");

            migrationBuilder.AddForeignKey(
                name: "fk_driver_vehicle_assignment_driver_assigned_drivers_id",
                table: "driver_vehicle_assignment",
                column: "assigned_drivers_id",
                principalTable: "driver",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
