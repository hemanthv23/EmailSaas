using EmailSaas.Application;
using EmailSaas.Infrastructure;
using EmailSaas.API.Middleware;
using EmailSaas.API.Swagger;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Application layer (MediatR, FluentValidation, pipeline behaviors)
builder.Services.AddApplicationServices();
// Infrastructure layer (DbContext + IApplicationDbContext + EmailSenderService)
builder.Services.AddInfrastructureServices(builder.Configuration);

// ✅ CORS — allow any origin since actual security is enforced .
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});



builder.Services.AddEndpointsApiExplorer();

var isDev = builder.Environment.IsDevelopment();

// ✅ Swagger — full config in Dev, filtered to [Tags("Public")] only in Production
builder.Services.AddSwaggerGen(options =>
{
    SwaggerConfiguration.ConfigureSwaggerGen(options); // Removed isDev argument

    options.DocInclusionPredicate((docName, apiDesc) =>
    {
        if (isDev) return true; // Dev: show everything

        return apiDesc.ActionDescriptor.EndpointMetadata
            .OfType<Microsoft.AspNetCore.Http.TagsAttribute>()
            .Any(t => t.Tags.Contains("Public"));
    });
});

var app = builder.Build();
// Global Exception Handler — must be first
app.UseGlobalExceptionHandler();

// ✅ Swagger — always on now; content shown differs per environment (see predicate above)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "EmailSaaS API v1");
    c.RoutePrefix = "swagger";
    c.DocumentTitle = "EmailSaaS API";
    c.DefaultModelsExpandDepth(-1);
});

// ✅ Redirect root URL to Swagger UI
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        context.Response.Redirect("/swagger/index.html");
        return;
    }
    await next();
});

app.UseHttpsRedirection();
// ✅ CORS — must be before ApiKey/Authorization middleware
app.UseCors("AllowAll");
app.UseWebhookSignatureValidation();
// API Key Authentication — after Swagger, before controllers
app.UseApiKeyAuthentication();
app.UseAuthorization();

// ✅ Restrict admin-only routes in Production
// Only /api/send-email stays open to everyone.
// Everything else requires a special admin bypass header.
app.Use(async (context, next) =>
{
    if (!app.Environment.IsDevelopment())
    {
        var path = context.Request.Path.Value?.ToLower() ?? "";

        var publicRoutes = new[]
        {
            "/api/applications/create-application",
            "/api/clients/create-client",
            "/api/email-providers/create-email-provider",
            "/api/email-templates/create-email-template",
            "/api/email-logs/fetch-all-email-logs",
            "/api/send-email/send"
        };

        var publicRoutePrefixes = new[]
        {
            "/api/track/"
        };

        var isPublicRoute = publicRoutes.Any(p => path.Equals(p, StringComparison.OrdinalIgnoreCase)) ||
                            publicRoutePrefixes.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase));

        // ✅ Admin bypass — only YOU know this header/value
        var isAdminBypass = context.Request.Headers.TryGetValue(
            "X-Admin-Access", out var adminHeader)
            && adminHeader == "EmailSaaS-Admin-2026-Secret";

        if (!isPublicRoute && !isAdminBypass && path.StartsWith("/api/"))
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsync("Not Found");
            return;
        }
    }

    await next();
});

app.MapControllers();
app.Run();

// Make Program visible to integration tests
public partial class Program { }