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
    public class EmailLinkClickConfiguration : IEntityTypeConfiguration<EmailLinkClick>
    {
        public void Configure(EntityTypeBuilder<EmailLinkClick> builder)
        {
            builder.ToTable("EmailLinkClicks");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.OriginalUrl)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(x => x.ClickedAt)
                .IsRequired();

            builder.Property(x => x.IpAddress)
                .HasMaxLength(45); // supports IPv6

            builder.Property(x => x.UserAgent)
                .HasMaxLength(500);

            builder.HasOne(x => x.EmailLog)
                .WithMany(x => x.LinkClicks)
                .HasForeignKey(x => x.EmailLogId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.EmailLogId);
        }
    }
}
