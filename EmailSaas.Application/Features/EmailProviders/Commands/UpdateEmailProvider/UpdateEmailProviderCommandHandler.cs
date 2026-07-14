using EmailSaas.Application.Common.Exceptions;
using EmailSaas.Application.Common.Interfaces;
using EmailSaas.Application.Common.Models;
using EmailSaas.Application.DTOs.EmailProvider;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace EmailSaas.Application.Features.EmailProviders.Commands.UpdateEmailProvider
{
    public class UpdateEmailProviderCommandHandler : IRequestHandler<UpdateEmailProviderCommand, Result<EmailProviderResponseDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IEncryptionService _encryptionService;
        public UpdateEmailProviderCommandHandler(
            IApplicationDbContext context,
            IEncryptionService encryptionService)
        {
            _context = context;
            _encryptionService = encryptionService;
        }
        public async Task<Result<EmailProviderResponseDto>> Handle(
            UpdateEmailProviderCommand request,
            CancellationToken cancellationToken)
        {
            var entity = await _context.EmailProviderConfigs
                .Include(x => x.Client)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (entity == null)
                throw new NotFoundException("EmailProviderConfig", request.Id);
            // If setting this as default, unset other defaults for the same client
            if (request.IsDefault && !entity.IsDefault)
            {
                var existingDefaults = await _context.EmailProviderConfigs
                    .Where(x => x.ClientId == entity.ClientId
                             && x.IsDefault
                             && x.Id != entity.Id)
                    .ToListAsync(cancellationToken);
                foreach (var existing in existingDefaults)
                    existing.IsDefault = false;
            }
            entity.ProviderName = request.ProviderName;
            entity.SenderName = request.SenderName;
            entity.SenderEmail = request.SenderEmail;
            entity.ReplyToEmail = request.ReplyToEmail;
            entity.SmtpHost = request.SmtpHost;
            entity.SmtpPort = request.SmtpPort;
            entity.UserName = request.UserName;
            entity.IsDefault = request.IsDefault;
            entity.UpdatedBy = request.UpdatedBy;
            entity.UpdatedDate = DateTime.UtcNow;
            // ✅ Only update Password/ApiKey if a new value was actually provided
            // — keeps existing encrypted credential if left blank
            if (!string.IsNullOrEmpty(request.Password))
                entity.PasswordEncrypted = _encryptionService.Encrypt(request.Password);
            if (!string.IsNullOrWhiteSpace(request.ApiKey))
                entity.ApiKeyEncrypted = request.ApiKey;

            // ─── New: IMAP bounce mailbox fields — only update if provided ───
            if (!string.IsNullOrWhiteSpace(request.ImapHost))
                entity.ImapHost = request.ImapHost;
            if (request.ImapPort.HasValue)
                entity.ImapPort = request.ImapPort;
            if (!string.IsNullOrWhiteSpace(request.ImapUserName))
                entity.ImapUserName = request.ImapUserName;
            if (!string.IsNullOrEmpty(request.ImapPassword))
                entity.ImapPasswordEncrypted = _encryptionService.Encrypt(request.ImapPassword);
            if (request.ImapUseSsl.HasValue)
                entity.ImapUseSsl = request.ImapUseSsl.Value;
            if (request.BounceMonitoringEnabled.HasValue)
                entity.BounceMonitoringEnabled = request.BounceMonitoringEnabled.Value;

            await _context.SaveChangesAsync(cancellationToken);
            var response = new EmailProviderResponseDto
            {
                Id = entity.Id,
                ClientId = entity.ClientId,
                ClientCode = entity.Client.ClientCode,
                ClientName = entity.Client.ClientName,
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
}