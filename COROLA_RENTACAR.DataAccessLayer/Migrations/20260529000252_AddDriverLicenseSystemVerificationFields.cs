using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace COROLA_RENTACAR.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddDriverLicenseSystemVerificationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DriverLicenseIssueDate",
                table: "Customers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DriverLicenseSystemMessage",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsDriverLicenseSystemVerified",
                table: "Customers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DriverLicenseIssueDate",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "DriverLicenseSystemMessage",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "IsDriverLicenseSystemVerified",
                table: "Customers");
        }
    }
}
