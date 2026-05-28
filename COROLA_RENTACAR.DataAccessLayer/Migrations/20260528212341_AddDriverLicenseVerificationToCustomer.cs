using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace COROLA_RENTACAR.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddDriverLicenseVerificationToCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DriverLicenseImageUrl",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DriverLicenseRejectionReason",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "DriverLicenseVerificationStatus",
                table: "Customers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DriverLicenseVerifiedDate",
                table: "Customers",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DriverLicenseImageUrl",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "DriverLicenseRejectionReason",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "DriverLicenseVerificationStatus",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "DriverLicenseVerifiedDate",
                table: "Customers");
        }
    }
}
