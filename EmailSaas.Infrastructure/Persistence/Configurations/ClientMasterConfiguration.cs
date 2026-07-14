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
    public class ClientMasterConfiguration : IEntityTypeConfiguration<ClientMaster>
    {
        public void Configure(EntityTypeBuilder<ClientMaster> builder)
        {
            builder.ToTable("MasterClient");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ClientCode).HasMaxLength(50).IsRequired();
            builder.Property(x => x.ClientName).HasMaxLength(200).IsRequired();
            builder.Property(x => x.LogoUrl).HasMaxLength(500);
            builder.Property(x => x.PrimaryColor).HasMaxLength(50);
            builder.Property(x => x.Status).IsRequired();
            builder.Property(x => x.CreatedBy).HasMaxLength(100).IsRequired();
            builder.Property(x => x.UpdatedBy).HasMaxLength(100);

            builder.HasIndex(x => x.ClientCode).IsUnique();

            builder.HasOne(x => x.Application)
                   .WithMany(a => a.Clients)
                   .HasForeignKey(x => x.ApplicationId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
