using System;
using System.Collections.Generic;
using Application.Dtos.Temporary;

namespace Application.Cache
{
    public class CacheCollocationsInfoElement : CacheWordInfoElement
    {
        public List<TokenDto> Collocations = new List<TokenDto>();
        public CacheCollocationsInfoElement(Guid corpusId, string word) : base(corpusId, word) {}
    }
}