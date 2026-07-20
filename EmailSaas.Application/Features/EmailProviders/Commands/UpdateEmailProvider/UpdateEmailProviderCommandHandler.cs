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
            var entity = await _context.MasterEmailProviders
                .Include(x => x.Client)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (entity == null)
                throw new NotFoundException("MasterEmailProvider", request.Id);
            // If setting this as default, unset other defaults for the same client
            if (request.IsDefault && !entity.IsDefault)
            {
                var existingDefaults = await _context.MasterEmailProviders
                    .Where(x => x.ClientID == entity.ClientID
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
            entity.SMTPHost = request.SMTPHost;
            entity.SMTPPort = request.SMTPPort;
            entity.UserName = request.UserName;
            entity.IsDefault = request.IsDefault;
            entity.UpdatedBy = request.UpdatedBy;
            entity.UpdatedDate = DateTime.UtcNow;
            // ✅ Only update Password/ApiKey if a new value was actually provided
            // — keeps existing encrypted credential if left blank
            if (!string.IsNullOrEmpty(request.Password))
                entity.Password = _encryptionService.Encrypt(request.Password);
            if (!string.IsNullOrWhiteSpace(request.ApiKey))
                entity.APIKey = request.ApiKey;
            if (!string.IsNullOrWhiteSpace(request.APIEndPoint))
                entity.APIEndPoint = request.APIEndPoint;

            // ─── New: IMAP bounce mailbox fields — only update if provided ───
            if (!string.IsNullOrWhiteSpace(request.IMAPHost))
                entity.IMAPHost = request.IMAPHost;
            if (request.IMAPPort.HasValue)
                entity.IMAPPort = request.IMAPPort;
            if (!string.IsNullOrWhiteSpace(request.IMPAUserName))
                entity.IMPAUserName = request.IMPAUserName;
            if (!string.IsNullOrEmpty(request.IMAPPassword))
                entity.IMAPPassword = _encryptionService.Encrypt(request.IMAPPassword);
            if (request.IMAPSSL.HasValue)
                entity.IMAPSSL = request.IMAPSSL.Value;
//            if (request.BounceMonitoringEnabled.HasValue)
//                entity.BounceMonitoringEnabled = request.BounceMonitoringEnabled.Value;

            await _context.SaveChangesAsync(cancellationToken);
            var response = new EmailProviderResponseDto
            {
                Id = entity.Id,
                ClientID = entity.ClientID,
                ClientCode = entity.Client.ClientCode,
                ClientName = entity.Client.ClientName,
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
}