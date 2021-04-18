using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces
{
    public interface ISlowikContext
    {
        public DbSet<CorpusMetaData> CorpusesMetaDataXml { get; set; }
        public DbSet<Corpus> Corpuses { get; set; }
        public DbSet<Sentence> Sentences { get; set; }
        public DbSet<Chunk> Chunks { get; set; }
        public DbSet<ChunkList> Chunklists { get; set; }
        public DbSet<ChunkListMetaData> ChunkListMetaData { get; set; }
        public int SaveChanges();

    }
}