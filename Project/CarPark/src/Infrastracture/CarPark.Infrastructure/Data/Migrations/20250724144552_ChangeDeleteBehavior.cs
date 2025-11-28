using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarPark.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDeleteBehavior : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_driver_enterprise_enterprise_id",
                table: "driver");

            migrationBuilder.DropForeignKey(
                name: "fk_manager_asp_net_users_identity_user_id",
                table: "manager");

            migrationBuilder.DropForeignKey(
                name: "fk_vehicle_enterprise_enterprise_id",
                table: "vehicle");

            migrationBuilder.DropForeignKey(
                name: "fk_vehicle_model_model_id",
                table: "vehicle");

            migrationBuilder.AddForeignKey(
                name: "fk_driver_enterprise_enterprise_id",
                table: "driver",
                column: "enterprise_id",
                principalTable: "enterprise",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_manager_asp_net_users_identity_user_id",
                table: "manager",
                column: "identity_user_id",
                principalTable: "AspNetUsers",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_vehicle_enterprise_enterprise_id",
                table: "vehicle",
                column: "enterprise_id",
                principalTable: "enterprise",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_vehicle_model_model_id",
                table: "vehicle",
                column: "model_id",
                principalTable: "model",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_driver_enterprise_enterprise_id",
                table: "driver");

            migrationBuilder.DropForeignKey(
                name: "fk_manager_asp_net_users_identity_user_id",
                table: "manager");

            migrationBuilder.DropForeignKey(
                name: "fk_vehicle_enterprise_enterprise_id",
                table: "vehicle");

            migrationBuilder.DropForeignKey(
                name: "fk_vehicle_model_model_id",
                table: "vehicle");

            migrationBuilder.AddForeignKey(
                name: "fk_driver_enterprise_enterprise_id",
                table: "driver",
                column: "enterprise_id",
                principalTable: "enterprise",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_manager_asp_net_users_identity_user_id",
                table: "manager",
                column: "identity_user_id",
                principalTable: "AspNetUsers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_vehicle_enterprise_enterprise_id",
                table: "vehicle",
                column: "enterprise_id",
                principalTable: "enterprise",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_vehicle_model_model_id",
                table: "vehicle",
                column: "model_id",
                principalTable: "model",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
