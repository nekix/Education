using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarPark.Data.Migrations
{
    /// <inheritdoc />
    public partial class VehiclesIdIntToIdGuid_5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "vehicle_guid_id",
                table: "vehicle_geo_time_point",
                type: "uuid",
                nullable: false,
                defaultValue: Guid.Empty);

            migrationBuilder.Sql(@"
                UPDATE vehicle_geo_time_point r
                SET vehicle_guid_id = v.guid_id
                FROM vehicle v
                WHERE r.vehicle_id = v.id;
            ");

            migrationBuilder.DropForeignKey(
                name: "fk_vehicle_geo_time_point_vehicle_vehicle_id",
                table: "vehicle_geo_time_point");

            migrationBuilder.DropColumn(
                name: "vehicle_id",
                table: "vehicle_geo_time_point");

            migrationBuilder.RenameColumn(
                name: "vehicle_guid_id",
                table: "vehicle_geo_time_point",
                newName: "vehicle_id");

            migrationBuilder.AddForeignKey(
                name: "fk_vehicle_geo_time_point_vehicle_vehicle_id",
                table: "vehicle_geo_time_point",
                column: "vehicle_id",
                principalTable: "vehicle",
                principalColumn: "guid_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "vehicle_int_id",
                table: "vehicle_geo_time_point",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(@"
                UPDATE vehicle_geo_time_point r
                SET vehicle_int_id = v.id
                FROM vehicle v
                WHERE r.vehicle_id = v.guid_id;
            ");

            migrationBuilder.DropForeignKey(
                name: "fk_vehicle_geo_time_point_vehicle_vehicle_id",
                table: "vehicle_geo_time_point");

            migrationBuilder.DropColumn(
                name: "vehicle_id",
                table: "vehicle_geo_time_point");

            migrationBuilder.RenameColumn(
                name: "vehicle_int_id",
                table: "vehicle_geo_time_point",
                newName: "vehicle_id");

            migrationBuilder.AddForeignKey(
                name: "fk_vehicle_geo_time_point_vehicle_vehicle_id",
                table: "vehicle_geo_time_point",
                column: "vehicle_id",
                principalTable: "vehicle",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
