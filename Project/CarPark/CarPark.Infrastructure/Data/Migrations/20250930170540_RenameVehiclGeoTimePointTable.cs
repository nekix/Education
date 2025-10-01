using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace CarPark.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameVehiclGeoTimePointTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // + Rides

            migrationBuilder.DropForeignKey(
                name: "fk_ride_vehicle_get_time_end_geo_time_point_id",
                table: "ride");

            migrationBuilder.DropForeignKey(
                name: "fk_ride_vehicle_get_time_start_geo_time_point_id",
                table: "ride");

            // + ForeignKey

            migrationBuilder.DropForeignKey(
                name: "fk_vehicle_get_time_vehicle_vehicle_id",
                table: "vehicle_get_time");

            // + Primary key

            migrationBuilder.DropPrimaryKey(
                name: "pk_vehicle_get_time",
                table: "vehicle_get_time");

            // ===== ПЕРЕИМЕНОВАНИЕ =====

            migrationBuilder.RenameTable(
                name: "vehicle_get_time",
                newName: "vehicle_geo_time_point");

            // ===== ПЕРЕИМЕНОВАНИЕ =====

            // + Primary key

            migrationBuilder.AddPrimaryKey(
                name: "pk_vehicle_geo_time_point",
                table: "vehicle_geo_time_point",
                column: "id");

            // + ForeignKey

            migrationBuilder.AddForeignKey(
                name: "fk_vehicle_geo_time_point_vehicle_vehicle_id",
                table: "vehicle_geo_time_point",
                column: "vehicle_id",
                principalTable: "vehicle",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            // Index

            migrationBuilder.RenameIndex(
                name: "ix_vehicle_get_time_vehicle_id",
                table: "vehicle_geo_time_point",
                newName: "ix_vehicle_geo_time_point_vehicle_id");

            // + Rides

            migrationBuilder.AddForeignKey(
                name: "fk_ride_vehicle_geo_time_point_end_geo_time_point_id",
                table: "ride",
                column: "end_geo_time_point_id",
                principalTable: "vehicle_geo_time_point",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_ride_vehicle_geo_time_point_start_geo_time_point_id",
                table: "ride",
                column: "start_geo_time_point_id",
                principalTable: "vehicle_geo_time_point",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // + Rides

            migrationBuilder.DropForeignKey(
                name: "fk_ride_vehicle_geo_time_point_end_geo_time_point_id",
                table: "ride");

            migrationBuilder.DropForeignKey(
                name: "fk_ride_vehicle_geo_time_point_start_geo_time_point_id",
                table: "ride");

            // + VehicleGeoTimePoint → Vehicle
            migrationBuilder.DropForeignKey(
                name: "fk_vehicle_geo_time_point_vehicle_vehicle_id",
                table: "vehicle_geo_time_point");

            // + Primary key
            migrationBuilder.DropPrimaryKey(
                name: "pk_vehicle_geo_time_point",
                table: "vehicle_geo_time_point");

            // ===== ПЕРЕИМЕНОВАНИЕ =====
            migrationBuilder.RenameTable(
                name: "vehicle_geo_time_point",
                newName: "vehicle_get_time");

            // ===== ПЕРЕИМЕНОВАНИЕ =====

            migrationBuilder.RenameIndex(
                name: "ix_vehicle_geo_time_point_vehicle_id",
                table: "vehicle_get_time",
                newName: "ix_vehicle_get_time_vehicle_id");

            // + Primary key
            migrationBuilder.AddPrimaryKey(
                name: "pk_vehicle_get_time",
                table: "vehicle_get_time",
                column: "id");

            // + ForeignKey (vehicle_get_time → vehicle)
            migrationBuilder.AddForeignKey(
                name: "fk_vehicle_get_time_vehicle_vehicle_id",
                table: "vehicle_get_time",
                column: "vehicle_id",
                principalTable: "vehicle",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            // + Rides
            migrationBuilder.AddForeignKey(
                name: "fk_ride_vehicle_get_time_end_geo_time_point_id",
                table: "ride",
                column: "end_geo_time_point_id",
                principalTable: "vehicle_get_time",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_ride_vehicle_get_time_start_geo_time_point_id",
                table: "ride",
                column: "start_geo_time_point_id",
                principalTable: "vehicle_get_time",
                principalColumn: "id");
        }
    }
}
