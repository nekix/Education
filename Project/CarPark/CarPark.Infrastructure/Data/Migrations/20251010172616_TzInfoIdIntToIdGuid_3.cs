using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarPark.Data.Migrations
{
    /// <inheritdoc />
    public partial class TzInfoIdIntToIdGuid_3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "time_zone_guid_id",
                table: "enterprise",
                type: "uuid",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE enterprise e
                SET time_zone_guid_id = t.guid_id
                FROM time_zone t
                WHERE e.time_zone_id = t.id
            ");

            migrationBuilder.DropForeignKey(
                name: "fk_enterprise_tz_infos_time_zone_id",
                table: "enterprise");

            migrationBuilder.DropIndex(
                name: "ix_enterprise_time_zone_id",
                table: "enterprise");


            migrationBuilder.DropColumn(
                name: "time_zone_id",
                table: "enterprise");

            migrationBuilder.DropUniqueConstraint(
                name: "ak_time_zone_guid_id",
                table: "time_zone");

            migrationBuilder.AddUniqueConstraint(
                name: "ak_tz_infos_guid_id",
                table: "time_zone",
                column: "guid_id");

            migrationBuilder.CreateIndex(
                name: "ix_enterprise_time_zone_guid_id",
                table: "enterprise",
                column: "time_zone_guid_id");

            migrationBuilder.AddForeignKey(
                name: "fk_enterprise_tz_infos_time_zone_guid_id",
                table: "enterprise",
                column: "time_zone_guid_id",
                principalTable: "time_zone",
                principalColumn: "guid_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_enterprise_tz_infos_time_zone_guid_id",
                table: "enterprise");

            migrationBuilder.DropUniqueConstraint(
                name: "ak_tz_infos_guid_id",
                table: "time_zone");

            migrationBuilder.DropIndex(
                name: "ix_enterprise_time_zone_guid_id",
                table: "enterprise");

            migrationBuilder.AddColumn<int>(
                name: "time_zone_id",
                table: "enterprise",
                type: "integer",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE enterprise e
                SET time_zone_id = t.id
                FROM time_zone t
                WHERE e.time_zone_guid_id = t.guid_id
            ");

            migrationBuilder.AddUniqueConstraint(
                name: "ak_time_zone_guid_id",
                table: "time_zone",
                column: "guid_id");

            migrationBuilder.CreateIndex(
                name: "ix_enterprise_time_zone_id",
                table: "enterprise",
                column: "time_zone_id");

            migrationBuilder.AddForeignKey(
                name: "fk_enterprise_tz_infos_time_zone_id",
                table: "enterprise",
                column: "time_zone_id",
                principalTable: "time_zone",
                principalColumn: "id");

            migrationBuilder.DropColumn(
                name: "time_zone_guid_id",
                table: "enterprise");
        }
    }
}
