using EmailSaas.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.Common.Interfaces
{
    public interface IEmailSenderService
    {
        Task<(bool Success, string? ErrorMessage)> SendAsync(
            EmailProviderConfig providerConfig,
            string toEmail,
            string? ccEmail,
            string? bccEmail,
            string subject,
            string htmlBody,
            Dictionary<string, string>? customHeaders = null,
            CancellationToken cancellationToken = default);
    }
}
