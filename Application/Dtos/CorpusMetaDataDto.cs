using System;
using System.Collections.Generic;
using Application.Dtos.Temporary;

namespace Application.Dtos
{
    public class CorpusMetaDataDto
    {
        public string OriginFileName { get; set; }
        public int NumberOfProcessedFiles { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }

        public CorpusMetaDataDto(CorpusDto corpus, string originFileName, string createdBy)
        {
            NumberOfProcessedFiles = corpus.ChunkLists.Count;
            CreatedAt = DateTime.Now;
            CreatedBy = createdBy;
            OriginFileName = originFileName;
        }
    }
}