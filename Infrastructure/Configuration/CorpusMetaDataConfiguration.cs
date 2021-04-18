using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace  Infrastructure.Configuration
{
    public class CorpusMetaDataConfiguration : IEntityTypeConfiguration<CorpusMetaData>
    {
        public void Configure(EntityTypeBuilder<CorpusMetaData> builder)
        {
            builder.HasKey(i => i.Id);
            builder.Property(i => i.Id)
                .HasDefaultValueSql("NEWID()");

            builder.Property(i => i.NumberOfProcessedFiles)
                .IsRequired();

            builder.Property(i => i.CreatedBy)
                .HasMaxLength(30);
        }
    }
}