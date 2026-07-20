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
    public class MasterApplicationConfiguration : IEntityTypeConfiguration<MasterApplication>
    {
        public void Configure(EntityTypeBuilder<MasterApplication> builder)
        {
            builder.ToTable("MasterApplication");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("AppID");

            builder.Property(x => x.ApplicationCode).HasMaxLength(50).IsRequired();
            builder.Property(x => x.ApplicationName).HasMaxLength(200).IsRequired();
            builder.Property(x => x.ApiKey).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Status).IsRequired();
            builder.Property(x => x.CreatedBy).HasMaxLength(100).IsRequired();
            builder.Property(x => x.UpdatedBy).HasMaxLength(100);

            builder.HasIndex(x => x.ApplicationCode).IsUnique();
        }
    }
}
