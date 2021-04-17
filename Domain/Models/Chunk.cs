using System;
using System.Collections.Generic;

namespace Domain.Models
{
    public class Chunk
    {
        public Guid Id { get; set; }
        public int XmlChunkId { get; set; }
        public ICollection<Sentence> Sentences { get; set; }
        public ChunkList Chunklist { get; set; }
    }
}