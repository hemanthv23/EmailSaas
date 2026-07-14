using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmailSaas.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updatedEmailProviderConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "BounceMonitoringEnabled",
                table: "MasterEmailProviderConfig",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ImapHost",
                table: "MasterEmailProviderConfig",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImapPasswordEncrypted",
                table: "MasterEmailProviderConfig",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ImapPort",
                table: "MasterEmailProviderConfig",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ImapUseSsl",
                table: "MasterEmailProviderConfig",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "ImapUserName",
                table: "MasterEmailProviderConfig",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BounceMonitoringEnabled",
                table: "MasterEmailProviderConfig");

            migrationBuilder.DropColumn(
                name: "ImapHost",
                table: "MasterEmailProviderConfig");

            migrationBuilder.DropColumn(
                name: "ImapPasswordEncrypted",
                table: "MasterEmailProviderConfig");

            migrationBuilder.DropColumn(
                name: "ImapPort",
                table: "MasterEmailProviderConfig");

            migrationBuilder.DropColumn(
                name: "ImapUseSsl",
                table: "MasterEmailProviderConfig");

            migrationBuilder.DropColumn(
                name: "ImapUserName",
                table: "MasterEmailProviderConfig");
        }
    }
}
