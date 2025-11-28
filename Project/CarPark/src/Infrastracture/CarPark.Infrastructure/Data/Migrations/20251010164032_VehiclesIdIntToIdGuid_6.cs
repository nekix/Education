using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarPark.Data.Migrations
{
    /// <inheritdoc />
    public partial class VehiclesIdIntToIdGuid_6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "new_active_assigned_vehicle_id",
                table: "driver",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_driver_new_active_assigned_vehicle_id",
                table: "driver",
                column: "new_active_assigned_vehicle_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_driver_vehicle_new_active_assigned_vehicle_id",
                table: "driver",
                column: "new_active_assigned_vehicle_id",
                principalTable: "vehicle",
                principalColumn: "guid_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.Sql(@"
                UPDATE driver AS d
                SET new_active_assigned_vehicle_id = v.guid_id
                FROM vehicle v
                WHERE d.assigned_vehicle_id = v.id;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_driver_vehicle_new_active_assigned_vehicle_id",
                table: "driver");

            migrationBuilder.DropIndex(
                name: "ix_driver_new_active_assigned_vehicle_id",
                table: "driver");

            migrationBuilder.DropColumn(
                name: "new_active_assigned_vehicle_id",
                table: "driver");
        }
    }
}
