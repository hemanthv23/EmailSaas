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
        var application = await _context.ApplicationMasters
            .FirstOrDefaultAsync(x => x.Id == request.ApplicationId,
                cancellationToken);

        if (application == null)
            return Result<SendEmailResponseDto>.Failure(
                $"Application with Id '{request.ApplicationId}' not found.");

        // Step 2: Validate Client exists and belongs to Application
        var client = await _context.ClientMasters
            .FirstOrDefaultAsync(x => x.Id == request.ClientId
                && x.ApplicationId == request.ApplicationId,
                cancellationToken);

        if (client == null)
            return Result<SendEmailResponseDto>.Failure(
                $"Client with Id '{request.ClientId}' not found under this Application.");

        // Step 3: Fetch Template by ClientId + TemplateCode
        var template = await _context.EmailTemplateMasters
            .FirstOrDefaultAsync(x => x.ClientId == request.ClientId
                && x.TemplateCode == request.TemplateCode
                && x.Status == (byte)CommonStatus.Active,
                cancellationToken);

        if (template == null)
            return Result<SendEmailResponseDto>.Failure(
                $"Template '{request.TemplateCode}' not found for this client.");

        // Step 4: Fetch active default Email Provider for this Client
        var providerConfig = await _context.EmailProviderConfigs
            .FirstOrDefaultAsync(x => x.ClientId == request.ClientId
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

        // Step 6: Generate unique MessageId for tracking
        var messageId = _trackingService.GenerateMessageId();

        // Step 7: Inject tracking pixel + wrap all links
        var baseUrl = _configuration["TrackingSettings:BaseUrl"]
            ?? "https://localhost:7008";

        var trackedBody = _trackingService.InjectTracking(
            renderedBody, messageId, baseUrl);

        // Step 8: Create EmailLog with Pending status
        var emailLog = new EmailLog
        {
            ApplicationId = request.ApplicationId,
            ClientId = request.ClientId,
            TemplateId = template.Id,
            ToEmail = request.ToEmail,
            CcEmail = request.CcEmail,
            BccEmail = request.BccEmail,
            Subject = renderedSubject,
            ParameterValues = System.Text.Json.JsonSerializer
                .Serialize(request.Parameters),
            RenderedBody = trackedBody,
            MessageId = messageId,
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

        // Step 10: Update EmailLog with send result
        // deliveredAt stays null until a REAL delivery confirmation event is received
        // (bounce mailbox listener, or a future provider callback). WebhookStatus similarly
        // stays null here — it only gets set by an actual tracked event (open, click, bounce).
        emailLog.Status = success
            ? (byte)EmailSendStatus.Sent
            : (byte)EmailSendStatus.Failed;
        emailLog.ErrorMessage = errorMessage;
        emailLog.SentDate = success ? DateTime.UtcNow : null;
        emailLog.UpdatedBy = request.CreatedBy;
        emailLog.UpdatedDate = DateTime.UtcNow;

        // Step 11: Record raw event for audit trail (Sent or Failed)
        var eventType = success ? EmailEventType.Sent : EmailEventType.Failed;

        _context.EmailEvents.Add(new EmailEvent
        {
            EmailLogId = emailLog.Id,
            EventType = eventType.ToString(),
            EventData = System.Text.Json.JsonSerializer.Serialize(new { errorMessage }),
            OccurredAt = DateTime.UtcNow,
            CreatedBy = "System",
            CreatedDate = DateTime.UtcNow
        });

        await _context.SaveChangesAsync(cancellationToken);

        // Step 12: Queue outbound webhook — only for Failed (clients subscribe to Failed, not Sent,
        // per WebhookEventType enum — Sent is an internal-only lifecycle event)
        if (!success)
        {
            await _webhookDispatcher.QueueWebhookAsync(
                emailLog.Id, EmailEventType.Failed.ToString(), new { errorMessage }, cancellationToken);
        }

        return Result<SendEmailResponseDto>.Success(new SendEmailResponseDto
        {
            EmailLogId = emailLog.Id,
            ToEmail = request.ToEmail,
            Subject = renderedSubject,
            Status = success ? "Sent" : "Failed",
            ErrorMessage = errorMessage,
            SentDate = emailLog.SentDate
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