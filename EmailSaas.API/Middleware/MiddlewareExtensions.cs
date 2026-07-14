namespace EmailSaas.API.Middleware;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        return app;
    }

    public static IApplicationBuilder UseApiKeyAuthentication(this IApplicationBuilder app)
    {
        app.UseMiddleware<ApiKeyMiddleware>();
        return app;
    }

    public static IApplicationBuilder UseWebhookSignatureValidation(this IApplicationBuilder app)
    {
        app.UseMiddleware<WebhookSignatureValidationMiddleware>();
        return app;
    }
}