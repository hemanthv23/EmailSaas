using MediatR;
using Microsoft.EntityFrameworkCore;
using EmailSaas.Application.Common.Interfaces;
using EmailSaas.Application.Common.Models;
using EmailSaas.Application.DTOs.EmailProvider;
using EmailSaas.Domain.Entities;
using EmailSaas.Domain.Enums;

namespace EmailSaas.Application.Features.EmailProviders.Commands.CreateEmailProvider;

public class CreateEmailProviderCommandHandler : IRequestHandler<CreateEmailProviderCommand, Result<EmailProviderResponseDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IEncryptionService _encryptionService;

    public CreateEmailProviderCommandHandler(
        IApplicationDbContext context,
        IEncryptionService encryptionService)
    {
        _context = context;
        _encryptionService = encryptionService;
    }

    public async Task<Result<EmailProviderResponseDto>> Handle(
        CreateEmailProviderCommand request,
        CancellationToken cancellationToken)
    {
        // Check client exists
        var client = await _context.ClientMasters
            .FirstOrDefaultAsync(x => x.Id == request.ClientId, cancellationToken);

        if (client == null)
            return Result<EmailProviderResponseDto>.Failure(
                $"Client with Id '{request.ClientId}' not found.");

        // If IsDefault unset existing default for this client
        if (request.IsDefault)
        {
            var existingDefaults = await _context.EmailProviderConfigs
                .Where(x => x.ClientId == request.ClientId && x.IsDefault)
                .ToListAsync(cancellationToken);

            foreach (var existing in existingDefaults)
                existing.IsDefault = false;
        }

        var entity = new EmailProviderConfig
        {
            ClientId = request.ClientId,
            ProviderName = request.ProviderName,
            SenderName = request.SenderName,
            SenderEmail = request.SenderEmail,
            ReplyToEmail = request.ReplyToEmail,
            SmtpHost = request.SmtpHost,
            SmtpPort = request.SmtpPort,

            // Microsoft Graph:
            // UserName column stores Tenant ID (Plain Text)
            UserName = request.UserName,

            // Client Secret (Encrypted)
            PasswordEncrypted = !string.IsNullOrWhiteSpace(request.Password)
                ? _encryptionService.Encrypt(request.Password)
                : null,

            // Client ID (Plain Text)
            ApiKeyEncrypted = !string.IsNullOrWhiteSpace(request.ApiKey)
                ? request.ApiKey
                : null,
            ApiEndpoint = request.ApiEndpoint,

            // IMAP Bounce Monitoring Fields
            ImapHost = request.ImapHost,
            ImapPort = request.ImapPort,
            ImapUserName = request.ImapUserName,
            ImapPasswordEncrypted = !string.IsNullOrWhiteSpace(request.ImapPassword)
                ? _encryptionService.Encrypt(request.ImapPassword)
                : null,
            ImapUseSsl = request.ImapUseSsl ?? true,
            BounceMonitoringEnabled = request.BounceMonitoringEnabled ?? false,

            IsDefault = request.IsDefault,
            Status = (byte)CommonStatus.Active,
            CreatedBy = request.CreatedBy,
            CreatedDate = DateTime.UtcNow
        };

        _context.EmailProviderConfigs.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        var response = new EmailProviderResponseDto
        {
            Id = entity.Id,
            ClientId = entity.ClientId,
            ClientCode = client.ClientCode,
            ClientName = client.ClientName,
            ProviderName = entity.ProviderName,
            SenderName = entity.SenderName,
            SenderEmail = entity.SenderEmail,
            ReplyToEmail = entity.ReplyToEmail,
            SmtpHost = entity.SmtpHost,
            SmtpPort = entity.SmtpPort,
            UserName = entity.UserName,
            IsDefault = entity.IsDefault,
            Status = entity.Status,
            CreatedBy = entity.CreatedBy,
            CreatedDate = entity.CreatedDate,
            UpdatedBy = entity.UpdatedBy,
            UpdatedDate = entity.UpdatedDate
        };

        return Result<EmailProviderResponseDto>.Success(response);
    }
}