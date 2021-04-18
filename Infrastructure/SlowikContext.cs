using System.Reflection;
using Application.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class SlowikContext : DbContext, ISlowikContext
    {
        public DbSet<CorpusMetaData> CorpusesMetaDataXml { get; set; }
        public DbSet<Sentence> Sentences { get; set; }
        public DbSet<Corpus> Corpuses { get; set; }
        public DbSet<Chunk> Chunks { get; set; }
        public DbSet<ChunkList> Chunklists { get; set; }
        public DbSet<ChunkListMetaData> ChunkListMetaData { get; set; }

        public SlowikContext(DbContextOptions<SlowikContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            builder.Entity<Chunk>()
                .HasMany(s => s.Sentences)
                .WithOne(c => c.Chunk);

            builder.Entity<ChunkList>()
                .HasMany(c => c.Chunks)
                .WithOne(c => c.Chunklist);

            builder.Entity<ChunkList>()
                .HasOne(c => c.ChunkListMetaData)
                .WithOne(c => c.ChunkList)
                .HasForeignKey<ChunkListMetaData>(c => c.Id);
            
            builder.Entity<Corpus>()
                .HasMany(c => c.ChunkLists)
                .WithOne(c => c.Corpus);

            builder.Entity<Corpus>()
                .HasOne(c => c.CorpusMetaData)
                .WithOne(c => c.Corpus)
                .HasForeignKey<CorpusMetaData>(c => c.Id);

            // add configuration
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}