using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarPark.Data.Migrations
{
    /// <inheritdoc />
    public partial class EnterpriseIdIntToIdGuid_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "enterprises_guid_id",
                table: "enterprise_manager",
                type: "uuid",
                nullable: false,
                defaultValue: Guid.Empty);

            migrationBuilder.Sql(@"
                UPDATE enterprise_manager em
                SET enterprises_guid_id = e.guid_id
                FROM enterprise e
                WHERE em.enterprises_id = e.id;
            ");

            migrationBuilder.DropColumn(
                name: "enterprises_id",
                table: "enterprise_manager");

            migrationBuilder.RenameColumn(
                name: "enterprises_guid_id",
                table: "enterprise_manager",
                newName: "enterprises_id");

            migrationBuilder.AddForeignKey(
                name: "fk_enterprise_manager_enterprise_enterprises_id",
                table: "enterprise_manager",
                column: "enterprises_id",
                principalTable: "enterprise",
                principalColumn: "guid_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "enterprises_int_id",
                table: "enterprise_manager",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(@"
                UPDATE enterprise_manager em
                SET enterprises_int_id = e.id
                FROM enterprise e
                WHERE em.enterprises_id = e.guid_id;
            ");

            migrationBuilder.DropColumn(
                name: "enterprises_id",
                table: "enterprise_manager");

            migrationBuilder.RenameColumn(
                name: "enterprises_int_id",
                table: "enterprise_manager",
                newName: "enterprises_id");

            migrationBuilder.AddForeignKey(
                name: "fk_enterprise_manager_enterprise_enterprises_id",
                table: "enterprise_manager",
                column: "enterprises_id",
                principalTable: "enterprise",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
