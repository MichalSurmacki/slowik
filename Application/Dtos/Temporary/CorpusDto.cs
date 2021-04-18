using System;
using System.Collections.Generic;

namespace Application.Dtos.Temporary
{
    public class CorpusDto
    {
        public Guid Id { get; set; }
        public ICollection<ChunkListDto> ChunkLists { get; set; }
        public CorpusMetaDataDto CorpusMetaData { get; set; }

        public CorpusDto()
        {
            ChunkLists = new List<ChunkListDto>();
        }
    }
}