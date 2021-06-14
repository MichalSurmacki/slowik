using System;
using System.Collections.Generic;

namespace Domain.Models
{
    public class ChunkList
    {
        public Guid Id { get; set; }
        public ChunkListMetaData ChunkListMetaData { get; set; }
        public ICollection<Chunk> Chunks { get; set; }
        public Corpus Corpus { get; set; }
    }
}