using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CarPark.Data.Migrations
{
    /// <inheritdoc />
    public partial class VehiclesIdIntToIdGuid_9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_ride_vehicle_vehicle_id",
                table: "ride");

            migrationBuilder.DropForeignKey(
                name: "fk_vehicle_geo_time_point_vehicle_vehicle_id",
                table: "vehicle_geo_time_point");

            migrationBuilder.DropForeignKey(
                name: "fk_driver_vehicle_new_active_assigned_vehicle_id",
                table: "driver");

            migrationBuilder.DropForeignKey(
                name: "fk_driver_vehicle_assignment_vehicle_assigned_vehicles_id",
                table: "driver_vehicle_assignment");

            migrationBuilder.DropUniqueConstraint(
                name: "ak_vehicle_guid_id",
                table: "vehicle");

            migrationBuilder.DropPrimaryKey(
                name: "pk_vehicle",
                table: "vehicle");

            migrationBuilder.DropColumn(
                name: "id",
                table: "vehicle");

            migrationBuilder.AddPrimaryKey(
                name: "pk_vehicle",
                table: "vehicle",
                column: "guid_id");

            migrationBuilder.AddForeignKey(
                name: "fk_ride_vehicle_vehicle_id",
                table: "ride",
                column: "vehicle_id",
                principalTable: "vehicle",
                principalColumn: "guid_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_vehicle_geo_time_point_vehicle_vehicle_id",
                table: "vehicle_geo_time_point",
                column: "vehicle_id",
                principalTable: "vehicle",
                principalColumn: "guid_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_driver_vehicle_new_active_assigned_vehicle_id",
                table: "driver",
                column: "new_active_assigned_vehicle_id",
                principalTable: "vehicle",
                principalColumn: "guid_id",
                onDelete: ReferentialAction.Restrict);

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
            migrationBuilder.DropForeignKey(
                name: "fk_ride_vehicle_vehicle_id",
                table: "ride");

            migrationBuilder.DropForeignKey(
                name: "fk_vehicle_geo_time_point_vehicle_vehicle_id",
                table: "vehicle_geo_time_point");

            migrationBuilder.DropForeignKey(
                name: "fk_driver_vehicle_new_active_assigned_vehicle_id",
                table: "driver");

            migrationBuilder.DropForeignKey(
                name: "fk_driver_vehicle_assignment_vehicle_assigned_vehicles_id",
                table: "driver_vehicle_assignment");

            migrationBuilder.DropPrimaryKey(
                name: "pk_vehicle",
                table: "vehicle");

            migrationBuilder.AddColumn<int>(
                name: "id",
                table: "vehicle",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddUniqueConstraint(
                name: "ak_vehicle_guid_id",
                table: "vehicle",
                column: "guid_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_vehicle",
                table: "vehicle",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_ride_vehicle_vehicle_id",
                table: "ride",
                column: "vehicle_id",
                principalTable: "vehicle",
                principalColumn: "guid_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_vehicle_geo_time_point_vehicle_vehicle_id",
                table: "vehicle_geo_time_point",
                column: "vehicle_id",
                principalTable: "vehicle",
                principalColumn: "guid_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_driver_vehicle_new_active_assigned_vehicle_id",
                table: "driver",
                column: "new_active_assigned_vehicle_id",
                principalTable: "vehicle",
                principalColumn: "guid_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_driver_vehicle_assignment_vehicle_assigned_vehicles_id",
                table: "driver_vehicle_assignment",
                column: "assigned_vehicles_id",
                principalTable: "vehicle",
                principalColumn: "guid_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
