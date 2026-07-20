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
        var client = await _context.MasterClients
            .FirstOrDefaultAsync(x => x.Id == request.ClientID, cancellationToken);

        if (client == null)
            return Result<EmailProviderResponseDto>.Failure(
                $"Client with Id '{request.ClientID}' not found.");

        // If IsDefault unset existing default for this client
        if (request.IsDefault)
        {
            var existingDefaults = await _context.MasterEmailProviders
                .Where(x => x.ClientID == request.ClientID && x.IsDefault)
                .ToListAsync(cancellationToken);

            foreach (var existing in existingDefaults)
                existing.IsDefault = false;
        }

        var entity = new MasterEmailProvider
        {
            ClientID = request.ClientID,
            ProviderName = request.ProviderName,
            SenderName = request.SenderName,
            SenderEmail = request.SenderEmail,
            ReplyToEmail = request.ReplyToEmail,
            SMTPHost = request.SMTPHost,
            SMTPPort = request.SMTPPort,

            // Microsoft Graph:
            // UserName column stores Tenant ID (Plain Text)
            UserName = request.UserName,

            // Client Secret (Encrypted)
            Password = !string.IsNullOrWhiteSpace(request.Password)
                ? _encryptionService.Encrypt(request.Password)
                : null,

            // Client ID (Plain Text)
            APIKey = !string.IsNullOrWhiteSpace(request.ApiKey)
                ? request.ApiKey
                : null,
            APIEndPoint = request.APIEndPoint,

            // IMAP Bounce Monitoring Fields
            IMAPHost = request.IMAPHost,
            IMAPPort = request.IMAPPort,
            IMPAUserName = request.IMPAUserName,
            IMAPPassword = !string.IsNullOrWhiteSpace(request.IMAPPassword)
                ? _encryptionService.Encrypt(request.IMAPPassword)
                : null,
            IMAPSSL = request.IMAPSSL ?? true,
//            BounceMonitoringEnabled = request.BounceMonitoringEnabled ?? false,

            IsDefault = request.IsDefault,
            Status = (byte)CommonStatus.Active,
            CreatedBy = request.CreatedBy,
            CreatedDate = DateTime.UtcNow
        };

        _context.MasterEmailProviders.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        var response = new EmailProviderResponseDto
        {
            Id = entity.Id,
            ClientID = entity.ClientID,
            ClientCode = client.ClientCode,
            ClientName = client.ClientName,
            ProviderName = entity.ProviderName,
            SenderName = entity.SenderName,
            SenderEmail = entity.SenderEmail,
            ReplyToEmail = entity.ReplyToEmail,
            SMTPHost = entity.SMTPHost,
            SMTPPort = entity.SMTPPort,
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