using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EmailSaas.API.Swagger;

public static class SwaggerConfiguration
{
    public static void ConfigureSwaggerGen(SwaggerGenOptions options)
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "EmailSaaS API",
            Version = "v1",
            Description = """
                ## EmailSaaS — Centralized Email Notification Service
                
                A multi-tenant email template management and sending service.
                
                ### How to use:
                1. Register your application → get your **ApiKey**
                2. Create a client under your application
                3. Configure your email provider (SMTP/API)
                4. Create email templates with placeholders
                5. Call **Send Email** API to send emails
                
                ### Authentication:
                All endpoints (except `create-application`) require 
                **X-Api-Key** header with your application API key.
                """,
            Contact = new OpenApiContact
            {
                Name = "RPNC Systems Pvt Ltd",
                Email = "hemanth@rpncsystems.com"
            }
        });

        // API Key Security Definition
        options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
        {
            Description = "Enter your API Key. Example: ESAAS-XXXXXXXX",
            Name = "X-Api-Key",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "ApiKeyScheme"
        });

        // Security requirement — v10 uses a delegate that receives the built document
        options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecuritySchemeReference("ApiKey", document),
                new List<string>()
            }
        });

        // Order endpoints by name
        options.OrderActionsBy(x => x.RelativePath);

        // Enable XML comments
        var xmlFile = $"{System.Reflection.Assembly
            .GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
            options.IncludeXmlComments(xmlPath);
    }
}