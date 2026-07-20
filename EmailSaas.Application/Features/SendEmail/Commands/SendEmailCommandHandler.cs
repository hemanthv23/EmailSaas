using EmailSaas.Application.Common.Interfaces;
using EmailSaas.Application.Common.Models;
using EmailSaas.Application.DTOs.SendEmail;
using EmailSaas.Domain.Entities;
using EmailSaas.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EmailSaas.Application.Features.SendEmail.Commands;

public class SendEmailCommandHandler : IRequestHandler<SendEmailCommand, Result<SendEmailResponseDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IEmailSenderService _emailSenderService;
    private readonly IEmailTrackingService _trackingService;
    private readonly IWebhookDispatcher _webhookDispatcher;
    private readonly IConfiguration _configuration;

    public SendEmailCommandHandler(
        IApplicationDbContext context,
        IEmailSenderService emailSenderService,
        IEmailTrackingService trackingService,
        IWebhookDispatcher webhookDispatcher,
        IConfiguration configuration)
    {
        _context = context;
        _emailSenderService = emailSenderService;
        _trackingService = trackingService;
        _webhookDispatcher = webhookDispatcher;
        _configuration = configuration;
    }

    public async Task<Result<SendEmailResponseDto>> Handle(
        SendEmailCommand request,
        CancellationToken cancellationToken)
    {
        // Step 1: Validate Application exists
        var application = await _context.MasterApplications
            .FirstOrDefaultAsync(x => x.Id == request.ApplicationId,
                cancellationToken);

        if (application == null)
            return Result<SendEmailResponseDto>.Failure(
                $"Application with Id '{request.ApplicationId}' not found.");

        // Step 2: Validate Client exists and belongs to Application
        var client = await _context.MasterClients
            .FirstOrDefaultAsync(x => x.Id == request.ClientID
                && x.ApplicationId == request.ApplicationId,
                cancellationToken);

        if (client == null)
            return Result<SendEmailResponseDto>.Failure(
                $"Client with Id '{request.ClientID}' not found under this Application.");

        // Step 3: Fetch Template by ClientID + TemplateCode
        var template = await _context.MasterEmailTemplates
            .FirstOrDefaultAsync(x => x.ClientID == request.ClientID
                && x.TemplateCode == request.TemplateCode
                && x.Status == (byte)CommonStatus.Active,
                cancellationToken);

        if (template == null)
            return Result<SendEmailResponseDto>.Failure(
                $"Template '{request.TemplateCode}' not found for this client.");

        // Step 4: Fetch active default Email Provider for this Client
        var providerConfig = await _context.MasterEmailProviders
            .FirstOrDefaultAsync(x => x.ClientID == request.ClientID
                && x.IsDefault
                && x.Status == (byte)CommonStatus.Active,
                cancellationToken);

        if (providerConfig == null)
            return Result<SendEmailResponseDto>.Failure(
                $"No active default email provider found for this client.");

        // Step 5: Replace placeholders in Subject and Body
        var renderedSubject = ReplacePlaceholders(
            template.SubjectTemplate, request.Parameters);
        var renderedBody = ReplacePlaceholders(
            template.BodyTemplate, request.Parameters);

        // Step 6: Generate unique MessageID for tracking
        var messageId = _trackingService.GenerateMessageID();

        // Step 7: Inject tracking pixel + wrap all links
        var baseUrl = _configuration["TrackingSettings:BaseUrl"]
            ?? "https://localhost:7008";

        var trackedBody = _trackingService.InjectTracking(
            renderedBody, messageId, baseUrl);

        // Step 8: Create EmailLog with Pending status
        var emailLog = new EmailLog
        {
            ApplicationId = request.ApplicationId,
            ClientID = request.ClientID,
            TemplateID = template.Id,
            ProviderID = providerConfig.Id,
            ToEmail = request.ToEmail,
            CcEmail = request.CcEmail,
            BccEmail = request.BccEmail,
            Subject = renderedSubject,
            ParameterValues = System.Text.Json.JsonSerializer
                .Serialize(request.Parameters),
            RenderedBody = trackedBody,
            MessageID = messageId,
            Status = (byte)EmailSendStatus.Pending,
            CreatedBy = request.CreatedBy,
            CreatedDate = DateTime.UtcNow
        };

        _context.EmailLogs.Add(emailLog);
        await _context.SaveChangesAsync(cancellationToken);

        // Step 9: Send email with tracked body + custom bounce-matching header
        var customHeaders = new Dictionary<string, string>
{
    { _trackingService.GetBounceHeaderName(), messageId }
};

        var (success, errorMessage) = await _emailSenderService.SendAsync(
            providerConfig,
            request.ToEmail,
            request.CcEmail,
            request.BccEmail,
            renderedSubject,
            trackedBody,
            customHeaders,
            cancellationToken);

        // Step 10: Update EmailLog with send result.
        // SendDate = when email left our server.
        // DeliveredAt = set later when tracking pixel fires (proof email reached inbox).
        var now = DateTime.UtcNow;

        if (success)
        {
            emailLog.Status = (byte)EmailSendStatus.Sent;
            //emailLog.WebhookStatus = EmailEventLogType.Sent.ToString();
            emailLog.SendDate = now;
            // DeliveredAt is NOT set here — it gets set automatically
            // when the recipient opens the email (tracking pixel fires).
        }
        else
        {
            emailLog.Status = (byte)EmailSendStatus.Failed;
            //emailLog.WebhookStatus = EmailEventLogType.Failed.ToString();
            emailLog.ErrorMessage = errorMessage;
        }

        emailLog.UpdatedBy = request.CreatedBy;
        emailLog.UpdatedDate = now;

        // Step 11: Record raw event for audit trail (Sent or Failed)
        var eventType = success ? EmailEventLogType.Sent : EmailEventLogType.Failed;

        _context.EmailEventLogs.Add(new EmailEventLog
        {
            LogID = emailLog.Id,
            MessageID = messageId,
            EventType = eventType.ToString(),
            LogData = System.Text.Json.JsonSerializer.Serialize(new { errorMessage }),
            EventLogDate = now,
            Status = 1,
            CreatedBy = "System",
            CreatedDate = now
        });

        await _context.SaveChangesAsync(cancellationToken);

        // Step 12: Queue outbound webhook notification to subscribers
        if (!success)
        {
            await _webhookDispatcher.QueueWebhookAsync(
                emailLog.Id, EmailEventLogType.Failed.ToString(), new { errorMessage }, cancellationToken);
        }

        return Result<SendEmailResponseDto>.Success(new SendEmailResponseDto
        {
            LogID = emailLog.Id,
            ToEmail = request.ToEmail,
            Subject = renderedSubject,
            Status = success ? "Sent" : "Failed",
            ErrorMessage = errorMessage,
            SendDate = emailLog.SendDate
        });
    }

    private static string ReplacePlaceholders(
        string template,
        Dictionary<string, string> parameters)
    {
        foreach (var param in parameters)
            template = template.Replace(
                $"{{{{{param.Key}}}}}", param.Value);
        return template;
    }
}