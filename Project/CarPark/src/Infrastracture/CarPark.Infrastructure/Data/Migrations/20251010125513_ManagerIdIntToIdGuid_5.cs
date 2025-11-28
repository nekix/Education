using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CarPark.Data.Migrations
{
    /// <inheritdoc />
    public partial class ManagerIdIntToIdGuid_5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // old
            //migrationBuilder.DropForeignKey(
            //    name: "fk_new_enterprsise_manager_manager_managers_id",
            //    table: "new_enterprsise_manager");

            migrationBuilder.DropPrimaryKey(
                name: "pk_manager",
                table: "manager");

            // old
            //migrationBuilder.DropColumn(
            //    name: "guid_id",
            //    table: "manager");

            //migrationBuilder.AlterColumn<Guid>(
            //    name: "id",
            //    table: "manager",
            //    type: "uuid",
            //    nullable: false,
            //    oldClrType: typeof(int),
            //    oldType: "integer")
            //    .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            // new 
            migrationBuilder.DropColumn(
                name: "id",
                table: "manager");

            // new
            migrationBuilder.RenameColumn(
                name: "guid_id",
                table: "manager",
                newName: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_manager",
                table: "manager",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_new_enterprsise_manager_manager_managers_id",
                table: "new_enterprsise_manager",
                column: "managers_id",
                principalTable: "manager",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
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

            // old
            //migrationBuilder.AlterColumn<int>(
            //    name: "id",
            //    table: "manager",
            //    type: "integer",
            //    nullable: false,
            //    oldClrType: typeof(Guid),
            //    oldType: "uuid")
            //    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            // old
            //migrationBuilder.AddColumn<Guid>(
            //    name: "guid_id",
            //    table: "manager",
            //    type: "uuid",
            //    nullable: false,
            //    defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            // new
            migrationBuilder.RenameColumn(
                name: "id",
                table: "manager",
                newName: "guid_id");

            // new
            migrationBuilder.AddColumn<int>(
                name: "id",
                table: "manager",
                type: "integer",
                nullable: false,
                defaultValue: 0)
            .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "pk_manager",
                table: "manager",
                column: "guid_id");

            migrationBuilder.AddForeignKey(
                name: "fk_new_enterprsise_manager_manager_managers_id",
                table: "new_enterprsise_manager",
                column: "managers_id",
                principalTable: "manager",
                principalColumn: "guid_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
