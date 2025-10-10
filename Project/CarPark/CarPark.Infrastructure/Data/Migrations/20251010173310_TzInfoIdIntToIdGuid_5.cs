using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarPark.Data.Migrations
{
    /// <inheritdoc />
    public partial class TzInfoIdIntToIdGuid_5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_enterprise_tz_infos_time_zone_guid_id",
                table: "enterprise");

            migrationBuilder.DropPrimaryKey(
                name: "ak_tz_infos_guid_id",
                table: "time_zone");

            migrationBuilder.RenameColumn(
                name: "guid_id",
                table: "time_zone",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "time_zone_guid_id",
                table: "enterprise",
                newName: "time_zone_id");

            migrationBuilder.RenameIndex(
                name: "ix_enterprise_time_zone_guid_id",
                table: "enterprise",
                newName: "ix_enterprise_time_zone_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_time_zone",
                table: "time_zone",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_enterprise_tz_infos_time_zone_id",
                table: "enterprise",
                column: "time_zone_id",
                principalTable: "time_zone",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_enterprise_tz_infos_time_zone_id",
                table: "enterprise");

            migrationBuilder.DropPrimaryKey(
                name: "pk_time_zone",
                table: "time_zone");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "time_zone",
                newName: "guid_id");

            migrationBuilder.RenameColumn(
                name: "time_zone_id",
                table: "enterprise",
                newName: "time_zone_guid_id");

            migrationBuilder.RenameIndex(
                name: "ix_enterprise_time_zone_id",
                table: "enterprise",
                newName: "ix_enterprise_time_zone_guid_id");

            migrationBuilder.AddPrimaryKey(
                name: "ak_tz_infos_guid_id",
                table: "time_zone",
                column: "guid_id");

            migrationBuilder.AddForeignKey(
                name: "fk_enterprise_tz_infos_time_zone_guid_id",
                table: "enterprise",
                column: "time_zone_guid_id",
                principalTable: "time_zone",
                principalColumn: "guid_id");
        }
    }
}
