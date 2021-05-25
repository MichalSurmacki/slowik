using System;
using System.Collections.Generic;
using Application.Dtos.Temporary;

namespace Application.Cache
{
    public class CacheCollocationsInfoElement : CacheWordInfoElement
    {
        public List<TokenDto> Collocations { get; set; } = new List<TokenDto>();
        public List<List<TokenDto>> CollocationsByParagraph { get; set; } = new List<List<TokenDto>>();
        public List<List<TokenDto>> CollocationsBySentence { get; set; } = new List<List<TokenDto>>();
        public CacheCollocationsInfoElement(Guid corpusId, string word) : base(corpusId, word) {}
    }
}