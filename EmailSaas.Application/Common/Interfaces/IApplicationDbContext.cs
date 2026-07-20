using EmailSaas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EmailSaas.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<MasterApplication> MasterApplications { get; }
        DbSet<MasterClient> MasterClients { get; }
        DbSet<MasterEmailProvider> MasterEmailProviders { get; }
        DbSet<MasterEmailTemplate> MasterEmailTemplates { get; }
        DbSet<EmailLog> EmailLogs { get; }
        DbSet<EmailEventLog> EmailEventLogs { get; }
        // DbSet<EmailLinkClick> EmailLinkClicks { get; }
        // DbSet<WebhookSubscription> WebhookSubscriptions { get; }
        // DbSet<WebhookDeliveryLog> WebhookDeliveryLogs { get; }

        DatabaseFacade Database { get; }


        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
