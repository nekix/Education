using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarPark.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRidesStartEndGeoPoints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "end_geo_time_point_id",
                table: "ride",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "start_geo_time_point_id",
                table: "ride",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_ride_end_geo_time_point_id",
                table: "ride",
                column: "end_geo_time_point_id");

            migrationBuilder.CreateIndex(
                name: "ix_ride_start_geo_time_point_id",
                table: "ride",
                column: "start_geo_time_point_id");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_ride_vehicle_get_time_end_geo_time_point_id",
                table: "ride");

            migrationBuilder.DropForeignKey(
                name: "fk_ride_vehicle_get_time_start_geo_time_point_id",
                table: "ride");

            migrationBuilder.DropIndex(
                name: "ix_ride_end_geo_time_point_id",
                table: "ride");

            migrationBuilder.DropIndex(
                name: "ix_ride_start_geo_time_point_id",
                table: "ride");

            migrationBuilder.DropColumn(
                name: "end_geo_time_point_id",
                table: "ride");

            migrationBuilder.DropColumn(
                name: "start_geo_time_point_id",
                table: "ride");
        }
    }
}
