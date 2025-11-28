using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarPark.Data.Migrations
{
    /// <inheritdoc />
    public partial class EnterpriseIdIntToIdGuid_3 : Migration
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

            migrationBuilder.DropIndex(
                name: "ix_vehicle_enterprise_id",
                table: "vehicle");




            migrationBuilder.AddColumn<Guid>(
                name: "enterprise_guid_id",
                table: "vehicle",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "enterprise_id_new",
                table: "driver",
                type: "uuid",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE vehicle 
                SET enterprise_guid_id = e.guid_id 
                FROM enterprise e 
                WHERE vehicle.enterprise_id = e.id;
            ");

            migrationBuilder.Sql(@"
                UPDATE driver 
                SET enterprise_id_new = e.guid_id 
                FROM enterprise e 
                WHERE driver.enterprise_id = e.id;
            ");

            migrationBuilder.DropColumn(
                name: "enterprise_id",
                table: "vehicle");

            migrationBuilder.DropColumn(
                name: "enterprise_id",
                table: "driver");

            migrationBuilder.RenameColumn(
                name: "enterprise_id_new",
                table: "driver",
                newName: "enterprise_id");

            migrationBuilder.CreateIndex(
                name: "ix_vehicle_enterprise_guid_id",
                table: "vehicle",
                column: "enterprise_guid_id");

            // 9. Создаем внешние ключи
            migrationBuilder.AddForeignKey(
                name: "fk_driver_enterprise_enterprise_id",
                table: "driver",
                column: "enterprise_id",
                principalTable: "enterprise",
                principalColumn: "guid_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_vehicle_enterprise_enterprise_id",
                table: "vehicle",
                column: "enterprise_guid_id",
                principalTable: "enterprise",
                principalColumn: "guid_id",
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

            migrationBuilder.DropIndex(
                name: "ix_vehicle_enterprise_guid_id",
                table: "vehicle");

            migrationBuilder.DropColumn(
                name: "enterprise_guid_id",
                table: "vehicle");

            migrationBuilder.AddColumn<int>(
                name: "enterprise_id",
                table: "vehicle",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.RenameColumn(
                name: "enterprise_id",
                table: "driver",
                newName: "enterprise_id_old");

            migrationBuilder.AddColumn<int>(
                name: "enterprise_id",
                table: "driver",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.DropColumn(
                name: "enterprise_id_old",
                table: "driver");

            migrationBuilder.CreateIndex(
                name: "ix_vehicle_enterprise_id",
                table: "vehicle",
                column: "enterprise_id");

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
    }
}
