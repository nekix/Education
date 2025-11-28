using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarPark.Data.Migrations
{
    /// <inheritdoc />
    public partial class VehiclesIdIntToIdGuid_7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "assigned_vehicles_guid",
                table: "driver_vehicle_assignment",
                type: "uuid",
                nullable: false,
                defaultValue: Guid.Empty);

            migrationBuilder.Sql(@"
                UPDATE driver_vehicle_assignment AS dva
                SET assigned_vehicles_guid = v.guid_id
                FROM vehicle v
                WHERE dva.assigned_vehicles_id = v.id;
            ");

            migrationBuilder.DropColumn(
                name: "assigned_vehicles_id",
                table: "driver_vehicle_assignment");

            migrationBuilder.RenameColumn(
                name: "assigned_vehicles_guid",
                table: "driver_vehicle_assignment",
                newName: "assigned_vehicles_id");

            migrationBuilder.AddForeignKey(
                name: "fk_driver_vehicle_assignment_vehicle_assigned_vehicles_id",
                table: "driver_vehicle_assignment",
                column: "assigned_vehicles_id",
                principalTable: "vehicle",
                principalColumn: "guid_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "assigned_vehicles_int",
                table: "driver_vehicle_assignment",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(@"
                UPDATE driver_vehicle_assignment AS dva
                SET assigned_vehicles_int = v.id
                FROM vehicle v
                WHERE dva.assigned_vehicles_id = v.guid_id;
            ");

            migrationBuilder.DropColumn(
                name: "assigned_vehicles_id",
                table: "driver_vehicle_assignment");

            migrationBuilder.RenameColumn(
                name: "assigned_vehicles_int",
                table: "driver_vehicle_assignment",
                newName: "assigned_vehicles_id");

            migrationBuilder.AddForeignKey(
                name: "fk_driver_vehicle_assignment_vehicle_assigned_vehicles_id",
                table: "driver_vehicle_assignment",
                column: "assigned_vehicles_id",
                principalTable: "vehicle",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
