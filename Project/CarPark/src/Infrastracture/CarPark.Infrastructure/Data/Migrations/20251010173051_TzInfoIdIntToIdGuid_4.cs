using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CarPark.Data.Migrations
{
    /// <inheritdoc />
    public partial class TzInfoIdIntToIdGuid_4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_enterprise_tz_infos_time_zone_guid_id",
                table: "enterprise");

            migrationBuilder.DropUniqueConstraint(
                name: "ak_tz_infos_guid_id",
                table: "time_zone");

            migrationBuilder.DropPrimaryKey(
                name: "pk_time_zone",
                table: "time_zone");

            migrationBuilder.DropColumn(
                name: "id",
                table: "time_zone");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_enterprise_tz_infos_time_zone_guid_id",
                table: "enterprise");

            migrationBuilder.DropPrimaryKey(
                name: "ak_tz_infos_guid_id",
                table: "time_zone");

            migrationBuilder.AddColumn<int>(
                name: "id",
                table: "time_zone",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddUniqueConstraint(
                name: "ak_tz_infos_guid_id",
                table: "time_zone",
                column: "guid_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_time_zone",
                table: "time_zone",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_enterprise_tz_infos_time_zone_guid_id",
                table: "enterprise",
                column: "time_zone_guid_id",
                principalTable: "time_zone",
                principalColumn: "guid_id");
        }
    }
}
