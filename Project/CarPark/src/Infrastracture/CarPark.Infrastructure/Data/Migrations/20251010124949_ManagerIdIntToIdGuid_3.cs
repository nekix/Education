using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarPark.Data.Migrations
{
    /// <inheritdoc />
    public partial class ManagerIdIntToIdGuid_3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_new_enterprsise_manager_enterprise_enterprise_id",
                table: "new_enterprsise_manager");

            migrationBuilder.DropForeignKey(
                name: "fk_new_enterprsise_manager_manager_manager_id",
                table: "new_enterprsise_manager");

            migrationBuilder.RenameColumn(
                name: "manager_id",
                table: "new_enterprsise_manager",
                newName: "managers_id");

            migrationBuilder.RenameColumn(
                name: "enterprise_id",
                table: "new_enterprsise_manager",
                newName: "enterprises_id");

            migrationBuilder.RenameIndex(
                name: "ix_new_enterprsise_manager_manager_id",
                table: "new_enterprsise_manager",
                newName: "ix_new_enterprsise_manager_managers_id");

            migrationBuilder.AddForeignKey(
                name: "fk_new_enterprsise_manager_enterprise_enterprises_id",
                table: "new_enterprsise_manager",
                column: "enterprises_id",
                principalTable: "enterprise",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_new_enterprsise_manager_manager_managers_id",
                table: "new_enterprsise_manager",
                column: "managers_id",
                principalTable: "manager",
                principalColumn: "guid_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_new_enterprsise_manager_enterprise_enterprises_id",
                table: "new_enterprsise_manager");

            migrationBuilder.DropForeignKey(
                name: "fk_new_enterprsise_manager_manager_managers_id",
                table: "new_enterprsise_manager");

            migrationBuilder.RenameColumn(
                name: "managers_id",
                table: "new_enterprsise_manager",
                newName: "manager_id");

            migrationBuilder.RenameColumn(
                name: "enterprises_id",
                table: "new_enterprsise_manager",
                newName: "enterprise_id");

            migrationBuilder.RenameIndex(
                name: "ix_new_enterprsise_manager_managers_id",
                table: "new_enterprsise_manager",
                newName: "ix_new_enterprsise_manager_manager_id");

            migrationBuilder.AddForeignKey(
                name: "fk_new_enterprsise_manager_enterprise_enterprise_id",
                table: "new_enterprsise_manager",
                column: "enterprise_id",
                principalTable: "enterprise",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_new_enterprsise_manager_manager_manager_id",
                table: "new_enterprsise_manager",
                column: "manager_id",
                principalTable: "manager",
                principalColumn: "guid_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
