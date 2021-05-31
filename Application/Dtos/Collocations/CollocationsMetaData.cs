using System.Collections.Generic;
using Application.Dtos.Temporary;

namespace Application.Dtos.Collocations
{
    public class CollocationsMetaData
    {
        public string FileName { get; }
        public int ParagraphId { get; }
        public int SentenceId { get; }
        public List<TokenDto> Collocations { get; set; } = new List<TokenDto>();
        public CollocationsMetaData(string fileName, int paragraphId, int sentenceId = -1)
        {
            FileName = fileName;
            ParagraphId = paragraphId;
            SentenceId = sentenceId;
        }
    }
}