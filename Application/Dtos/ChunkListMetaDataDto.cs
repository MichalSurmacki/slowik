using System;
using System.Collections.Generic;

namespace Application.Dtos
{
    public class ChunkListMetaDataDto
    {
        public Guid Id { get; set; }
        public int NumberOfChunks { get; set; } = 0;
        public int NumberOfSentences { get; set; } = 0;
        public int NumberOfTokens { get; set; } = 0;
        public string OriginFileName { get; set; }
        /// Summary:
        ///     Works with not case sensitive keys.
        public Dictionary<string, List<int>> WordsLookupDictionary { get; set; }
        public string JsonDictionaryLookUp { get; set; }

        public ChunkListMetaDataDto()
        {
            WordsLookupDictionary = new Dictionary<string, List<int>>(StringComparer.InvariantCultureIgnoreCase);
        }
    }
}