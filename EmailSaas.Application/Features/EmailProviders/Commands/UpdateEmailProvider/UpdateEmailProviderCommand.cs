using EmailSaas.Application.Common.Models;
using EmailSaas.Application.DTOs.EmailProvider;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace EmailSaas.Application.Features.EmailProviders.Commands.UpdateEmailProvider
{
    public class UpdateEmailProviderCommand : IRequest<Result<EmailProviderResponseDto>>
    {
        public int Id { get; set; }
        public string ProviderName { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public string SenderEmail { get; set; } = string.Empty;
        public string? ReplyToEmail { get; set; }
        public string? SmtpHost { get; set; }
        public int? SmtpPort { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? ApiKey { get; set; }
        public bool IsDefault { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;

        // ─── New: IMAP bounce mailbox fields ───────────────────
        public string? ImapHost { get; set; }
        public int? ImapPort { get; set; }
        public string? ImapUserName { get; set; }
        public string? ImapPassword { get; set; }
        public bool? ImapUseSsl { get; set; }
        public bool? BounceMonitoringEnabled { get; set; }
    }
}