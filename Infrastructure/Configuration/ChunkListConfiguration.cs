using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration
{
    public class ChunkListConfiguration : IEntityTypeConfiguration<ChunkList>
    {
        public void Configure(EntityTypeBuilder<ChunkList> builder)
        {
            builder.HasKey(i => i.Id);
            builder.Property(i => i.Id)
                .HasDefaultValueSql("NEWID()");
        }
    }
}