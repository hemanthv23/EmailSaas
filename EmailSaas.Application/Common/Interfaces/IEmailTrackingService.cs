using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.Common.Interfaces
{
    public interface IEmailTrackingService
    {
        // Inject tracking pixel + wrap all links in HTML
        string InjectTracking(string htmlBody, string messageId, string baseUrl);

        // Generate unique MessageId for each email
        string GenerateMessageId();

        // ─── New: centralizes the header name used for bounce-mail matching ───
        string GetBounceHeaderName();
    }
}
