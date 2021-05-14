using System;
using System.Collections.Generic;

namespace Domain.Models
{
    public class ChunkListMetaData
    {
        public Guid Id { get; set; }
        public int NumberOfChunks { get; set; } = 0;
        public int NumberOfSentences { get; set; } = 0;
        public int NumberOfTokens { get; set; } = 0;
        public string OriginFileName { get; set; }
        
        // klucz - szukane słowo, wartosc - lista numerow zdan gdzie wystepuje slowo
        public string JsonDictionaryLookUp { get; set; }
        public ChunkList ChunkList { get; set; }
    }
}