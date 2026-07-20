using EmailSaas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmailSaas.Infrastructure.Persistence.Configurations
{
    public class EmailEventLogConfiguration : IEntityTypeConfiguration<EmailEventLog>
    {
        public void Configure(EntityTypeBuilder<EmailEventLog> builder)
        {
            builder.ToTable("EmailEventLog");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("EmailEventLogID");

            builder.Property(x => x.LogID).HasColumnName("EmailLogID");
            builder.Property(x => x.MessageID).HasMaxLength(500).IsRequired();
            builder.Property(x => x.EventType).HasMaxLength(50).IsRequired();
            builder.Property(x => x.LogData).HasColumnType("nvarchar(max)");
            builder.Property(x => x.EventLogDate).IsRequired();
            builder.Property(x => x.Status).IsRequired();
            builder.Property(x => x.CreatedBy).HasMaxLength(100).IsRequired();
            builder.Property(x => x.UpdatedBy).HasMaxLength(100);

            builder.HasOne(x => x.EmailLog).WithMany(e => e.Events).HasForeignKey(x => x.LogID).OnDelete(DeleteBehavior.Cascade);
        }
    }
}