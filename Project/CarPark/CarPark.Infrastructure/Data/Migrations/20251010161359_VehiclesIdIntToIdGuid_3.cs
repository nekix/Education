using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarPark.Data.Migrations
{
    /// <inheritdoc />
    public partial class VehiclesIdIntToIdGuid_3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "vehicle_guid_id",
                table: "ride",
                type: "uuid",
                nullable: false,
                defaultValue: Guid.Empty);

            migrationBuilder.Sql(@"
                UPDATE ride r
                SET vehicle_guid_id = v.guid_id
                FROM vehicle v
                WHERE r.vehicle_id = v.id;
            ");

            migrationBuilder.DropForeignKey(
                name: "fk_ride_vehicle_vehicle_id",
                table: "ride");

            migrationBuilder.DropColumn(
                name: "vehicle_id",
                table: "ride");

            migrationBuilder.RenameColumn(
                name: "vehicle_guid_id",
                table: "ride",
                newName: "vehicle_id");

            migrationBuilder.AddForeignKey(
                name: "fk_ride_vehicle_vehicle_id",
                table: "ride",
                column: "vehicle_id",
                principalTable: "vehicle",
                principalColumn: "guid_id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "vehicle_int_id",
                table: "ride",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(@"
                UPDATE ride r
                SET vehicle_int_id = v.id
                FROM vehicle v
                WHERE r.vehicle_id = v.guid_id;
            ");

            migrationBuilder.DropForeignKey(
                name: "fk_ride_vehicle_vehicle_id",
                table: "ride");

            migrationBuilder.DropColumn(
                name: "vehicle_id",
                table: "ride");

            migrationBuilder.RenameColumn(
                name: "vehicle_int_id",
                table: "ride",
                newName: "vehicle_id");

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
