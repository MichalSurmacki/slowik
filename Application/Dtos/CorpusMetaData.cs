using System;
using System.Collections.Generic;

namespace Application.Dtos
{
    public class CorpusMetaData
    {
        public CorpusMetaData()
        {
            WordsLookupDictionary = new Dictionary<string, List<int>>();
        }

        public Guid CorpusId { get; set; }
        public int NumberOfChunks { get; set; } = 0;
        public int NumberOfSentences { get; set; } = 0;
        public int NumberOfTokens { get; set; } = 0;
        public Dictionary<string, List<int>> WordsLookupDictionary { get; set; }
    }
}