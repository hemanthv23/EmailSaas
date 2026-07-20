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

        // Generate unique MessageID for each email
        string GenerateMessageID();

        // ─── New: centralizes the header name used for bounce-mail matching ───
        string GetBounceHeaderName();
    }
}
