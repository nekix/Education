using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarPark.Data.Migrations
{
    /// <inheritdoc />
    public partial class ManagerIdIntToIdGuid_4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // new
            migrationBuilder.DropForeignKey(
                name: "fk_new_enterprsise_manager_manager_managers_id",
                table: "new_enterprsise_manager");

            migrationBuilder.DropTable(
                name: "enterprise_manager");

            migrationBuilder.DropUniqueConstraint(
                name: "ak_manager_guid_id",
                table: "manager");

            migrationBuilder.DropPrimaryKey(
                name: "pk_manager",
                table: "manager");

            migrationBuilder.AddPrimaryKey(
                name: "pk_manager",
                table: "manager",
                column: "guid_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_new_enterprsise_manager_manager_managers_id",
                table: "new_enterprsise_manager");

            migrationBuilder.DropPrimaryKey(
                name: "pk_manager",
                table: "manager");

            migrationBuilder.AddUniqueConstraint(
                name: "ak_manager_guid_id",
                table: "manager",
                column: "guid_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_manager",
                table: "manager",
                column: "id");

            migrationBuilder.CreateTable(
                name: "enterprise_manager",
                columns: table => new
                {
                    enterprises_id = table.Column<int>(type: "integer", nullable: false),
                    managers_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_enterprise_manager", x => new { x.enterprises_id, x.managers_id });
                    table.ForeignKey(
                        name: "fk_enterprise_manager_enterprises_enterprises_id",
                        column: x => x.enterprises_id,
                        principalTable: "enterprise",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_enterprise_manager_manager_managers_id",
                        column: x => x.managers_id,
                        principalTable: "manager",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_enterprise_manager_managers_id",
                table: "enterprise_manager",
                column: "managers_id");
        }
    }
}
