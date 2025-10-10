using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarPark.Data.Migrations
{
    /// <inheritdoc />
    public partial class VehiclesIdIntToIdGuid_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_driver_vehicle_assigned_vehicle_id",
                table: "driver");

            migrationBuilder.DropForeignKey(
                name: "fk_manager_asp_net_users_identity_user_id",
                table: "manager");

            migrationBuilder.AddColumn<Guid>(
                name: "guid_id",
                table: "vehicle",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.Sql("UPDATE vehicle SET guid_id = gen_random_uuid();");

            migrationBuilder.AddUniqueConstraint(
                name: "ak_vehicle_guid_id",
                table: "vehicle",
                column: "guid_id");

            migrationBuilder.AddForeignKey(
                name: "fk_driver_vehicle_assigned_vehicle_id",
                table: "driver",
                column: "assigned_vehicle_id",
                principalTable: "vehicle",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_manager_asp_net_users_identity_user_id",
                table: "manager",
                column: "identity_user_id",
                principalTable: "AspNetUsers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_driver_vehicle_assigned_vehicle_id",
                table: "driver");

            migrationBuilder.DropForeignKey(
                name: "fk_manager_asp_net_users_identity_user_id",
                table: "manager");

            migrationBuilder.DropUniqueConstraint(
                name: "ak_vehicle_guid_id",
                table: "vehicle");

            migrationBuilder.DropColumn(
                name: "guid_id",
                table: "vehicle");

            migrationBuilder.AddForeignKey(
                name: "fk_driver_vehicle_assigned_vehicle_id",
                table: "driver",
                column: "assigned_vehicle_id",
                principalTable: "vehicle",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_manager_asp_net_users_identity_user_id",
                table: "manager",
                column: "identity_user_id",
                principalTable: "AspNetUsers",
                principalColumn: "id");
        }
    }
}
