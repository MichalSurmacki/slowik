using System;
using System.Collections.Generic;
using Application.Dtos.Temporary;

namespace Application.Dtos
{
    public class CorpusMetaDataDto
    {
        public int NumberOfProcessedFiles { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }

        public CorpusMetaDataDto(CorpusDto corpus, string createdBy)
        {
            NumberOfProcessedFiles = corpus.ChunkLists.Count;
            CreatedAt = DateTime.Now;
            CreatedBy = createdBy;
        }
    }
}