using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarPark.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRides : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_enterprise_manager_enterprise_enterprises_id",
                table: "enterprise_manager");

            migrationBuilder.CreateTable(
                name: "ride",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    vehicle_id = table.Column<int>(type: "integer", nullable: false),
                    start_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    end_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ride", x => x.id);
                    table.ForeignKey(
                        name: "fk_ride_vehicle_vehicle_id",
                        column: x => x.vehicle_id,
                        principalTable: "vehicle",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_ride_vehicle_id",
                table: "ride",
                column: "vehicle_id");

            migrationBuilder.AddForeignKey(
                name: "fk_enterprise_manager_enterprises_enterprises_id",
                table: "enterprise_manager",
                column: "enterprises_id",
                principalTable: "enterprise",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_enterprise_manager_enterprises_enterprises_id",
                table: "enterprise_manager");

            migrationBuilder.DropTable(
                name: "ride");

            migrationBuilder.AddForeignKey(
                name: "fk_enterprise_manager_enterprise_enterprises_id",
                table: "enterprise_manager",
                column: "enterprises_id",
                principalTable: "enterprise",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
