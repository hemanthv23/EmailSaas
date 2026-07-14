using EmailSaas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using EmailSaas.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Infrastructure.Persistence
{
    public class AppDbContext : DbContext, IApplicationDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<ApplicationMaster> ApplicationMasters => Set<ApplicationMaster>();
        public DbSet<ClientMaster> ClientMasters => Set<ClientMaster>();
        public DbSet<EmailProviderConfig> EmailProviderConfigs => Set<EmailProviderConfig>();
        public DbSet<EmailTemplateMaster> EmailTemplateMasters => Set<EmailTemplateMaster>();
        public DbSet<EmailLog> EmailLogs => Set<EmailLog>();

        public DbSet<EmailEvent> EmailEvents => Set<EmailEvent>();
        public DbSet<EmailLinkClick> EmailLinkClicks => Set<EmailLinkClick>();
        public DbSet<WebhookSubscription> WebhookSubscriptions => Set<WebhookSubscription>();
        public DbSet<WebhookDeliveryLog> WebhookDeliveryLogs => Set<WebhookDeliveryLog>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
