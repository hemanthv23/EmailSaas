using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EmailSaas.Domain.Entities;

namespace EmailSaas.Infrastructure.Persistence.Configurations;

public class EmailLogConfiguration : IEntityTypeConfiguration<EmailLog>
{
    public void Configure(EntityTypeBuilder<EmailLog> builder)
    {
        builder.ToTable("EmailLog");

        builder.HasKey(x => x.Id);

        // ─── Existing columns ─────────────────────────────────
        builder.Property(x => x.ToEmail)
            .HasMaxLength(500).IsRequired();

        builder.Property(x => x.CcEmail)
            .HasMaxLength(1000);

        builder.Property(x => x.BccEmail)
            .HasMaxLength(1000);

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.ErrorMessage)
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.CreatedBy)
            .HasMaxLength(100).IsRequired();

        builder.Property(x => x.UpdatedBy)
            .HasMaxLength(100);

        builder.Property(x => x.Subject)
            .HasMaxLength(500).IsRequired();

        builder.Property(x => x.ParameterValues)
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.RenderedBody)
            .HasColumnType("nvarchar(max)");

        // ─── Webhook tracking ─────────────────────────────────
        builder.Property(x => x.MessageId)
            .HasMaxLength(500);

        builder.Property(x => x.WebhookStatus)
            .HasMaxLength(100);

        // ─── Open tracking ────────────────────────────────────
        // ✅ Fixed — removed duplicate HasDefaultValue(0)
        // was causing EF Core to not detect OpenCount changes
        builder.Property(x => x.OpenCount)
            .IsRequired()
            .HasDefaultValueSql("0");

        // ─── Click tracking ───────────────────────────────────
        builder.Property(x => x.ClickedAt);

        builder.Property(x => x.LastClickedAt);

        builder.Property(x => x.ClickCount)
            .IsRequired()
            .HasDefaultValueSql("0");

        // ─── Attachment tracking ──────────────────────────────
        builder.Property(x => x.HasAttachment)
            .HasDefaultValue(false);

        builder.Property(x => x.AttachmentNames)
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.AttachmentOpenCount)
            .IsRequired()
            .HasDefaultValueSql("0");

        // ─── Bounce tracking ──────────────────────────────────
        builder.Property(x => x.BounceReason)
            .HasMaxLength(1000);

        // ─── Index on MessageId for fast webhook lookup ────────
        builder.HasIndex(x => x.MessageId);

        // ─── Relationships (existing) ──────────────────────────
        builder.HasOne(x => x.Application)
               .WithMany()
               .HasForeignKey(x => x.ApplicationId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Client)
               .WithMany()
               .HasForeignKey(x => x.ClientId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Template)
               .WithMany(t => t.EmailLogs)
               .HasForeignKey(x => x.TemplateId)
               .OnDelete(DeleteBehavior.Restrict);

        // ─── Relationships (new — webhook/tracking feature) ────
        builder.HasMany(x => x.Events)
               .WithOne(x => x.EmailLog)
               .HasForeignKey(x => x.EmailLogId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.LinkClicks)
               .WithOne(x => x.EmailLog)
               .HasForeignKey(x => x.EmailLogId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.WebhookDeliveryLogs)
               .WithOne(x => x.EmailLog)
               .HasForeignKey(x => x.EmailLogId)
               .OnDelete(DeleteBehavior.Restrict); // avoid multi-cascade path errors with SQL Server
    }
}