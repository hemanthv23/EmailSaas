using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmailSaas.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MasterApplication",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
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
                    table.PrimaryKey("PK_MasterApplication", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MasterClient",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_MasterClient", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MasterClient_MasterApplication_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "MasterApplication",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MasterEmailProviderConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    ProviderName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SenderName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SenderEmail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ReplyToEmail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SmtpHost = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SmtpPort = table.Column<int>(type: "int", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PasswordEncrypted = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ApiKeyEncrypted = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_MasterEmailProviderConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MasterEmailProviderConfig_MasterClient_ClientId",
                        column: x => x.ClientId,
                        principalTable: "MasterClient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MasterEmailTemplate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_MasterEmailTemplate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MasterEmailTemplate_MasterClient_ClientId",
                        column: x => x.ClientId,
                        principalTable: "MasterClient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WebhookSubscriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    CallbackUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Secret = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    EventTypes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)1),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebhookSubscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WebhookSubscriptions_MasterClient_ClientId",
                        column: x => x.ClientId,
                        principalTable: "MasterClient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmailLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    TemplateId = table.Column<int>(type: "int", nullable: false),
                    ToEmail = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CcEmail = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    BccEmail = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ParameterValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RenderedBody = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MessageId = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    WebhookStatus = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeliveredAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OpenedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastOpenedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OpenCount = table.Column<int>(type: "int", nullable: false, defaultValueSql: "0"),
                    ClickedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastClickedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClickCount = table.Column<int>(type: "int", nullable: false, defaultValueSql: "0"),
                    HasAttachment = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    AttachmentNames = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AttachmentOpenedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AttachmentOpenCount = table.Column<int>(type: "int", nullable: false, defaultValueSql: "0"),
                    BouncedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BounceReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailLog_MasterApplication_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "MasterApplication",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailLog_MasterClient_ClientId",
                        column: x => x.ClientId,
                        principalTable: "MasterClient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailLog_MasterEmailTemplate_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "MasterEmailTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmailEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmailLogId = table.Column<int>(type: "int", nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EventData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OccurredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailEvents_EmailLog_EmailLogId",
                        column: x => x.EmailLogId,
                        principalTable: "EmailLog",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmailLinkClicks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmailLogId = table.Column<int>(type: "int", nullable: false),
                    OriginalUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ClickedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailLinkClicks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailLinkClicks_EmailLog_EmailLogId",
                        column: x => x.EmailLogId,
                        principalTable: "EmailLog",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WebhookDeliveryLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WebhookSubscriptionId = table.Column<int>(type: "int", nullable: false),
                    EmailLogId = table.Column<int>(type: "int", nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Payload = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResponseStatusCode = table.Column<int>(type: "int", nullable: true),
                    ResponseBody = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AttemptNumber = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    IsSuccess = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    NextRetryAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeliveredAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebhookDeliveryLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WebhookDeliveryLogs_EmailLog_EmailLogId",
                        column: x => x.EmailLogId,
                        principalTable: "EmailLog",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WebhookDeliveryLogs_WebhookSubscriptions_WebhookSubscriptionId",
                        column: x => x.WebhookSubscriptionId,
                        principalTable: "WebhookSubscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmailEvents_EmailLogId",
                table: "EmailEvents",
                column: "EmailLogId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailEvents_EventType",
                table: "EmailEvents",
                column: "EventType");

            migrationBuilder.CreateIndex(
                name: "IX_EmailLinkClicks_EmailLogId",
                table: "EmailLinkClicks",
                column: "EmailLogId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailLog_ApplicationId",
                table: "EmailLog",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailLog_ClientId",
                table: "EmailLog",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailLog_MessageId",
                table: "EmailLog",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailLog_TemplateId",
                table: "EmailLog",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_MasterApplication_ApplicationCode",
                table: "MasterApplication",
                column: "ApplicationCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MasterClient_ApplicationId",
                table: "MasterClient",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_MasterClient_ClientCode",
                table: "MasterClient",
                column: "ClientCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MasterEmailProviderConfig_ClientId",
                table: "MasterEmailProviderConfig",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_MasterEmailTemplate_ClientId_TemplateCode",
                table: "MasterEmailTemplate",
                columns: new[] { "ClientId", "TemplateCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WebhookDeliveryLogs_EmailLogId",
                table: "WebhookDeliveryLogs",
                column: "EmailLogId");

            migrationBuilder.CreateIndex(
                name: "IX_WebhookDeliveryLogs_NextRetryAt",
                table: "WebhookDeliveryLogs",
                column: "NextRetryAt");

            migrationBuilder.CreateIndex(
                name: "IX_WebhookDeliveryLogs_WebhookSubscriptionId",
                table: "WebhookDeliveryLogs",
                column: "WebhookSubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_WebhookSubscriptions_ClientId",
                table: "WebhookSubscriptions",
                column: "ClientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailEvents");

            migrationBuilder.DropTable(
                name: "EmailLinkClicks");

            migrationBuilder.DropTable(
                name: "MasterEmailProviderConfig");

            migrationBuilder.DropTable(
                name: "WebhookDeliveryLogs");

            migrationBuilder.DropTable(
                name: "EmailLog");

            migrationBuilder.DropTable(
                name: "WebhookSubscriptions");

            migrationBuilder.DropTable(
                name: "MasterEmailTemplate");

            migrationBuilder.DropTable(
                name: "MasterClient");

            migrationBuilder.DropTable(
                name: "MasterApplication");
        }
    }
}
