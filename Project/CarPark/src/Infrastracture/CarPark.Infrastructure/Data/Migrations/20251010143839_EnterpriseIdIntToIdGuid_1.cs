using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarPark.Data.Migrations
{
    /// <inheritdoc />
    public partial class EnterpriseIdIntToIdGuid_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_driver_enterprise_enterprise_id",
                table: "driver");

            migrationBuilder.DropForeignKey(
                name: "fk_vehicle_enterprise_enterprise_id",
                table: "vehicle");

            migrationBuilder.AddColumn<Guid>(
                name: "guid_id",
                table: "enterprise",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.Sql("UPDATE enterprise SET guid_id = gen_random_uuid();");

            migrationBuilder.AddUniqueConstraint(
                name: "ak_enterprise_guid_id",
                table: "enterprise",
                column: "guid_id");

            migrationBuilder.AddForeignKey(
                name: "fk_driver_enterprise_enterprise_id",
                table: "driver",
                column: "enterprise_id",
                principalTable: "enterprise",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_vehicle_enterprise_enterprise_id",
                table: "vehicle",
                column: "enterprise_id",
                principalTable: "enterprise",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_driver_enterprise_enterprise_id",
                table: "driver");

            migrationBuilder.DropForeignKey(
                name: "fk_vehicle_enterprise_enterprise_id",
                table: "vehicle");

            migrationBuilder.DropUniqueConstraint(
                name: "ak_enterprise_guid_id",
                table: "enterprise");

            migrationBuilder.DropColumn(
                name: "guid_id",
                table: "enterprise");

            migrationBuilder.AddForeignKey(
                name: "fk_driver_enterprise_enterprise_id",
                table: "driver",
                column: "enterprise_id",
                principalTable: "enterprise",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_vehicle_enterprise_enterprise_id",
                table: "vehicle",
                column: "enterprise_id",
                principalTable: "enterprise",
                principalColumn: "id");
        }
    }
}
