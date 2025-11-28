using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarPark.Data.Migrations
{
    /// <inheritdoc />
    public partial class ManagerIdIntToIdGuid_6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_new_enterprsise_manager_enterprise_enterprises_id",
                table: "new_enterprsise_manager");

            migrationBuilder.DropForeignKey(
                name: "fk_new_enterprsise_manager_manager_managers_id",
                table: "new_enterprsise_manager");

            migrationBuilder.DropPrimaryKey(
                name: "pk_new_enterprsise_manager",
                table: "new_enterprsise_manager");

            migrationBuilder.RenameTable(
                name: "new_enterprsise_manager",
                newName: "enterprsise_manager");

            migrationBuilder.RenameIndex(
                name: "ix_new_enterprsise_manager_managers_id",
                table: "enterprsise_manager",
                newName: "ix_enterprsise_manager_managers_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_enterprsise_manager",
                table: "enterprsise_manager",
                columns: new[] { "enterprises_id", "managers_id" });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_enterprsise_manager_enterprise_enterprises_id",
                table: "enterprsise_manager");

            migrationBuilder.DropForeignKey(
                name: "fk_enterprsise_manager_manager_managers_id",
                table: "enterprsise_manager");

            migrationBuilder.DropPrimaryKey(
                name: "pk_enterprsise_manager",
                table: "enterprsise_manager");

            migrationBuilder.RenameTable(
                name: "enterprsise_manager",
                newName: "new_enterprsise_manager");

            migrationBuilder.RenameIndex(
                name: "ix_enterprsise_manager_managers_id",
                table: "new_enterprsise_manager",
                newName: "ix_new_enterprsise_manager_managers_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_new_enterprsise_manager",
                table: "new_enterprsise_manager",
                columns: new[] { "enterprises_id", "managers_id" });

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
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
