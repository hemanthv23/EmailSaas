using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmailSaas.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddApiEndpointToEmailProviderConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApiEndpoint",
                table: "MasterEmailProviderConfig",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApiEndpoint",
                table: "MasterEmailProviderConfig");
        }
    }
}
