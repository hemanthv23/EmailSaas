using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace EmailSaas.API.Middleware;

public class WebhookSignatureValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    private const string SignatureHeaderName = "X-Webhook-Signature";
    private static readonly string[] ProtectedWebhookPaths =
    [
        "/api/track/delivery"
    ];

    public WebhookSignatureValidationMiddleware(
        RequestDelegate next,
        IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLower() ?? "";

        // Only apply this check to actual webhook endpoints
        var isProtectedWebhook = ProtectedWebhookPaths
            .Any(p => path.StartsWith(p));

        if (!isProtectedWebhook)
        {
            await _next(context);
            return;
        }

        var signingKey = _configuration["WebhookSettings:SigningKey"];

        // If no signing key is configured, block the endpoint entirely
        // rather than silently accepting unverified requests
        if (string.IsNullOrEmpty(signingKey))
        {
            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            await context.Response.WriteAsync(
                "{\"message\":\"Webhook signing key not configured.\"}");
            return;
        }

        if (!context.Request.Headers.TryGetValue(
                SignatureHeaderName, out var providedSignature))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync(
                "{\"message\":\"Missing webhook signature.\"}");
            return;
        }

        // Read the raw body so we can compute its HMAC signature
        context.Request.EnableBuffering();
        using var reader = new StreamReader(
            context.Request.Body, Encoding.UTF8, leaveOpen: true);
        var rawBody = await reader.ReadToEndAsync();
        context.Request.Body.Position = 0;

        var computedSignature = ComputeHmacSignature(rawBody, signingKey);

        if (!SignaturesMatch(computedSignature, providedSignature.ToString()))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync(
                "{\"message\":\"Invalid webhook signature.\"}");
            return;
        }

        await _next(context);
    }

    private static string ComputeHmacSignature(string payload, string key)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var payloadBytes = Encoding.UTF8.GetBytes(payload);

        using var hmac = new HMACSHA256(keyBytes);
        var hash = hmac.ComputeHash(payloadBytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    // Constant-time comparison to avoid timing attacks
    private static bool SignaturesMatch(string a, string b)
    {
        if (a.Length != b.Length) return false;

        var result = 0;
        for (var i = 0; i < a.Length; i++)
            result |= a[i] ^ b[i];

        return result == 0;
    }
}