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
    public class EmailProviderConfigConfiguration : IEntityTypeConfiguration<EmailProviderConfig>
    {
        public void Configure(EntityTypeBuilder<EmailProviderConfig> builder)
        {
            builder.ToTable("MasterEmailProviderConfig");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ProviderName).HasMaxLength(100).IsRequired();
            builder.Property(x => x.SenderName).HasMaxLength(200).IsRequired();
            builder.Property(x => x.SenderEmail).HasMaxLength(200).IsRequired();
            builder.Property(x => x.ReplyToEmail).HasMaxLength(200);
            builder.Property(x => x.SmtpHost).HasMaxLength(200);
            builder.Property(x => x.UserName).HasMaxLength(200);
            builder.Property(x => x.PasswordEncrypted).HasMaxLength(1000);
            builder.Property(x => x.ApiKeyEncrypted).HasMaxLength(2000);
            builder.Property(x => x.ImapHost).HasMaxLength(255);
            builder.Property(x => x.ImapUserName).HasMaxLength(255);
            builder.Property(x => x.ImapPasswordEncrypted).HasMaxLength(500);
            builder.Property(x => x.BounceMonitoringEnabled).HasDefaultValue(false);
            builder.Property(x => x.ImapUseSsl).HasDefaultValue(true);
            builder.Property(x => x.IsDefault).IsRequired();
            builder.Property(x => x.Status).IsRequired();
            builder.Property(x => x.CreatedBy).HasMaxLength(100).IsRequired();
            builder.Property(x => x.UpdatedBy).HasMaxLength(100);

            builder.HasOne(x => x.Client)
                   .WithMany(c => c.EmailProviderConfigs)
                   .HasForeignKey(x => x.ClientId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
