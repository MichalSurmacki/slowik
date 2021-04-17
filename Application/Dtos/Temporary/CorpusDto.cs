using System;
using System.Collections.Generic;

namespace Application.Dtos.Temporary
{
    public class CorpusDto
    {
        public Guid Id { get; set; }
        public List<ChunkListDto> IndividualCorpusFileList { get; set; }
    }
}