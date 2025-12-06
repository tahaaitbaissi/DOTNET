using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarRental.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthFieldsAndTariff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentMileage",
                table: "Vehicles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastMaintenanceDate",
                table: "Vehicles",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LastMaintenanceMileage",
                table: "Vehicles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "VehicleTypeId1",
                table: "Vehicles",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<bool>(
                name: "IsEmailVerified",
                table: "Users",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PasswordResetToken",
                table: "Users",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "PasswordResetTokenExpiry",
                table: "Users",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VerificationToken",
                table: "Users",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "VerificationTokenExpiry",
                table: "Users",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_VehicleTypeId1",
                table: "Vehicles",
                column: "VehicleTypeId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_VehicleTypes_VehicleTypeId1",
                table: "Vehicles",
                column: "VehicleTypeId1",
                principalTable: "VehicleTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_VehicleTypes_VehicleTypeId1",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_VehicleTypeId1",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "CurrentMileage",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "LastMaintenanceDate",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "LastMaintenanceMileage",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "VehicleTypeId1",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "IsEmailVerified",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PasswordResetToken",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PasswordResetTokenExpiry",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "VerificationToken",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "VerificationTokenExpiry",
                table: "Users");
        }
    }
}
