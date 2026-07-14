using EmailSaas.Application.Common.Interfaces;
using EmailSaas.Infrastructure.Persistence;
using EmailSaas.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmailSaas.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<AppDbContext>());

            services.AddScoped<IEmailSenderService, EmailSenderService>();

            // Register AES Encryption Service
            services.AddSingleton<IEncryptionService, AesEncryptionService>();

            // ✅ Register tracking service
            services.AddScoped<IEmailTrackingService, EmailTrackingService>();

            services.AddScoped<IWebhookDispatcher, WebhookDispatcher>();

            services.Configure<WebhookSettings>(configuration.GetSection("WebhookSettings"));

            services.AddHttpClient("WebhookClient", client =>
            {
                client.DefaultRequestHeaders.Add("User-Agent", "EmailSaas-Webhook/1.0");
            });

            services.AddHostedService<WebhookDispatchBackgroundService>();

            services.AddHostedService<BounceMailboxListenerService>();

            return services;
        }
    }
}
