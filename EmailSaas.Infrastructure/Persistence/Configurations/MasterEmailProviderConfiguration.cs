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
    public class MasterMasterEmailProvideruration : IEntityTypeConfiguration<MasterEmailProvider>
    {
        public void Configure(EntityTypeBuilder<MasterEmailProvider> builder)
        {
            builder.ToTable("MasterEmailProvider");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("ProviderID");
            builder.Property(x => x.ClientID).HasColumnName("ClientID");

            builder.Property(x => x.ProviderName).HasMaxLength(100).IsRequired();
            builder.Property(x => x.SenderName).HasMaxLength(200).IsRequired();
            builder.Property(x => x.SenderEmail).HasMaxLength(200).IsRequired();
            builder.Property(x => x.ReplyToEmail).HasMaxLength(200);
            builder.Property(x => x.UserName).HasMaxLength(200);
            builder.Property(x => x.Password).HasMaxLength(1000);
            builder.Property(x => x.SMTPHost).HasMaxLength(200);
            builder.Property(x => x.APIKey).HasMaxLength(2000);
            builder.Property(x => x.IMAPHost).HasMaxLength(255);
            builder.Property(x => x.IMPAUserName).HasMaxLength(255);
            builder.Property(x => x.IMAPPassword).HasMaxLength(500);
            builder.Property(x => x.IMAPSSL).HasDefaultValue(true);
            builder.Property(x => x.IsDefault).IsRequired();
            builder.Property(x => x.Status).IsRequired();
            builder.Property(x => x.CreatedBy).HasMaxLength(100).IsRequired();
            builder.Property(x => x.UpdatedBy).HasMaxLength(100);

            builder.HasOne(x => x.Client)
                   .WithMany(c => c.MasterEmailProviders)
                   .HasForeignKey(x => x.ClientID)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
