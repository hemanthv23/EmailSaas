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
    public class EmailEventConfiguration : IEntityTypeConfiguration<EmailEvent>
    {
        public void Configure(EntityTypeBuilder<EmailEvent> builder)
        {
            builder.ToTable("EmailEvents");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.EventType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.EventData)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.OccurredAt)
                .IsRequired();

            builder.HasOne(x => x.EmailLog)
                .WithMany(x => x.Events)
                .HasForeignKey(x => x.EmailLogId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.EmailLogId);
            builder.HasIndex(x => x.EventType);
        }
    }
}
