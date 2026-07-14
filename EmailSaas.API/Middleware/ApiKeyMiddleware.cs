using Microsoft.EntityFrameworkCore;
using EmailSaas.Infrastructure.Persistence;

namespace EmailSaas.API.Middleware;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private const string ApiKeyHeaderName = "X-Api-Key";

    // These paths skip API key check
    private static readonly string[] WhitelistedPaths =
    [
        "/swagger",
        "/api/applications/create-application",
        "/api/applications/regenerate-apikey",  // ← added
        "/api/track/open",
        "/api/track/click",
         "/api/test-receiver"   // ← added, for local webhook delivery testing
    ];

    public ApiKeyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, AppDbContext dbContext)
    {
        // Skip API key check for whitelisted paths
        var path = context.Request.Path;
        if (WhitelistedPaths.Any(p =>
                path.StartsWithSegments(p, StringComparison.OrdinalIgnoreCase)))
        {
            await _next(context);
            return;
        }

        // Check header exists
        if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(
                "{\"statusCode\":401,\"message\":\"API Key is missing.\",\"errors\":[\"X-Api-Key header is required.\"],\"timestamp\":\"" + DateTime.UtcNow + "\"}");
            return;
        }

        // Validate API key against DB
        var apiKey = extractedApiKey.ToString();
        var application = await dbContext.ApplicationMasters
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ApiKey == apiKey && x.Status == 1);

        if (application == null)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(
                "{\"statusCode\":401,\"message\":\"Invalid or inactive API Key.\",\"errors\":[\"The provided API Key is not valid.\"],\"timestamp\":\"" + DateTime.UtcNow + "\"}");
            return;
        }

        // Store application info in HttpContext
        context.Items["ApplicationId"] = application.Id;
        context.Items["ApplicationCode"] = application.ApplicationCode;

        await _next(context);
    }
}