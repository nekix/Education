using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarPark.Data.Migrations
{
    /// <inheritdoc />
    public partial class ManagerIdIntToIdGuid_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_enterprise_tz_infos_time_zone_id",
                table: "enterprise");

            migrationBuilder.CreateTable(
                name: "new_enterprsise_manager",
                columns: table => new
                {
                    enterprise_id = table.Column<int>(type: "integer", nullable: false),
                    manager_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_new_enterprsise_manager", x => new { x.enterprise_id, x.manager_id });
                    table.ForeignKey(
                        name: "fk_new_enterprsise_manager_enterprise_enterprise_id",
                        column: x => x.enterprise_id,
                        principalTable: "enterprise",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_new_enterprsise_manager_manager_manager_id",
                        column: x => x.manager_id,
                        principalTable: "manager",
                        principalColumn: "guid_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_new_enterprsise_manager_manager_id",
                table: "new_enterprsise_manager",
                column: "manager_id");

            migrationBuilder.AddForeignKey(
                name: "fk_enterprise_tz_infos_time_zone_id",
                table: "enterprise",
                column: "time_zone_id",
                principalTable: "time_zone",
                principalColumn: "id");

            migrationBuilder.Sql(@"
                INSERT INTO new_enterprsise_manager (enterprise_id, manager_id)
                SELECT em.enterprises_id, m.guid_id
                FROM enterprise_manager em
                JOIN manager m ON m.id = em.managers_id;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_enterprise_tz_infos_time_zone_id",
                table: "enterprise");

            migrationBuilder.DropTable(
                name: "new_enterprsise_manager");

            migrationBuilder.AddForeignKey(
                name: "fk_enterprise_tz_infos_time_zone_id",
                table: "enterprise",
                column: "time_zone_id",
                principalTable: "time_zone",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
