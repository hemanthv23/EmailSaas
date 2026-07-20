using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmailSaas.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixEmailLogTemplateFk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MasterApplication",
                columns: table => new
                {
                    AppID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ApplicationName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ApiKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasterApplication", x => x.AppID);
                });

            migrationBuilder.CreateTable(
                name: "MasterClient",
                columns: table => new
                {
                    ClientID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppID = table.Column<int>(type: "int", nullable: false),
                    ClientCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ClientName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LogoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PrimaryColor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasterClient", x => x.ClientID);
                    table.ForeignKey(
                        name: "FK_MasterClient_MasterApplication_AppID",
                        column: x => x.AppID,
                        principalTable: "MasterApplication",
                        principalColumn: "AppID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MasterEmailProvider",
                columns: table => new
                {
                    ProviderID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientID = table.Column<int>(type: "int", nullable: false),
                    ProviderName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SenderName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SenderEmail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ReplyToEmail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Password = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    SMTPHost = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SMTPPort = table.Column<int>(type: "int", nullable: true),
                    APIKey = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    IMAPHost = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IMPAUserName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IMAPPassword = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IMAPPort = table.Column<int>(type: "int", nullable: true),
                    IMAPSSL = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    APIEndPoint = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasterEmailProvider", x => x.ProviderID);
                    table.ForeignKey(
                        name: "FK_MasterEmailProvider_MasterClient_ClientID",
                        column: x => x.ClientID,
                        principalTable: "MasterClient",
                        principalColumn: "ClientID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MasterEmailTemplate",
                columns: table => new
                {
                    TemplateID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientID = table.Column<int>(type: "int", nullable: false),
                    AppID = table.Column<int>(type: "int", nullable: false),
                    TemplateCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TemplateName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SubjectTemplate = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SubjectVariables = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BodyTemplate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BodyVariables = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FromEmailOverride = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasterEmailTemplate", x => x.TemplateID);
                    table.ForeignKey(
                        name: "FK_MasterEmailTemplate_MasterApplication_AppID",
                        column: x => x.AppID,
                        principalTable: "MasterApplication",
                        principalColumn: "AppID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MasterEmailTemplate_MasterClient_ClientID",
                        column: x => x.ClientID,
                        principalTable: "MasterClient",
                        principalColumn: "ClientID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmailLog",
                columns: table => new
                {
                    EmailLogID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationID = table.Column<int>(type: "int", nullable: false),
                    ClientID = table.Column<int>(type: "int", nullable: false),
                    TemplateID = table.Column<int>(type: "int", nullable: false),
                    ProviderID = table.Column<int>(type: "int", nullable: false),
                    ToEmail = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CcEmail = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    BccEmail = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ParameterValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RenderedBody = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SendDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MessageID = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    HasAttachment = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    AttachmentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailLog", x => x.EmailLogID);
                    table.ForeignKey(
                        name: "FK_EmailLog_MasterApplication_ApplicationID",
                        column: x => x.ApplicationID,
                        principalTable: "MasterApplication",
                        principalColumn: "AppID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailLog_MasterClient_ClientID",
                        column: x => x.ClientID,
                        principalTable: "MasterClient",
                        principalColumn: "ClientID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailLog_MasterEmailProvider_ProviderID",
                        column: x => x.ProviderID,
                        principalTable: "MasterEmailProvider",
                        principalColumn: "ProviderID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailLog_MasterEmailTemplate_TemplateID",
                        column: x => x.TemplateID,
                        principalTable: "MasterEmailTemplate",
                        principalColumn: "TemplateID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmailEventLog",
                columns: table => new
                {
                    EmailEventLogID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmailLogID = table.Column<int>(type: "int", nullable: false),
                    MessageID = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LogData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventLogDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailEventLog", x => x.EmailEventLogID);
                    table.ForeignKey(
                        name: "FK_EmailEventLog_EmailLog_EmailLogID",
                        column: x => x.EmailLogID,
                        principalTable: "EmailLog",
                        principalColumn: "EmailLogID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmailEventLog_EmailLogID",
                table: "EmailEventLog",
                column: "EmailLogID");

            migrationBuilder.CreateIndex(
                name: "IX_EmailLog_ApplicationID",
                table: "EmailLog",
                column: "ApplicationID");

            migrationBuilder.CreateIndex(
                name: "IX_EmailLog_ClientID",
                table: "EmailLog",
                column: "ClientID");

            migrationBuilder.CreateIndex(
                name: "IX_EmailLog_ProviderID",
                table: "EmailLog",
                column: "ProviderID");

            migrationBuilder.CreateIndex(
                name: "IX_EmailLog_TemplateID",
                table: "EmailLog",
                column: "TemplateID");

            migrationBuilder.CreateIndex(
                name: "IX_MasterApplication_ApplicationCode",
                table: "MasterApplication",
                column: "ApplicationCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MasterClient_AppID",
                table: "MasterClient",
                column: "AppID");

            migrationBuilder.CreateIndex(
                name: "IX_MasterClient_ClientCode",
                table: "MasterClient",
                column: "ClientCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MasterEmailProvider_ClientID",
                table: "MasterEmailProvider",
                column: "ClientID");

            migrationBuilder.CreateIndex(
                name: "IX_MasterEmailTemplate_AppID",
                table: "MasterEmailTemplate",
                column: "AppID");

            migrationBuilder.CreateIndex(
                name: "IX_MasterEmailTemplate_ClientID_TemplateCode",
                table: "MasterEmailTemplate",
                columns: new[] { "ClientID", "TemplateCode" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailEventLog");

            migrationBuilder.DropTable(
                name: "EmailLog");

            migrationBuilder.DropTable(
                name: "MasterEmailProvider");

            migrationBuilder.DropTable(
                name: "MasterEmailTemplate");

            migrationBuilder.DropTable(
                name: "MasterClient");

            migrationBuilder.DropTable(
                name: "MasterApplication");
        }
    }
}
