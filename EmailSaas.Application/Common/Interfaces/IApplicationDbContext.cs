using EmailSaas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EmailSaas.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<ApplicationMaster> ApplicationMasters { get; }
        DbSet<ClientMaster> ClientMasters { get; }
        DbSet<EmailProviderConfig> EmailProviderConfigs { get; }
        DbSet<EmailTemplateMaster> EmailTemplateMasters { get; }
        DbSet<EmailLog> EmailLogs { get; }
        DbSet<EmailEvent> EmailEvents { get; }
        DbSet<EmailLinkClick> EmailLinkClicks { get; }
        DbSet<WebhookSubscription> WebhookSubscriptions { get; }
        DbSet<WebhookDeliveryLog> WebhookDeliveryLogs { get; }

        DatabaseFacade Database { get; }


        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
