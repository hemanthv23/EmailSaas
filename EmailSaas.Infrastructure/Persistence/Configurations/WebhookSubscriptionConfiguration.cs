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
    public class WebhookSubscriptionConfiguration : IEntityTypeConfiguration<WebhookSubscription>
    {
        public void Configure(EntityTypeBuilder<WebhookSubscription> builder)
        {
            builder.ToTable("WebhookSubscriptions");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ClientID)
                .IsRequired();

            builder.Property(x => x.CallbackUrl)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.Secret)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(x => x.EventTypes)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.Status)
                .IsRequired()
                .HasDefaultValue((byte)1);

            builder.HasOne(x => x.Client)
                .WithMany() // add ICollection<WebhookSubscription> on MasterClient if you want reverse nav
                .HasForeignKey(x => x.ClientID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.ClientID);
        }
    }
}

*/