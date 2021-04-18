using System;
using System.Collections.Generic;

namespace Domain.Models
{
    public class Corpus
    {
        public Guid Id { get; set; }
        public ICollection<ChunkList> ChunkLists { get; set; }
        public CorpusMetaData CorpusMetaData { get; set; }
    }
}