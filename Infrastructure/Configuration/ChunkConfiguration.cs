using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Configuration
{
    public class ChunkConfiguration : IEntityTypeConfiguration<Chunk>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Chunk> builder)
        {
            builder.HasKey(i => i.Id);
            builder.Property(i => i.Id)
                .HasDefaultValueSql("NEWID()");

            builder.Property(i => i.XmlChunkId)
                .IsRequired();
        }
    }
}