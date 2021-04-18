using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration
{
    public class ChunkListMetaDataConfiguration : IEntityTypeConfiguration<ChunkListMetaData>
    {
        public void Configure(EntityTypeBuilder<ChunkListMetaData> builder)
        {
            builder.HasKey(i => i.Id);
            builder.Property(i => i.Id)
                .HasDefaultValueSql("NEWID()");

            builder.Property(i => i.NumberOfChunks)
                .IsRequired();

            builder.Property(i => i.NumberOfSentences)
                .IsRequired();

            builder.Property(i => i.NumberOfTokens)
                .IsRequired();

            //TODO: length of string containing XmlDictionatyLookUp
            builder.Property(i => i.JsonDictionaryLookUp)
                .IsRequired();
        }
    }
}