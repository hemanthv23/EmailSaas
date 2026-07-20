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
        public string? SMTPHost { get; set; }
        public int? SMTPPort { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? ApiKey { get; set; }
        public string? APIEndPoint { get; set; }
        public bool IsDefault { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;

        // ─── New: IMAP bounce mailbox fields ───────────────────
        public string? IMAPHost { get; set; }
        public int? IMAPPort { get; set; }
        public string? IMPAUserName { get; set; }
        public string? IMAPPassword { get; set; }
        public bool? IMAPSSL { get; set; }
//        public bool? BounceMonitoringEnabled { get; set; }
    }
}