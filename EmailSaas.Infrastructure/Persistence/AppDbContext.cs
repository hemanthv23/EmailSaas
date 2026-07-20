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

        public DbSet<MasterApplication> MasterApplications => Set<MasterApplication>();
        public DbSet<MasterClient> MasterClients => Set<MasterClient>();
        public DbSet<MasterEmailProvider> MasterEmailProviders => Set<MasterEmailProvider>();
        public DbSet<MasterEmailTemplate> MasterEmailTemplates => Set<MasterEmailTemplate>();
        public DbSet<EmailLog> EmailLogs => Set<EmailLog>();

        public DbSet<EmailEventLog> EmailEventLogs => Set<EmailEventLog>();
        // public DbSet<EmailLinkClick> EmailLinkClicks => Set<EmailLinkClick>();
        // public DbSet<WebhookSubscription> WebhookSubscriptions => Set<WebhookSubscription>();
        // public DbSet<WebhookDeliveryLog> WebhookDeliveryLogs => Set<WebhookDeliveryLog>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
