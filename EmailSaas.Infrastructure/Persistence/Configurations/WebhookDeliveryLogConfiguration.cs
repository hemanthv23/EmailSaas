/*
using EmailSaas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Infrastructure.Persistence.Configurations
{
    public class WebhookDeliveryLogConfiguration : IEntityTypeConfiguration<WebhookDeliveryLog>
    {
        public void Configure(EntityTypeBuilder<WebhookDeliveryLog> builder)
        {
            builder.ToTable("WebhookDeliveryLogs");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.EventType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Payload)
                .IsRequired()
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.ResponseBody)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.AttemptNumber)
                .IsRequired()
                .HasDefaultValue(1);

            builder.Property(x => x.IsSuccess)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasOne(x => x.WebhookSubscription)
                .WithMany(x => x.DeliveryLogs)
                .HasForeignKey(x => x.WebhookSubscriptionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.EmailLog)
                .WithMany(x => x.WebhookDeliveryLogs)
                .HasForeignKey(x => x.LogID)
                .OnDelete(DeleteBehavior.Restrict); // avoid multiple cascade paths

            builder.HasIndex(x => x.WebhookSubscriptionId);
            builder.HasIndex(x => x.LogID);
            builder.HasIndex(x => x.NextRetryAt); // for retry background job scans
        }
    }
}

*/