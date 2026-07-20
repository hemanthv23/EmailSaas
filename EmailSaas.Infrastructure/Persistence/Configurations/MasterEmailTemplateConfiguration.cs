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
    public class MasterEmailTemplateConfiguration : IEntityTypeConfiguration<MasterEmailTemplate>
    {
        public void Configure(EntityTypeBuilder<MasterEmailTemplate> builder)
        {
            builder.ToTable("MasterEmailTemplate");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("TemplateID");
            builder.Property(x => x.ClientID).HasColumnName("ClientID");
            builder.Property(x => x.ApplicationId).HasColumnName("AppID");

            builder.Property(x => x.TemplateCode).HasMaxLength(100).IsRequired();
            builder.Property(x => x.TemplateName).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(500);
            builder.Property(x => x.SubjectTemplate).HasMaxLength(500).IsRequired();
            builder.Property(x => x.SubjectVariables).HasColumnType("nvarchar(max)");
            builder.Property(x => x.BodyTemplate).HasColumnType("nvarchar(max)").IsRequired();
            builder.Property(x => x.BodyVariables).HasColumnType("nvarchar(max)");
            builder.Property(x => x.FromEmailOverride).HasMaxLength(200);
            builder.Property(x => x.Status).IsRequired();
            builder.Property(x => x.CreatedBy).HasMaxLength(100).IsRequired();
            builder.Property(x => x.UpdatedBy).HasMaxLength(100);

        
            builder.HasIndex(x => new { x.ClientID, x.TemplateCode }).IsUnique();

            builder.HasOne(x => x.Client)
                   .WithMany(c => c.EmailTemplates)
                   .HasForeignKey(x => x.ClientID)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Application)
                   .WithMany()
                   .HasForeignKey(x => x.ApplicationId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
