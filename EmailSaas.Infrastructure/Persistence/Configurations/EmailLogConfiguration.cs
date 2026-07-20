using EmailSaas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmailSaas.Infrastructure.Persistence.Configurations
{
    public class EmailLogConfiguration : IEntityTypeConfiguration<EmailLog>
    {
        public void Configure(EntityTypeBuilder<EmailLog> builder)
        {
            builder.ToTable("EmailLog");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("EmailLogID");

            builder.Property(x => x.ApplicationId).HasColumnName("ApplicationID");
            builder.Property(x => x.ClientID).HasColumnName("ClientID");
            builder.Property(x => x.TemplateID).HasColumnName("TemplateID");
            builder.Property(x => x.ProviderID).HasColumnName("ProviderID");
            
            builder.Property(x => x.ToEmail).HasMaxLength(500).IsRequired();
            builder.Property(x => x.CcEmail).HasMaxLength(1000);
            builder.Property(x => x.BccEmail).HasMaxLength(1000);
            builder.Property(x => x.Status).IsRequired();
            builder.Property(x => x.Subject).HasMaxLength(500).IsRequired();
            builder.Property(x => x.ParameterValues).HasColumnType("nvarchar(max)");
            builder.Property(x => x.RenderedBody).HasColumnType("nvarchar(max)");
            builder.Property(x => x.ErrorMessage).HasColumnType("nvarchar(max)");
            builder.Property(x => x.MessageID).HasMaxLength(500);
            builder.Property(x => x.HasAttachment).HasDefaultValue(false);
            builder.Property(x => x.AttachmentName).HasColumnType("nvarchar(max)");
            builder.Property(x => x.CreatedBy).HasMaxLength(100).IsRequired();
            builder.Property(x => x.UpdatedBy).HasMaxLength(100);

            builder.HasOne(x => x.Application).WithMany().HasForeignKey(x => x.ApplicationId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Client).WithMany().HasForeignKey(x => x.ClientID).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Template).WithMany(t => t.EmailLogs).HasForeignKey(x => x.TemplateID).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Provider).WithMany().HasForeignKey(x => x.ProviderID).OnDelete(DeleteBehavior.Restrict);
        }
    }
}