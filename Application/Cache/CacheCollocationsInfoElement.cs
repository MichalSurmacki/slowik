using System;
using System.Collections.Generic;
using Application.Dtos.Temporary;

namespace Application.Cache
{
    public class CacheCollocationsInfoElement : CacheWordInfoElement
    {
        public List<TokenDto> Collocations;
        public CacheCollocationsInfoElement(Guid corpusId, string word) : base(corpusId, word) 
        {
            Collocations = new List<TokenDto>();
        }
    }
}