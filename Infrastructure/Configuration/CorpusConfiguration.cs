using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration
{
    public class CorpusConfiguration : IEntityTypeConfiguration<Corpus>
    {
        public void Configure(EntityTypeBuilder<Corpus> builder)
        {
            builder.HasKey(i => i.Id);
            builder.Property(i => i.Id)
                .HasDefaultValueSql("NEWID()");
        }
    }
}