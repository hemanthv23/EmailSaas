using System.Text.RegularExpressions;
using EmailSaas.Application.Common.Interfaces;
namespace EmailSaas.Infrastructure.Services;
public class EmailTrackingService : IEmailTrackingService
{
    public string GenerateMessageID()
    {
        return $"MSG-{Guid.NewGuid():N}".ToUpper();
    }

    public string InjectTracking(
        string htmlBody,
        string messageId,
        string baseUrl)
    {
        htmlBody = WrapLinks(htmlBody, messageId, baseUrl);
        htmlBody = InjectOpenPixel(htmlBody, messageId, baseUrl);
        return htmlBody;
    }

    // ─── New: centralizes the header name used for bounce-mail matching ───
    public string GetBounceHeaderName()
    {
        return "X-EmailSaas-MessageID";
    }

    private static string WrapLinks(
        string htmlBody,
        string messageId,
        string baseUrl)
    {
        var pattern = @"href=[""'](https?://[^""']+)[""']";
        return Regex.Replace(htmlBody, pattern, match =>
        {
            var originalUrl = match.Groups[1].Value;
            if (originalUrl.Contains("/api/track/"))
                return match.Value;
            if (originalUrl.Contains("unsubscribe"))
                return match.Value;
            var encodedUrl = Uri.EscapeDataString(originalUrl);
            var trackingUrl =
                $"{baseUrl}/api/track/click/{messageId}?url={encodedUrl}";
            return $"href=\"{trackingUrl}\"";
        });
    }

    private static string InjectOpenPixel(
        string htmlBody,
        string messageId,
        string baseUrl)
    {
        // ✅ Unique timestamp prevents Gmail caching
        var uniqueParam = Guid.NewGuid().ToString("N");
        var pixelUrl =
            $"{baseUrl}/api/track/open/{messageId}?t={uniqueParam}";
        var pixel =
            $"<img src=\"{pixelUrl}\" " +
            $"width=\"1\" height=\"1\" " +
            $"style=\"display:none;\" " +
            $"alt=\"\" />";
        if (htmlBody.Contains("</body>",
            StringComparison.OrdinalIgnoreCase))
        {
            return htmlBody.Replace(
                "</body>",
                $"{pixel}</body>",
                StringComparison.OrdinalIgnoreCase);
        }
        return htmlBody + pixel;
    }
}