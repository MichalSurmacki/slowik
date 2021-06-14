using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration
{
    public class SentenceConfiguration : IEntityTypeConfiguration<Sentence>
    {
        public void Configure(EntityTypeBuilder<Sentence> builder)
        {
            builder.HasKey(i => i.Id);
            builder.Property(i => i.Id)
                .HasDefaultValueSql("NEWID()");

            builder.Property(i => i.Xml)
                .IsRequired();
            
            builder.Property(i => i.XmlSentenceId)
                .IsRequired();
        }
    }
}