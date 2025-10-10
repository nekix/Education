using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarPark.Data.Migrations
{
    /// <inheritdoc />
    public partial class ManagerIdIntToIdGuid_7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "enterprsise_manager",
                newName: "enterprise_manager");

            migrationBuilder.RenameIndex(
                name: "ix_enterprsise_manager_managers_id",
                table: "enterprise_manager",
                newName: "ix_enterprise_manager_managers_id");

            migrationBuilder.AddForeignKey(
                name: "fk_enterprise_manager_enterprise_enterprises_id",
                table: "enterprise_manager",
                column: "enterprises_id",
                principalTable: "enterprise",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_enterprise_manager_manager_managers_id",
                table: "enterprise_manager",
                column: "managers_id",
                principalTable: "manager",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "enterprise_manager",
                newName: "enterprsise_manager");

            migrationBuilder.RenameIndex(
                name: "ix_enterprise_manager_managers_id",
                table: "enterprsise_manager",
                newName: "ix_enterprsise_manager_managers_id");

            migrationBuilder.AddForeignKey(
                name: "fk_enterprsise_manager_enterprise_enterprises_id",
                table: "enterprsise_manager",
                column: "enterprises_id",
                principalTable: "enterprise",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_enterprsise_manager_manager_managers_id",
                table: "enterprsise_manager",
                column: "managers_id",
                principalTable: "manager",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
