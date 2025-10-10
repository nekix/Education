using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CarPark.Data.Migrations
{
    /// <inheritdoc />
    public partial class VehicleIdIntToIdGuid_3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_driver_vehicle_assigned_vehicle_id",
                table: "driver");

            migrationBuilder.DropForeignKey(
                name: "fk_driver_vehicle_assignment_driver_assigned_drivers_id",
                table: "driver_vehicle_assignment");

            migrationBuilder.DropUniqueConstraint(
                name: "ak_driver_guid_id",
                table: "driver");

            migrationBuilder.DropPrimaryKey(
                name: "pk_driver",
                table: "driver");

            migrationBuilder.DropColumn(
                name: "id",
                table: "driver");

            migrationBuilder.RenameColumn(
                name: "assigned_drivers_id",
                table: "driver_vehicle_assignment",
                newName: "assigned_drivers_guid_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_driver",
                table: "driver",
                column: "guid_id");

            migrationBuilder.AddForeignKey(
                name: "fk_driver_vehicle_assigned_vehicle_id",
                table: "driver",
                column: "assigned_vehicle_id",
                principalTable: "vehicle",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_driver_vehicle_assignment_driver_assigned_drivers_guid_id",
                table: "driver_vehicle_assignment",
                column: "assigned_drivers_guid_id",
                principalTable: "driver",
                principalColumn: "guid_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_driver_vehicle_assigned_vehicle_id",
                table: "driver");

            migrationBuilder.DropForeignKey(
                name: "fk_driver_vehicle_assignment_driver_assigned_drivers_guid_id",
                table: "driver_vehicle_assignment");

            migrationBuilder.DropPrimaryKey(
                name: "pk_driver",
                table: "driver");

            migrationBuilder.RenameColumn(
                name: "assigned_drivers_guid_id",
                table: "driver_vehicle_assignment",
                newName: "assigned_drivers_id");

            migrationBuilder.AddColumn<int>(
                name: "id",
                table: "driver",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddUniqueConstraint(
                name: "ak_driver_guid_id",
                table: "driver",
                column: "guid_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_driver",
                table: "driver",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_driver_vehicle_assigned_vehicle_id",
                table: "driver",
                column: "assigned_vehicle_id",
                principalTable: "vehicle",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_driver_vehicle_assignment_driver_assigned_drivers_id",
                table: "driver_vehicle_assignment",
                column: "assigned_drivers_id",
                principalTable: "driver",
                principalColumn: "guid_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
