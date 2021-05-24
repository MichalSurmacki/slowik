using System;
using System.Collections.Generic;
using Application.Dtos.Temporary;

namespace Application.Cache
{
    public class CollocationsModel
    {
        public Guid CorpusId { get; set; }
        public string Word { get; set; }
        public List<TokenDto> Collocations { get; set; }
        public int WordCountInCorpus { get; set; }
        public List<Tuple<int, string>> WordApperancesWithFilenames { get; set; }

        public CollocationsModel(Guid corpusId, string word)
        {
            Word = word;
            CorpusId = corpusId;
        }
    }
}