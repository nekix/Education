using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarPark.Data.Migrations
{
    /// <inheritdoc />
    public partial class VehiclesIdIntToIdGuid_8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_driver_vehicle_assigned_vehicle_id",
                table: "driver");

            migrationBuilder.DropIndex(
                name: "ix_driver_assigned_vehicle_id",
                table: "driver");

            migrationBuilder.DropColumn(
                name: "assigned_vehicle_id",
                table: "driver");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "assigned_vehicle_id",
                table: "driver",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_driver_assigned_vehicle_id",
                table: "driver",
                column: "assigned_vehicle_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_driver_vehicle_assigned_vehicle_id",
                table: "driver",
                column: "assigned_vehicle_id",
                principalTable: "vehicle",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
